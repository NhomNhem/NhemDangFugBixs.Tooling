using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using UnityEngine;
using DangFugBixs.Sandbox.Scopes;

namespace DangFugBixs.Sandbox.Services;

/// <summary>
/// Example: Gameplay service registered in child scope.
/// Demonstrates: Type-safe scope reference, Scoped lifetime, EntryPoint (IInitializable).
/// Child scope can inject parent scope services (GameService).
/// </summary>
[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = NhemLifetime.Scoped)]
public class EnemySpawner : IInitializable {
    private readonly GameService _gameService;

    // Parent scope service injected into child scope service ✅
    public EnemySpawner(GameService gameService) {
        _gameService = gameService;
    }

    public void Initialize() {
        UnityEngine.Debug.Log($"EnemySpawner initialized at game time: {_gameService.GameTime}");
    }
}

/// <summary>
/// Example: Player controller in child scope.
/// Demonstrates: Type-safe scope reference, Scoped lifetime, EntryPoint (ITickable).
/// </summary>
[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = NhemLifetime.Scoped)]
public class PlayerController : ITickable {
    private readonly GameService _gameService;
    private readonly AudioService _audioService;

    // Multiple parent scope services injected ✅
    public PlayerController(GameService gameService, AudioService audioService) {
        _gameService = gameService;
        _audioService = audioService;
    }

    public void Tick() {
        // Player update logic
    }
}

/// <summary>
/// Example: MonoBehaviour component in child scope.
/// Demonstrates: Component registration on new GameObject.
/// </summary>
[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = NhemLifetime.Scoped)]
public class BulletPool : MonoBehaviour {
    public void SpawnBullet(Vector3 position, Vector3 direction) {
        UnityEngine.Debug.Log($"Bullet spawned at {position}");
    }
}

/// <summary>
/// Example: MonoBehaviour found in hierarchy.
/// Demonstrates: RegisterInHierarchy = true for existing scene objects.
/// </summary>
[AutoRegisterIn(typeof(GameplayLifetimeScope), RegisterInHierarchy = true)]
public class CameraController : MonoBehaviour {
    public void FollowTarget(Transform target) {
        // Camera follow logic
    }
}
