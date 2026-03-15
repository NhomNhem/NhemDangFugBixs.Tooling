# v3.0 Migration Guide for GameFeelUnity

## ⚡ Quick Start (AUTOMATIC!)

**Great news!** The Source Generator now **automatically generates** the global usings file for you!

When Unity compiles, the generator creates:
```
Assets/Scripts/Generated/NhemDangFugBixs.Generators/NhemDangFugBixs.GlobalUsings.g.cs
```

This file contains:
```csharp
global using NLifetime = NhemDangFugBixs.Attributes.Lifetime;
```

**You don't need to do anything!** Just migrate your attributes:

```csharp
// BEFORE (v2.x):
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Transient, scope: "Gameplay")]

// AFTER (v3.0):
[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = NLifetime.Transient)]
```

**No manual setup needed!** The generator handles it automatically.

---

## Migration Summary

**Total Files to Migrate:** 9 files

### Scope Mapping

| Old Scope Name | New Scope Type |
|---------------|----------------|
| `"Gameplay"` | `GameplayLifetimeScope` |
| `"Game"` | `GameLifetimeScope` |
| `"Global"` | `GameLifetimeScope` |

---

## File-by-File Migration

### 1. HealthPresenter.cs

**Location:** `Assets/Scripts/Runtime/Core/Combat/HealthPresenter.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Transient, scope: "Gameplay")]
public class HealthPresenter : IHealthPresenter {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using
using NLifetime = NhemDangFugBixs.Attributes.Lifetime; // Alias for convenience

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = NLifetime.Transient)]
public class HealthPresenter : IHealthPresenter {
```

---

### 2. PlayerAnimationPresenter.cs

**Location:** `Assets/Scripts/Runtime/Core/Player/PlayerAnimationPresenter.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, scope: "Gameplay")]
public class PlayerAnimationPresenter : IPlayerAnimationPresenter {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Singleton)]
public class PlayerAnimationPresenter : IPlayerAnimationPresenter {
```

**Note:** Also ensure `IPlayerAnimationPresenter` interface exists or remove the reference.

---

### 3. BulletPresenter.cs

**Location:** `Assets/Scripts/Runtime/Core/Combat/BulletPresenter.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Transient, scope: "Gameplay")]
public class BulletPresenter {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Transient)]
public class BulletPresenter {
```

---

### 4. PlayerHungerPresenter.cs

**Location:** `Assets/Scripts/Runtime/Core/Player/PlayerHungerPresenter.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, scope: "Gameplay")]
public class PlayerHungerPresenter {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Singleton)]
public class PlayerHungerPresenter {
```

---

### 5. PlayerPresenter.cs

**Location:** `Assets/Scripts/Runtime/Core/Player/PlayerPresenter.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, scope: "Gameplay", AsSelf = true)]
public class PlayerPresenter : IPlayerPresenter, ITickable {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using
using VContainer.Unity; // For ITickable

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Singleton, AsSelf = true)]
public class PlayerPresenter : IPlayerPresenter, ITickable {
```

---

### 6. EnemyPresenter.cs

**Location:** `Assets/Scripts/Runtime/Core/Enemy/EnemyPresenter.cs`

**Before (v2.x):**
```csharp
[NhemDangFugBixs.Attributes.AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Transient, scope: "Gameplay")]
public class EnemyPresenter : IEnemyPresenter, ITickable {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using
using VContainer.Unity; // For ITickable

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Transient)]
public class EnemyPresenter : IEnemyPresenter, ITickable {
```

---

### 7. EnemyPoolManager.cs

**Location:** `Assets/Scripts/Runtime/Gameplay/Enemy/EnemyPoolManager.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, scope: "Gameplay")]
public class EnemyPoolManager {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Singleton)]
public class EnemyPoolManager {
```

---

### 8. MapPoolManager.cs

**Location:** `Assets/Scripts/Runtime/Gameplay/Map/MapPoolManager.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, scope: "Gameplay")]
public class MapPoolManager {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Singleton)]
public class MapPoolManager {
```

---

### 9. DeterministicWorldGenerator.cs

**Location:** `Assets/Scripts/Runtime/Services/Map/DeterministicWorldGenerator.cs`

**Before (v2.x):**
```csharp
[AutoRegister(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, scope: "Gameplay")]
public class DeterministicWorldGenerator {
```

**After (v3.0):**
```csharp
using Assets.Scripts.Runtime.Tests.Scopes; // Add this using

[AutoRegisterIn(typeof(GameplayLifetimeScope), Lifetime = Lifetime.Singleton)]
public class DeterministicWorldGenerator {
```

---

## Required Using Statements

Add these to the top of each migrated file:

```csharp
using NhemDangFugBixs.Attributes;  // For AutoRegisterIn<T>
using VContainer.Unity;             // For Lifetime enum and ITickable/IInitializable
using Assets.Scripts.Runtime.Tests.Scopes; // For GameplayLifetimeScope
```

## Verification Checklist

After making changes:

- [ ] All `[AutoRegister(...)]` replaced with `[AutoRegisterIn(typeof(GameplayLifetimeScope), ...)]`
- [ ] All `scope: "Gameplay"` removed (replaced by generic type parameter)
- [ ] All `NhemDangFugBixs.Attributes.Lifetime.` prefixes removed (just use `Lifetime.`)
- [ ] Added `using Assets.Scripts.Runtime.Tests.Scopes;` to each file
- [ ] Added `using VContainer.Unity;` for files using ITickable/IInitializable
- [ ] Build succeeds with no errors

## Expected Build Output

After migration, you should see:
- ✅ 0 errors
- ⚠️ 0 warnings (obsolete warnings should be gone)
- ✅ Generated code at `Assets/Scripts/Generated/NhemDangFugBixs.Generators/`

---

## Quick Fix Script (Optional)

For advanced users, you can use Find & Replace in your IDE:

**Find:**
```
\[AutoRegister\(Lifetime = NhemDangFugBixs.Attributes.Lifetime.(\w+), scope: "(\w+)"\)
```

**Replace:**
```
[AutoRegisterIn(typeof($2LifetimeScope), Lifetime = Lifetime.$1)
```

**Then manually add the using statements.**
