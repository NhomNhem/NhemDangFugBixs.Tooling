namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiScopeIdentity {
    public DiTypeIdentity MarkerType { get; }
    public string? Alias { get; }
    public DiTypeIdentity? CompositionRootType { get; }
    public DiAssemblyProvenance Provenance { get; }

    public DiScopeIdentity(
        DiTypeIdentity markerType,
        DiAssemblyProvenance provenance,
        string? alias = null,
        DiTypeIdentity? compositionRootType = null) {
        MarkerType = markerType;
        Provenance = provenance;
        Alias = alias;
        CompositionRootType = compositionRootType;
    }
}
