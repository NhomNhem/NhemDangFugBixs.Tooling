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

    public static readonly DiagnosticDescriptor RuntimeMissing = new(
        id: "NDFG001",
        title: "NhemDangFugBixs Attributes Missing",
        messageFormat: "NhemDangFugBixs.Attributes assembly not found. Code generation degraded. " +
            "Fix: Ensure NhemDangFugBixs package is imported via Unity Package Manager from branch=deploy. " +
            "App code only needs 'using NhemDangFugBixs.Attributes;' - Runtime is optional.",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor VersionMismatch = new(
        id: "NDFG002",
        title: "Attribute Version Mismatch",
        messageFormat: "Assembly version mismatch detected for NhemDangFugBixs attributes. Expected {0}, found {1}.",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnsupportedAsmdefSetup = new(
        id: "NDFG003",
        title: "Unsupported Unity Asmdef Setup",
        messageFormat: "The generator encountered an unsupported Unity assembly setup: {0}",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor GeneratorInitializationFailed = new(
        id: "NDFG004",
        title: "Generator Initialization Failed",
        messageFormat: "NhemDangFugBixs generator failed to initialize: {0}. Code generation will be skipped.",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
