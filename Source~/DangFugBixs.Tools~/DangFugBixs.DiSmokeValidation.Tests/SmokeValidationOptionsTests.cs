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

        Assert.Equal("bin/Debug/Test.dll", options.AssemblyPaths[0]);
        Assert.Equal(SmokeValidationOutputFormat.Text, options.Format);
    }

    [Fact]
    public void Parse_JsonFormat_UsesRequestedOutput() {
        var options = SmokeValidationOptions.Parse(["--format", "json", "bin/Debug/Test.dll"]);

        Assert.Equal("bin/Debug/Test.dll", options.AssemblyPaths[0]);
        Assert.Equal(SmokeValidationOutputFormat.Json, options.Format);
    }

    [Fact]
    public void Parse_MultipleAssemblyPaths_UsesAllPaths() {
        var options = SmokeValidationOptions.Parse([
            "Game.Shared.dll",
            "Game.Application.dll",
            "Game.Composition.dll"
        ]);

        Assert.Equal(3, options.AssemblyPaths.Count);
        Assert.Equal("Game.Shared.dll", options.AssemblyPaths[0]);
        Assert.Equal("Game.Application.dll", options.AssemblyPaths[1]);
        Assert.Equal("Game.Composition.dll", options.AssemblyPaths[2]);
    }
}
