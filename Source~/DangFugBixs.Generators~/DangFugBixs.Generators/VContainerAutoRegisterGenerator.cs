using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NhemDangFugBixs.Generators.Analyzers;
using NhemDangFugBixs.Generators.Emitters;
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
        // phase 1: Input Processing 
        var services = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: (ctx, token) => ClassAnalyzer.ExtractInfo(ctx, token)
                )
            .Where(info => info != null);

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

        // phase 2: Output Generation - combine with compilation to get assembly name
        var combined = services.Collect()
            .Combine(sceneServices.Collect())
            .Combine(scopeMappings.Collect())
            .Combine(context.CompilationProvider);
        
        context.RegisterSourceOutput(combined, Execute);
    }

    private static void Execute(SourceProductionContext context,
        (((System.Collections.Immutable.ImmutableArray<ServiceInfo?> Services, System.Collections.Immutable.ImmutableArray<SceneInjectionInfo?> SceneServices) BaseData, System.Collections.Immutable.ImmutableArray<ScopeMappingInfo?> ScopeMappings) Data, Compilation Compilation) input) {

        try {
            var assemblyName = input.Compilation.AssemblyName ?? "";
            
            // Use stable hint name that allows overwriting older v3.1 files if possible
            // We keep the dots but handle other invalid chars
            string sanitizedHint = new string(assemblyName.Select(c => char.IsLetterOrDigit(c) || c == '.' ? c : '_').ToArray());

            // v3.1 Logic: If we have ScopeMappings, we perform a global scan of referenced assemblies
            var scopeMappings = input.Data.ScopeMappings
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();

            List<ServiceInfo> discoveredServices = new List<ServiceInfo>();
            if (scopeMappings.Count > 0) {
                discoveredServices = ReferencedAssemblyScanner.Scan(input.Compilation);
            }

            // Filter valid local services
            var validServices = input.Data.BaseData.Services
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();

            // Combine local and discovered services
            var allServices = validServices.Concat(discoveredServices).ToList();

            // Guard: only emit for allowed assemblies OR if we found services (opt-in via attribute)
            bool assemblyAllowed = AllowedAssemblies.Contains(assemblyName);
            if (!assemblyAllowed && allServices.Count == 0) return;

            var validSceneServices = input.Data.BaseData.SceneServices
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();

            if (allServices.Count == 0 && validSceneServices.Count == 0) return;

            var sourceCode = RegistrationEmitter.GenerateSource(allServices, validSceneServices, assemblyName, scopeMappings);

            // phase 3: Encapsulation
            // Generate ONE file per assembly containing everything including the global usings
            var hintName = string.IsNullOrEmpty(sanitizedHint) ? "VContainerRegistration.g.cs" : $"{sanitizedHint}.VContainerRegistration.g.cs";
            context.AddSource(hintName, SourceText.From(sourceCode, Encoding.UTF8));

        } catch (Exception ex) {
            var descriptor = new DiagnosticDescriptor("ND999", "Generator Error", $"VContainer generator failed: {ex.Message}", "Logic", DiagnosticSeverity.Error, true);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
        }
    }
}
