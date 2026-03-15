using NhemDangFugBixs.Attributes;
using DangFugBixs.Sandbox.Scopes;

namespace DangFugBixs.Sandbox.Services;

/// <summary>
/// Example: UI service registered with custom scope name.
/// </summary>
[AutoRegisterIn(typeof(UserInterfaceLifetimeScope))]
public class UIService {
    public void ShowMenu(string menuName) {
        UnityEngine.Debug.Log($"Showing menu: {menuName}");
    }
}

/// <summary>
/// Example: HUD service also using the custom "UI" scope name.
/// </summary>
[AutoRegisterIn(typeof(UserInterfaceLifetimeScope))]
public class HUDService {
    public void UpdateHealth(float health) {
        UnityEngine.Debug.Log($"Health updated: {health}");
    }
}
