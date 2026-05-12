using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace NhemDangFugBixs.Analyzers.Tests.TestHost;

internal static class AnalyzerTestHost {
    public static ImmutableArray<Diagnostic> Run(string source, DiagnosticAnalyzer analyzer, string assemblyName = "AnalyzerTests") {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            CreateDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer));
        return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();
    }

    private static IEnumerable<MetadataReference> CreateDefaultReferences() {
        var assemblies = new[] {
            typeof(object).Assembly,
            typeof(Console).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Attribute).Assembly
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
