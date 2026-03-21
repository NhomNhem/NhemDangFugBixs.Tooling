using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NhemDangFugBixs.Common.Utils;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ViewComponentRegistrationRule : DiagnosticAnalyzer {
    public const string ND110Id = "ND110";

    public static readonly DiagnosticDescriptor ND110 = new(
        ND110Id,
        "View interface injection requires Component registration",
        "Type '{0}' injects view interface '{1}' but the view implementation '{2}' is not registered as a Component. " +
        "Fix: use [AutoRegisterIn] with RegisterInHierarchy = true on the MonoBehaviour view or register with RegisterComponentInHierarchy(). " +
        "Docs: https://docs.nhemdangfugbixs.com/diagnostics/ND110",
        "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "MonoBehaviour views must be registered as Components in the scene hierarchy to be injectable.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND110);

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
        foreach (var ctor in typeSymbol.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)) {
            foreach (var parameter in ctor.Parameters) {
                var paramType = parameter.Type;

                // Check if parameter is a view interface (I*View pattern)
                if (!IsViewInterface(paramType)) continue;

                // Find implementations of this view interface
                var viewImplementations = FindViewImplementations(paramType, context.Compilation);

                foreach (var viewImpl in viewImplementations) {
                    // Check if view implementation is a MonoBehaviour
                    if (!IsMonoBehaviour(viewImpl)) {
                        continue;
                    }

                    // Check if view is registered in the same scope
                    var viewScope = SemanticScopeUtils.GetScopeSymbol(viewImpl);
                    
                    if (viewScope == null || !IsScopeReachable(currentScope, viewScope)) {
                        var properties = ImmutableDictionary.Create<string, string>()
                            .Add("TypeName", typeSymbol.Name)
                            .Add("ViewInterface", paramType.Name)
                            .Add("ViewImplementation", viewImpl.Name);

                        context.ReportDiagnostic(Diagnostic.Create(
                            ND110,
                            parameter.Locations[0],
                            properties,
                            typeSymbol.Name,
                            paramType.Name,
                            viewImpl.Name));
                    }
                }
            }
        }
    }

    private static bool IsViewInterface(ITypeSymbol typeSymbol) {
        if (typeSymbol.TypeKind != TypeKind.Interface) return false;
        if (!typeSymbol.Name.StartsWith("I", System.StringComparison.Ordinal)) return false;
        return typeSymbol.Name.EndsWith("View", System.StringComparison.Ordinal);
    }

    private static bool IsMonoBehaviour(INamedTypeSymbol typeSymbol) {
        var current = typeSymbol;
        while (current != null) {
            var fullName = current.ToDisplayString();
            if (fullName == "UnityEngine.MonoBehaviour") return true;
            current = current.BaseType;
        }
        return false;
    }

    private static IEnumerable<INamedTypeSymbol> FindViewImplementations(ITypeSymbol viewInterface, Compilation compilation) {
        var implementations = new List<INamedTypeSymbol>();
        
        foreach (var syntaxTree in compilation.SyntaxTrees) {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();
            
            foreach (var classDecl in root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()) {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                if (classSymbol == null) continue;

                if (classSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, viewInterface))) {
                    implementations.Add(classSymbol);
                }
            }
        }

        return implementations;
    }

    private static bool IsScopeReachable(INamedTypeSymbol currentScope, INamedTypeSymbol targetScope) {
        if (SymbolEqualityComparer.Default.Equals(currentScope, targetScope)) return true;
        
        // Parent scope is always reachable (VContainer hierarchy)
        var targetName = targetScope.Name;
        if (targetName is "LifetimeScope" or "ProjectLifetimeScope" or "GlobalLifetimeScope") return true;

        return false;
    }
}
