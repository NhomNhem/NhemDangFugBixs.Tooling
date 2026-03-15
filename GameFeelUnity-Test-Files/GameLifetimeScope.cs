using VContainer;
using VContainer.Unity;

namespace GameFeelUnity.Scopes
{
    /// <summary>
    /// Parent lifetime scope for game-wide services (persistent, DontDestroyOnLoad).
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // v3.0 generated registration method
            // Convention-based naming: GameLifetimeScope → RegisterGame()
            NhemDangFugBixs.Generated.GameFeel_Scopes_TestFiles.VContainerRegistration.RegisterGame(builder);
            
            // Explicitly register child scopes (parent→child wiring)
            // Uncomment when GameplayLifetimeScope is ready:
            // NhemDangFugBixs.Generated.GameFeel_Scopes_TestFiles.VContainerRegistration.RegisterGameplay(builder);
        }
    }
}
