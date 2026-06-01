namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiManualRegistration {
    public DiTypeIdentity ImplementationType { get; }
    public DiTypeIdentity? ContractType { get; }
    public DiTypeIdentity? ScopeMarkerType { get; }
    public string ApiName { get; }
    public string Location { get; }
    public bool IsGeneratedRoute { get; }
    public DiAssemblyProvenance Provenance { get; }

    public DiManualRegistration(
        DiTypeIdentity implementationType,
        string apiName,
        string location,
        DiAssemblyProvenance provenance,
        DiTypeIdentity? contractType = null,
        DiTypeIdentity? scopeMarkerType = null,
        bool isGeneratedRoute = false) {
        ImplementationType = implementationType;
        ContractType = contractType;
        ScopeMarkerType = scopeMarkerType;
        ApiName = apiName ?? string.Empty;
        Location = location ?? string.Empty;
        IsGeneratedRoute = isGeneratedRoute;
        Provenance = provenance;
    }
}
