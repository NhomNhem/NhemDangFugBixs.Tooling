using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConflictCheckRule : DiagnosticAnalyzer {
    public const string ND005Id = "ND005";
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute";

    public static readonly DiagnosticDescriptor ND005 = new(
        ND005Id,
        "Registration Conflict Detected",
        "Conflict! Type '{0}' is already marked for auto-registration via [AutoRegisterIn]. Remove the manual registration in '{1}' or remove the attribute from '{0}' to resolve this ambiguity.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND005);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context) {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

        if (memberAccess == null) return;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken);
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol
            ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();

        if (methodSymbol == null || !IsVContainerRegistrationMethod(methodSymbol)) return;

        // Extract the generic type T from Register<T>
        var genericName = memberAccess.Name as GenericNameSyntax;
        if (genericName == null || genericName.TypeArgumentList.Arguments.Count != 1) return;

        var typeArg = genericName.TypeArgumentList.Arguments[0];
        var typeInfo = context.SemanticModel.GetTypeInfo(typeArg);
        var typeSymbol = typeInfo.Type;

        if (typeSymbol == null) return;

        // Check if the type has [AutoRegisterIn] attribute
        if (HasAutoRegisterAttribute(typeSymbol)) {
            var location = invocation.GetLocation();
            var containingClass = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>()?.Identifier.Text ?? "Unknown Scope";
            context.ReportDiagnostic(Diagnostic.Create(ND005, location, typeSymbol.Name, containingClass));
        }
    }

    private static bool IsVContainerRegistrationMethod(IMethodSymbol methodSymbol) {
        var name = methodSymbol.Name;

        if (name != "Register" &&
            name != "RegisterEntryPoint" &&
            name != "RegisterComponent" &&
            name != "RegisterComponentOnNewGameObject" &&
            name != "RegisterComponentInHierarchy" &&
            name != "RegisterFactory") {
            return false;
        }

        var containingType = methodSymbol.ContainingType;
        if (containingType == null) return false;

        var containingNamespace = containingType.ContainingNamespace?.ToDisplayString();
        return containingNamespace == "VContainer" ||
               containingNamespace?.StartsWith("VContainer.") == true;
    }

    private static bool HasAutoRegisterAttribute(ITypeSymbol typeSymbol) {
        return typeSymbol.GetAttributes().Any(ad => 
            ad.AttributeClass?.ToDisplayString() == AutoRegisterInAttributeName ||
            (ad.AttributeClass?.IsGenericType == true && ad.AttributeClass.OriginalDefinition.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute<TScope>"));
    }
}
