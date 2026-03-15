# NhemDangFugBixs.VContainer.SourceGenerator

A powerful C# Source Generator for **VContainer** that automates dependency registration in Unity projects. Reduce boilerplate code and maintain a clean architecture by using simple attributes.

## 🌟 Key Features

- **Cross-Layer Discovery** (v3.1): Register services across different assemblies (`asmdef`) using **Identity Types**. Break the circular dependency trap in layered architectures!
- **Type-Safe Scopes** (v3.0): Register services to specific `LifetimeScope` types using `[AutoRegisterIn(typeof(TScope))]` - full IntelliSense support, no more string typos!
- **Unified Master Registration**: Single `VContainerRegistration.RegisterAll(builder)` call in your `LifetimeScope` registers every discovered service across all layers.
- **Convention-Based Naming**: Automatic registration method names by stripping "LifetimeScope" suffix (e.g., `GameplayLifetimeScope` → `RegisterGameplay()`).
- **Zero Boilerplate**: Register services and components using simple attributes.
- **Smart Component Detection**: Automatically handles `MonoBehaviour` with options for `InHierarchy` or `NewGameObject`.
- **Dynamic Binding**: Fine-grained control over interface binding using `AsTypes`.
- **Auto EntryPoints**: Automatically detects and registers VContainer lifecycle interfaces (`ITickable`, `IInitializable`, etc.).
- **Auto Factory Support**: Easily register factory delegates (`Func<T>`) for dynamic object creation.
- **Multi-Assembly Safe**: Unique hint names prevent file collisions across different `asmdef` files.
- **Compile-Time Validation**: Roslyn analyzers catch scope errors before runtime (ND001-ND103).

---

## 📦 Installation

### 1. Requirements
- Unity 2021.3+ (or any version supporting Roslyn Analyzers).
- VContainer installed in your project.

### 2. Setup
1. Copy the `Analyzers` folder containing `NhemDangFugBixs.Generators.dll` into your Unity project.
2. Select the DLL in the Unity Inspector and ensure it is marked as a **Roslyn Analyzer**.
3. Add `NhemDangFugBixs.Runtime.dll` to your project references to access the attributes.

---

## 🚀 Quick Start (Basic Usage)

### Step 1: Define Your LifetimeScopes

```csharp
using VContainer.Unity;

// Parent scope - persistent game services (DontDestroyOnLoad)
public class GameLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        VContainerRegistration.RegisterGame(builder);
        VContainerRegistration.RegisterGameplay(builder); // Child scope
    }
}

// Child scope - scene-specific services
public class GameplayLifetimeScope : LifetimeScope { }
```

### Step 2: Register Services with Type-Safe Scopes

```csharp
using NhemDangFugBixs.Attributes;
using VContainer.Unity;

// Parent scope service (Singleton, accessible to all children)
[AutoRegisterIn(typeof(GameLifetimeScope), Lifetime = Lifetime.Singleton)]
public class GameService : ITickable {
    public float GameTime { get; private set; }
    public void Tick() => GameTime += UnityEngine.Time.deltaTime;
}

// Child scope service (Scoped, can inject parent services) ✅
[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Scoped)]
public class EnemySpawner : IInitializable {
    // Parent scope service injected into child scope - works!
    public EnemySpawner(GameService gameService) { }
    
    public void Initialize() { }
}
```

---

## 🏗️ Cross-Layer Bridge (NEW in v3.1)

If your project is split into multiple assemblies (e.g., `Core.asmdef`, `Gameplay.asmdef`, `Main.asmdef`), you can use **Identity Types** to register services from a low-level layer into a high-level scope without creating circular dependencies.

### Step 1: Define an Identity (In a base assembly)
```csharp
// Shared.asmdef
public class GameScope {} 
```

### Step 2: Use Identity in Services (In a low-level assembly)
```csharp
// Core.asmdef (References Shared)
[AutoRegisterIn(typeof(GameScope))]
public class MyService {}
```

