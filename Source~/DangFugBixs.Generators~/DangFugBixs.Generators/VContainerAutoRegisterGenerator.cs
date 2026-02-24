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

        // phase 2: Output Generation
        var combined = services.Collect().Combine(sceneServices.Collect());
        context.RegisterSourceOutput(combined, Execute);
    }

    private static void Execute(SourceProductionContext context,
        (System.Collections.Immutable.ImmutableArray<ServiceInfo?> Services, System.Collections.Immutable.ImmutableArray<SceneInjectionInfo?> SceneServices) input) {
        
        var validServices = input.Services
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();
            
        var validSceneServices = input.SceneServices
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();
        
        if (validServices.Count == 0 && validSceneServices.Count == 0) return;
        
        var sourceCode = RegistrationEmitter.GenerateSource(validServices, validSceneServices);
        
        // phase 3: Encapsulation
        context.AddSource("VContainerRegistration.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}