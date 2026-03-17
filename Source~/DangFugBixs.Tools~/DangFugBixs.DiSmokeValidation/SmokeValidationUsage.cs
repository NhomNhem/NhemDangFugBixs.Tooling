namespace NhemDangFugBixs.DiSmokeValidation;

internal static class SmokeValidationUsage {
    public const string Text = """
DI Smoke Validation

Usage:
  dotnet run --project Source~/DangFugBixs.Tools~/DangFugBixs.DiSmokeValidation/DangFugBixs.DiSmokeValidation.csproj -- [--format text|json] <assembly-path>

The command validates generated DI output from a built assembly and exits non-zero on graph errors.
""";
}
