using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class EntryPointGenerationTests {
    [Test]
    public void AutoRegisterIn_TypeImplementingITickable_IsRegisteredAsEntryPoint() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
public interface IGameplayScope { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class PhaseFlowEntryPoint : ITickable { public void Tick() { } }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain("RegisterEntryPoint<global::PhaseFlowEntryPoint>(global::VContainer.Lifetime.Scoped)"));
    }
}
