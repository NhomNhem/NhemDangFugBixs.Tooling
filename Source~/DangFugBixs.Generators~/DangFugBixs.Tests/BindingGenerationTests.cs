using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class BindingGenerationTests {
    [Test]
    public void AutoRegisterIn_AsSelfFalse_DoesNotEmitAsSelfBinding() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
public interface IService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped, AsSelf = false)]
public sealed class Service : IService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain("Service"));
        Assert.That(generated, Does.Not.Contain(".AsSelf()"));
    }
}
