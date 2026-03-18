using System.Reflection;

namespace NhemDangFugBixs.DiSmokeValidation;

/// <summary>
/// Validates DI container resolution at runtime without Unity Editor.
/// Uses reflection to avoid direct VContainer dependency.
/// </summary>
internal sealed class ResolveSmokeValidator {
    public ResolveValidationResult Validate(string assemblyPath, ResolveSmokeOptions options) {
        var result = new ResolveValidationResult();

        if (string.IsNullOrWhiteSpace(assemblyPath)) {
            result.AddError("No assembly path was provided.");
            return result;
        }

        if (!File.Exists(assemblyPath)) {
            result.AddError($"Assembly not found: {assemblyPath}");
            return result;
        }

        Assembly assembly;
        try {
            assembly = Assembly.LoadFrom(assemblyPath);
        } catch (Exception ex) {
            result.AddError($"Failed to load assembly: {ex.Message}");
            return result;
        }

        // Find VContainerRegistration type
        var injectorTypes = assembly
            .GetTypes()
            .Where(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name.Contains("VContainerRegistration", StringComparison.Ordinal))
            .ToList();

        if (injectorTypes.Count == 0) {
            result.AddError("No generated VContainerRegistration type was found.");
            return result;
        }

        // Try to load VContainer
        Type? containerBuilderType = null;
        Type? containerType = null;
        try {
            var vcontainerAssembly = Assembly.Load("VContainer");
            containerBuilderType = vcontainerAssembly.GetType("VContainer.IContainerBuilder");
            containerType = vcontainerAssembly.GetType("VContainer.IObjectResolver");
        } catch {
            result.AddWarning("VContainer not available, skipping runtime resolution");
        }

        // Find entry point types
        var entryPointTypes = assembly.GetTypes()
            .Where(t => ImplementsEntryPoint(t))
            .ToList();

        result.TotalServices = entryPointTypes.Count;

        if (containerBuilderType != null && containerType != null) {
            // Build container and resolve
            try {
                var builder = Activator.CreateInstance(containerBuilderType!);
                if (builder == null) {
                    result.AddError("Failed to create ContainerBuilder");
                    return result;
                }

                // Invoke RegisterAll
                var registerAllMethod = injectorTypes[0].GetMethod("RegisterAll", BindingFlags.Public | BindingFlags.Static);
                registerAllMethod?.Invoke(null, new[] { builder });

                // Build container
                var buildMethod = containerBuilderType!.GetMethod("Build");
                var container = buildMethod?.Invoke(builder, null);

                result.ContainerBuilt = container != null;

                // Resolve entry points
                foreach (var epType in entryPointTypes) {
                    try {
                        var resolveMethod = containerType!.GetMethod("Resolve", new[] { typeof(Type) });
                        var service = resolveMethod?.Invoke(container, new object[] { epType });
                        if (service != null) {
                            result.ResolvedServices.Add(epType.FullName ?? epType.Name);
                        }
                    } catch (TargetInvocationException tex) {
                        result.AddError($"Failed to resolve '{epType.Name}': {tex.InnerException?.Message ?? tex.Message}");
                    } catch (Exception ex) {
                        result.AddError($"Failed to resolve '{epType.Name}': {ex.Message}");
                    }
                }

                result.ResolvedCount = result.ResolvedServices.Count;
                result.SkippedCount = result.TotalServices - result.ResolvedCount;

            } catch (Exception ex) {
                result.AddError($"Container build failed: {ex.InnerException?.Message ?? ex.Message}");
            }
        } else {
            // Can't resolve, but report what we found
            result.SkippedCount = result.TotalServices;
            foreach (var epType in entryPointTypes) {
                result.ResolvedServices.Add($"[Skipped] {epType.FullName ?? epType.Name}");
            }
        }

        return result;
    }

    private static bool ImplementsEntryPoint(Type type) {
        if (!type.IsClass || type.IsAbstract) return false;

        var entryPointInterfaces = new[] {
            "VContainer.Unity.ITickable",
            "VContainer.Unity.IInitializable",
            "VContainer.Unity.IStartable",
            "VContainer.Unity.IPostFixedUpdate",
            "VContainer.Unity.IPostLateUpdate",
            "VContainer.Unity.IPostStart",
            "VContainer.Unity.IFixedTickable",
            "VContainer.Unity.ILateTickable"
        };

        return type.GetInterfaces()
            .Any(i => entryPointInterfaces.Contains(i.FullName ?? i.Name));
    }
}

/// <summary>
/// Options for resolve smoke validation.
/// </summary>
internal sealed class ResolveSmokeOptions {
    public bool Verbose { get; init; }
    public bool SkipMonoBehaviour { get; init; } = true;
}

/// <summary>
/// Result of resolve smoke validation.
/// </summary>
internal sealed class ResolveValidationResult {
    public bool IsSuccess => Errors.Count == 0;
    public bool ContainerBuilt { get; set; }
    public int TotalServices { get; set; }
    public int ResolvedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> ResolvedServices { get; } = new List<string>();
    public List<string> Errors { get; } = new List<string>();
    public List<string> Warnings { get; } = new List<string>();

    public void AddError(string message) { Errors.Add(message); }
    public void AddWarning(string message) { Warnings.Add(message); }

    public string ToHumanReadableText() {
        var lines = new List<string>();

        lines.Add("Runtime Resolve Smoke Test");
        lines.Add("==========================");
        lines.Add("");
        lines.Add($"Container Built: {(ContainerBuilt ? "✅ Yes" : "❌ No")}");
        lines.Add($"Total Services: {TotalServices}");
        lines.Add($"Resolved: {ResolvedCount}");
        lines.Add($"Skipped: {SkippedCount}");
        lines.Add("");

        if (ResolvedServices.Count > 0) {
            lines.Add("Services:");
            foreach (var service in ResolvedServices.OrderBy(s => s)) {
                var prefix = service.StartsWith("[Skipped]") ? "⏭️ " : "✅ ";
                lines.Add($"  {prefix}{service}");
            }
            lines.Add("");
        }

        if (Warnings.Count > 0) {
            lines.Add("Warnings:");
            foreach (var warning in Warnings) {
                lines.Add($"  ⚠️ {warning}");
            }
            lines.Add("");
        }

        if (Errors.Count > 0) {
            lines.Add("Errors:");
            foreach (var error in Errors) {
                lines.Add($"  ❌ {error}");
            }
        }

        return string.Join("\n", lines);
    }
}
