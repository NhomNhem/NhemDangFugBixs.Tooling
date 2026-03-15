using VContainer;
using VContainer.Unity;

namespace GameFeelUnity.Tests.Scopes
{
    /// <summary>
    /// Parent lifetime scope for game-wide services (persistent, DontDestroyOnLoad).
    /// Tests v3.0 type-safe scope registration.
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // v3.0 generated registration method
            // Convention-based naming: GameLifetimeScope → RegisterGame()
            // Note: Namespace may vary based on your assembly name
            NhemDangFugBixs.Generated.AssemblyCSharp.VContainerRegistration.RegisterGame(builder);
            
            // Explicitly register child scope (parent→child wiring)
            NhemDangFugBixs.Generated.AssemblyCSharp.VContainerRegistration.RegisterGameplay(builder);
        }
    }
}
