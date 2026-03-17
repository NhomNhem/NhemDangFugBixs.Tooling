using Xunit;

namespace NhemDangFugBixs.DiSmokeValidation.Tests;

public class SmokeValidationOptionsTests {
    [Fact]
    public void Parse_HelpFlag_ShowsHelp() {
        var options = SmokeValidationOptions.Parse(["--help"]);

        Assert.True(options.ShowHelp);
    }

    [Fact]
    public void Parse_AssemblyPath_UsesFirstArgument() {
        var options = SmokeValidationOptions.Parse(["bin/Debug/Test.dll"]);

        Assert.Equal("bin/Debug/Test.dll", options.AssemblyPath);
        Assert.Equal(SmokeValidationOutputFormat.Text, options.Format);
    }

    [Fact]
    public void Parse_JsonFormat_UsesRequestedOutput() {
        var options = SmokeValidationOptions.Parse(["--format", "json", "bin/Debug/Test.dll"]);

        Assert.Equal("bin/Debug/Test.dll", options.AssemblyPath);
        Assert.Equal(SmokeValidationOutputFormat.Json, options.Format);
    }
}
