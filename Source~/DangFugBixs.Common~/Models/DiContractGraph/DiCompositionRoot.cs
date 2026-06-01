using System;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiCompositionRoot {
    public DiTypeIdentity LifetimeScopeType { get; }
    public DiTypeIdentity ScopeMarkerType { get; }
    public IReadOnlyList<string> ConfigureCalls { get; }
    public bool HasGeneratedRegistrationCall { get; }
    public DiAssemblyProvenance Provenance { get; }

    public DiCompositionRoot(
        DiTypeIdentity lifetimeScopeType,
        DiTypeIdentity scopeMarkerType,
        DiAssemblyProvenance provenance,
        IEnumerable<string>? configureCalls = null,
        bool hasGeneratedRegistrationCall = false) {
        LifetimeScopeType = lifetimeScopeType;
        ScopeMarkerType = scopeMarkerType;
        Provenance = provenance;
        ConfigureCalls = (configureCalls ?? Array.Empty<string>())
            .Where(static call => !string.IsNullOrWhiteSpace(call))
            .OrderBy(static call => call, StringComparer.Ordinal)
            .ToArray();
        HasGeneratedRegistrationCall = hasGeneratedRegistrationCall;
    }
}
