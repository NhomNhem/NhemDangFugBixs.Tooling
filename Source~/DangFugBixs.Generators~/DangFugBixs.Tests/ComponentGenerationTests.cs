using DangFugBixs.Tests.TestHost;
using NUnit.Framework;

namespace DangFugBixs.Tests;

[TestFixture]
public class ComponentGenerationTests {
    [Test]
    public void AutoRegisterIn_MonoBehaviourWithRegisterInHierarchy_UsesComponentInHierarchy() {
        const string source = """
using NhemDangFugBixs.Attributes;
using UnityEngine;
public interface IGameplayScope { }
[AutoRegisterIn(typeof(IGameplayScope), RegisterInHierarchy = true)]
public sealed class PlayerView : MonoBehaviour { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
namespace UnityEngine { public class Component {} public class MonoBehaviour : Component {} }
""";

        var (_, generated) = GeneratorTestHost.Run(source);
        Assert.That(generated, Does.Contain("RegisterComponentInHierarchy<global::PlayerView>"));
    }
}
