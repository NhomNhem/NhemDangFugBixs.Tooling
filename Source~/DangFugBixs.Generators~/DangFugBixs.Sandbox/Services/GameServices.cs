using NhemDangFugBixs.Attributes;
using VContainer.Unity;
using DangFugBixs.Sandbox.Scopes;

namespace DangFugBixs.Sandbox.Services;

/// <summary>
/// Example: Game-wide singleton service registered in parent scope.
/// Demonstrates: Type-safe scope reference, Singleton lifetime, EntryPoint (ITickable).
/// </summary>
[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime = Lifetime.Singleton)]
public class GameService : ITickable {
    public float GameTime { get; private set; }

    public void Tick() {
        GameTime += UnityEngine.Time.deltaTime;
    }
}

/// <summary>
/// Example: Audio service registered in parent scope, accessible to all child scopes.
/// Demonstrates: Type-safe scope reference, AsImplementedInterfaces = false (explicit binding).
/// </summary>
[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime = Lifetime.Singleton, AsImplementedInterfaces = false, AsSelf = true)]
public class AudioService {
    public void PlaySound(string soundName) {
        UnityEngine.Debug.Log($"Playing sound: {soundName}");
    }
}

/// <summary>
/// Example: Save system registered in parent scope.
/// Demonstrates: Type-safe scope reference, Singleton lifetime.
/// </summary>
[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime = Lifetime.Singleton)]
public class SaveSystem {
    public void Save() {
        UnityEngine.Debug.Log("Game saved");
    }

    public void Load() {
        UnityEngine.Debug.Log("Game loaded");
    }
}
