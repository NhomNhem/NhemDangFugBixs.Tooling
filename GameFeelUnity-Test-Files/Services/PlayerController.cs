using GameFeelUnity.Scopes;
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

namespace GameFeelUnity.Tests.Services
{
    /// <summary>
    /// Example: Player controller in child scope.
    /// Tests: Type-safe scope reference, Scoped lifetime, EntryPoint (ITickable).
    /// CRITICAL TEST: Multiple parent scope services injected.
    /// </summary>
    [AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Scoped)]
    public class PlayerController : ITickable
    {
        private readonly GameService _gameService;
        private readonly AudioService _audioService;

        // Multiple parent scope services injected ✅
        public PlayerController(GameService gameService, AudioService audioService)
        {
            _gameService = gameService;
            _audioService = audioService;
            
            Debug.Log($"[PlayerController] Constructor called. Game running: {_gameService.IsRunning}");
        }

        public void Tick()
        {
            // Player update logic - verify we can access parent service
            if (Time.frameCount % 120 == 0) // Log every 2 seconds
            {
                Debug.Log($"[PlayerController] Tick. Game time: {_gameService.GameTime:F2}s");
            }
        }
    }
}
