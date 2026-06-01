using NhemDangFugBixs.Analyzers.Rules;
using NhemDangFugBixs.Analyzers.Tests.TestHost;
using Xunit;

namespace NhemDangFugBixs.Analyzers.Tests;

public class OpenSpecAnalyzerMvpTests {
    [Fact]
    public void AsAttribute_InvalidContract_ReportsNhemDi001() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IService {}
[AutoRegisterIn(typeof(IGameplayScope))]
[As(typeof(System.IDisposable))]
public sealed class CombatCore : IService {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class AsAttribute : System.Attribute { public AsAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_001");
    }

    [Fact]
    public void AutoRegisterIn_InvalidScopeMarker_ReportsNhemDi002() {
        const string source = """
using NhemDangFugBixs.Attributes;
public sealed class NotScopeMarker {}
[AutoRegisterIn(typeof(NotScopeMarker))]
public sealed class CombatCore {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_002");
    }

    [Fact]
    public void AutoRegisterIn_NoExposureIntent_ReportsNhemDi003() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope), AsImplementedInterfaces = false, AsSelf = false)]
public sealed class CombatCore {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute {
    public AutoRegisterInAttribute(System.Type t) {}
    public bool AsImplementedInterfaces { get; set; }
    public bool AsSelf { get; set; }
  }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_003");
    }

    [Fact]
    public void EntryPointWithoutLifecycle_ReportsNhemDi040() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
[EntryPoint]
public sealed class GameplayLoop {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class EntryPointAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_040");
    }

    [Fact]
    public void MissingLifetimeScopeMapping_DoesNotReportNhemDi010() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class CombatCore {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new ScopeMappingAnalyzer());
        Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_010");
    }

    [Fact]
    public void LifetimeScopeWithoutRegisterGeneratedFor_ReportsNhemDi011() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer;
using VContainer.Unity;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class CombatCore {}
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) { }
}
namespace VContainer { public interface IContainerBuilder {} }
namespace VContainer.Unity { public abstract class LifetimeScope { protected abstract void Configure(global::VContainer.IContainerBuilder builder); } }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new ScopeMappingAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_011");
    }

    [Fact]
    public void LifetimeScopeWrongMarker_ReportsNhemDi012() {
        const string source = """
using NhemDangFugBixs.Attributes;
using NhemDangFugBixs.VContainer;
using VContainer;
using VContainer.Unity;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IProjectRootScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class CombatCore {}
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.RegisterGeneratedFor<IProjectRootScope>();
    }
}
namespace VContainer { public interface IContainerBuilder {} }
namespace VContainer.Unity { public abstract class LifetimeScope { protected abstract void Configure(global::VContainer.IContainerBuilder builder); } }
namespace NhemDangFugBixs.VContainer {
  public static class NhemGeneratedVContainerExtensions { public static void RegisterGeneratedFor<T>(this global::VContainer.IContainerBuilder builder) {} }
}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new ScopeMappingAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_012");
    }

    [Fact]
    public void DuplicateGeneratedInvocation_ReportsNhemDi022() {
        const string source = """
using NhemDangFugBixs.Attributes;
using NhemDangFugBixs.VContainer;
using VContainer;
using VContainer.Unity;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class CombatCore {}
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.RegisterGeneratedFor<IGameplayScope>();
        NhemGeneratedGameplayScopeInstaller.Register(builder);
    }
}
public static class NhemGeneratedGameplayScopeInstaller { public static void Register(global::VContainer.IContainerBuilder builder) {} }
namespace VContainer { public interface IContainerBuilder {} }
namespace VContainer.Unity { public abstract class LifetimeScope { protected abstract void Configure(global::VContainer.IContainerBuilder builder); } }
namespace NhemDangFugBixs.VContainer {
  public static class NhemGeneratedVContainerExtensions { public static void RegisterGeneratedFor<T>(this global::VContainer.IContainerBuilder builder) {} }
}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class LifetimeScopeForAttribute : System.Attribute { public LifetimeScopeForAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new ScopeMappingAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_022");
    }

    [Fact]
    public void IObjectResolverInjection_ReportsNhemDi050() {
        const string source = """
using NhemDangFugBixs.Attributes;
using VContainer;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class CombatCore {
    public CombatCore(IObjectResolver resolver) {}
}
namespace VContainer { public interface IObjectResolver {} }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new ServiceLocatorAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_050");
    }

    [Fact]
    public void DuplicateAsAttribute_SameContract_ReportsNhemDi061() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IPlayerView {}
[AutoRegisterIn(typeof(IGameplayScope))]
[As(typeof(IPlayerView))]
[As(typeof(IPlayerView))]
public sealed class PlayerView : IPlayerView {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class AsAttribute : System.Attribute { public AsAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_061");
    }

    [Fact]
    public void DuplicateAsAttribute_DifferentContracts_NoNhemDi061() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IPlayerView {}
public interface ICombatTarget {}
[AutoRegisterIn(typeof(IGameplayScope))]
[As(typeof(IPlayerView))]
[As(typeof(ICombatTarget))]
public sealed class PlayerView : IPlayerView, ICombatTarget {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class AsAttribute : System.Attribute { public AsAttribute(System.Type t) {} }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_061");
    }

    [Fact]
    public void RegisterComponentInHierarchy_NonMonoBehaviour_ReportsNhemDi066() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class PlayerViewService {}
namespace UnityEngine { public class MonoBehaviour {} }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class RegisterComponentInHierarchyAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_066");
    }

    [Fact]
    public void RegisterComponentInHierarchy_MonoBehaviourSubclass_NoNhemDi066() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope))]
public sealed class PlayerView : UnityEngine.MonoBehaviour {}
namespace UnityEngine { public class MonoBehaviour {} }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class RegisterComponentInHierarchyAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_066");
    }

    [Fact]
    public void EntryPointWithoutLifecycle_ReportsNhemDi067() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
[EntryPoint]
public sealed class GameplayLoop {}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class EntryPointAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_067");
    }

    [Fact]
    public void EntryPointWithIStartable_NoNhemDi067() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
[EntryPoint]
public sealed class GameplayLoop : VContainer.Unity.IStartable {
    public void Start() {}
}
namespace VContainer.Unity { public interface IStartable { void Start(); } }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class EntryPointAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_067");
    }

    [Fact]
    public void EntryPointWithITickable_NoNhemDi067() {
        const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
[EntryPoint]
public sealed class GameplayLoop : VContainer.Unity.ITickable {
    public void Tick() {}
}
namespace VContainer.Unity { public interface ITickable { void Tick(); } }
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class EntryPointAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_067");
    }

    [Fact]
    public void EntryPointWithIDisposable_NoNhemDi067() {
        const string source = """
using NhemDangFugBixs.Attributes;
using System;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
[AutoRegisterIn(typeof(IGameplayScope))]
[EntryPoint]
public sealed class GameplayLoop : IDisposable {
    public void Dispose() {}
}
namespace NhemDangFugBixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { public AutoRegisterInAttribute(System.Type t) {} }
  public class EntryPointAttribute : System.Attribute { }
}
""";

        var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
        Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_067");
    }
}
