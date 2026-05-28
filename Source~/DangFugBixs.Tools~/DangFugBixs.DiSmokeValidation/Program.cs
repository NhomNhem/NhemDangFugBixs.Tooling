namespace NhemDangFugBixs.DiSmokeValidation;

internal static class Program {
    public static int Main(string[] args) {
        var options = SmokeValidationOptions.Parse(args);
        if (options.ShowHelp) {
            Console.WriteLine(SmokeValidationUsage.Text);
            return 0;
        }

        var result = new SmokeValidationResult();

        // Single-assembly reflection validation (backward compatible)
        if (options.AssemblyPaths.Count == 1) {
            var singleOptions = new SmokeValidationOptions {
                AssemblyPaths = options.AssemblyPaths,
                Format = options.Format,
                ShowHelp = false
            };
            var validator = new ReflectionSmokeValidator();
            var singleResult = validator.Validate(singleOptions);
            MergeResult(result, singleResult);
        }

        // Cross-asmdef composition validation for composition-only generation (Tasks 6.1, 6.2, 6.3)
        if (options.AssemblyPaths.Count >= 1) {
            var crossValidator = new CrossAsmdefCompositionValidator();
            var crossResult = crossValidator.Validate(options.AssemblyPaths);
            MergeResult(result, crossResult);
        }

        Console.WriteLine(options.Format == SmokeValidationOutputFormat.Json
            ? result.ToJson()
            : result.ToHumanReadableText());
        return result.IsSuccess ? 0 : 1;
    }

    private static void MergeResult(SmokeValidationResult target, SmokeValidationResult source) {
        foreach (var warning in source.Warnings) {
            target.AddWarning(warning);
        }
        foreach (var error in source.Errors) {
            target.AddError(error);
        }
    }
}
