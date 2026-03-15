# NhemDangFugBixs.VContainer.SourceGenerator

A powerful C# Source Generator for **VContainer** that automates dependency registration in Unity projects. Reduce boilerplate code and maintain a clean architecture by using simple attributes.

## 🌟 Key Features

- **Cross-Layer Discovery** (v3.1): Register services across different assemblies (`asmdef`) using **Identity Types**. Break the circular dependency trap in layered architectures!
- **Type-Safe Scopes** (v3.0): Register services to specific `LifetimeScope` types using `[AutoRegisterIn(typeof(TScope))]` - full IntelliSense support, no more string typos!
- **Unified Master Registration**: Single `VContainerRegistration.RegisterAll(builder)` call in your `LifetimeScope` registers every discovered service across all layers.
- **Convention-Based Naming**: Automatic registration method names by stripping "LifetimeScope" suffix (e.g., `GameplayLifetimeScope` → `RegisterGameplay()`).
- **Zero Boilerplate**: Register services and components using simple attributes.
- **Smart Component Detection**: Automatically handles `MonoBehaviour` with options for `InHierarchy` or `NewGameObject`.
- **Compile-Time Validation**: Roslyn analyzers catch scope errors before runtime (ND001-ND103).

---

## 🏗️ Cross-Layer Bridge (v3.1)

If your project is split into multiple assemblies (e.g., `Core.asmdef`, `Gameplay.asmdef`, `Main.asmdef`), you can use **Identity Types** to register services from a low-level layer into a high-level scope without creating circular dependencies.

### Real-World Example: Enemy Pooling System

#### 1. Define an Identity (Shared Layer)
```csharp
// Shared.asmdef
public class GameplayScope {} 
```

#### 2. Register Your Service (Service Layer)
```csharp
// Core.asmdef (References Shared)
using NhemDangFugBixs.Attributes;

[AutoRegisterIn(typeof(GameplayScope), Lifetime = NLifetime.Scoped)]
public class EnemyPoolManager : IEnemyPoolManager, IDisposable {
    // This service will be automatically registered into 
    // any LifetimeScope mapped to GameplayScope.
}
```

#### 3. Map Identity to LifetimeScope (Main Layer)
```csharp
// Main.asmdef (References Core and Shared)
using NhemDangFugBixs.Attributes;

[LifetimeScopeFor(typeof(GameplayScope))]
public class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        // ONE call to register EVERYTHING from ALL layers
        VContainerRegistration.RegisterAll(builder);
    }
}
```

---

## 🛠 Feature Guide

### 1. Type-Safe Scope Registration (v3.0+)

The recommended way to register services with compile-time safety:

```csharp
// Primary syntax - typeof() for Unity compatibility
[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class EnemySpawner { }

// Cross-layer syntax - use an Identity Type (v3.1+)
[AutoRegisterIn(typeof(GameplayScope))]
public class DecoupledService { }
```

**Benefits:**
- ✅ Full IntelliSense support (IDE suggests scope types)
- ✅ Compile-time validation (typos become compiler errors)
- ✅ Refactoring-safe (rename scope class → all usages update)
- ✅ **Layer-Safe** (v3.1): No circular dependencies in complex asmdef setups.

### 2. Convention-Based Naming

The generator automatically strips "LifetimeScope" suffix:

| Scope Type Name | Generated Method |
|-----------------|------------------|
| `GameLifetimeScope` | `RegisterGame()` |
| `GameplayLifetimeScope` | `RegisterGameplay()` |
| `UILifetimeScope` | `RegisterUI()` |

Override with `[ScopeName("Custom")]` when needed.

---

## ⚠️ Migration Guide (v3.0 → v3.1)

v3.1 switched from generic attributes to `typeof()` arguments for better compatibility with different C# language versions in Unity.

### Before (v3.0)
```csharp
[AutoRegisterIn<GameLifetimeScope>]
```

### After (v3.1)
```csharp
[AutoRegisterIn(typeof(GameLifetimeScope))]
```

---

## 📋 Diagnostic Codes

| Code | Severity | Description |
|------|----------|-------------|
| ND001 | Error | Scope type not found |
| ND002 | Error | Type must inherit from LifetimeScope |
| ND003 | Error | Circular scope dependency detected |
| ND004 | Warning | Parent scope cannot depend on child scope |

---

## 📜 License

This project is part of the NhemDangFugBixs Tooling collection.
