using System.Text;
using DangFugBixs.Generators.Analyzers;
using DangFugBixs.Generators.Emitters;
using DangFugBixs.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace DangFugBixs.Generators;

[Generator]
public class VContainerAutoRegisterGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // phase 1: Input Processing 
        // ussing classAnlyzer to make SyntaxNode variable raw to ServiceInfo clean

        var services = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: (ctx, token) => ClassAnalyzer.ExtractInfo(ctx, token)
                )
            .Where(info => info != null);
        
        // phase 2: Output Generation
        // collect all serviceInfo finded arrray and send it to Emitter
        context.RegisterSourceOutput(services.Collect(), Execute);
    }

    private static void Execute(SourceProductionContext context,
        System.Collections.Immutable.ImmutableArray<ServiceInfo?> serviceInfos) {
        if (serviceInfos.IsDefaultOrEmpty) return;
        
        var validServices = serviceInfos
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .ToList();
        
        if (validServices.Count == 0) return;
        
        var sourceCode = RegistrationEmitter.GenerateSource(validServices);
        
        // phase 3: Encapsulation
        context.AddSource("VContainerRegistration.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}