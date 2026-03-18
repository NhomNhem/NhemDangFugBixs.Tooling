using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EntryPointAsSelfRule : DiagnosticAnalyzer {
    public const string ND108Id = "ND108";

    public static readonly DiagnosticDescriptor ND108 = new(
        ND108Id,
        "EntryPoint must use AsSelf for concrete injection",
        "EntryPoint type '{0}' injects concrete dependencies but does not implement any interface. Add .AsSelf() or implement an interface.",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND108);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        // Check for [AutoRegisterIn] attribute with EntryPoint
        var attributes = typeSymbol.GetAttributes();
        var hasAutoRegisterIn = attributes.Any(ad => 
            ad.AttributeClass?.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute" ||
            (ad.AttributeClass?.IsGenericType == true && 
             ad.AttributeClass.OriginalDefinition.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1"));

        if (!hasAutoRegisterIn) return;

        // Check if type implements VContainer entry point interfaces
        var isEntryPoint = ImplementsVContainerEntryPoint(typeSymbol);

        if (!isEntryPoint) return;

        // Check if type has only concrete dependencies (no interfaces)
        var hasInterfaces = typeSymbol.AllInterfaces.Any();

        if (!hasInterfaces) {
            // Type is an EntryPoint but doesn't implement any interface
            // This is a warning - user should add .AsSelf() or implement interface
            context.ReportDiagnostic(Diagnostic.Create(
                ND108,
                typeSymbol.Locations[0],
                typeSymbol.Name));
        }
    }

    private static bool ImplementsVContainerEntryPoint(INamedTypeSymbol typeSymbol) {
        var entryPointInterfaces = new[] {
            "VContainer.Unity.ITickable",
            "VContainer.Unity.IInitializable",
            "VContainer.Unity.IStartable",
            "VContainer.Unity.IPostFixedUpdate",
            "VContainer.Unity.IPostLateUpdate",
            "VContainer.Unity.IPostStart",
            "VContainer.Unity.IFixedTickable",
            "VContainer.Unity.ILateTickable"
        };

        foreach (var iface in typeSymbol.AllInterfaces) {
            var fullName = iface.ToDisplayString();
            if (entryPointInterfaces.Contains(fullName)) {
                return true;
            }
        }

        return false;
    }
}
