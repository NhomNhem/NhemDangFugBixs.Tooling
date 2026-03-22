---
sidebar_position: 1
---

# NhemDangFugBixs Tooling v6.0 Documentation

Welcome to the official documentation for the NhemDangFugBixs Tooling project (version 6.0). This toolkit provides a powerful C# Source Generator and Roslyn Analyzer suite for Unity projects using VContainer, automating dependency registration and ensuring compile-time safety.

## 🌟 Key Features
- **Module/Installer Pattern**: Decouple complex DI logic into installers, auto-executed in registration order.
- **Cross-Layer Discovery**: Register services across assemblies without circular dependencies.
- **Type-Safe Scopes**: Register to specific LifetimeScope types with full IntelliSense.
- **Unified Registration**: One call registers all discovered services.
- **Compile-Time Validation**: Roslyn analyzers catch DI errors before runtime.
- **Code Fix Providers**: IDE auto-fix for common DI mistakes.
- **Preflight CLI**: Validate DI setup before entering Unity Play Mode.

## Quick Start
1. Build the solution:
   ```bash
   dotnet build NhemDangFugBixs.Tooling.sln
   ```
2. Copy DLLs from `Analyzers/` and `Runtime/` into your Unity project (`Assets/Plugins/Analyzers`).
3. Mark DLLs as Roslyn Analyzer in Unity Inspector.
4. Use `[AutoRegisterIn(typeof(MyScope))]` on your service classes.
5. Call `VContainerRegistration.RegisterAll(builder)` in your LifetimeScope.

## Example: AutoRegister Usage
```csharp
[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime.Singleton)]
public class EnemySpawner : IEnemySpawner { /* ... */ }
```
This will auto-register `EnemySpawner` as a singleton in the `GameLifetimeScope`.

> See the sidebar for detailed guides, migration notes, and troubleshooting.