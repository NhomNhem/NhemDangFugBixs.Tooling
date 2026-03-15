using NhemDangFugBixs.Attributes;
using VContainer.Unity;

namespace DangFugBixs.Sandbox.Scopes;

/// <summary>
/// Root lifetime scope for game-wide services (persistent, DontDestroyOnLoad).
/// </summary>
public class GameLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        // Auto-generated registration method for Game scope
        NhemDangFugBixs.Generated.DangFugBixsSandbox.VContainerRegistration.RegisterGame(builder);
        
        // Explicitly register child scopes (parent→child wiring)
        NhemDangFugBixs.Generated.DangFugBixsSandbox.VContainerRegistration.RegisterGameplay(builder);
    }
}

/// <summary>
/// Gameplay-specific lifetime scope (scene-based, loaded/unloaded with scenes).
/// </summary>
public class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        // Child scope registration - inherits parent services
        NhemDangFugBixs.Generated.DangFugBixsSandbox.VContainerRegistration.RegisterGameplay(builder);
    }
}

/// <summary>
/// UI-specific lifetime scope with custom name override.
/// </summary>
public class UserInterfaceLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        NhemDangFugBixs.Generated.DangFugBixsSandbox.VContainerRegistration.RegisterUI(builder);
    }
}
