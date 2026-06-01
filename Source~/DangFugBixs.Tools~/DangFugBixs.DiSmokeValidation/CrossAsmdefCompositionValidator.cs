using System.Reflection;
using NhemDangFugBixs.Common.Models.DiContractGraph;

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
        var assemblyReferences = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

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
                assemblyReferences[name] = assembly.GetReferencedAssemblies()
                    .Select(reference => reference.Name ?? string.Empty)
                    .Where(reference => !string.IsNullOrWhiteSpace(reference))
                    .ToHashSet(StringComparer.Ordinal);
            } catch (Exception ex) {
                result.AddWarning($"Failed to load assembly {path}: {ex.Message}");
            }
        }

        if (assemblies.Count == 0) {
            result.AddError("No assemblies could be loaded for cross-asmdef validation.");
            return result;
        }

        var graph = BuildGraph(assemblyScopeMappings, assemblyServiceRegistrations);

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
            result.AddEvidence(new SmokeDiagnosticEvidence {
                Kind = "duplicate-composition-target",
                ScopeMarker = dup.Scope,
                CompositionAssembly = string.Join(",", dup.Assemblies)
            });
        }

        // Task 6.3: Detect project-level discovery drift
        // Find services whose scope marker has no composition target in any assembly
        var allScopeMarkers = graph.CompositionRoots.Select(root => root.ScopeMarkerType.MetadataName).ToHashSet(StringComparer.Ordinal);
        var allServiceRegistrations = graph.Services
            .ToList();

        var servicesWithoutComposition = allServiceRegistrations
            .Where(r => r.ScopeMarkerType.HasValue)
            .Where(r => !allScopeMarkers.Contains(r.ScopeMarkerType!.Value.MetadataName))
            .ToList();

        foreach (var orphan in servicesWithoutComposition) {
            result.AddError(
                $"Service '{orphan.ImplementationType.FullName}' in assembly '{orphan.Provenance.DeclaringAssembly}' " +
                $"references scope marker '{orphan.ScopeMarkerType!.Value.FullName}', but no composition target (LifetimeScopeFor) " +
                "was found for that scope in any scanned assembly. The service will never be registered.");
            result.AddEvidence(new SmokeDiagnosticEvidence {
                Kind = "missing-composition-target",
                ScopeMarker = orphan.ScopeMarkerType.Value.FullName,
                Service = orphan.ImplementationType.FullName,
                SourceAssembly = orphan.Provenance.DeclaringAssembly,
                ReferencePath = orphan.Provenance.ReferencePath
            });
        }

        foreach (var composition in graph.CompositionRoots) {
            var compositionAssembly = composition.Provenance.DeclaringAssembly;
            var references = assemblyReferences.TryGetValue(compositionAssembly, out var foundReferences)
                ? foundReferences
                : new HashSet<string>(StringComparer.Ordinal);
            foreach (var registration in graph.ServicesForScope(composition.ScopeMarkerType)) {
                var serviceAssembly = registration.Provenance.DeclaringAssembly;
                if (string.Equals(compositionAssembly, serviceAssembly, StringComparison.Ordinal) ||
                    references.Contains(serviceAssembly)) {
                    continue;
                }

                result.AddError(
                    $"Composition assembly '{compositionAssembly}' maps scope '{composition.ScopeMarkerType.FullName}' " +
                    $"but does not reference service assembly '{serviceAssembly}' containing '{registration.ImplementationType.FullName}'.");
                result.AddEvidence(new SmokeDiagnosticEvidence {
                    Kind = "composition-reference-gap",
                    ScopeMarker = composition.ScopeMarkerType.FullName,
                    Service = registration.ImplementationType.FullName,
                    CompositionRoot = composition.LifetimeScopeType.FullName,
                    SourceAssembly = serviceAssembly,
                    CompositionAssembly = compositionAssembly,
                    ReferencePath = new[] { compositionAssembly, serviceAssembly }
                });
            }
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
        var totalCompositionTargets = graph.CompositionRoots.Count;
        var totalServiceRegistrations = graph.Services.Count;
        var compositionAsmdefs = assemblyScopeMappings.Count(kvp => kvp.Value.Count > 0);
        var serviceAsmdefs = assemblyServiceRegistrations.Count(kvp => kvp.Value.Count > 0);

        result.AddWarning($"Cross-asmdef scan: {compositionAsmdefs} composition asmdef(s), {serviceAsmdefs} service asmdef(s), {totalCompositionTargets} scope mapping(s), {totalServiceRegistrations} service registration(s).");

        return result;
    }

    private static DiContractGraph BuildGraph(
        IReadOnlyDictionary<string, List<CompositionTarget>> scopeMappings,
        IReadOnlyDictionary<string, List<ServiceRegistration>> serviceRegistrations) {
        var compositionRoots = scopeMappings.SelectMany(kvp =>
            kvp.Value.Select(mapping =>
                new DiCompositionRoot(
                    DiTypeIdentity.FromFullName(mapping.LifetimeScopeFullName, kvp.Key),
                    DiTypeIdentity.FromFullName(mapping.ScopeMarkerFullName),
                    new DiAssemblyProvenance(kvp.Key, kvp.Key, DiEvidenceSource.ProjectWideSmokeValidation, new[] { kvp.Key }))));

        var scopes = scopeMappings.SelectMany(kvp =>
            kvp.Value.Select(mapping =>
                new DiScopeIdentity(
                    DiTypeIdentity.FromFullName(mapping.ScopeMarkerFullName),
                    new DiAssemblyProvenance(kvp.Key, kvp.Key, DiEvidenceSource.ProjectWideSmokeValidation, new[] { kvp.Key }),
                    compositionRootType: DiTypeIdentity.FromFullName(mapping.LifetimeScopeFullName, kvp.Key))));

        var services = serviceRegistrations.SelectMany(kvp =>
            kvp.Value.Select(registration =>
                new DiServiceRegistration(
                    DiTypeIdentity.FromFullName(registration.ServiceFullName, kvp.Key),
                    Array.Empty<DiTypeIdentity>(),
                    string.Empty,
                    new DiAssemblyProvenance(kvp.Key, string.Empty, DiEvidenceSource.ProjectWideSmokeValidation, new[] { kvp.Key }),
                    DiTypeIdentity.FromFullName(registration.ScopeMarkerFullName))));

        return new DiContractGraph(scopes, services, compositionRoots);
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
