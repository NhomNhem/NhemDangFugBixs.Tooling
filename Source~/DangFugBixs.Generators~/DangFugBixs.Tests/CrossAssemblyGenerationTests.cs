using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class CrossAssemblyGenerationTests {
    [Test]
    public void ScopeOwnerAssembly_CanDiscoverServiceDeclaredInDirectReferencedAssembly() {
        const string sharedSource = """
namespace Game.Shared;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
""";
        const string applicationSource = """
using NhemDangFugBixs.Attributes;
using Game.Shared;
namespace Game.Application;
public interface IDayService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IDayService))]
public sealed class DayService : IDayService { }
""";
        var sharedRef = GeneratorTestHost.CompileToReference(sharedSource, "Game.Shared");
        var appRef = GeneratorTestHost.CompileToReference(applicationSource, "Game.Application", new[] { sharedRef });

        const string compositionSource = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;
using Game.Shared;
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope { protected override void Configure(IContainerBuilder builder) { } }

namespace VContainer {
    public enum Lifetime { Singleton, Transient, Scoped }
    public interface IRegistrationBuilder {
        IRegistrationBuilder AsImplementedInterfaces();
        IRegistrationBuilder AsSelf();
        IRegistrationBuilder As<T>();
    }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
        IRegistrationBuilder RegisterEntryPoint<T>();
        IRegistrationBuilder RegisterComponentInHierarchy<T>();
    }
    public sealed class RegistrationBuilder : IRegistrationBuilder {
        public IRegistrationBuilder AsImplementedInterfaces() => this;
        public IRegistrationBuilder AsSelf() => this;
        public IRegistrationBuilder As<T>() => this;
    }
}

namespace VContainer.Unity {
    public abstract class LifetimeScope {
        protected abstract void Configure(global::VContainer.IContainerBuilder builder);
    }
}
""";

        var (result, generated) = GeneratorTestHost.Run(
            compositionSource,
            "Game.Composition",
            new[] { sharedRef, appRef });

        Assert.That(result.Diagnostics, Has.None.Matches<Microsoft.CodeAnalysis.Diagnostic>(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
        Assert.That(generated, Does.Contain("Register<global::Game.Application.DayService>(global::VContainer.Lifetime.Scoped).As<global::Game.Application.IDayService>()"));
    }

    [Test]
    public void ScopeOwnerAssembly_DoesNotDiscoverUnreferencedServiceAssembly() {
        const string sharedSource = """
namespace Game.Shared;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
""";
        const string hiddenApplicationSource = """
using NhemDangFugBixs.Attributes;
using Game.Shared;
namespace Game.HiddenApplication;
public interface IHiddenService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IHiddenService))]
public sealed class HiddenService : IHiddenService { }
""";

        var sharedRef = GeneratorTestHost.CompileToReference(sharedSource, "Game.Shared");
        _ = GeneratorTestHost.CompileToReference(hiddenApplicationSource, "Game.HiddenApplication", new[] { sharedRef });

        const string compositionSource = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;
using Game.Shared;
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope { protected override void Configure(IContainerBuilder builder) { } }

namespace VContainer {
    public enum Lifetime { Singleton, Transient, Scoped }
    public interface IRegistrationBuilder {
        IRegistrationBuilder AsImplementedInterfaces();
        IRegistrationBuilder AsSelf();
        IRegistrationBuilder As<T>();
    }
    public interface IContainerBuilder {
        IRegistrationBuilder Register<T>(Lifetime lifetime);
        IRegistrationBuilder RegisterEntryPoint<T>();
        IRegistrationBuilder RegisterComponentInHierarchy<T>();
    }
    public sealed class RegistrationBuilder : IRegistrationBuilder {
        public IRegistrationBuilder AsImplementedInterfaces() => this;
        public IRegistrationBuilder AsSelf() => this;
        public IRegistrationBuilder As<T>() => this;
    }
}

namespace VContainer.Unity {
    public abstract class LifetimeScope {
        protected abstract void Configure(global::VContainer.IContainerBuilder builder);
    }
}
""";

        var (_, generated) = GeneratorTestHost.Run(
            compositionSource,
            "Game.Composition",
            new[] { sharedRef });

        Assert.That(generated, Does.Not.Contain("HiddenService"));
    }
}
