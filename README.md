# NhemDangFugBixs.VContainer.SourceGenerator

A powerful C# Source Generator for **VContainer** that automates dependency registration in Unity projects. Reduce boilerplate code and maintain a clean architecture by using simple attributes.

## 🌟 Key Features

- **Module/Installer Pattern** (v4.0): Decouple complex registration logic into dedicated `IVContainerInstaller` classes. The generator automatically instantiates, sorts, and executes them before other registrations.
- **Cross-Layer Discovery** (v3.1): Register services across different assemblies (`asmdef`) using **Identity Types**. Break the circular dependency trap in layered architectures!
- **Type-Safe Scopes** (v3.0): Register services to specific `LifetimeScope` types using `[AutoRegisterIn(typeof(TScope))]` - full IntelliSense support!
- **Unified Master Registration**: Single `VContainerRegistration.RegisterAll(builder)` call in your `LifetimeScope` registers every discovered service across all layers.
- **Robust Code Emission**: Generated code uses `partial` classes and `global::` prefixes, ensuring compatibility with complex multi-assembly projects.
- **Compile-Time Validation**: Roslyn analyzers catch scope and installer errors before runtime (ND001-ND110).
- **Code Fix Providers** (v5.1.0): IDE auto-fix for common DI mistakes (ND008, ND009, ND110).
- **Preflight CLI** (v5.1.0): `dotnet di-smoke` command for validation before Unity Play Mode.
- **Runtime Resolve Smoke** (v5.1.0): Headless validation of DI container resolution.

---

## 🏗️ Module/Installer Pattern (v4.0)

For complex configurations (third-party libs, ScriptableObjects, factories), use **Installers** to keep your `LifetimeScope` clean.

### 1. Define an Installer
```csharp
using NhemDangFugBixs.Attributes;
using VContainer;

[AutoRegisterIn(typeof(GameScope))]
[InstallerOrder(-100)] // Run before other installers
public class LoggingInstaller : IVContainerInstaller {
    public void Install(IContainerBuilder builder) {
        builder.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
        // Add complex setup here...
    }
}
```

### 2. Automatic Execution
The generator detects the installer and emits it into the scope registration:
```csharp
// Generated code
public static void RegisterGame(IContainerBuilder builder) {
    new global::LoggingInstaller().Install(builder);
    // ... other registrations
}
```

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

[AutoRegisterIn(typeof(GameplayScope), Lifetime = NhemLifetime.Scoped)]
public class EnemyPoolManager : IEnemyPoolManager, IDisposable { }
```

#### 3. Map Identity to LifetimeScope (Main Layer)
```csharp
// Main.asmdef (References Core and Shared)
using NhemDangFugBixs.Attributes;

[LifetimeScopeFor(typeof(GameplayScope))]
public class GameplayLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        VContainerRegistration.RegisterAll(builder);
    }
}
```

---

## 🛠 Feature Guide

### 1. Type-Safe Scope Registration (v3.0+)

The recommended way to register services with compile-time safety:

```csharp
[AutoRegisterIn(typeof(GameplayLifetimeScope))]
public class EnemySpawner { }
```

**Benefits:**
- ✅ Full IntelliSense support
- ✅ Compile-time validation
- ✅ Refactoring-safe

### 2. Convention-Based Naming

The generator automatically strips "LifetimeScope" suffix:

| Scope Type Name | Generated Method |
|-----------------|------------------|
| `GameLifetimeScope` | `RegisterGame()` |
| `GameplayLifetimeScope` | `RegisterGameplay()` |

---

## ⚠️ Migration Guide (v3.x → v4.0)

v4.0 is a major breaking change that removes legacy attributes and introduces stricter validation.

### BREAKING CHANGES:
- **Removed**: `[AutoRegister(string scope)]` (Legacy string-based registration).
- **Removed**: `[AutoRegisterFactory]` (Use `IVContainerInstaller` for complex factory setup).
- **Stricter Validation**: Installers MUST be `public` and have a parameterless constructor.

### Migration Steps:
1. Replace any remaining `[AutoRegister("Name")]` with `[AutoRegisterIn(typeof(Scope))]`.
2. Convert `[AutoRegisterFactory]` logic into an `IVContainerInstaller` implementation.
3. Ensure all services use `NhemLifetime` instead of the deprecated `Lifetime` enum alias if applicable.

---

## 🛠️ Preflight CLI (v5.1.0)

Validate your DI setup before entering Unity Play Mode with the `dotnet di-smoke` CLI tool.

### Installation
```bash
dotnet tool install -g DangFugBixs.Cli
```

### Commands

**Preflight Validation** (clean + build + validate):
```bash
dotnet di-smoke preflight MyGame.csproj
dotnet di-smoke preflight MyGame.csproj --clean
dotnet di-smoke preflight MyGame.csproj --resolve-smoke
dotnet di-smoke preflight MyGame.csproj --format json --output report.json
```

**Direct Validation** (existing assembly):
```bash
dotnet di-smoke validate bin/Debug/net10.0/MyGame.dll
dotnet di-smoke validate bin/Debug/net10.0/MyGame.dll --format json
```

### Options
| Option | Description |
|--------|-------------|
| `--format <text|json>` | Output format (default: text) |
| `--output <file>` | Save report to file |
| `--clean` | Clean generated files before build |
| `--resolve-smoke` | Run runtime resolve smoke test |

---

## 💡 Code Fix Providers (v5.1.0)

Auto-fix common DI mistakes directly in Visual Studio / Rider.

### ND008: MessagePipe Broker Registration
**When:** Type injects `IPublisher<T>` or `ISubscriber<T>` but no broker registered

**Fix Options:**
1. Create broker class with `[AutoRegisterMessageBrokerIn]`
2. Add manual registration to scope

### ND009: ILogger Root Infrastructure
**When:** Type injects `ILogger<T>` but root scope missing LoggerFactory

**Fix Options:**
1. Add LoggerFactory registration snippet
2. Navigate to root scope

### ND110: View Component Registration
**When:** Type injects `I*View` interface but view not registered as Component

**Fix Options:**
1. Add `[AutoRegisterIn]` to View class
2. Register with `RegisterComponentInHierarchy()` in Installer

---

## 📋 Diagnostic Codes

### Analyzer Rules

| Code | Severity | Description |
|------|----------|-------------|
| ND001 | Error | Invalid AutoRegisterIn target (Static/Abstract) |
| ND002 | Warning | Missing interface implementation |
| ND003 | Warning | Invalid constructor for VContainer |
| ND005 | Error | Manual registration of auto-registered type (double registration) |
| ND006 | Warning | Inaccessible dependency scope (cross-scope violation) |
| ND008 | Warning | Missing MessagePipe broker registration |
| ND009 | Warning | Missing ILogger root infrastructure |
| ND103 | Warning | Unused scope registration |
| ND104 | Warning | Assembly scan failure (resilience) |
| ND105 | Error | Installer missing public parameterless constructor |
| ND106 | Error | Installer must be public |
| ND107 | Error | Installer cannot be a Component (MonoBehaviour) |
| ND108 | Warning | EntryPoint must use .AsSelf() or implement interface |
| ND110 | Error | View interface injection requires Component registration |

### Code Fix Providers

| Code | Fix Available | Description |
|------|---------------|-------------|
| ND008 | ✅ Yes | MessagePipe Broker registration snippet |
| ND009 | ✅ Yes | ILogger Factory registration snippet |
| ND110 | ✅ Yes | View Component registration snippet |

---

## 📜 License

This project is part of the NhemDangFugBixs Tooling collection.
