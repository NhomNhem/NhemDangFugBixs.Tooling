using Assets.Scripts.Runtime.Shared.Interfaces.Scope;
using NhemDangFugBixs.Attributes;
using NhemDangFugBixs.Generated.GameFeel_Scopes_TestFiles;
using VContainer;
using VContainer.Unity;

namespace GameFeelUnity.Scopes {
    /// <summary>
    /// Child lifetime scope for gameplay-specific services (scene-based).
    /// </summary>
    [LifetimeScopeFor(typeof(IGameplayScope))]
    public class GameplayLifetimeScope : LifetimeScope {
        protected override void Configure(IContainerBuilder builder) {
            VContainerRegistration.RegisterAll(builder);
        }
    }
}