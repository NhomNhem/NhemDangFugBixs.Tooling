namespace NhemDangFugBixs.DiSmokeValidation;

internal enum SmokeValidationOutputFormat {
    Text,
    Json
}

internal sealed class SmokeValidationOptions {
    public string? AssemblyPath { get; init; }
    public bool ShowHelp { get; init; }
    public SmokeValidationOutputFormat Format { get; init; } = SmokeValidationOutputFormat.Text;

    public static SmokeValidationOptions Parse(string[] args) {
        string? assemblyPath = null;
        var showHelp = false;
        var format = SmokeValidationOutputFormat.Text;

        for (var i = 0; i < args.Length; i++) {
            var arg = args[i];

            if (arg is "--help" or "-h") {
                showHelp = true;
                continue;
            }

            if (arg == "--format") {
                if (i + 1 >= args.Length) {
                    showHelp = true;
                    continue;
                }

                format = ParseFormat(args[++i]);
                continue;
            }

            assemblyPath ??= arg;
        }

        if (args.Length == 0) {
            showHelp = true;
        }

        return new SmokeValidationOptions {
            AssemblyPath = assemblyPath,
            ShowHelp = showHelp,
            Format = format
        };
    }

    private static SmokeValidationOutputFormat ParseFormat(string value) {
        return value.ToLowerInvariant() switch {
            "json" => SmokeValidationOutputFormat.Json,
            _ => SmokeValidationOutputFormat.Text
        };
    }
}
