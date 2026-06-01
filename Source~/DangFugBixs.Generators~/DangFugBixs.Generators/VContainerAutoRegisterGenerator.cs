using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NhemDangFugBixs.Generators.Analyzers;
using NhemDangFugBixs.Generators.Emitters;
using NhemDangFugBixs.Generators.Utils;
using NhemDangFugBixs.Common.Models;
using NhemDangFugBixs.Common.Models.DiContractGraph;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NhemDangFugBixs.Generators;

[Generator]
public class VContainerAutoRegisterGenerator : IIncrementalGenerator {
    // Only emit code for these assemblies (Unity main + Sandbox test)
    private static readonly HashSet<string> AllowedAssemblies = new(StringComparer.OrdinalIgnoreCase) {
        "Assembly-CSharp",
        "DangFugBixs.Sandbox",
        "Shared",
        "Core",
        "Services",
        "Gameplay",
        "Data",
        "Runtime",
        "GameFeel_Shared",
        "GameFeel_Core",
        "GameFeel_Services",
        "GameFeel_Gameplay",
        "GameFeel_Data",
        "GameFeel_Runtime"
    };

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        try {
            InitializeCore(context);
        } catch {
            // Gracefully degrade: if initialization fails, generator silently does nothing
            // This prevents hard failures when dependencies can't be loaded
        }
    }

    private void InitializeCore(IncrementalGeneratorInitializationContext context) {
        // phase 1: Input Processing 
        var services = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } and (ClassDeclarationSyntax or StructDeclarationSyntax),
                transform: (ctx, token) => ClassAnalyzer.ExtractInfos(ctx, token)
                )
            .SelectMany((infos, _) => infos);

        var sceneServices = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: (ctx, token) => ClassAnalyzer.ExtractSceneInfo(ctx, token)
                )
            .Where(info => info != null);

        var scopeMappings = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: (ctx, token) => ClassAnalyzer.ExtractScopeMapping(ctx, token)
                )
            .Where(info => info != null);

        var rootLogging = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, token) => ClassAnalyzer.ExtractRootLoggingInfo(ctx, token)
                )
            .Where(info => info != null);

        // phase 2: Output Generation - combine with compilation to get assembly name
        var combined = services.Collect()
            .Combine(sceneServices.Collect())
            .Combine(scopeMappings.Collect())
            .Combine(rootLogging.Collect())
            .Combine(context.CompilationProvider);
        
        context.RegisterSourceOutput(combined, (spc, input) => Execute(spc, input));
    }

    private static void Execute(SourceProductionContext context,
        ((((System.Collections.Immutable.ImmutableArray<ServiceInfo> Services, System.Collections.Immutable.ImmutableArray<SceneInjectionInfo?> SceneServices) BaseData, System.Collections.Immutable.ImmutableArray<ScopeMappingInfo?> ScopeMappings) Data, System.Collections.Immutable.ImmutableArray<RootLoggingInfo?> RootLogging) LoggingData, Compilation Compilation) input) {

        try {
            var assemblyName = input.Compilation.AssemblyName ?? "";
            
            // Incremental generator pattern: predicate filters to syntax nodes with attributes,
            // transform performs lightweight semantic analysis only for candidate nodes.
            // Semantic analysis is avoided for non-candidate syntax nodes by the predicate filter.
            // See design.md: "Generator path remains incremental and bounded to candidate nodes."

            // Check for required attribute types to verify assembly availability
            var attrCheck = input.Compilation.GetTypeByMetadataName("NhemDangFugBixs.Attributes.AutoRegisterInAttribute");
            if (attrCheck == null) {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.RuntimeMissing, Location.None));
                // We don't return here, we try to continue if it's just a resolution issue, 
                // but discovery will naturally find nothing.
            }

            // NOTE: Deferred validations (pushed to di-smoke because Roslyn per-compilation
            // analysis cannot reliably prove them across assembly boundaries):
            // - Duplicate composition targets across separate Unity asmdefs (Task 6.2)
            // - Missing intended composition roots across a whole Unity project (Task 6.3)
            // - Drift between composition targets and referenced service assemblies beyond
            //   one compilation graph (Task 6.3)
            // These require project-wide or multi-assembly analysis and are documented in
            // the di-smoke validation layer.

            // Initialize stats
            var packageVersion = typeof(VContainerAutoRegisterGenerator).Assembly.GetName().Version?.ToString() ?? "0.0.0";
            var stats = new GenerationStats {
                Version = packageVersion,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Use stable hint name that allows overwriting older v3.1 files if possible
            // We keep the dots but handle other invalid chars
            string sanitizedHint = new string(assemblyName.Select(c => char.IsLetterOrDigit(c) || c == '.' ? c : '_').ToArray());

            // Composition-only generation:
            // - service-only assemblies still get diagnostics
            // - VContainer installers/extensions are emitted only when local LifetimeScopeFor mappings exist
            var scopeMappings = input.LoggingData.Data.ScopeMappings
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();
            var rootLogging = input.LoggingData.RootLogging
                .Where(info => info.HasValue)
                .Select(info => info!.Value)
                .GroupBy(info => info.ScopeName)
                .Select(group => new RootLoggingInfo(
                    group.Key,
                    group.Any(i => i.HasLoggerFactory),
                    group.Any(i => i.HasLoggerAdapter)));

            // Always validate local services so service-only assemblies still report local diagnostics.
            var localServices = input.LoggingData.Data.BaseData.Services
                .Select(s => new ServiceInfo(
                    s.Namespace, s.ClassName, s.Lifetime, s.ScopeName,
                    s.InterfaceNames, s.IsComponent, s.AsImplementedInterfaces, s.AsSelf,
                    s.RegisterInHierarchy, s.AsTypes, s.IsEntryPoint, s.IsFactory,
                    s.ScopeTypeName, s.UsesTypeSafeScope, s.IsExceptionHandler, s.IsBuildCallback,
                    s.IsInstaller, s.InstallerOrder, s.IsMessagePipeBroker, s.MessageType, s.MessagePipeKind,
                    s.Metadata, isFromCurrentCompilation: true))
                .ToList();
            var validatedLocalServices = ValidateAndFilterServices(localServices, input.Compilation, context);

            if (!scopeMappings.Any()) {
                return;
            }

            // Task 4.3: Detect duplicate local composition targets for the same scope
            var duplicateScopes = scopeMappings
                .GroupBy(m => m.IdentityTypeName, StringComparer.Ordinal)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            foreach (var dupScope in duplicateScopes) {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.DuplicateCompositionTarget, Location.None, dupScope));
            }

            // Task 4.5: Verify VContainer symbols are visible in composition targets
            bool hasLifetimeScope = input.Compilation.GetTypeByMetadataName("VContainer.Unity.LifetimeScope") != null ||
                                    input.Compilation.GetTypeByMetadataName("LifetimeScope") != null;
            bool hasContainerBuilder = input.Compilation.GetTypeByMetadataName("VContainer.IContainerBuilder") != null ||
                                       input.Compilation.GetTypeByMetadataName("IContainerBuilder") != null;
            if (!hasLifetimeScope || !hasContainerBuilder) {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.MissingVContainerReference, Location.None, assemblyName));
            }

            IEnumerable<ServiceInfo> discoveredServices = Enumerable.Empty<ServiceInfo>();
            var scanResult = ReferencedAssemblyScanner.Scan(input.Compilation);
            discoveredServices = scanResult.Services ?? Enumerable.Empty<ServiceInfo>();

            // Report warnings as diagnostics
            foreach (var warning in scanResult.Warnings) {
                stats.Warnings.Add(warning);
                var parts = warning.Split(new[] { ':' }, 2);
                string asmName = parts.Length > 0 ? parts[0].Trim() : "Unknown";
                string msg = parts.Length > 1 ? parts[1].Trim() : warning;
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnresolvedAssemblyScan, Location.None, asmName, msg));
            }

            // Combine local and directly referenced services.
            IEnumerable<ServiceInfo> allServices = validatedLocalServices.Concat(discoveredServices);

            // Task 4.4: Detect duplicate discovered registrations from different assemblies (before deduplication)
            var duplicateDiscovered = allServices
                .GroupBy(s => s.FullName)
                .Where(g => {
                    var list = g.ToList();
                    var distinctAssemblies = list
                        .Select(s => s.Metadata.TryGetValue("DeclaringAssembly", out var asm) ? asm : null)
                        .Where(a => !string.IsNullOrEmpty(a))
                        .Distinct(StringComparer.Ordinal)
                        .Count();
                    return distinctAssemblies > 1;
                })
                .Select(g => g.Key)
                .ToList();
            foreach (var dup in duplicateDiscovered) {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.DuplicateDiscoveredRegistration, Location.None, dup));
            }

            // Deduplicate by implementation identity. Prefer services declared in the current assembly when duplicates exist.
            allServices = allServices
                .GroupBy(s => s.FullName)
                .Select(g => {
                    var list = g.ToList();
                    var local = list.FirstOrDefault(s => s.IsFromCurrentCompilation);
                    if (!string.IsNullOrEmpty(local.ClassName)) return local;
                    return list.First();
                });

            var validatedServices = ValidateAndFilterServices(allServices, input.Compilation, context);
            stats.ServiceCount = validatedServices.Count;
            var contractGraph = DiContractGraphFactory.FromLegacy(assemblyName, validatedServices, scopeMappings);

            // Guard: only emit for allowed assemblies OR if we found services (opt-in via attribute)
            bool assemblyAllowed = AllowedAssemblies.Contains(assemblyName);
            if (!assemblyAllowed && validatedServices.Count == 0 && !scopeMappings.Any()) return;

            var validSceneServices = input.LoggingData.Data.BaseData.SceneServices
                .Where(s => s.HasValue)
                .Select(s => s!.Value);

            _ = contractGraph;
            var sourceCode = RegistrationEmitter.GenerateSource(validatedServices, validSceneServices, assemblyName, scopeMappings, stats);
            var reportCode = ReportEmitter.GenerateSource(validatedServices, rootLogging, scopeMappings, assemblyName, packageVersion, contractGraph);

            // phase 3: Encapsulation
            // Generate ONE file per assembly containing everything including the global usings
            // v3.3: Use stable hint name {sanitizedHint}.g.cs to overwrite older versions correctly
            var hintName = string.IsNullOrEmpty(sanitizedHint) ? "VContainerRegistration.g.cs" : $"{sanitizedHint}.g.cs";
            context.AddSource(hintName, SourceText.From(sourceCode, Encoding.UTF8));

            var reportHintName = string.IsNullOrEmpty(sanitizedHint) ? "VContainerRegistration.Report.g.cs" : $"{sanitizedHint}.Report.g.cs";
            context.AddSource(reportHintName, SourceText.From(reportCode, Encoding.UTF8));

        } catch (Exception ex) {
            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.GeneratorError, Location.None, ex.Message));
        }
    }

    private static List<ServiceInfo> ValidateAndFilterServices(
        IEnumerable<ServiceInfo> services,
        Compilation compilation,
        SourceProductionContext context) {
        var validated = new List<ServiceInfo>();

        foreach (var service in services) {
            if (service.IsExceptionHandler || service.IsBuildCallback) {
                continue;
            }

            if (!ValidateScopeMarker(service, compilation, context)) {
                continue;
            }

            if (!ValidateContracts(service, compilation, context)) {
                continue;
            }

            if (!ValidateExplicitEntryPoint(service, context)) {
                continue;
            }

            validated.Add(service);
        }

        return validated;
    }

    private static bool ValidateScopeMarker(ServiceInfo service, Compilation compilation, SourceProductionContext context) {
        if (!service.UsesTypeSafeScope || string.IsNullOrWhiteSpace(service.ScopeTypeName)) {
            return true;
        }

        var markerSymbol = compilation.GetTypeByMetadataName(service.ScopeTypeName);
        if (markerSymbol == null || IsValidScopeTarget(markerSymbol, compilation)) {
            return true;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.InvalidScopeMarker,
            Location.None,
            service.ClassName,
            service.ScopeTypeName));
        return false;
    }

    private static bool ValidateContracts(ServiceInfo service, Compilation compilation, SourceProductionContext context) {
        if (service.AsTypes == null || service.AsTypes.Length == 0) {
            return true;
        }

        var implementationSymbol = compilation.GetTypeByMetadataName(service.FullName);
        if (implementationSymbol == null) {
            return true;
        }

        foreach (var contractName in service.AsTypes) {
            if (string.IsNullOrWhiteSpace(contractName)) {
                continue;
            }

            var contractSymbol = compilation.GetTypeByMetadataName(contractName);
            if (contractSymbol == null || !ImplementsContract(implementationSymbol, contractSymbol)) {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.InvalidContract,
                    Location.None,
                    service.ClassName,
                    contractName));
                return false;
            }
        }

        return true;
    }

    private static bool ValidateExplicitEntryPoint(ServiceInfo service, SourceProductionContext context) {
        if (!service.Metadata.TryGetValue("ExplicitEntryPoint", out var explicitEntryPoint) ||
            !string.Equals(explicitEntryPoint, "true", StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if (service.InterfaceNames.Any(InterfaceUtils.IsVContainerEntryPoint)) {
            return true;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.InvalidEntryPoint,
            Location.None,
            service.ClassName));
        return false;
    }

    private static bool ImplementsContract(INamedTypeSymbol implementation, ITypeSymbol contract) {
        if (SymbolEqualityComparer.Default.Equals(implementation, contract)) {
            return true;
        }

        if (implementation.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, contract))) {
            return true;
        }

        var current = implementation.BaseType;
        while (current != null) {
            if (SymbolEqualityComparer.Default.Equals(current, contract)) {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static bool IsValidScopeTarget(INamedTypeSymbol marker, Compilation compilation) {
        if (IsLifetimeScopeType(marker)) {
            return true;
        }

        if (marker.TypeKind == TypeKind.Interface && !CompilationDeclaresScopeMarkerContract(compilation)) {
            return true;
        }

        return IsValidScopeMarker(marker);
    }

    private static bool IsValidScopeMarker(INamedTypeSymbol marker) {
        if (marker.TypeKind != TypeKind.Interface) {
            return false;
        }

        if (marker.Name == "IScopeMarker") {
            return true;
        }

        return marker.AllInterfaces.Any(i => i.Name == "IScopeMarker" || i.ToDisplayString().EndsWith(".IScopeMarker", StringComparison.Ordinal));
    }

    private static bool IsLifetimeScopeType(INamedTypeSymbol type) {
        var current = type;
        while (current != null) {
            if (current.Name == "LifetimeScope" || current.ToDisplayString() == "VContainer.Unity.LifetimeScope") {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static bool CompilationDeclaresScopeMarkerContract(Compilation compilation) {
        return ContainsTypeNamed(compilation.Assembly.GlobalNamespace, "IScopeMarker");
    }

    private static bool ContainsTypeNamed(INamespaceOrTypeSymbol symbol, string typeName) {
        if (symbol is INamedTypeSymbol namedType) {
            if (namedType.Name == typeName) {
                return true;
            }

            foreach (var member in namedType.GetTypeMembers()) {
                if (ContainsTypeNamed(member, typeName)) {
                    return true;
                }
            }
        }

        foreach (var member in symbol.GetMembers()) {
            if (member is INamespaceOrTypeSymbol nested && ContainsTypeNamed(nested, typeName)) {
                return true;
            }
        }

        return false;
    }
}
