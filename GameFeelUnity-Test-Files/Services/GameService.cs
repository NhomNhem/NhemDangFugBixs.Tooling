using GameFeelUnity.Scopes;
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

namespace GameFeelUnity.Tests.Services
{
    /// <summary>
    /// Example: Game-wide singleton service registered in parent scope.
    /// Tests: Type-safe scope reference, Singleton lifetime, EntryPoint (ITickable).
    /// </summary>
    [AutoRegisterIn(typeof(GameLifetimeScope), Lifetime = Lifetime.Singleton)]
    public class GameService : ITickable
    {
        public float GameTime { get; private set; }
        public bool IsRunning { get; private set; }

        public void Tick()
        {
            GameTime += Time.deltaTime;
            IsRunning = true;
            
            if (Time.frameCount % 60 == 0) // Log every second (approx)
            {
                Debug.Log($"[GameService] Ticking... GameTime: {GameTime:F2}s");
            }
        }
    }
}
