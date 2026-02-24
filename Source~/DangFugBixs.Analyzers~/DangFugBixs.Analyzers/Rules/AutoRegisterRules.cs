using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoRegisterRules : DiagnosticAnalyzer {
    public const string NHM001Id = "NHM001";
    public const string NHM002Id = "NHM002";
    public const string NHM003Id = "NHM003";

    private static readonly DiagnosticDescriptor NHM001 = new(
        NHM001Id,
        "Invalid AutoRegister target",
        "Class '{0}' must be non-static and non-abstract to use [AutoRegister]",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor NHM002 = new(
        NHM002Id,
        "Missing interface implementation",
        "Class '{0}' with [AutoRegister] should implement at least one interface for better DI practice",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor NHM003 = new(
        NHM003Id,
        "Invalid constructor for VContainer",
        "Class '{0}' should have exactly one public constructor or use [Inject]",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        => ImmutableArray.Create(NHM001, NHM002, NHM003);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        // Check for [AutoRegister] attribute
        var attributes = namedTypeSymbol.GetAttributes();
        var hasAutoRegister = attributes.Any(ad => ad.AttributeClass?.Name == "AutoRegisterAttribute");

        if (!hasAutoRegister) return;

        // NHM001: Non-static and Non-abstract
        if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract) {
            context.ReportDiagnostic(Diagnostic.Create(NHM001, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }

        // NHM002: Should implement interface or be a Component
        bool isComponent = false;
        var current = namedTypeSymbol;
        while (current != null) {
            string fullName = current.ToDisplayString();
            if (fullName == "UnityEngine.Component" || fullName == "UnityEngine.MonoBehaviour") {
                isComponent = true;
                break;
            }
            current = current.BaseType;
        }

        if (namedTypeSymbol.AllInterfaces.IsEmpty && !isComponent) {
            context.ReportDiagnostic(Diagnostic.Create(NHM002, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }

        // NHM003: Constructor check
        var publicCtors = namedTypeSymbol.InstanceConstructors
            .Where(c => c.DeclaredAccessibility == Accessibility.Public)
            .ToList();

        if (publicCtors.Count > 1 && !publicCtors.Any(c => c.GetAttributes().Any(ad => ad.AttributeClass?.Name == "InjectAttribute"))) {
             context.ReportDiagnostic(Diagnostic.Create(NHM003, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }
    }
}
