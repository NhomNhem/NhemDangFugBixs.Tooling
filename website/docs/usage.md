---
sidebar_position: 3
---

# Usage Guide (v6.0)

## 1. Installation
1. Build the solution:
   ```bash
   dotnet build NhemDangFugBixs.Tooling.sln
   ```
2. Copy DLLs from `Analyzers/` and `Runtime/` into your Unity project (`Assets/Plugins/Analyzers`).
3. In Unity, mark DLLs as Roslyn Analyzer in the Inspector.

## 2. Registering Services
- Use `[AutoRegisterIn(typeof(MyScope), Lifetime.Singleton)]` on your service class for type-safe registration.
- For complex setup, implement `IVContainerInstaller` and use `[InstallerOrder]` to control execution order.
- Call `VContainerRegistration.RegisterAll(builder)` in your LifetimeScope's `Configure` method.

### Example: Simple Service
```csharp
[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime.Singleton)]
public class EnemySpawner : IEnemySpawner { /* ... */ }
```

### Example: Installer
```csharp
[AutoRegisterIn(typeof(GameLifetimeScope))]
[InstallerOrder(-100)]
public class LoggingInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) {
        builder.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
    }
}
```

## 3. CLI Validation
- Install CLI: `dotnet tool install -g DangFugBixs.Cli`
- Validate Unity project:
   ```bash
   dotnet-di-smoke preflight MyUnityProject.csproj
   ```
- Validate built assembly:
   ```bash
   dotnet-di-smoke validate bin/Debug/net10.0/MyGame.dll
   ```
- Output as JSON:
   ```bash
   dotnet-di-smoke preflight MyUnityProject.csproj --format json --output report.json
   ```

## 4. Testing
- Run `dotnet test` to check analyzer rules and code fixes.

> See the Changelog and FAQ for more advanced usage and troubleshooting.
