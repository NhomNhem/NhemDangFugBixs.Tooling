using VContainer;
using VContainer.Unity;

namespace GameFeelUnity.Tests.Scopes
{
    /// <summary>
    /// Child lifetime scope for gameplay-specific services (scene-based).
    /// Tests v3.0 type-safe scope registration and parent-child injection.
    /// </summary>
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Child scope registration - inherits parent services
            NhemDangFugBixs.Generated.AssemblyCSharp.VContainerRegistration.RegisterGameplay(builder);
        }
    }
}
