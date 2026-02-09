using Microsoft.CodeAnalysis;

namespace DangFugBixs.Generators;

internal static class Diagnostics {
    private const string Category = "DangFugBixs.Generator";

    public static readonly DiagnosticDescriptor InvalidAttributeType = new(
        id: "DANGFUG001",
        title: "Invalid AutoRegister attribute",
        messageFormat: "Attribute '{0}' is not the expected AutoRegister attribute type",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor InvalidLifetimeValue = new(
        id: "DANGFUG002",
        title: "Invalid lifetime value",
        messageFormat: "Lifetime value '{0}' is not valid. Expected Singleton, Transient, or Scoped",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor NonConstantScopeParameter = new(
        id: "DANGFUG003",
        title: "Non-constant scope parameter",
        messageFormat: "Scope parameter must be a constant string value",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor LifetimeArgumentRequired = new(
        id: "DANGFUG004",
        title: "Lifetime argument cannot be resolved",
        messageFormat: "Could not resolve lifetime argument. Using default: Singleton",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );
}
