using DangFugBixs.Tests.TestHost;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class CompositionOnlyDiagnosticsTests {

    [Test]
    public void DuplicateCompositionTarget_SameScopeMarker_ReportsError() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;

public interface IGameplayScope { }

[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class FirstLifetimeScope : LifetimeScope { }

[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class SecondLifetimeScope : LifetimeScope { }

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

        var (result, _) = GeneratorTestHost.Run(source, "TestComposition");

        var errors = result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.Id)
            .ToList();

        Assert.That(errors, Does.Contain("NDFG005"), "Expected NDFG005 for duplicate composition target");
    }

    [Test]
    public void DuplicateDiscoveredRegistration_DifferentAssemblies_ReportsWarning() {
        const string sharedSource = """
namespace Game.Shared;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
""";
        const string app1Source = """
using NhemDangFugBixs.Attributes;
using Game.Shared;
namespace Game.Application;
public interface IDayService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IDayService))]
public sealed class DayService : IDayService { }
""";
        const string app2Source = """
using NhemDangFugBixs.Attributes;
using Game.Shared;
namespace Game.Application;
public interface IDayService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IDayService))]
public sealed class DayService : IDayService { }
""";

        var sharedRef = GeneratorTestHost.CompileToReference(sharedSource, "Game.Shared");
        var app1Ref = GeneratorTestHost.CompileToReference(app1Source, "Game.Application1", new[] { sharedRef });
        var app2Ref = GeneratorTestHost.CompileToReference(app2Source, "Game.Application2", new[] { sharedRef });

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

        var (result, _) = GeneratorTestHost.Run(
            compositionSource,
            "Game.Composition",
            new[] { sharedRef, app1Ref, app2Ref });

        var warnings = result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Warning)
            .Select(d => d.Id)
            .ToList();

        Assert.That(warnings, Does.Contain("NDFG006"), "Expected NDFG006 for duplicate discovered registration");
    }

    [Test]
    public void MissingVContainerReference_ReportsError() {
        const string source = """
using NhemDangFugBixs.Attributes;

public interface IGameplayScope { }

[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

        var (result, _) = GeneratorTestHost.Run(source, "TestComposition");

        var errors = result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.Id)
            .ToList();

        Assert.That(errors, Does.Contain("NDFG007"), "Expected NDFG007 for missing VContainer reference");
    }

    [Test]
    public void LocalServiceMarked_IsFromCurrentCompilation() {
        const string sharedSource = """
namespace Game.Shared;
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
""";
        const string appSource = """
using NhemDangFugBixs.Attributes;
using Game.Shared;
namespace Game.Application;
public interface IDayService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IDayService))]
public sealed class DayService : IDayService { }
""";

        var sharedRef = GeneratorTestHost.CompileToReference(sharedSource, "Game.Shared");
        var appRef = GeneratorTestHost.CompileToReference(appSource, "Game.Application", new[] { sharedRef });

        const string compositionSource = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;
using Game.Shared;
using Game.Application;
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

        Assert.That(result.Diagnostics, Has.None.Matches<Diagnostic>(d => d.Severity == DiagnosticSeverity.Error));
        Assert.That(generated, Does.Contain("Register<global::Game.Application.DayService>(global::VContainer.Lifetime.Scoped)"));
    }
}
