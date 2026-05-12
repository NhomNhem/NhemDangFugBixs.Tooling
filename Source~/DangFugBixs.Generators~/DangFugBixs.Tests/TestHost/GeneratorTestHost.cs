using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NhemDangFugBixs.Generators;

namespace DangFugBixs.Tests.TestHost;

internal static class GeneratorTestHost {
    public static (GeneratorDriverRunResult Result, string CombinedSource) Run(
        string source,
        string assemblyName = "TestAssembly",
        IEnumerable<MetadataReference>? additionalReferences = null) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = CreateDefaultReferences()
            .Concat(additionalReferences ?? Array.Empty<MetadataReference>())
            .ToArray();

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new VContainerAutoRegisterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var runDriver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);
        var result = runDriver.GetRunResult();
        var combinedSource = string.Join(
            "\n\n// ---- generated file separator ----\n\n",
            result.Results
                .SelectMany(r => r.GeneratedSources)
                .Select(s => s.SourceText.ToString()));
        return (result, combinedSource);
    }

    public static MetadataReference CompileToReference(string source, string assemblyName) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            CreateDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        if (!emitResult.Success) {
            var errors = string.Join(Environment.NewLine, emitResult.Diagnostics);
            throw new InvalidOperationException(errors);
        }

        stream.Position = 0;
        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    private static IEnumerable<MetadataReference> CreateDefaultReferences() {
        var assemblies = new[] {
            typeof(object).Assembly,
            typeof(Console).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Attribute).Assembly,
            typeof(NhemDangFugBixs.Attributes.AutoRegisterInAttribute).Assembly
        };

        foreach (var assembly in assemblies.Distinct()) {
            yield return MetadataReference.CreateFromFile(assembly.Location);
        }

        var trustedAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            ?.Split(Path.PathSeparator)
            ?? Array.Empty<string>();

        foreach (var assemblyPath in trustedAssemblies) {
            var fileName = Path.GetFileName(assemblyPath);
            if (fileName is "System.Runtime.dll" or "netstandard.dll" or "System.Collections.dll") {
                yield return MetadataReference.CreateFromFile(assemblyPath);
            }
        }
    }
}
