using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace NhemDangFugBixs.DiSmokeValidation.Tests;

public class CrossAsmdefCompositionValidatorTests {

    [Fact]
    public void DuplicateCompositionTarget_AcrossAssemblies_ReportsError() {
        var asm1 = CompileToFile("""
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""", "Game.Composition1");

        var asm2 = CompileToFile("""
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class AnotherGameplayLifetimeScope { }
""", "Game.Composition2");

        var validator = new CrossAsmdefCompositionValidator();
        var result = validator.Validate(new[] { asm1, asm2 });

        var errors = result.Errors.ToList();
        Assert.True(errors.Count >= 1, "Expected at least one error for duplicate composition target");
        Assert.Contains(errors, e => e.Contains("Duplicate composition target"));

        Cleanup(asm1, asm2);
    }

    [Fact]
    public void OrphanService_NoCompositionTarget_ReportsError() {
        var serviceAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class DayService { }
""", "Game.Services");

        var compositionAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IUIScope : IScopeMarker { }
[LifetimeScopeFor(typeof(IUIScope))]
public sealed class UILifetimeScope { }
""", "Game.UI");

        var validator = new CrossAsmdefCompositionValidator();
        var result = validator.Validate(new[] { serviceAsm, compositionAsm });

        var errors = result.Errors.ToList();
        Assert.True(errors.Count >= 1, "Expected at least one error for orphan service");
        Assert.Contains(errors, e => e.Contains("DayService"));
        Assert.Contains(errors, e => e.Contains("IGameplayScope"));

        Cleanup(serviceAsm, compositionAsm);
    }

    [Fact]
    public void ValidComposition_NoErrors() {
        var serviceAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class DayService { }
""", "Game.Services");

        var compositionAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""", "Game.Composition");

        var validator = new CrossAsmdefCompositionValidator();
        var result = validator.Validate(new[] { serviceAsm, compositionAsm });

        Assert.Empty(result.Errors);

        Cleanup(serviceAsm, compositionAsm);
    }

    private static string CompileToFile(string source, string assemblyName) {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = new List<MetadataReference> {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(NhemDangFugBixs.Attributes.AutoRegisterInAttribute).Assembly.Location)
        };

        // Add netstandard reference for Attribute/Type base types
        var trustedAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            ?.Split(Path.PathSeparator)
            ?? Array.Empty<string>();

        foreach (var assemblyPath in trustedAssemblies) {
            var fileName = Path.GetFileName(assemblyPath);
            if (fileName is "netstandard.dll" or "System.Runtime.dll") {
                references.Add(MetadataReference.CreateFromFile(assemblyPath));
            }
        }

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{assemblyName}_{Guid.NewGuid()}.dll");
        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write)) {
            var emitResult = compilation.Emit(stream);
            if (!emitResult.Success) {
                var errors = string.Join(Environment.NewLine, emitResult.Diagnostics);
                throw new InvalidOperationException(errors);
            }
        }

        return path;
    }

    private static void Cleanup(params string[] paths) {
        foreach (var path in paths) {
            try { File.Delete(path); } catch { /* ignore */ }
            try { File.Delete(path + ".pdb"); } catch { /* ignore */ }
        }
    }
}
