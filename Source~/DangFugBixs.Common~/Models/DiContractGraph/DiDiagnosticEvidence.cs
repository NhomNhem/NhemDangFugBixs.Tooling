using System;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiDiagnosticEvidence {
    public string Id { get; }
    public string Kind { get; }
    public string Message { get; }
    public DiEvidenceSource EvidenceSource { get; }
    public IReadOnlyList<DiTypeIdentity> RelatedTypes { get; }

    public DiDiagnosticEvidence(
        string id,
        string kind,
        string message,
        DiEvidenceSource evidenceSource,
        IEnumerable<DiTypeIdentity>? relatedTypes = null) {
        Id = id ?? string.Empty;
        Kind = kind ?? string.Empty;
        Message = message ?? string.Empty;
        EvidenceSource = evidenceSource;
        RelatedTypes = (relatedTypes ?? Array.Empty<DiTypeIdentity>())
            .Distinct()
            .OrderBy(static type => type)
            .ToArray();
    }
}
