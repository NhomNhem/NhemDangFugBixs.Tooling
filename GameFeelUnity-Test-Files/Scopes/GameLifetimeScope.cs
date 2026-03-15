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
            
        }
    }
}
