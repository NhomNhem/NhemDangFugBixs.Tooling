using System;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiServiceRegistration {
    public DiTypeIdentity ImplementationType { get; }
    public IReadOnlyList<DiTypeIdentity> ContractTypes { get; }
    public string Lifetime { get; }
    public DiTypeIdentity? ScopeMarkerType { get; }
    public bool IsComponent { get; }
    public bool IsEntryPoint { get; }
    public bool IsFactory { get; }
    public bool IsMessagePipeBroker { get; }
    public DiAssemblyProvenance Provenance { get; }
    public IReadOnlyDictionary<string, string> Metadata { get; }

    public DiServiceRegistration(
        DiTypeIdentity implementationType,
        IEnumerable<DiTypeIdentity>? contractTypes,
        string lifetime,
        DiAssemblyProvenance provenance,
        DiTypeIdentity? scopeMarkerType = null,
        bool isComponent = false,
        bool isEntryPoint = false,
        bool isFactory = false,
        bool isMessagePipeBroker = false,
        IReadOnlyDictionary<string, string>? metadata = null) {
        ImplementationType = implementationType;
        ContractTypes = (contractTypes ?? Array.Empty<DiTypeIdentity>())
            .Distinct()
            .OrderBy(static type => type)
            .ToArray();
        Lifetime = lifetime ?? string.Empty;
        ScopeMarkerType = scopeMarkerType;
        IsComponent = isComponent;
        IsEntryPoint = isEntryPoint;
        IsFactory = isFactory;
        IsMessagePipeBroker = isMessagePipeBroker;
        Provenance = provenance;
        Metadata = metadata ?? new Dictionary<string, string>();
    }
}
