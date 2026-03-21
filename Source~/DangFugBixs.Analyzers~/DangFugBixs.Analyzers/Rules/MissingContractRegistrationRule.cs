using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MissingContractRegistrationRule : DiagnosticAnalyzer {
    public const string ND111Id = "ND111";

    public static readonly DiagnosticDescriptor ND111 = new(
        ND111Id,
        "Missing contract registration",
        "Type '{0}' implements interface '{1}' but it will not be registered. " +
        "Fix: set AsImplementedInterfaces = true or add explicit AsTypes contracts. " +
        "Docs: https://docs.nhemdangfugbixs.com/diagnostics/ND111",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Detects when a type implements interfaces that are not being registered with VContainer, which may lead to resolution failures.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND111);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        // Check for [AutoRegisterIn] attribute
        var attr = typeSymbol.GetAttributes()
            .FirstOrDefault(ad => 
                ad.AttributeClass?.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute" ||
                (ad.AttributeClass?.IsGenericType == true && 
                 ad.AttributeClass.OriginalDefinition.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1"));

        if (attr == null) return;

        // Extract AsImplementedInterfaces setting
        var asImplementedInterfaces = true; // default
        var asTypesCount = 0;

        foreach (var namedArg in attr.NamedArguments) {
            if (namedArg.Key == "AsImplementedInterfaces" && namedArg.Value.Value is bool value) {
                asImplementedInterfaces = value;
            }
            else if (namedArg.Key == "AsTypes" && namedArg.Value.Values != null) {
                asTypesCount = namedArg.Value.Values.Length;
            }
        }

        // Get all interfaces except VContainer lifecycle interfaces
        var lifecycleInterfaces = new[] {
            "VContainer.Unity.ITickable",
            "VContainer.Unity.IInitializable",
            "VContainer.Unity.IStartable",
            "VContainer.Unity.IPostFixedUpdate",
            "VContainer.Unity.IPostLateUpdate",
            "VContainer.Unity.IPostStart",
            "VContainer.Unity.IFixedTickable",
            "VContainer.Unity.ILateTickable",
            "VContainer.Unity.IEntryPointExceptionHandler",
            "VContainer.Unity.IBuildCallback"
        };

        var implementedInterfaces = typeSymbol.AllInterfaces
            .Where(i => !lifecycleInterfaces.Contains(i.ToDisplayString()))
            .ToList();

        // Check if any interfaces won't be registered
        if (implementedInterfaces.Count > 0 && !asImplementedInterfaces && asTypesCount == 0) {
            // User has interfaces but disabled auto-detection without specifying explicit contracts
            foreach (var iface in implementedInterfaces.Take(3)) { // Report up to 3 interfaces
                var properties = ImmutableDictionary<string, string>.Empty
                    .Add("TypeName", typeSymbol.Name)
                    .Add("InterfaceName", iface.Name);

                context.ReportDiagnostic(Diagnostic.Create(
                    ND111,
                    typeSymbol.Locations[0],
                    properties,
                    typeSymbol.Name,
                    iface.Name));
            }
        }
    }
}
