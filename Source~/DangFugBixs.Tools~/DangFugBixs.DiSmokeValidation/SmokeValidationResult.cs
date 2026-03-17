using System.Text.Json;

namespace NhemDangFugBixs.DiSmokeValidation;

internal sealed class SmokeValidationResult {
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();

    public IReadOnlyList<string> Errors => _errors;
    public IReadOnlyList<string> Warnings => _warnings;
    public bool IsSuccess => _errors.Count == 0;

    public void AddError(string message) => _errors.Add(message);
    public void AddWarning(string message) => _warnings.Add(message);

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
            errors = _errors
        }, new JsonSerializerOptions {
            WriteIndented = true
        });
    }
}
