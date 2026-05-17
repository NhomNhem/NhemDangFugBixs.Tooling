using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ServiceLocatorAnalyzer : DiagnosticAnalyzer {
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticCatalog.ResolverInjection);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context) {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind != TypeKind.Class || FindAttribute(type, "AutoRegisterInAttribute") == null) {
            return;
        }

        if (IsAllowedResolverOwner(type) || InheritsLifetimeScope(type)) {
            return;
        }

        foreach (var ctor in type.InstanceConstructors.Where(ctor => ctor.DeclaredAccessibility == Accessibility.Public)) {
            foreach (var parameter in ctor.Parameters) {
                if (parameter.Type.Name == "IObjectResolver" || parameter.Type.ToDisplayString() == "VContainer.IObjectResolver") {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticCatalog.ResolverInjection,
                        parameter.Locations.FirstOrDefault(),
                        type.Name));
                }
            }
        }
    }

    private static bool IsAllowedResolverOwner(INamedTypeSymbol type) {
        return type.Name.Contains("Factory") ||
               type.Name.Contains("Spawner") ||
               type.Name.Contains("Bootstrapper");
    }

    private static bool InheritsLifetimeScope(INamedTypeSymbol type) {
        var current = type;
        while (current != null) {
            if (current.Name == "LifetimeScope" || current.ToDisplayString() == "VContainer.Unity.LifetimeScope") {
                return true;
            }
            current = current.BaseType;
        }

        return false;
    }

    private static AttributeData? FindAttribute(INamedTypeSymbol type, string attributeName) {
        return type.GetAttributes().FirstOrDefault(attr =>
            attr.AttributeClass?.Name == attributeName ||
            (attr.AttributeClass?.IsGenericType == true && attr.AttributeClass.OriginalDefinition.Name == attributeName.Replace("Attribute", "Attribute`1")));
    }
}
