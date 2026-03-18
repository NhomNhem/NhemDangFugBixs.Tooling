using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoRegisterRules : DiagnosticAnalyzer {
    public const string ND001Id = "ND001";
    public const string ND002Id = "ND002";
    public const string ND003Id = "ND003";
    public const string ND105Id = "ND105";
    public const string ND106Id = "ND106";
    public const string ND107Id = "ND107";

    private static readonly DiagnosticDescriptor ND001 = new(
        ND001Id,
        "Invalid AutoRegisterIn target",
        "Class '{0}' must be non-static and non-abstract to use [AutoRegisterIn]",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ND002 = new(
        ND002Id,
        "Missing interface implementation",
        "Class '{0}' with [AutoRegisterIn] should implement at least one interface or be a Component",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ND003 = new(
        ND003Id,
        "Invalid constructor for VContainer",
        "Class '{0}' should have exactly one public constructor or use [Inject]",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ND105 = new(
        ND105Id,
        "Installer missing parameterless constructor",
        "Installer class '{0}' must have a public parameterless constructor to be instantiated by the generator",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ND106 = new(
        ND106Id,
        "Installer must be public",
        "Installer class '{0}' must be public to be accessible by the generated registration code",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ND107 = new(
        ND107Id,
        "Installer cannot be a Component",
        "Installer class '{0}' cannot inherit from Component or MonoBehaviour",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(ND001, ND002, ND003, ND105, ND106, ND107, EntryPointAsSelfRule.ND108, ViewComponentRegistrationRule.ND110, MissingContractRegistrationRule.ND111, DuplicateContractRegistrationRule.ND112, ConflictCheckRule.ND005);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        
        // Add ConflictCheckRule initialization logic here if we want to combine them, 
        // but it's cleaner to have it as its own analyzer if configured correctly in the project.
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        // Check for [AutoRegisterIn] attribute
        var attributes = namedTypeSymbol.GetAttributes();
        var hasAutoRegister = attributes.Any(ad => ad.AttributeClass?.Name == "AutoRegisterInAttribute");

        if (!hasAutoRegister) return;

        bool isInstaller = namedTypeSymbol.AllInterfaces.Any(i => i.Name == "IVContainerInstaller");

        // ND001: Non-static and Non-abstract (unless it's an installer, which has its own checks)
        if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract) {
            context.ReportDiagnostic(Diagnostic.Create(ND001, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }

        // MonoBehaviour check
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

        // ND107: Installer cannot be a Component
        if (isInstaller && isComponent) {
            context.ReportDiagnostic(Diagnostic.Create(ND107, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }

        // ND106: Installer must be public
        if (isInstaller && namedTypeSymbol.DeclaredAccessibility != Accessibility.Public) {
            context.ReportDiagnostic(Diagnostic.Create(ND106, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }

        // ND105: Installer must have parameterless ctor
        if (isInstaller) {
            bool hasPublicParameterlessCtor = namedTypeSymbol.InstanceConstructors
                .Any(c => c.DeclaredAccessibility == Accessibility.Public && c.Parameters.IsEmpty);
            
            if (!hasPublicParameterlessCtor) {
                context.ReportDiagnostic(Diagnostic.Create(ND105, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
            }
        }

        // ND002: Should implement interface or be a Component (only for non-installers)
        if (!isInstaller && namedTypeSymbol.AllInterfaces.IsEmpty && !isComponent) {
            context.ReportDiagnostic(Diagnostic.Create(ND002, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
        }

        // ND003: Constructor check (only for non-installers)
        if (!isInstaller) {
            var publicCtors = namedTypeSymbol.InstanceConstructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                .ToList();

            if (publicCtors.Count > 1 && !publicCtors.Any(c => c.GetAttributes().Any(ad => ad.AttributeClass?.Name == "InjectAttribute"))) {
                 context.ReportDiagnostic(Diagnostic.Create(ND003, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
            }
        }
    }
}
