using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DangFugBixs.Tests.TestHost;

namespace DangFugBixs.Tests;

[TestFixture]
public class PerformanceSmokeTests {
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(500)]
    public void CompositionOnlyGeneration_PerformanceSmoke_IsDeterministic(int serviceCount) {
        var source = BuildSource(serviceCount);

        var firstRun = Stopwatch.StartNew();
        var (_, generated1) = GeneratorTestHost.Run(source, $"PerfAssembly{serviceCount}");
        firstRun.Stop();

        var secondRun = Stopwatch.StartNew();
        var (_, generated2) = GeneratorTestHost.Run(source, $"PerfAssembly{serviceCount}");
        secondRun.Stop();

        Assert.That(generated1, Is.Not.Empty);
        Assert.That(NormalizeNonDeterministicFields(generated1), Is.EqualTo(NormalizeNonDeterministicFields(generated2)));
        Assert.That(generated1, Does.Not.Contain("Resolve<"));
        Assert.That(firstRun.ElapsedMilliseconds, Is.LessThan(30_000));
        Assert.That(secondRun.ElapsedMilliseconds, Is.LessThan(30_000));
    }

    private static string BuildSource(int serviceCount) {
        var sb = new StringBuilder();
        sb.AppendLine("using NhemDangFugBixs.Attributes;");
        sb.AppendLine("using VContainer;");
        sb.AppendLine("using VContainer.Unity;");
        sb.AppendLine("public interface IScopeMarker { }");
        sb.AppendLine("public interface IGameplayScope : IScopeMarker { }");
        sb.AppendLine("[LifetimeScopeFor(typeof(IGameplayScope))]");
        sb.AppendLine("public sealed class GameplayLifetimeScope : LifetimeScope { protected override void Configure(IContainerBuilder builder) { } }");
        sb.AppendLine();

        for (var i = 0; i < serviceCount; i++) {
            sb.AppendLine($"public interface IService{i} {{ }}");
            sb.AppendLine($"[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = false, AsSelf = false)]");
            sb.AppendLine($"[As(typeof(IService{i}))]");
            sb.AppendLine($"public sealed class Service{i} : IService{i} {{ }}");
            sb.AppendLine();
        }

        sb.AppendLine("namespace VContainer {");
        sb.AppendLine("  public enum Lifetime { Singleton, Transient, Scoped }");
        sb.AppendLine("  public interface IRegistrationBuilder { IRegistrationBuilder AsImplementedInterfaces(); IRegistrationBuilder AsSelf(); IRegistrationBuilder As<T>(); }");
        sb.AppendLine("  public interface IContainerBuilder { IRegistrationBuilder Register<T>(Lifetime lifetime); IRegistrationBuilder RegisterEntryPoint<T>(); IRegistrationBuilder RegisterComponentInHierarchy<T>(); }");
        sb.AppendLine("}");
        sb.AppendLine("namespace VContainer.Unity {");
        sb.AppendLine("  public abstract class LifetimeScope { protected abstract void Configure(global::VContainer.IContainerBuilder builder); }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string NormalizeNonDeterministicFields(string generated) {
        var normalized = generated.Replace("\r\n", "\n", StringComparison.Ordinal);
        var dateLine = DatePrefix(normalized);
        if (string.IsNullOrEmpty(dateLine)) {
            return Regex.Replace(
                normalized,
                @"on \d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}",
                "on <normalized>",
                RegexOptions.CultureInvariant);
        }

        normalized = normalized.Replace(dateLine, "// Date: <normalized>", StringComparison.Ordinal);
        return Regex.Replace(
            normalized,
            @"on \d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}",
            "on <normalized>",
            RegexOptions.CultureInvariant);
    }

    private static string DatePrefix(string generated) {
        foreach (var line in generated.Split('\n')) {
            if (line.StartsWith("// Date: ", StringComparison.Ordinal)) {
                return line;
            }
        }

        return string.Empty;
    }
}
