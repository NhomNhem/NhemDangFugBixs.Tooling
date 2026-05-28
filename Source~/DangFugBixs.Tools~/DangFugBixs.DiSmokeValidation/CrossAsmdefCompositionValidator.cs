using System.Reflection;

namespace NhemDangFugBixs.DiSmokeValidation;

/// <summary>
/// Validates composition-only generation across multiple assemblies.
/// This covers checks that Roslyn per-compilation analysis cannot reliably
/// prove because they span assembly boundaries (Tasks 6.1, 6.2, 6.3).
/// </summary>
internal sealed class CrossAsmdefCompositionValidator {
    public SmokeValidationResult Validate(IEnumerable<string> assemblyPaths) {
        var result = new SmokeValidationResult();
        var assemblies = new List<Assembly>();
        var assemblyScopeMappings = new Dictionary<string, List<CompositionTarget>>();
        var assemblyServiceRegistrations = new Dictionary<string, List<ServiceRegistration>>();

        foreach (var path in assemblyPaths) {
            if (!File.Exists(path)) {
                result.AddWarning($"Assembly not found: {path}");
                continue;
            }

            try {
                var assembly = Assembly.LoadFrom(path);
                assemblies.Add(assembly);
                var name = assembly.GetName().Name ?? Path.GetFileNameWithoutExtension(path);
                assemblyScopeMappings[name] = ScanCompositionTargets(assembly);
                assemblyServiceRegistrations[name] = ScanServiceRegistrations(assembly);
            } catch (Exception ex) {
                result.AddWarning($"Failed to load assembly {path}: {ex.Message}");
            }
        }

        if (assemblies.Count == 0) {
            result.AddError("No assemblies could be loaded for cross-asmdef validation.");
            return result;
        }

        // Task 6.2: Detect duplicate composition targets across separate Unity asmdefs
        var allScopeMappings = assemblyScopeMappings
            .SelectMany(kvp => kvp.Value.Select(m => new { Assembly = kvp.Key, Mapping = m }))
            .ToList();

        var duplicateScopes = allScopeMappings
            .GroupBy(x => x.Mapping.ScopeMarkerFullName, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => new {
                Scope = g.Key,
                Assemblies = g.Select(x => x.Assembly).ToList()
            })
            .ToList();

        foreach (var dup in duplicateScopes) {
            result.AddError(
                $"Duplicate composition target for scope '{dup.Scope}' found across assemblies: {string.Join(", ", dup.Assemblies)}. " +
                "Only one LifetimeScope should map to a given scope marker.");
        }

        // Task 6.3: Detect project-level discovery drift
        // Find services whose scope marker has no composition target in any assembly
        var allScopeMarkers = allScopeMappings.Select(x => x.Mapping.ScopeMarkerFullName).ToHashSet(StringComparer.Ordinal);
        var allServiceRegistrations = assemblyServiceRegistrations
            .SelectMany(kvp => kvp.Value.Select(r => new { Assembly = kvp.Key, Registration = r }))
            .ToList();

        var servicesWithoutComposition = allServiceRegistrations
            .Where(r => r.Registration.UsesTypeSafeScope && !string.IsNullOrEmpty(r.Registration.ScopeMarkerFullName))
            .Where(r => !allScopeMarkers.Contains(r.Registration.ScopeMarkerFullName))
            .ToList();

        foreach (var orphan in servicesWithoutComposition) {
            result.AddError(
                $"Service '{orphan.Registration.ServiceFullName}' in assembly '{orphan.Assembly}' " +
                $"references scope marker '{orphan.Registration.ScopeMarkerFullName}', but no composition target (LifetimeScopeFor) " +
                "was found for that scope in any scanned assembly. The service will never be registered.");
        }

        // Task 6.1: Verify composition-only generation invariants
        foreach (var kvp in assemblyScopeMappings) {
            var asmName = kvp.Key;
            var hasCompositionTargets = kvp.Value.Count > 0;
            var hasServiceRegistrations = assemblyServiceRegistrations[asmName].Count > 0;

            if (hasCompositionTargets && !hasServiceRegistrations) {
                result.AddWarning($"Assembly '{asmName}' has composition targets but no local service registrations. This is valid if it only composes referenced services.");
            }
        }

        // Summary
        var totalCompositionTargets = allScopeMappings.Count;
        var totalServiceRegistrations = allServiceRegistrations.Count;
        var compositionAsmdefs = assemblyScopeMappings.Count(kvp => kvp.Value.Count > 0);
        var serviceAsmdefs = assemblyServiceRegistrations.Count(kvp => kvp.Value.Count > 0);

        result.AddWarning($"Cross-asmdef scan: {compositionAsmdefs} composition asmdef(s), {serviceAsmdefs} service asmdef(s), {totalCompositionTargets} scope mapping(s), {totalServiceRegistrations} service registration(s).");

        return result;
    }

