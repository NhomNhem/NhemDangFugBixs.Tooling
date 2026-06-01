using Microsoft.CodeAnalysis;

namespace NhemDangFugBixs.Analyzers.Rules;

internal static class DiagnosticCatalog {
    public static readonly DiagnosticDescriptor InvalidContract = new(
        DiagnosticIds.InvalidContract,
        "Contract is not implemented",
        "Type '{0}' uses As contract '{1}' but does not implement it.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidScopeMarker = new(
        DiagnosticIds.InvalidScopeMarker,
        "Invalid scope marker",
        "Type '{0}' uses scope marker '{1}', but the marker does not implement IScopeMarker.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingExposureIntent = new(
        DiagnosticIds.MissingExposureIntent,
        "No contract exposure declared",
        "Type '{0}' is auto-registered but exposes neither As<TContract> nor AsSelf.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingScopeMapping = new(
        DiagnosticIds.MissingScopeMapping,
        "Missing LifetimeScope mapping",
        "Scope marker '{0}' has auto-registered services but no LifetimeScopeFor mapping exists.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingGeneratedCall = new(
        DiagnosticIds.MissingGeneratedCall,
        "LifetimeScope does not call generated registration",
        "LifetimeScope '{0}' maps '{1}' but Configure does not call builder.RegisterGeneratedFor<{1}>().",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor WrongGeneratedCall = new(
        DiagnosticIds.WrongGeneratedCall,
        "LifetimeScope calls wrong generated registration",
        "LifetimeScope '{0}' maps '{1}' but Configure calls RegisterGeneratedFor<{2}>().",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DuplicateGeneratedInvocation = new(
        DiagnosticIds.DuplicateGeneratedInvocation,
        "Generated registration invoked multiple times",
        "LifetimeScope '{0}' invokes generated registrations for scope '{1}' more than once.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CrossScopeDependency = new(
        DiagnosticIds.CrossScopeDependency,
        "Unreachable cross-scope dependency",
        "Service '{0}' in scope '{1}' depends on '{2}' in unreachable scope '{3}'.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidEntryPoint = new(
        DiagnosticIds.InvalidEntryPoint,
        "Invalid entry point type",
        "Type '{0}' uses [EntryPoint] but does not implement a supported VContainer lifecycle interface.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ResolverInjection = new(
        DiagnosticIds.ResolverInjection,
        "IObjectResolver injected into regular service",
        "Type '{0}' injects IObjectResolver. Prefer constructor injection of concrete dependencies or a factory/provider.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DuplicateContractExposure = new(
        DiagnosticIds.DuplicateContractExposure,
        "Duplicate explicit contract exposure",
        "Duplicate contract exposure. Remove duplicate [As] declaration for the same contract.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor RegisterComponentInHierarchyOnNonMonoBehaviour = new(
        DiagnosticIds.RegisterComponentInHierarchyOnNonMonoBehaviour,
        "RegisterComponentInHierarchy on non-MonoBehaviour",
        "RegisterComponentInHierarchy can only be used on MonoBehaviour types.",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EntryPointWithoutLifecycleInterface = new(
        DiagnosticIds.EntryPointWithoutLifecycleInterface,
        "EntryPoint without known lifecycle contract",
        "EntryPoint should implement a known lifecycle interface such as IStartable, ITickable, IInitializable, or IDisposable.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
