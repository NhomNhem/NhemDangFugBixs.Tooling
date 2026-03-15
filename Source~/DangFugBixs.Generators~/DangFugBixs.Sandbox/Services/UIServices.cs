using NhemDangFugBixs.Attributes;
using DangFugBixs.Sandbox.Scopes;

namespace DangFugBixs.Sandbox.Services;

/// <summary>
/// Example: UI service with custom scope name override.
/// Demonstrates: [ScopeName("UI")] attribute for custom registration method name.
/// Without this attribute, it would generate RegisterUserInterface() instead of RegisterUI().
/// </summary>
[ScopeName("UI")]
public class UserInterfaceLifetimeScope : VContainer.Unity.LifetimeScope {
    protected override void Configure(VContainer.IContainerBuilder builder) {
        // This will call RegisterUI() (custom name) instead of RegisterUserInterface()
        NhemDangFugBixs.Generated.DangFugBixsSandbox.VContainerRegistration.RegisterUI(builder);
    }
}

/// <summary>
/// Example: UI service registered with custom scope name.
/// </summary>
[AutoRegisterIn<UserInterfaceLifetimeScope>]
public class UIService {
    public void ShowMenu(string menuName) {
        UnityEngine.Debug.Log($"Showing menu: {menuName}");
    }
}

/// <summary>
/// Example: HUD service also using the custom "UI" scope name.
/// </summary>
[AutoRegisterIn<UserInterfaceLifetimeScope>]
public class HUDService {
    public void UpdateHealth(float health) {
        UnityEngine.Debug.Log($"Health updated: {health}");
    }
}
