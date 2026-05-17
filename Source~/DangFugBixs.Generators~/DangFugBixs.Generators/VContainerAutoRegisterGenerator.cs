using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NhemDangFugBixs.Generators.Analyzers;
using NhemDangFugBixs.Generators.Emitters;
using NhemDangFugBixs.Generators.Utils;
using NhemDangFugBixs.Common.Models;
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
            
            // Check for required attribute types to verify assembly availability
            var attrCheck = input.Compilation.GetTypeByMetadataName("NhemDangFugBixs.Attributes.AutoRegisterInAttribute");
            if (attrCheck == null) {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.RuntimeMissing, Location.None));
                // We don't return here, we try to continue if it's just a resolution issue, 
                // but discovery will naturally find nothing.
            }

            // Initialize stats
            var packageVersion = typeof(VContainerAutoRegisterGenerator).Assembly.GetName().Version?.ToString() ?? "0.0.0";
            var stats = new GenerationStats {
                Version = packageVersion,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Use stable hint name that allows overwriting older v3.1 files if possible
            // We keep the dots but handle other invalid chars
            string sanitizedHint = new string(assemblyName.Select(c => char.IsLetterOrDigit(c) || c == '.' ? c : '_').ToArray());

            // v3.1 Logic: If we have ScopeMappings, we perform a global scan of referenced assemblies
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

            IEnumerable<ServiceInfo> discoveredServices = Enumerable.Empty<ServiceInfo>();
            if (scopeMappings.Any()) {
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
            }

            // Filter valid local services
            IEnumerable<ServiceInfo> validServices = input.LoggingData.Data.BaseData.Services;

            // Combine local and discovered services
            // Only include discovered services that are declared in THIS assembly. This prevents emitting services
            // from referenced assemblies into every compilation that references them, avoiding duplicated
            // registrations across generated files.
            var discoveredLocal = discoveredServices.Where(s => s.Metadata != null && s.Metadata.TryGetValue("DeclaringAssembly", out var asm) && asm == assemblyName);
            IEnumerable<ServiceInfo> allServices = validServices.Concat(discoveredLocal);

            // v4.1: Deduplication pass - ensure each unique class is registered only once globally
            // Prefer services declared in the current assembly when duplicates exist
            allServices = allServices
                .GroupBy(s => s.FullName)
                .Select(g => {
                    var list = g.ToList();
                    var local = list.FirstOrDefault(s => s.Metadata != null && s.Metadata.TryGetValue("DeclaringAssembly", out var asm) && asm == assemblyName);
                    if (!string.IsNullOrEmpty(local.ClassName)) return local;
                    return list.First();
                });

            var validatedServices = ValidateAndFilterServices(allServices, input.Compilation, context);
            stats.ServiceCount = validatedServices.Count;

            // Guard: only emit for allowed assemblies OR if we found services (opt-in via attribute)
            bool assemblyAllowed = AllowedAssemblies.Contains(assemblyName);
            if (!assemblyAllowed && validatedServices.Count == 0) return;

            var validSceneServices = input.LoggingData.Data.BaseData.SceneServices
                .Where(s => s.HasValue)
                .Select(s => s!.Value);

            if (validatedServices.Count == 0 && !validSceneServices.Any()) return;

            var sourceCode = RegistrationEmitter.GenerateSource(validatedServices, validSceneServices, assemblyName, scopeMappings, stats);
            var reportCode = ReportEmitter.GenerateSource(validatedServices, rootLogging, scopeMappings, assemblyName, packageVersion);

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
