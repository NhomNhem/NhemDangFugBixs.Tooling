namespace NhemDangFugBixs.DiSmokeValidation;

internal static class Program {
    public static int Main(string[] args) {
        var options = SmokeValidationOptions.Parse(args);
        if (options.ShowHelp) {
            Console.WriteLine(SmokeValidationUsage.Text);
            return 0;
        }

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(options);

        Console.WriteLine(options.Format == SmokeValidationOutputFormat.Json
            ? result.ToJson()
            : result.ToHumanReadableText());
        return result.IsSuccess ? 0 : 1;
    }
}
