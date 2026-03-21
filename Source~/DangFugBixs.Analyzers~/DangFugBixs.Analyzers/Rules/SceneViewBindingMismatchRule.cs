using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SceneViewBindingMismatchRule : DiagnosticAnalyzer {
    public const string ND113Id = "ND113";

    public static readonly DiagnosticDescriptor ND113 = new(
        ND113Id,
        "Scene view binding mismatch",
        "Presenter '{0}' depends on view interface '{1}' but no MonoBehaviour implementing it is registered in scope '{2}'. " +
        "Fix: add [AutoRegisterIn(typeof({2}), RegisterInHierarchy = true)] to the view or register manually in installer. " +
        "Docs: https://docs.nhemdangfugbixs.com/diagnostics/ND113",
        "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Detects when a Presenter class injects an interface but no MonoBehaviour implementing that interface is registered in the same scope.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND113);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private static void AnalyzeCompilation(CompilationAnalysisContext context) {
        // Step 1: Find all View interfaces implemented by MonoBehaviours
        var viewInterfaces = new HashSet<string>();
        var monoBehaviourViews = new Dictionary<string, List<(string Scope, string Registration)>>();

        foreach (var syntaxTree in context.Compilation.SyntaxTrees) {
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            foreach (var classDecl in root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()) {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                if (classSymbol == null) continue;

                // Check if it's a MonoBehaviour
                if (!IsMonoBehaviour(classSymbol)) continue;

                // Check if it has [AutoRegisterIn]
                var attr = classSymbol.GetAttributes()
                    .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute");

                if (attr == null) continue;

                // Get scope and interfaces
                var scopeName = GetScopeName(attr);
                if (string.IsNullOrEmpty(scopeName)) continue;

                foreach (var iface in classSymbol.AllInterfaces) {
                    var ifaceName = iface.Name;
                    if (!viewInterfaces.Contains(ifaceName)) {
                        viewInterfaces.Add(ifaceName);
                    }

                    if (!monoBehaviourViews.ContainsKey(ifaceName)) {
                        monoBehaviourViews[ifaceName] = new List<(string, string)>();
                    }
                    monoBehaviourViews[ifaceName].Add((scopeName, classSymbol.Name));
                }
            }
        }

        // Step 2: Find Presenters that inject view interfaces
        foreach (var syntaxTree in context.Compilation.SyntaxTrees) {
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            foreach (var classDecl in root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()) {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                if (classSymbol == null) continue;

                // Skip MonoBehaviours (they are views, not presenters)
                if (IsMonoBehaviour(classSymbol)) continue;

                // Check if it has [AutoRegisterIn]
                var attr = classSymbol.GetAttributes()
                    .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute");

                if (attr == null) continue;

                var presenterScope = GetScopeName(attr);
                if (string.IsNullOrEmpty(presenterScope)) continue;

                // Check constructor parameters for view interfaces
                foreach (var ctor in classSymbol.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)) {
                    foreach (var param in ctor.Parameters) {
                        if (param.Type is INamedTypeSymbol paramType && paramType.TypeKind == TypeKind.Interface) {
                            var ifaceName = paramType.Name;

                            // Check if this interface is implemented by any registered MonoBehaviour
                            if (viewInterfaces.Contains(ifaceName) && monoBehaviourViews.ContainsKey(ifaceName)) {
                                var registrations = monoBehaviourViews[ifaceName];
                                var isRegisteredInSameScope = registrations.Any(r => r.Scope == presenterScope);

                                if (!isRegisteredInSameScope) {
                                    var suggestedViewImplementation = registrations.FirstOrDefault().Item2 ?? string.Empty;
                                    var properties = ImmutableDictionary<string, string>.Empty
                                        .Add("PresenterName", classSymbol.Name)
                                        .Add("ViewInterface", ifaceName)
                                        .Add("ScopeName", presenterScope)
                                        .Add("ViewImplementation", suggestedViewImplementation);

                                    context.ReportDiagnostic(Diagnostic.Create(
                                        ND113,
                                        param.Locations[0],
                                        properties,
                                        classSymbol.Name,
                                        ifaceName,
                                        presenterScope));
                                }
                            }
                        }
                    }
                }
            }
        }
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

    private static string? GetScopeName(AttributeData attr) {
        if (attr.ConstructorArguments.Length > 0) {
            var arg = attr.ConstructorArguments[0];
            if (arg.Kind == TypedConstantKind.Type && arg.Value is INamedTypeSymbol scopeType) {
                return scopeType.Name;
            }
        }
        return null;
    }
}
