using System;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiAssemblyProvenance {
    public string DeclaringAssembly { get; }
    public string ObservedAssembly { get; }
    public DiEvidenceSource EvidenceSource { get; }
    public IReadOnlyList<string> ReferencePath { get; }

    public DiAssemblyProvenance(
        string declaringAssembly,
        string observedAssembly,
        DiEvidenceSource evidenceSource,
        IEnumerable<string>? referencePath = null) {
        DeclaringAssembly = declaringAssembly ?? string.Empty;
        ObservedAssembly = observedAssembly ?? string.Empty;
        EvidenceSource = evidenceSource;
        ReferencePath = (referencePath ?? Array.Empty<string>())
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(static path => path.Trim())
            .ToArray();
    }

    public static DiAssemblyProvenance CurrentCompilation(string assemblyName)
        => new DiAssemblyProvenance(assemblyName, assemblyName, DiEvidenceSource.CurrentCompilation, new[] { assemblyName });
}
