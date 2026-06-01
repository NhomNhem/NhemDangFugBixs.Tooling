namespace NhemDangFugBixs.Analyzers.Rules;

internal static class DiagnosticIds {
    public const string InvalidContract = "NHEM_DI_001";
    public const string InvalidScopeMarker = "NHEM_DI_002";
    public const string MissingExposureIntent = "NHEM_DI_003";
    public const string MissingScopeMapping = "NHEM_DI_010";
    public const string MissingGeneratedCall = "NHEM_DI_011";
    public const string WrongGeneratedCall = "NHEM_DI_012";
    public const string DuplicateGeneratedInvocation = "NHEM_DI_022";
    public const string CrossScopeDependency = "ND006";
    public const string InvalidEntryPoint = "NHEM_DI_040";
    public const string ResolverInjection = "NHEM_DI_050";
    public const string DuplicateContractExposure = "NHEM_DI_061";
    public const string RegisterComponentInHierarchyOnNonMonoBehaviour = "NHEM_DI_066";
    public const string EntryPointWithoutLifecycleInterface = "NHEM_DI_067";
}
