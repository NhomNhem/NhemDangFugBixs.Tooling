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
        Assert.Contains(result.Evidence, e =>
            e.Kind == "missing-composition-target" &&
            e.Service.Contains("DayService", StringComparison.Ordinal) &&
            e.ScopeMarker.Contains("IGameplayScope", StringComparison.Ordinal));

        var json = result.ToJson();
        Assert.Contains("missing-composition-target", json);
        Assert.Contains("IGameplayScope", json);

        Cleanup(serviceAsm, compositionAsm);
    }

    [Fact]
    public void ValidComposition_NoErrors() {
        var sharedAsm = CompileToFile("""
namespace Game.Shared;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
""", "Game.Shared.Valid");

        var serviceAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
using Game.Shared;
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class DayService { }
""", "Game.Services.Valid", sharedAsm);

        var compositionAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
using Game.Shared;
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
public sealed class CompositionReferencesServiceAssembly { private DayService? _service; }
""", "Game.Composition.Valid", sharedAsm, serviceAsm);

        var validator = new CrossAsmdefCompositionValidator();
        var result = validator.Validate(new[] { sharedAsm, serviceAsm, compositionAsm });

        Assert.Empty(result.Errors);

        Cleanup(sharedAsm, serviceAsm, compositionAsm);
    }

    [Fact]
    public void CompositionAssemblyCannotReferenceServiceAssembly_ReportsEvidence() {
        var sharedAsm = CompileToFile("""
namespace Game.Shared;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
""", "Game.Shared.ReferenceGap");

        var serviceAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
using Game.Shared;
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class DayService { }
""", "Game.Services.ReferenceGap", sharedAsm);

        var compositionAsm = CompileToFile("""
using NhemDangFugBixs.Attributes;
using Game.Shared;
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""", "Game.Composition.ReferenceGap", sharedAsm);

        var validator = new CrossAsmdefCompositionValidator();
        var result = validator.Validate(new[] { sharedAsm, serviceAsm, compositionAsm });

        Assert.Contains(result.Errors, e => e.Contains("does not reference service assembly", StringComparison.Ordinal));
        Assert.Contains(result.Evidence, e =>
            e.Kind == "composition-reference-gap" &&
            e.SourceAssembly == "Game.Services.ReferenceGap" &&
            e.CompositionAssembly == "Game.Composition.ReferenceGap");

        Cleanup(sharedAsm, serviceAsm, compositionAsm);
    }

    private static string CompileToFile(string source, string assemblyName, params string[] additionalAssemblyPaths) {
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

        foreach (var assemblyPath in additionalAssemblyPaths) {
            references.Add(MetadataReference.CreateFromFile(assemblyPath));
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
