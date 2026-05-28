namespace NhemDangFugBixs.DiSmokeValidation;

internal enum SmokeValidationOutputFormat {
    Text,
    Json
}

internal sealed class SmokeValidationOptions {
    public IReadOnlyList<string> AssemblyPaths { get; init; } = Array.Empty<string>();
    public bool ShowHelp { get; init; }
    public SmokeValidationOutputFormat Format { get; init; } = SmokeValidationOutputFormat.Text;

    public static SmokeValidationOptions Parse(string[] args) {
        var assemblyPaths = new List<string>();
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

            if (!arg.StartsWith("--")) {
                assemblyPaths.Add(arg);
            }
        }

        if (args.Length == 0) {
            showHelp = true;
        }

        return new SmokeValidationOptions {
            AssemblyPaths = assemblyPaths,
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
