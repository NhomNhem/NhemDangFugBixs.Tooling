namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiGeneratedInstaller {
    public DiTypeIdentity ScopeMarkerType { get; }
    public string InstallerTypeName { get; }
    public bool IsNoOp { get; }
    public DiAssemblyProvenance Provenance { get; }

    public DiGeneratedInstaller(
        DiTypeIdentity scopeMarkerType,
        string installerTypeName,
        bool isNoOp,
        DiAssemblyProvenance provenance) {
        ScopeMarkerType = scopeMarkerType;
        InstallerTypeName = installerTypeName ?? string.Empty;
        IsNoOp = isNoOp;
        Provenance = provenance;
    }
}