### Step 3: Map Identity to LifetimeScope (In a high-level assembly)
```csharp
// Main.asmdef (References Core and Shared)
[LifetimeScopeFor(typeof(GameScope))]
public class GameLifetimeScope : LifetimeScope {
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
// Primary syntax - typeof() for type safety
[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class EnemySpawner { }

// Cross-layer syntax - use an Identity Type (v3.1+)
[AutoRegisterIn(typeof(GameScope))]
public class DecoupledService { }
```

**Benefits:**
- ✅ Full IntelliSense support (IDE suggests scope types)
- ✅ Compile-time validation (typos become compiler errors)
- ✅ Refactoring-safe (rename scope class → all usages update)
- ✅ **Layer-Safe** (v3.1): No circular dependencies in complex asmdef setups.
- ✅ Convention-based method names (`GameplayLifetimeScope` → `RegisterGameplay()`)

### 2. Convention-Based Naming

The generator automatically strips "LifetimeScope" suffix:

| Scope Type Name | Generated Method |
|-----------------|------------------|
| `GameLifetimeScope` | `RegisterGame()` |
| `GameplayLifetimeScope` | `RegisterGameplay()` |
| `UILifetimeScope` | `RegisterUI()` |
| `DungeonLifetimeScope` | `RegisterDungeon()` |

Override with `[ScopeName("Custom")]` when needed.

### 3. Parent-Child Scope Injection

Child scopes can inject parent scope services (but not vice versa):

```csharp
// ✅ VALID: Child scope injecting parent service
[AutoRegisterIn(typeof(GameLifetimeScope))]  // Parent
public class GameService { }

[AutoRegisterIn(typeof(GameplayLifetimeScope))]  // Child
public class EnemySpawner {
    public EnemySpawner(GameService game) { } // ✅ Works!
}

// ❌ INVALID: Parent scope injecting child service
// Analyzer produces warning ND004
```

### 4. Automatic EntryPoints

Classes implementing VContainer lifecycle interfaces are automatically registered using `RegisterEntryPoint`.

```csharp
[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class SmoothCamera : ITickable {
    public void Tick() { /* ... */ }
}
// Generates: builder.RegisterEntryPoint<SmoothCamera>(Lifetime.Scoped);
```

### 5. MonoBehaviour Support

Handles components based on their presence in the scene.

```csharp
// Find existing instance in Hierarchy
[AutoRegisterIn(typeof(GameplayLifetimeScope), RegisterInHierarchy = true)]
public class AudioManager : MonoBehaviour { }

// Or create on a new GameObject
[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class PooledObject : MonoBehaviour { }
```

### 6. Explicit Binding (`AsTypes`)

Restrict bindings to specific interfaces instead of all implemented ones.

```csharp
[AutoRegisterIn(typeof(GameLifetimeScope), AsTypes = new[] { typeof(IDebugOnly) })]
public class DebugService : IDebugOnly, IInternalSystem { }
```

### 7. Direct Factory Registration

Register a `Func<T>` factory for objects that need to be created at runtime.

```csharp
[AutoRegisterFactory]
public class Projectile { }

// Usage:
public class Weapon {
    public Weapon(Func<Projectile> projectileFactory) { ... }
}
```

---

## 📋 Diagnostic Codes

| Code | Severity | Description |
|------|----------|-------------|
| ND001 | Error | Scope type not found |
| ND002 | Error | Type must inherit from LifetimeScope |
| ND003 | Error | Circular scope dependency detected |
| ND004 | Warning | Parent scope cannot depend on child scope |
| ND103 | Warning | Scope has registrations but no LifetimeScope calls Register method |

---

## 🗺 Future Roadmap

- [ ] **Advanced Factory Patterns**: Support for `IFactory<T>` and parameterized factories.
- [ ] **Object Pooling**: `[AutoRegisterPool]` for automatic pool registration.
- [ ] **Signal Bus Integration**: `[SubscribeSignal]` for automatic event subscription.
- [ ] **Editor Scope Viewer**: Visual hierarchy of LifetimeScopes in Unity Editor.

---

## 📜 License

This project is part of the NhemDangFugBixs Tooling collection.
