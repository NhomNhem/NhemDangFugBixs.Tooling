using System.Diagnostics;
using NhemDangFugBixs.DiSmokeValidation;

namespace DangFugBixs.Cli;

public class Program {
    public static int Main(string[] args) {
        if (args.Length == 0 || args[0] == "--help" || args[0] == "-h") {
            PrintHelp();
            return 0;
        }

        var command = args[0];
        var commandArgs = args.Skip(1).ToArray();

        if (command == "preflight") return RunPreflight(commandArgs);
        if (command == "validate") return RunValidate(commandArgs);
        if (command == "list") return RunList(commandArgs);
        if (command == "graph") return RunGraph(commandArgs);
        if (command == "report") return RunReport(commandArgs);
        if (command == "--help" || command == "-h") {
            PrintHelp();
            return 0;
        }

        Console.WriteLine($"Unknown command: {command}");
        PrintHelp();
        return 1;
    }

    private static int RunPreflight(string[] args) {
        Console.WriteLine("🚀 Preflight Validation");
        Console.WriteLine("======================");
        Console.WriteLine();

        var projectPath = args.FirstOrDefault(a => a.EndsWith(".csproj", StringComparison.Ordinal));
        var assemblyPath = args.FirstOrDefault(a => a.EndsWith(".dll", StringComparison.Ordinal));
        var formatIndex = Array.IndexOf(args, "--format");
        var format = formatIndex >= 0 && formatIndex < args.Length - 1 ? args[formatIndex + 1] : "text";
        var outputIndex = Array.IndexOf(args, "--output");
        var outputPath = outputIndex >= 0 && outputIndex < args.Length - 1 ? args[outputIndex + 1] : null;
        var clean = args.Contains("--clean");
        var resolveSmoke = args.Contains("--resolve-smoke");

        if (string.IsNullOrEmpty(projectPath) && string.IsNullOrEmpty(assemblyPath)) {
            Console.Error.WriteLine("Error: Please provide a .csproj or .dll file");
            PrintHelp();
            return 1;
        }

        // Step 1: Clean (if requested)
        if (clean && !string.IsNullOrEmpty(projectPath)) {
            Console.WriteLine("📁 Cleaning generated files...");
            CleanGeneratedFiles(projectPath);
        }

        // Step 2: Build
        Console.WriteLine("🔨 Building project...");
        var buildResult = BuildProject(projectPath);
        if (buildResult != 0) {
            Console.Error.WriteLine("❌ Build failed!");
            return buildResult;
        }
        Console.WriteLine("✅ Build succeeded");
        Console.WriteLine();

        // Step 3: Find assembly
        if (string.IsNullOrEmpty(assemblyPath) && !string.IsNullOrEmpty(projectPath)) {
            assemblyPath = FindAssemblyPath(projectPath);
        }

        if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath)) {
            Console.Error.WriteLine("❌ Assembly not found after build");
            return 1;
        }

        // Step 4: Run smoke validation
        Console.WriteLine("🔍 Running DI smoke validation...");
        Console.WriteLine();

        var options = new SmokeValidationOptions {
            AssemblyPaths = new[] { assemblyPath },
            Format = format.ToLowerInvariant() == "json" ? SmokeValidationOutputFormat.Json : SmokeValidationOutputFormat.Text
        };

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(options);

        // Step 5: Output results
        if (!string.IsNullOrEmpty(outputPath)) {
            File.WriteAllText(outputPath, result.ToHumanReadableText());
            Console.WriteLine($"📄 Report saved to: {outputPath}");
        }

        Console.WriteLine();
        if (result.IsSuccess) {
            Console.WriteLine("✅ Preflight validation PASSED");

            // Optional: Runtime resolve smoke test
            if (resolveSmoke) {
                Console.WriteLine();
                Console.WriteLine("🔍 Running runtime resolve smoke test...");
                var resolveValidator = new ResolveSmokeValidator();
                var resolveOptions = new ResolveSmokeOptions { Verbose = true };
                var resolveResult = resolveValidator.Validate(assemblyPath, resolveOptions);
                Console.WriteLine(resolveResult.ToHumanReadableText());

                if (!resolveResult.IsSuccess) {
                    return 1;
                }
            }

            return 0;
        }

        Console.WriteLine("❌ Preflight validation FAILED");
        Console.WriteLine();
        Console.WriteLine(result.ToHumanReadableText());
        return 1;
    }

    private static int RunValidate(string[] args) {
        var assemblyPath = args.FirstOrDefault(a => a.EndsWith(".dll", StringComparison.Ordinal));
        var formatIndex = Array.IndexOf(args, "--format");
        var format = formatIndex >= 0 && formatIndex < args.Length - 1 ? args[formatIndex + 1] : "text";

        if (string.IsNullOrEmpty(assemblyPath)) {
            Console.Error.WriteLine("Error: Please provide a .dll file");
            return 1;
        }

        var options = new SmokeValidationOptions {
            AssemblyPaths = new[] { assemblyPath },
            Format = format.ToLowerInvariant() == "json" ? SmokeValidationOutputFormat.Json : SmokeValidationOutputFormat.Text
        };

        var validator = new ReflectionSmokeValidator();
        var result = validator.Validate(options);

        Console.WriteLine(result.ToHumanReadableText());
        return result.IsSuccess ? 0 : 1;
    }

    private static int RunList(string[] args) {
        try {
            var scope = ReadOption(args, "--scope");
            var assemblyPath = ResolveAssemblyPathForMetadataCommands(args);
            var snapshot = RegistrationReportReader.Load(assemblyPath);
            Console.WriteLine(RegistrationReportWriters.WriteList(snapshot, scope));
            return 0;
        } catch (Exception ex) {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static int RunGraph(string[] args) {
        try {
            var scope = ReadOption(args, "--scope");
            var format = ReadOption(args, "--format") ?? "mermaid";
            var outputPath = ReadOption(args, "--output");
            var assemblyPath = ResolveAssemblyPathForMetadataCommands(args);
            var snapshot = RegistrationReportReader.Load(assemblyPath);
            var content = format.Equals("mermaid", StringComparison.OrdinalIgnoreCase)
                ? RegistrationReportWriters.WriteMermaidGraph(snapshot, scope)
                : RegistrationReportWriters.WriteJson(snapshot, scope);
            WriteOutput(content, outputPath);
            return 0;
        } catch (Exception ex) {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static int RunReport(string[] args) {
        try {
            var scope = ReadOption(args, "--scope");
            var format = ReadOption(args, "--format") ?? "markdown";
            var outputPath = ReadOption(args, "--output");
            var assemblyPath = ResolveAssemblyPathForMetadataCommands(args);
            var snapshot = RegistrationReportReader.Load(assemblyPath);
            string content = format.ToLowerInvariant() switch {
                "json" => RegistrationReportWriters.WriteJson(snapshot, scope),
                "markdown" or "md" => RegistrationReportWriters.WriteMarkdown(snapshot, scope),
                _ => RegistrationReportWriters.WriteList(snapshot, scope)
            };

            WriteOutput(content, outputPath);
            return 0;
        } catch (Exception ex) {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static int BuildProject(string? projectPath) {
        if (string.IsNullOrEmpty(projectPath)) return 1;

        var startInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = $"build \"{projectPath}\" --no-restore",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null) return 1;

        process.OutputDataReceived += (sender, e) => {
            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) => {
            if (!string.IsNullOrEmpty(e.Data)) Console.Error.WriteLine(e.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return process.ExitCode;
    }

    private static void CleanGeneratedFiles(string projectPath) {
        var projectDir = Path.GetDirectoryName(projectPath);
        if (string.IsNullOrEmpty(projectDir)) return;

        var generatedPaths = new[] {
            Path.Combine(projectDir, "Generated"),
            Path.Combine(projectDir, "obj", "Generated"),
            Path.Combine(projectDir, "bin")
        };

        foreach (var path in generatedPaths) {
            if (Directory.Exists(path)) {
                try {
                    Directory.Delete(path, true);
                    Console.WriteLine($"  Cleaned: {path}");
                } catch {
                    // Ignore cleanup errors
                }
            }
        }
    }

    private static string? FindAssemblyPath(string projectPath) {
        var projectDir = Path.GetDirectoryName(projectPath);
        if (string.IsNullOrEmpty(projectDir)) return null;

        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        var binPaths = new[] {
            Path.Combine(projectDir, "bin", "Debug"),
            Path.Combine(projectDir, "bin", "Release")
        };

        foreach (var binPath in binPaths) {
            if (!Directory.Exists(binPath)) continue;

            foreach (var frameworkDir in Directory.GetDirectories(binPath)) {
                var assemblyPath = Path.Combine(frameworkDir, $"{projectName}.dll");
                if (File.Exists(assemblyPath)) {
                    return assemblyPath;
                }
            }
        }

        return null;
    }

    private static string ResolveAssemblyPathForMetadataCommands(string[] args) {
        var projectPath = args.FirstOrDefault(a => a.EndsWith(".csproj", StringComparison.Ordinal));
        var assemblyPath = args.FirstOrDefault(a => a.EndsWith(".dll", StringComparison.Ordinal));

        if (!string.IsNullOrEmpty(assemblyPath)) {
            return assemblyPath;
        }

        if (string.IsNullOrEmpty(projectPath)) {
            throw new InvalidOperationException("Please provide a .csproj or .dll file.");
        }

        Console.WriteLine("🔨 Building project...");
        var buildResult = BuildProject(projectPath);
        if (buildResult != 0) {
            throw new InvalidOperationException("Build failed.");
        }

        assemblyPath = FindAssemblyPath(projectPath);
        if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath)) {
            throw new InvalidOperationException("Assembly not found after build.");
        }

        return assemblyPath;
    }

    private static string? ReadOption(string[] args, string optionName) {
        var optionIndex = Array.IndexOf(args, optionName);
        return optionIndex >= 0 && optionIndex < args.Length - 1 ? args[optionIndex + 1] : null;
    }

    private static void WriteOutput(string content, string? outputPath) {
        if (!string.IsNullOrWhiteSpace(outputPath)) {
            File.WriteAllText(outputPath, content);
            Console.WriteLine($"📄 Report saved to: {outputPath}");
            return;
        }

        Console.WriteLine(content);
    }

    private static void PrintHelp() {
        Console.WriteLine(@"
DangFugBixs.Cli - DI Validation Tool

Usage:
  dotnet di-smoke <command> [options]

Commands:
  preflight <project.csproj>    Run full preflight validation (clean + build + validate)
  validate <assembly.dll>       Run validation on existing assembly
  list <project.csproj|dll>     List generated registrations by scope
  graph <project.csproj|dll>    Export a Mermaid or JSON graph from generated metadata
  report <project.csproj|dll>   Export generated DI report metadata

Options:
  --format <text|json>    Output format (default: text)
  --output <file>         Save report to file
  --scope <scope>         Filter metadata commands to one scope
  --clean                 Clean generated files before build
  --resolve-smoke         Run runtime resolve smoke test (Phase 1)
  --help, -h              Show this help message

Examples:
  dotnet di-smoke preflight MyGame.csproj
  dotnet di-smoke preflight MyGame.csproj --format json --output report.json
  dotnet di-smoke preflight MyGame.csproj --clean
  dotnet di-smoke validate bin/Debug/net10.0/MyGame.dll
  dotnet di-smoke list MyGame.csproj --scope GameplayLifetimeScope
  dotnet di-smoke graph MyGame.csproj --scope GameplayLifetimeScope --format mermaid
  dotnet di-smoke report MyGame.csproj --format markdown --output di-map.md
");
    }
}
