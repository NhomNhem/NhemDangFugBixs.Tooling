using Microsoft.CodeAnalysis;

namespace NhemDangFugBixs.Generators;

internal static class Diagnostics {
    public static readonly DiagnosticDescriptor InvalidContract = new(
        id: "NHEM_DI_001",
        title: "Contract is not implemented",
        messageFormat: "Type '{0}' uses As contract '{1}' but does not implement it. Generation skipped for this registration.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidScopeMarker = new(
        id: "NHEM_DI_002",
        title: "Invalid scope marker",
        messageFormat: "Type '{0}' uses scope marker '{1}', but the marker does not implement IScopeMarker. Generation skipped for this registration.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidEntryPoint = new(
        id: "NHEM_DI_040",
        title: "Invalid entry point type",
        messageFormat: "Type '{0}' uses [EntryPoint] but does not implement a supported VContainer lifecycle interface. Generation skipped for this registration.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

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

    public static readonly DiagnosticDescriptor DuplicateCompositionTarget = new(
        id: "NDFG005",
        title: "Duplicate composition target for scope",
        messageFormat: "Multiple LifetimeScope classes map to the same scope marker '{0}' in this assembly. Only one composition target per scope is supported.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DuplicateDiscoveredRegistration = new(
        id: "NDFG006",
        title: "Duplicate discovered registration",
        messageFormat: "Service '{0}' is discovered from multiple referenced assemblies. Consider deduplicating or using explicit contracts. Registration may be ambiguous.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingVContainerReference = new(
        id: "NDFG007",
        title: "Composition target missing VContainer reference",
        messageFormat: "Composition target assembly '{0}' cannot resolve required VContainer types (LifetimeScope or IContainerBuilder). Ensure the composition asmdef references VContainer.",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
