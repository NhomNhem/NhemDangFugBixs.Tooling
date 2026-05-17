using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeContractAnalyzer : DiagnosticAnalyzer {
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticCatalog.InvalidContract,
        DiagnosticCatalog.InvalidScopeMarker,
        DiagnosticCatalog.MissingExposureIntent,
        DiagnosticCatalog.InvalidEntryPoint);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context) {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind != TypeKind.Class) {
            return;
        }

        var autoRegister = FindAttribute(type, "AutoRegisterInAttribute");
        if (autoRegister == null) {
            return;
        }

        var scopeMarker = TryGetMarkerType(autoRegister);
        if (scopeMarker != null && !IsValidScopeMarker(scopeMarker)) {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticCatalog.InvalidScopeMarker,
                type.Locations.FirstOrDefault(),
                type.Name,
                scopeMarker.ToDisplayString()));
        }

        foreach (var contract in GetExplicitContracts(type)) {
            if (!ImplementsContract(type, contract)) {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticCatalog.InvalidContract,
                    type.Locations.FirstOrDefault(),
                    type.Name,
                    contract.ToDisplayString()));
            }
        }

        if (ShouldWarnMissingExposure(type, autoRegister)) {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticCatalog.MissingExposureIntent,
                type.Locations.FirstOrDefault(),
                type.Name));
        }

        if (HasAttribute(type, "EntryPointAttribute") && !ImplementsLifecycle(type)) {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticCatalog.InvalidEntryPoint,
                type.Locations.FirstOrDefault(),
                type.Name));
        }
    }

    private static bool ShouldWarnMissingExposure(INamedTypeSymbol type, AttributeData autoRegister) {
        if (GetExplicitContracts(type).Count > 0) {
            return false;
        }

        if (HasAttribute(type, "AsSelfAttribute")) {
            return false;
        }

        bool asSelf = TryGetBoolNamedArgument(autoRegister, "AsSelf") ?? true;
        bool asImplementedInterfaces = TryGetBoolNamedArgument(autoRegister, "AsImplementedInterfaces") ?? true;
        return !asSelf && !asImplementedInterfaces;
    }

    private static bool ImplementsLifecycle(INamedTypeSymbol type) {
        var lifecycleNames = new HashSet<string> {
            "VContainer.Unity.IInitializable",
            "VContainer.Unity.IStartable",
            "VContainer.Unity.ITickable",
            "VContainer.Unity.IFixedTickable",
            "VContainer.Unity.ILateTickable",
            "System.IDisposable"
        };

        return type.AllInterfaces.Any(i =>
            lifecycleNames.Contains(i.ToDisplayString()) ||
            lifecycleNames.Contains($"VContainer.Unity.{i.Name}") ||
            lifecycleNames.Contains(i.Name));
    }

    private static bool ImplementsContract(INamedTypeSymbol type, ITypeSymbol contract) {
        return SymbolEqualityComparer.Default.Equals(type, contract) ||
               type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, contract));
    }

    private static List<ITypeSymbol> GetExplicitContracts(INamedTypeSymbol type) {
        var contracts = new List<ITypeSymbol>();
        foreach (var attr in type.GetAttributes()) {
            if (!IsAttribute(attr, "AsAttribute")) {
                continue;
            }

            if (attr.AttributeClass?.IsGenericType == true && attr.AttributeClass.TypeArguments.Length > 0) {
                contracts.Add(attr.AttributeClass.TypeArguments[0]);
                continue;
            }

            if (attr.ConstructorArguments.Length > 0 &&
                attr.ConstructorArguments[0].Kind == TypedConstantKind.Type &&
                attr.ConstructorArguments[0].Value is ITypeSymbol typeSymbol) {
                contracts.Add(typeSymbol);
            }
        }

        return contracts.Distinct<ITypeSymbol>(SymbolEqualityComparer.Default).ToList();
    }

    private static bool IsValidScopeMarker(ITypeSymbol marker) {
        if (marker.TypeKind != TypeKind.Interface) {
            return false;
        }

        if (marker.Name == "IScopeMarker") {
            return true;
        }

        return marker.AllInterfaces.Any(i => i.Name == "IScopeMarker" || i.ToDisplayString().EndsWith(".IScopeMarker"));
    }

    private static bool HasAttribute(INamedTypeSymbol type, string attributeName) {
        return FindAttribute(type, attributeName) != null;
    }

    private static AttributeData? FindAttribute(INamedTypeSymbol type, string attributeName) {
        return type.GetAttributes().FirstOrDefault(attr => IsAttribute(attr, attributeName));
    }

    private static bool IsAttribute(AttributeData attr, string attributeName) {
        if (attr.AttributeClass == null) {
            return false;
        }

        return attr.AttributeClass.Name == attributeName ||
               (attr.AttributeClass.IsGenericType && attr.AttributeClass.OriginalDefinition.Name == attributeName.Replace("Attribute", "Attribute`1"));
    }

    private static ITypeSymbol? TryGetMarkerType(AttributeData attr) {
        if (attr.AttributeClass?.IsGenericType == true && attr.AttributeClass.TypeArguments.Length > 0) {
            return attr.AttributeClass.TypeArguments[0];
        }

        if (attr.ConstructorArguments.Length > 0 &&
            attr.ConstructorArguments[0].Kind == TypedConstantKind.Type &&
            attr.ConstructorArguments[0].Value is ITypeSymbol typeSymbol) {
            return typeSymbol;
        }

        return null;
    }

    private static bool? TryGetBoolNamedArgument(AttributeData attr, string argumentName) {
        foreach (var pair in attr.NamedArguments) {
            if (pair.Key == argumentName && pair.Value.Value is bool value) {
                return value;
            }
        }

        return null;
    }
}
