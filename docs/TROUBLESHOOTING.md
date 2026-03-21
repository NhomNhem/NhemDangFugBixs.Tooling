# Troubleshooting Guide

This guide helps you resolve common issues with NhemDangFugBixs.Tooling diagnostics.

## Quick Reference

| Diagnostic | Issue | Quick Fix |
|------------|-------|-----------|
| [ND001](#nd001-invalid-autoregisterin-target) | Static/abstract class with [AutoRegisterIn] | Make non-static/non-abstract |
| [ND002](#nd002-missing-interface-implementation) | No interface on service | Implement an interface |
| [ND003](#nd003-invalid-constructor) | Multiple constructors | Use single constructor or [Inject] |
| [ND005](#nd005-registration-conflict) | Double registration | Remove manual or attribute |
| [ND006](#nd006-cross-scope-dependency) | Unreachable scope dependency | Add bridge or move service |
| [ND008](#nd008-missing-messagepipe-broker) | MessagePipe missing | Add [AutoRegisterMessageBrokerIn] |
| [ND009](#nd009-missing-ilogger-infrastructure) | ILogger missing | Register ILoggerFactory |
| [ND105](#nd105-installer-missing-constructor) | Installer needs constructor | Add parameterless constructor |
| [ND106](#nd106-installer-not-public) | Installer not accessible | Make public |
| [ND107](#nd107-installer-is-component) | Installer inherits Component | Remove inheritance |
| [ND108](#nd108-entrypoint-needs-asself) | EntryPoint needs AsSelf | Add .AsSelf() |
| [ND110](#nd110-view-not-registered) | View interface not registered | Add RegisterInHierarchy |
| [ND111](#nd111-missing-contract) | Interface not registered | Enable AsImplementedInterfaces |
| [ND112](#nd112-duplicate-contract) | Same interface registered twice | Remove one registration |
| [ND113](#nd113-view-binding-mismatch) | View not registered in scope | Add [AutoRegisterIn] to view |

---

## Detailed Diagnostics

### ND001: Invalid AutoRegisterIn Target

**Error Message:**
```
Class 'MyService' must be non-static and non-abstract to use [AutoRegisterIn].
Fix: remove [AutoRegisterIn] or make the class non-static/non-abstract.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public static class MyService { } // ❌ Static class
```

**Solution:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService { } // ✅ Regular class
```

**Why:** VContainer needs to instantiate services. Static/abstract classes cannot be instantiated.

---

### ND002: Missing Interface Implementation

**Error Message:**
```
Class 'MyService' with [AutoRegisterIn] should implement at least one interface or be a Component.
Fix: implement an interface or remove [AutoRegisterIn] if registration as concrete type is intended.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService { // ❌ No interface
    public void DoWork() { }
}
```

**Solutions:**

**Option A: Add Interface (Recommended)**
```csharp
public interface IMyService {
    void DoWork();
}

[AutoRegisterIn(typeof(GameScope))]
public class MyService : IMyService { // ✅ Implements interface
    public void DoWork() { }
}
```

**Option B: Suppress Warning (If Intentional)**
```csharp
#pragma warning disable ND002
[AutoRegisterIn(typeof(GameScope))]
public class MyService { // Concrete registration intended
    public void DoWork() { }
}
#pragma warning restore ND002
```

**Why:** Services without interfaces can only be resolved by concrete type, limiting testability and flexibility.

---

### ND003: Invalid Constructor

**Error Message:**
```
Class 'MyService' should have exactly one public constructor or use [Inject].
Fix: use a single public constructor or mark the preferred constructor with [Inject].
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService {
    public MyService() { } // ❌ Two constructors
    public MyService(IDependency dep) { }
}
```

**Solutions:**

**Option A: Use Single Constructor**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService {
    public MyService(IDependency dep) { } // ✅ Only one constructor
}
```

**Option B: Mark with [Inject]**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService {
    public MyService() { }
    
    [Inject] // ✅ Explicit marking
    public MyService(IDependency dep) { }
}
```

**Why:** VContainer needs to know which constructor to use for dependency injection.

---

### ND005: Registration Conflict

**Error Message:**
```
Conflict! Type 'MyService' is already marked for auto-registration via [AutoRegisterIn].
Remove the manual registration in 'GameScope' or remove the attribute from 'MyService'.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))] // ← Auto-registered
public class MyService { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        builder.Register<MyService>(Lifetime.Singleton); // ❌ Also manually registered
    }
}
```

**Solution (💡 Code Fix Available):**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService { }

public class GameScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        // ✅ Manual registration removed
    }
}
```

**Why:** Registering the same type twice causes runtime errors.

---

### ND006: Cross-Scope Dependency

**Error Message:**
```
Type 'GameService' (scope 'GameScope') depends on 'MenuService' (scope 'MenuScope'),
but 'MenuScope' is not reachable from 'GameScope'.
Fix: add a bridge scope mapping with [LifetimeScopeFor(typeof(...))] or move registration to a reachable scope.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class GameService {
    public GameService(MenuService menu) { } // ❌ MenuScope not accessible from GameScope
}

[AutoRegisterIn(typeof(MenuScope))]
public class MenuService { }
```

**Solution (💡 Code Fix Available):**

**Option A: Move to Common Parent Scope**
```csharp
[AutoRegisterIn(typeof(RootScope))] // ✅ Root is accessible from both
public class MenuService { }
```

**Option B: Add Scope Bridge**
```csharp
[LifetimeScopeFor(typeof(MenuScope))] // ✅ Bridge mapping
public class MenuIdentity { }

[AutoRegisterIn(typeof(GameScope))]
public class GameService {
    public GameService(MenuIdentity identity) { } // Use bridge
}
```

**Why:** VContainer scope hierarchy must be respected. Child scopes can't access siblings.

---

### ND008: Missing MessagePipe Broker

**Error Message:**
```
Type 'MyPublisher' in scope 'GameScope' depends on MessagePipe IPublisher<MyMessage>,
but no reachable broker registration exists.
Fix: add [AutoRegisterMessageBrokerIn] to a type in this scope or a parent scope.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyPublisher {
    public MyPublisher(IPublisher<MyMessage> publisher) { } // ❌ No broker
}
```

**Solution (💡 Code Fix Available):**
```csharp
[AutoRegisterMessageBrokerIn(typeof(GameScope))]
public class MyMessage { } // ✅ Broker registered

[AutoRegisterIn(typeof(GameScope))]
public class MyPublisher {
    public MyPublisher(IPublisher<MyMessage> publisher) { } // ✅ Now works
}
```

**Why:** MessagePipe requires explicit broker registration for each message type.

---

### ND009: Missing ILogger Infrastructure

**Error Message:**
```
Type 'MyService' in scope 'GameScope' depends on ILogger<MyService>,
but no reachable root logging setup exists.
Fix: register ILoggerFactory and ILogger<> in root scope or use [AutoRegisterRootLogging].
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class MyService {
    public MyService(ILogger<MyService> logger) { } // ❌ No logger setup
}
```

**Solution (💡 Code Fix Available):**
```csharp
[AutoRegisterRootLogging(typeof(RootScope))] // ✅ Logging infrastructure
public class LoggingSetup { }

[AutoRegisterIn(typeof(GameScope))]
public class MyService {
    public MyService(ILogger<MyService> logger) { } // ✅ Now works
}
```

**Why:** ILogger requires factory infrastructure registered in root scope.

---

### ND111: Missing Contract Registration

**Error Message:**
```
Type 'MyService' implements interface 'IMyService' but it will not be registered.
Fix: set AsImplementedInterfaces = true or add explicit AsTypes contracts.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))] // ❌ Interfaces not registered
public class MyService : IMyService {
    public MyService(IMyService service) { } // Can't resolve
}
```

**Solution (💡 Code Fix Available):**
```csharp
[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)] // ✅ Registers IMyService
public class MyService : IMyService {
    public MyService(IMyService service) { } // ✅ Now works
}
```

**Why:** By default, only the concrete type is registered. Interfaces need explicit opt-in.

---

### ND112: Duplicate Contract Registration

**Error Message:**
```
Interface 'IMyService' is registered by multiple types in scope 'GameScope': MyService, OtherService.
Fix: remove one duplicate registration or narrow contracts with AsTypes.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)]
public class MyService : IMyService { } // ❌ Both register IMyService

[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)]
public class OtherService : IMyService { }
```

**Solution (💡 Code Fix Available):**

**Option A: Remove AsImplementedInterfaces from One**
```csharp
[AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)]
public class MyService : IMyService { } // ✅ Primary

[AutoRegisterIn(typeof(GameScope))] // ✅ Only concrete type
public class OtherService : IMyService { }
```

**Option B: Use AsTypes to Narrow**
```csharp
[AutoRegisterIn(typeof(GameScope), AsTypes = new[] { typeof(IMyService) })]
public class MyService : IMyService, IOtherInterface { } // Only IMyService

[AutoRegisterIn(typeof(GameScope), AsTypes = new[] { typeof(IOtherInterface) })]
public class OtherService : IMyService, IOtherInterface { } // Only IOtherInterface
```

**Why:** VContainer can't resolve an interface registered by multiple types (ambiguous).

---

### ND113: View Binding Mismatch

**Error Message:**
```
Presenter 'HealthPresenter' depends on view interface 'IHealthView' but no MonoBehaviour
implementing it is registered in scope 'GameScope'.
Fix: add [AutoRegisterIn(typeof(GameScope), RegisterInHierarchy = true)] to the view or register manually.
```

**Problem:**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class HealthPresenter {
    public HealthPresenter(IHealthView view) { } // ❌ View not registered
}

public class HealthView : MonoBehaviour, IHealthView { } // Not registered
```

**Solution (💡 Code Fix Available):**
```csharp
[AutoRegisterIn(typeof(GameScope))]
public class HealthPresenter {
    public HealthPresenter(IHealthView view) { } // ✅ Now works
}

[AutoRegisterIn(typeof(GameScope), RegisterInHierarchy = true)] // ✅ Register view
public class HealthView : MonoBehaviour, IHealthView { }
```

**Why:** MonoBehaviours must be registered with `RegisterInHierarchy = true` to find instances in the scene.

---

## General Troubleshooting

### Diagnostics Not Appearing

**Check:**
1. ✅ Analyzers DLL is in `Assets/Plugins/Analyzers/`
2. ✅ DLL is marked as "Roslyn Analyzer" in Inspector
3. ✅ Reimport DLL: Right-click → Reimport
4. ✅ Restart Unity

### Code Fixes Not Working

**Check:**
1. ✅ Using Visual Studio or Rider (not Unity Editor)
2. ✅ Lightbulb appears when cursor on diagnostic
3. ✅ Try `Ctrl+.` (VS) or `Alt+Enter` (Rider)

### Generated Code Missing

**Check:**
1. ✅ Rebuild project
2. ✅ Check for compilation errors
3. ✅ Look in `Temp/GeneratedCode/`

---

**Still having issues?** [Open an issue on GitHub](https://github.com/NhomNhem/NhemDangFugBixs.Tooling/issues)
