using VContainer;
using VContainer.Unity;

namespace GameFeelUnity.Scopes
{
    /// <summary>
    /// Child lifetime scope for gameplay-specific services (scene-based).
    /// </summary>
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Child scope registration - inherits parent services
            NhemDangFugBixs.Generated.GameFeel_Scopes_TestFiles.VContainerRegistration.RegisterGameplay(builder);
        }
    }
}
