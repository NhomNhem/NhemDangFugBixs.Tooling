using System;
using System.Collections.Generic;
using System.Text;
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
        "DangFugBixs.Sandbox"
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

        var sceneRegistrations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: (ctx, token) => ClassAnalyzer.ExtractSceneRegistrationInfo(ctx, token)
                )
            .Where(info => info != null);

        // phase 2: Output Generation - combine with compilation to get assembly name
        var combined = services.Collect()
            .Combine(sceneServices.Collect())
            .Combine(sceneRegistrations.Collect())
            .Combine(context.CompilationProvider);
        
        context.RegisterSourceOutput(combined, Execute);
    }

    private static void Execute(SourceProductionContext context,
        (((System.Collections.Immutable.ImmutableArray<ServiceInfo?> Services, System.Collections.Immutable.ImmutableArray<SceneInjectionInfo?> SceneServices) Inner, System.Collections.Immutable.ImmutableArray<SceneRegistrationInfo?> SceneRegistrations) Data, Compilation Compilation) input) {
        
        // Guard: only emit for allowed assemblies
        var assemblyName = input.Compilation.AssemblyName ?? "";
        if (!AllowedAssemblies.Contains(assemblyName)) return;
        
        var validServices = input.Data.Inner.Services
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();
            
        var validSceneServices = input.Data.Inner.SceneServices
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();

        var validSceneRegistrations = input.Data.SceneRegistrations
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();
        
        if (validServices.Count == 0 && validSceneServices.Count == 0 && validSceneRegistrations.Count == 0) return;
        
        var sourceCode = RegistrationEmitter.GenerateSource(validServices, validSceneServices, validSceneRegistrations);
        
        // phase 3: Encapsulation
        context.AddSource("VContainerRegistration.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}
