## Context

**Current State (v2.x):**
- Scope registration uses string-based syntax: `[AutoRegister(scope: "Gameplay")]`
- Method names manually derived from scope strings
- No compile-time validation—typos cause runtime failures
- No IDE autocomplete for scope names
- As projects scale (2 → 10+ scopes), error surface grows linearly

**Constraints:**
- Must maintain backward compatibility during migration (v2.x → v3.0)
- Generator must remain an `IIncrementalGenerator` for IDE performance
- VContainer integration must remain unchanged (no VContainer modifications)
- Unity 2021.3+ compatibility required (Roslyn Analyzer support)

**Stakeholders:**
- Primary: NhomNhem (author/maintainer)
- Users: Unity developers using VContainer for DI

## Goals / Non-Goals

**Goals:**
- Type-safe scope references using C# generics (`[AutoRegisterIn<TScope>]`)
- Convention-based method naming (auto-strip "LifetimeScope" suffix)
- Compile-time validation via Roslyn diagnostics (ND001-ND004)
- Parent-child scope hierarchy validation
- Clean migration path from v2.x

**Non-Goals:**
- Runtime scope discovery/reflection (compile-time only)
- Automatic scope registration (explicit wiring remains)
- VContainer modifications (works with existing VContainer API)
- Object pooling or signal bus integration (future v3.1+ features)

## Decisions

### Decision 1: Generic Attribute Syntax

**Choice:** `[AutoRegisterIn<TScope>]` as primary syntax

**Alternatives Considered:**
- `typeof()` parameter: `[AutoRegister(scope: typeof(TScope))]` — more verbose
- String with convention: `[AutoRegister(scope: "Gameplay")]` — no type safety

