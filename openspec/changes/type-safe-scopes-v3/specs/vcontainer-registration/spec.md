## MODIFIED Requirements

### Requirement: VContainer registration method generation
The generator SHALL generate registration methods for each scope using convention-based naming instead of string-based scope names.

**Previous Behavior:**
- Method names derived from string `scope` parameter: `[AutoRegister(scope: "Gameplay")]` → `RegisterGameplay()`
- No validation that scope string matches actual `LifetimeScope` type

**New Behavior:**
- Method names derived from scope type name: `[AutoRegisterIn<GameplayLifetimeScope>]` → `RegisterGameplay()`
- Compile-time validation that scope type exists and inherits `LifetimeScope`
- Convention-based naming strips "LifetimeScope" suffix automatically

#### Scenario: Generate registration for type-safe scope
- **WHEN** user writes `[AutoRegisterIn<GameplayLifetimeScope>] public class EnemySpawner { }`
- **THEN** generator creates `RegisterGameplay()` method containing `builder.Register<EnemySpawner>(...)`

#### Scenario: Generate registration with custom scope name
- **WHEN** user writes `[ScopeName("UI")]` on `UserInterfaceLifetimeScope` with registered services
- **THEN** generator creates `RegisterUI()` method

#### Scenario: Multiple scopes in same assembly
- **WHEN** assembly has services with `[AutoRegisterIn<GameLifetimeScope>]` and `[AutoRegisterIn<GameplayLifetimeScope>]`
- **THEN** generator creates both `RegisterGame()` and `RegisterGameplay()` methods in same `VContainerRegistration` class
