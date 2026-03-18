using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NhemDangFugBixs.Common.Utils;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ScopeReachabilityRule : DiagnosticAnalyzer {
    public const string ND006Id = "ND006";

    public static readonly DiagnosticDescriptor ND006 = new(
        ND006Id,
        "Inaccessible Dependency Scope",
        "Type '{0}' (in scope '{1}') depends on '{2}' (in scope '{3}'), but '{3}' is not reachable from '{1}'",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND006);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;
        
        // Get the scope of the current service
        var currentScope = SemanticScopeUtils.GetScopeSymbol(typeSymbol);
        if (currentScope == null) return;

        // Inspect constructor parameters
        foreach (var ctor in typeSymbol.InstanceConstructors) {
            foreach (var param in ctor.Parameters) {
                var dependencyType = param.Type;
                var dependencyScope = SemanticScopeUtils.GetScopeSymbol(dependencyType);

                if (dependencyScope != null) {
                    if (!SemanticScopeUtils.IsScopeReachable(currentScope, dependencyScope)) {
                        context.ReportDiagnostic(Diagnostic.Create(ND006, param.Locations[0], 
                            typeSymbol.Name, currentScope.Name, dependencyType.Name, dependencyScope.Name));
                    }
                }
            }
        }
    }
}
