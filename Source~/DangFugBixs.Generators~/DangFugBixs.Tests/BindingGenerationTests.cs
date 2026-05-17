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

    [Test]
    public void AutoRegisterIn_RegisterInHierarchy_EmitsRegisterComponentInHierarchy() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
public interface IService { }
[AutoRegisterIn(typeof(IGameplayScope), RegisterInHierarchy = true)]
public sealed class Service : IService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain("RegisterComponentInHierarchy<global::Service>()"));
    }

    [Test]
    public void AutoRegisterIn_Keyed_EmitsKeyedCall() {
        const string source = """
using NhemDangFugBixs.Attributes;
public enum WeaponType { Primary, Secondary }
public interface IGameplayScope { }
public interface IService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[Keyed(WeaponType.Primary)]
public sealed class Service : IService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain(".Keyed(global::WeaponType.Primary)"));
    }

    [Test]
    public void AutoRegisterIn_WithExplicitAsAttribute_EmitsTypedAsWithoutImplicitSelf() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
public interface IService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IService))]
public sealed class Service : IService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain(".As<global::IService>()"));
        Assert.That(generated, Does.Not.Contain(".AsImplementedInterfaces()"));
    }

    [Test]
    public void RegisterComponentInHierarchyAttribute_EmitsHierarchyRegistration() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
[AutoRegisterIn(typeof(IGameplayScope))]
[RegisterComponentInHierarchy]
public sealed class PlayerView { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain("RegisterComponentInHierarchy<global::PlayerView>()"));
    }
}
