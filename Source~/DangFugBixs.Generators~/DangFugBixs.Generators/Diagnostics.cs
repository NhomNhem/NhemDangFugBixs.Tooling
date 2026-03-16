using Microsoft.CodeAnalysis;

namespace NhemDangFugBixs.Generators;

internal static class Diagnostics {
    public static readonly DiagnosticDescriptor GeneratorError = new(
        id: "ND999",
        title: "Generator Error",
        messageFormat: "VContainer generator failed: {0}",
        category: "Logic",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnresolvedAssemblyScan = new(
        id: "ND104",
        title: "Unresolved Assembly Scan",
        messageFormat: "Could not scan referenced assembly '{0}': {1}",
        category: "Resiliency",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
