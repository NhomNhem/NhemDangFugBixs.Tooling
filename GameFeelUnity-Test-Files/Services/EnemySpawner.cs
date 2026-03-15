using GameFeelUnity.Scopes;
using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;

namespace GameFeelUnity.Tests.Services
{
    /// <summary>
    /// Example: Gameplay service registered in child scope.
    /// Tests: Type-safe scope reference, Scoped lifetime, EntryPoint (IInitializable).
    /// CRITICAL TEST: Parent scope service (GameService) injected into child scope service.
    /// </summary>
    [AutoRegisterIn<GameplayLifetimeScope>(Lifetime = Lifetime.Scoped)]
    public class EnemySpawner : IInitializable
    {
        private readonly GameService _gameService;
        private readonly AudioService _audioService;

        // Parent scope services injected into child scope service ✅
        // This tests the parent-child injection pattern
        public EnemySpawner(GameService gameService, AudioService audioService)
        {
            _gameService = gameService;
            _audioService = audioService;
            
            Debug.Log($"[EnemySpawner] Constructor called. Current game time: {_gameService.GameTime:F2}s");
        }

        public void Initialize()
        {
            Debug.Log($"[EnemySpawner] Initialized at game time: {_gameService.GameTime:F2}s");
            _audioService.PlaySound("EnemySpawn");
        }
    }
}
