# GameFeelUnity Assembly Analysis & v3.0 Migration Strategy

## Current Assembly Structure

```
┌─────────────────────────────────────────────────────────────────┐
│                    Assembly Dependency Graph                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Runtime.asmdef (GameFeel_Runtime)                              │
│  ├── References: All below assemblies                          │
│  └── Contains: Runtime layer bootstrap                          │
│                                                                 │
│  Core.asmdef (GameFeel_Core)                                    │
│  ├── References: Shared, Data, VContainer, MessagePipe         │
│  └── Contains: Core game logic, Presenters                     │
│      └── ❌ ERROR: Uses [AutoRegisterIn<GameplayLifetimeScope>]│
│                                                                 │
│  Gameplay.asmdef (GameFeel_Gameplay)                            │
│  ├── References: Core, Services, Shared, Data                  │
│  └── Contains: Gameplay-specific logic, EnemyPoolManager       │
│      └── ❌ ERROR: Uses [AutoRegisterIn<GameplayLifetimeScope>]│
│                                                                 │
│  Services.asmdef (GameFeel_Services)                            │
│  ├── References: Shared, Data, VContainer                      │
│  └── Contains: Service layer, DeterministicWorldGenerator      │
│      └── ❌ ERROR: Uses [AutoRegisterIn<GameplayLifetimeScope>]│
│                                                                 │
│  Data.asmdef (GameFeel_Data)                                    │
│  ├── References: Shared                                        │
│  └── Contains: Data models, Settings                           │
│                                                                 │
│  Shared.asmdef (GameFeel_Shared)                                │
│  ├── References: None (base layer)                             │
│  └── Contains: Shared interfaces, models                       │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Problem Identified

**Your LifetimeScopes are likely in one of these assemblies**, but services in **other assemblies** are trying to reference them:

```
Core.asmdef ──[references]──> GameplayLifetimeScope ???
    ❌ Can't resolve - scope not in reference chain!
```

## ✅ Recommended Solution: Create Scopes Assembly

### Step 1: Create New Assembly Definition

**File:** `Assets/Scripts/Runtime/Scopes/Scopes.asmdef`

```json
{
    "name": "GameFeel_Scopes",
    "rootNamespace": "GameFeelUnity.Scopes",
    "references": [
        "VContainer"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

### Step 2: Move LifetimeScopes Here

Move these files to `Assets/Scripts/Runtime/Scopes/`:
- `GameLifetimeScope.cs`
- `GameplayLifetimeScope.cs`
- Any other LifetimeScope classes

### Step 3: Add Reference to All Assemblies That Use Scopes

Update these `.asmdef` files to reference `GameFeel_Scopes`:

**Core.asmdef:**
```json
"references": [
    "GUID:xxxxx", // GameFeel_Scopes - ADD THIS
    // ... existing references
]
```

**Gameplay.asmdef:**
```json
"references": [
    "GUID:xxxxx", // GameFeel_Scopes - ADD THIS
    // ... existing references
]
```

**Services.asmdef:**
```json
"references": [
    "GUID:xxxxx", // GameFeel_Scopes - ADD THIS
    // ... existing references
]
```

### Step 4: Update Using Statements

In each scope file, ensure namespace is correct:

```csharp
namespace GameFeelUnity.Scopes {
    public class GameLifetimeScope : LifetimeScope {
        // ...
    }
}
```

In service files, add:

```csharp
using GameFeelUnity.Scopes; // For GameplayLifetimeScope
```

---

## Alternative Solution (Quick Fix)

If you don't want to create a new assembly, **put scopes in Shared.asmdef**:

**Pros:**
- ✅ All assemblies already reference Shared
- ✅ No new assembly definition needed
- ✅ Quick to implement

**Cons:**
- ❌ Shared should be for interfaces/models only
- ❌ VContainer dependency in Shared (may not be desired)

**If choosing this option:**
1. Move LifetimeScope files to `Assets/Scripts/Runtime/Shared/Scopes/`
2. Add `VContainer` reference to `Shared.asmdef`
3. Update namespace to `GameFeelUnity.Shared.Scopes`

---

## Final Assembly Structure (Recommended)

```
┌─────────────────────────────────────────────────────────────────┐
│             Recommended Assembly Structure                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Runtime                                                        │
│  ├── Scopes (NEW!) ← All LifetimeScopes here                   │
│  │   ├── GameLifetimeScope.cs                                  │
│  │   └── GameplayLifetimeScope.cs                              │
│  │                                                             │
│  ├── Core                                                      │
│  │   ├── PlayerPresenter.cs                                    │
│  │   ├── EnemyPresenter.cs                                     │
│  │   └── ...                                                   │
│  │                                                             │
│  ├── Gameplay                                                  │
│  │   ├── EnemyPoolManager.cs                                   │
│  │   └── ...                                                   │
│  │                                                             │
│  ├── Services                                                  │
│  │   └── DeterministicWorldGenerator.cs                        │
│  │                                                             │
│  ├── Data                                                      │
│  └── Shared                                                    │
│                                                                 │
│  Dependency Flow:                                               │
│  Scopes ← Core ← Gameplay                                      │
│       ↑    ↑      ↑                                            │
│       └────┴──────┘                                            │
│            ↓                                                   │
│         Services                                               │
│                                                                 │
│  All assemblies that use [AutoRegisterIn] reference Scopes     │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Migration Checklist

- [ ] Create `Assets/Scripts/Runtime/Scopes/` folder
- [ ] Create `Scopes.asmdef` with VContainer reference
- [ ] Move `GameLifetimeScope.cs` to Scopes folder
- [ ] Move `GameplayLifetimeScope.cs` to Scopes folder
- [ ] Update namespace in scope files to `GameFeelUnity.Scopes`
- [ ] Add Scopes assembly reference to Core.asmdef
- [ ] Add Scopes assembly reference to Gameplay.asmdef
- [ ] Add Scopes assembly reference to Services.asmdef
- [ ] Add `using GameFeelUnity.Scopes;` to service files
- [ ] Rebuild in Unity
- [ ] Verify no more "Cannot resolve symbol" errors

---

## Files Using AutoRegisterIn (Need Scopes Reference)

| File | Current Assembly | Needs Scopes Reference |
|------|-----------------|------------------------|
| `HealthPresenter.cs` | Core | ✅ Yes |
| `PlayerAnimationPresenter.cs` | Core | ✅ Yes |
| `BulletPresenter.cs` | Core | ✅ Yes |
| `PlayerHungerPresenter.cs` | Core | ✅ Yes |
| `PlayerPresenter.cs` | Core | ✅ Yes |
| `EnemyPresenter.cs` | Core | ✅ Yes |
| `EnemyPoolManager.cs` | Gameplay | ✅ Yes |
| `MapPoolManager.cs` | Gameplay | ✅ Yes |
| `DeterministicWorldGenerator.cs` | Services | ✅ Yes |

---

## Why This Works

The Source Generator runs **during compilation** of each assembly. When it processes `EnemyPoolManager.cs` in Gameplay assembly:

1. It sees `[AutoRegisterIn<GameplayLifetimeScope>]`
2. It needs to resolve `GameplayLifetimeScope` type
3. With Scopes assembly referenced, it can find the type
4. Generator validates the type exists and inherits LifetimeScope
5. Code generation succeeds!

Without the reference, step 3 fails → "Cannot resolve symbol" error.
