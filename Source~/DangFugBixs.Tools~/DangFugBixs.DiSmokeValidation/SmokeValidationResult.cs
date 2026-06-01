using System.Text.Json;

namespace NhemDangFugBixs.DiSmokeValidation;

internal sealed class SmokeValidationResult {
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();
    private readonly List<SmokeDiagnosticEvidence> _evidence = new();

    public IReadOnlyList<string> Errors => _errors;
    public IReadOnlyList<string> Warnings => _warnings;
    public IReadOnlyList<SmokeDiagnosticEvidence> Evidence => _evidence;
    public bool IsSuccess => _errors.Count == 0;

    public void AddError(string message) => _errors.Add(message);
    public void AddWarning(string message) => _warnings.Add(message);
    public void AddEvidence(SmokeDiagnosticEvidence evidence) => _evidence.Add(evidence);

    public string ToHumanReadableText() {
        var lines = new List<string> { IsSuccess ? "DI smoke validation passed." : "DI smoke validation failed." };
        lines.AddRange(_warnings.Select(w => $"warning: {w}"));
        lines.AddRange(_errors.Select(e => $"error: {e}"));
        return string.Join(Environment.NewLine, lines);
    }

    public string ToJson() {
        return JsonSerializer.Serialize(new {
            success = IsSuccess,
            warnings = _warnings,
            errors = _errors,
            evidence = _evidence
        }, new JsonSerializerOptions {
            WriteIndented = true
        });
    }
}

internal sealed class SmokeDiagnosticEvidence {
    public string Kind { get; init; } = string.Empty;
    public string ScopeMarker { get; init; } = string.Empty;
    public string Service { get; init; } = string.Empty;
    public string CompositionRoot { get; init; } = string.Empty;
    public string SourceAssembly { get; init; } = string.Empty;
    public string CompositionAssembly { get; init; } = string.Empty;
    public IReadOnlyList<string> ReferencePath { get; init; } = Array.Empty<string>();
}