    private static List<CompositionTarget> ScanCompositionTargets(Assembly assembly) {
        var results = new List<CompositionTarget>();
        var types = assembly.GetTypes();

        foreach (var type in types) {
            if (!type.IsClass || type.IsAbstract) continue;

            var attrs = type.GetCustomAttributes(false);
            foreach (var attr in attrs) {
                var attrType = attr.GetType();
                // Generic attributes have names like "LifetimeScopeForAttribute`1"
                if (attrType.Name == "LifetimeScopeForAttribute" || attrType.Name.StartsWith("LifetimeScopeForAttribute`", StringComparison.Ordinal)) {
                    string? scopeMarker = null;

                    // Try generic argument
                    if (attrType.IsGenericType) {
                        var genericArgs = attrType.GetGenericArguments();
                        if (genericArgs.Length > 0) {
                            scopeMarker = genericArgs[0].FullName ?? genericArgs[0].Name;
                        }
                    }

                    // Try constructor argument (non-generic version stores in IdentityType property)
                    if (string.IsNullOrEmpty(scopeMarker)) {
                        var ctorArg = attrType.GetProperty("IdentityType")?.GetValue(attr)
                            ?? attrType.GetField("<IdentityType>k__BackingField")?.GetValue(attr);
                        if (ctorArg is Type t) {
                            scopeMarker = t.FullName ?? t.Name;
                        }
                    }

                    if (!string.IsNullOrEmpty(scopeMarker)) {
                        results.Add(new CompositionTarget(type.FullName ?? type.Name, scopeMarker));
                    }
                }
            }
        }

        return results;
    }

    private static List<ServiceRegistration> ScanServiceRegistrations(Assembly assembly) {
        var results = new List<ServiceRegistration>();
        var types = assembly.GetTypes();

        foreach (var type in types) {
            if (!type.IsClass || type.IsAbstract) continue;

            var attrs = type.GetCustomAttributes(false);
            foreach (var attr in attrs) {
                var attrType = attr.GetType();
                // Generic attributes have names like "AutoRegisterInAttribute`1"
                if (attrType.Name == "AutoRegisterInAttribute" || attrType.Name.StartsWith("AutoRegisterInAttribute`", StringComparison.Ordinal)) {
                    string? scopeMarker = null;
                    bool usesTypeSafeScope = false;

                    // Try generic argument
                    if (attrType.IsGenericType) {
                        var genericArgs = attrType.GetGenericArguments();
                        if (genericArgs.Length > 0) {
                            scopeMarker = genericArgs[0].FullName ?? genericArgs[0].Name;
                            usesTypeSafeScope = true;
                        }
                    }

                    // Try constructor argument
                    if (string.IsNullOrEmpty(scopeMarker)) {
                        var ctorArg = attrType.GetProperty("ScopeType")?.GetValue(attr)
                            ?? attrType.GetField("_scopeType")?.GetValue(attr);
                        if (ctorArg is Type t) {
                            scopeMarker = t.FullName ?? t.Name;
                            usesTypeSafeScope = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(scopeMarker)) {
                        results.Add(new ServiceRegistration(type.FullName ?? type.Name, scopeMarker, usesTypeSafeScope));
                    }
                }
            }
        }

        return results;
    }

    private readonly record struct CompositionTarget(string LifetimeScopeFullName, string ScopeMarkerFullName);
    private readonly record struct ServiceRegistration(string ServiceFullName, string ScopeMarkerFullName, bool UsesTypeSafeScope);
}