**Rationale:**
- Generic syntax is 30% less typing than `typeof()`
- Full IntelliSense support (IDE suggests scope types)
- Compile-time type checking (can't pass non-LifetimeScope type)
- `typeof()` fallback retained for edge cases (dynamic scope selection)

### Decision 2: Convention-Based Naming

**Choice:** Auto-strip "LifetimeScope" suffix for method names

```
GameplayLifetimeScope → RegisterGameplay()
GameLifetimeScope → RegisterGame()
```

**Alternatives Considered:**
- Full type name: `RegisterGameplayLifetimeScope()` — too verbose
- Custom attribute: `[ScopeName("Gameplay")]` — extra boilerplate

**Rationale:**
- Matches common Unity naming conventions
- No extra attributes needed for 90% of cases
- Override available via `[ScopeName("Custom")]` for edge cases

### Decision 3: Explicit Parent-Child Wiring

**Choice:** User manually calls child registrations; analyzer validates

```csharp
public class GameLifetimeScope : LifetimeScope {
    protected override void Configure(IContainerBuilder builder) {
        VContainerRegistration.RegisterGame(builder);
        VContainerRegistration.RegisterGameplay(builder); // Explicit
    }
}
```

**Alternatives Considered:**
- Automatic chaining via `[ScopeParent(typeof(Game))]` — hidden behavior
- Attribute-based discovery — magic, harder to debug

**Rationale:**
- Explicit is clearer for debugging
- Analyzer safety net prevents forgetting registrations
- Matches VContainer's explicit registration philosophy

### Decision 4: Breaking Change Strategy

**Choice:** v3.0 removes string-based scopes entirely

**Alternatives Considered:**
- Deprecation cycle (v3.0 warn, v4.0 remove) — carries legacy forever
- Dual support indefinitely — increased complexity

**Rationale:**
- Early stage project (low migration cost)
- Clean API forever vs. permanent legacy burden
- Migration is simple find/replace operation

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    v3.0 ARCHITECTURE                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Runtime (DangFugBixs.Runtime)                                  │
│  ─────────────────────────────────                              │
│  - AutoRegisterInAttribute<TScope> (new)                        │
│  - ScopeNameAttribute (new)                                     │
│  - AutoRegisterAttribute (unchanged, string scope deprecated)   │
│                                                                 │
│  Generator (DangFugBixs.Generators)                             │
│  ─────────────────────────────────                              │
│  - ClassAnalyzer: Extract scope type from generic argument      │
│  - ServiceInfo: Add ScopeType property                          │
│  - RegistrationEmitter: Convention-based naming                 │
│                                                                 │
│  Analyzers (DangFugBixs.Analyzers)                              │
│  ─────────────────────────────────                              │
│  - ND001: Scope type not found                                  │
│  - ND002: Invalid scope type (not LifetimeScope)                │
│  - ND003: Circular scope dependency                             │
│  - ND004: Parent→Child dependency violation                     │
│  - ND103: Unused scope registration (warning)                   │
│                                                                 │
│  Generated Output                                               │
│  ─────────────────                                              │
│  - {Assembly}.VContainerRegistration.g.cs                       │
│    - RegisterGame()                                             │
│    - RegisterGameplay()                                         │
│    - etc.                                                       │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Data Flow

```
User Code                          Generator Pipeline
───────────                        ──────────────────

[AutoRegisterIn<GameplayScope>]    ClassAnalyzer extracts:
public class EnemySpawner { }      - Type: EnemySpawner
                                   - ScopeType: GameplayLifetimeScope
                                   - Validates: inherits LifetimeScope
                                           
                                   ServiceInfo created:
                                   - ScopeType = typeof(GameplayLifetimeScope)
                                           
                                   RegistrationEmitter generates:
                                   builder.Register<EnemySpawner>(...)
                                           
                                   Method naming:
                                   "GameplayLifetimeScope" → "RegisterGameplay"
                                           
                                   Output:
                                   public static void RegisterGameplay(...) { }
```

## Risks / Trade-offs

| Risk | Impact | Mitigation |
|------|--------|------------|
| **Breaking change frustrates users** | Medium | Clear migration guide, simple find/replace |
| **Convention doesn't fit all names** | Low | `[ScopeName("Custom")]` override available |
| **Generator complexity increases** | Medium | Incremental approach, thorough testing |
| **Circular scope dependencies** | High | ND003 diagnostic catches at compile-time |
| **Parent→Child injection confusion** | Medium | Documentation + ND004 warning |

## Migration Plan

### For Existing v2.x Code

**Step 1: Update attribute syntax**
```bash
# Find: [AutoRegister(scope: "
# Replace: [AutoRegisterIn<

# Manual fixes:
[AutoRegister(scope: "Gameplay")] → [AutoRegisterIn<GameplayLifetimeScope>]
[AutoRegister(scope: "Game")] → [AutoRegisterIn<GameLifetimeScope>]
```

**Step 2: Update Configure() calls**
```csharp
// Old (still works, method names unchanged)
VContainerRegistration.RegisterGameplay(builder);

// New (same method names, convention-based)
VContainerRegistration.RegisterGameplay(builder); // No change needed!
```

**Step 3: Build and fix diagnostics**
- Fix any ND001-ND004 errors
- Verify scope types exist and inherit `LifetimeScope`

### Rollback Strategy

If v3.0 has critical issues:
1. Revert to v2.x package version
2. No code changes needed (syntax is backward compatible during transition)
3. Report issues, wait for v3.0.1

## Open Questions

1. **Should we support multiple scopes per class?**
   - `[AutoRegisterIn<Game>, AutoRegisterIn<Gameplay>]`
   - Decision: Defer to v3.1 (low priority, adds complexity)

2. **Should scope hierarchy be attribute-declared?**
   - `[ScopeParent(typeof(Game))]` on `GameplayLifetimeScope`
   - Decision: No—explicit wiring is clearer (see Decision 3)

3. **Should we auto-generate scope bootstrapping?**
   - `ScopeRegistry.AutoRegisterForScope(this, builder)`
   - Decision: Defer to v3.1 (nice-to-have, not essential)
