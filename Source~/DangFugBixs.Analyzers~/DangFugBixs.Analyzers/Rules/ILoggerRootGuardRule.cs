using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NhemDangFugBixs.Common.Utils;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ILoggerRootGuardRule : DiagnosticAnalyzer {
    public const string ND009Id = "ND009";

    public static readonly DiagnosticDescriptor ND009 = new(
        ND009Id,
        "Missing ILogger root infrastructure",
        "Type '{0}' in scope '{1}' depends on ILogger<{2}>, but no reachable root logging setup exists. " +
        "Fix: register ILoggerFactory and ILogger<> in root scope or use [AutoRegisterRootLogging]. " +
        "Docs: https://docs.nhemdangfugbixs.com/diagnostics/ND009",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "ILogger dependencies require ILoggerFactory and generic ILogger<> registrations in a root scope accessible to all consumers.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND009);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilationStart);
    }

    private static void RegisterCompilationStart(CompilationStartAnalysisContext context) {
        var rootLogging = CollectRootLogging(context.Compilation, context.Compilation.Assembly.GlobalNamespace);
        context.RegisterSymbolAction(symbolContext => AnalyzeNamedType(symbolContext, rootLogging), SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context, IReadOnlyList<RootLoggerRegistration> rootLogging) {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;
        var serviceScope = SemanticScopeUtils.GetScopeSymbol(typeSymbol);
        if (serviceScope == null) {
            return;
        }

        foreach (var ctor in typeSymbol.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)) {
            foreach (var parameter in ctor.Parameters) {
                if (!SemanticScopeUtils.TryGetLoggerDependency(parameter.Type, out var categoryType) || categoryType == null) {
                    continue;
                }

                var hasReachableRoot = rootLogging.Any(root =>
                    root.HasLoggerFactory &&
                    root.HasLoggerAdapter &&
                    SemanticScopeUtils.IsScopeReachable(serviceScope, root.Scope));

                if (!hasReachableRoot && parameter.Locations.Length > 0) {
                    var properties = ImmutableDictionary.Create<string, string>()
                        .Add("TypeName", typeSymbol.Name)
                        .Add("ScopeName", serviceScope.Name)
                        .Add("CategoryType", categoryType.ToDisplayString());

                    context.ReportDiagnostic(Diagnostic.Create(
                        ND009,
                        parameter.Locations[0],
                        properties,
                        typeSymbol.Name,
                        serviceScope.Name,
                        categoryType.ToDisplayString()));
                }
            }
        }
    }

    private static List<RootLoggerRegistration> CollectRootLogging(Compilation compilation, INamespaceSymbol namespaceSymbol) {
        var results = new List<RootLoggerRegistration>();

        foreach (var member in namespaceSymbol.GetMembers()) {
            if (member is INamespaceSymbol childNamespace) {
                results.AddRange(CollectRootLogging(compilation, childNamespace));
                continue;
            }

            if (member is not INamedTypeSymbol typeSymbol ||
                !InheritsLifetimeScope(typeSymbol) ||
                !SemanticScopeUtils.IsRootScopeName(typeSymbol.Name)) {
                continue;
            }

            var hasLoggerFactory = false;
            var hasLoggerAdapter = false;

            foreach (var syntaxRef in typeSymbol.DeclaringSyntaxReferences) {
                if (syntaxRef.GetSyntax() is not ClassDeclarationSyntax classDecl) {
                    continue;
                }

                foreach (var method in classDecl.Members.OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.Text == "Configure")) {
                    var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
                    foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                        foreach (var referencedType in GetReferencedTypes(semanticModel, invocation)) {
                            if (!hasLoggerFactory && SemanticScopeUtils.IsLoggerFactoryType(referencedType)) {
                                hasLoggerFactory = true;
                            }

                            if (!hasLoggerAdapter && SemanticScopeUtils.IsLoggerAdapterType(referencedType)) {
                                hasLoggerAdapter = true;
                            }
                        }
                    }
                }
            }

            results.Add(new RootLoggerRegistration(typeSymbol, hasLoggerFactory, hasLoggerAdapter));
        }

        return results;
    }

    private static bool InheritsLifetimeScope(INamedTypeSymbol typeSymbol) {
        var current = typeSymbol.BaseType;
        while (current != null) {
            if (current.Name == "LifetimeScope" || current.ToDisplayString() == "VContainer.Unity.LifetimeScope") {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static IEnumerable<ITypeSymbol> GetReferencedTypes(SemanticModel semanticModel, InvocationExpressionSyntax invocation) {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name is GenericNameSyntax genericName) {
            foreach (var typeArg in genericName.TypeArgumentList.Arguments) {
                var type = semanticModel.GetTypeInfo(typeArg).Type;
                if (type != null) {
                    yield return type;
                }
            }
        }

        foreach (var argument in invocation.ArgumentList.Arguments) {
            if (argument.Expression is TypeOfExpressionSyntax typeOfExpr) {
                var type = semanticModel.GetTypeInfo(typeOfExpr.Type).Type;
                if (type != null) {
                    yield return type;
                }
            }
        }
    }

    private readonly struct RootLoggerRegistration {
        public INamedTypeSymbol Scope { get; }
        public bool HasLoggerFactory { get; }
        public bool HasLoggerAdapter { get; }

        public RootLoggerRegistration(INamedTypeSymbol scope, bool hasLoggerFactory, bool hasLoggerAdapter) {
            Scope = scope;
            HasLoggerFactory = hasLoggerFactory;
            HasLoggerAdapter = hasLoggerAdapter;
        }
    }
}
