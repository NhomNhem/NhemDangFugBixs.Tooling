## ADDED Requirements

### Requirement: ND001 - Scope type not found
The analyzer SHALL produce error ND001 when the specified scope type cannot be resolved.

#### Scenario: Type name is misspelled
- **WHEN** user writes `[AutoRegisterIn<GameplayLifeimeScope>]` (typo)
- **THEN** analyzer produces error ND001: "Type 'GameplayLifeimeScope' not found"

#### Scenario: Type is in unloaded assembly
- **WHEN** scope type exists but is in an assembly not referenced by the project
- **THEN** analyzer produces error ND001

### Requirement: ND002 - Invalid scope type
The analyzer SHALL produce error ND002 when the specified scope type does not inherit from `VContainer.Unity.LifetimeScope`.

#### Scenario: Type is a plain class
- **WHEN** user writes `[AutoRegisterIn<string>]` or `[AutoRegisterIn<MyService>]`
- **THEN** analyzer produces error ND002: "Type must inherit from LifetimeScope"

#### Scenario: Type is an interface
- **WHEN** user writes `[AutoRegisterIn<IService>]`
- **THEN** analyzer produces error ND002

### Requirement: ND003 - Circular scope dependency
The analyzer SHALL produce error ND003 when circular scope parent relationships are detected.

#### Scenario: Direct circular reference
- **WHEN** `ScopeA` declares `ScopeB` as parent AND `ScopeB` declares `ScopeA` as parent
- **THEN** analyzer produces error ND003: "Circular scope hierarchy detected"

#### Scenario: Indirect circular reference
- **WHEN** `ScopeA → ScopeB → ScopeC → ScopeA` (cycle through multiple scopes)
- **THEN** analyzer produces error ND003

### Requirement: ND004 - Parent→Child dependency violation
The analyzer SHALL produce warning ND004 when a parent scope service attempts to inject a child scope service.

#### Scenario: Parent service injects child service
- **WHEN** `[AutoRegisterIn<Game>]` class has constructor parameter of type `[AutoRegisterIn<Gameplay>]` class
- **THEN** analyzer produces warning ND004: "Parent scope service cannot depend on child scope service"

#### Scenario: Child service injects parent service
- **WHEN** `[AutoRegisterIn<Gameplay>]` class has constructor parameter of type `[AutoRegisterIn<Game>]` class
- **THEN** no diagnostic (this is valid)

### Requirement: ND103 - Unused scope registration
The analyzer SHALL produce warning ND103 when a scope has registrations but no `LifetimeScope` calls the corresponding `Register{ScopeName}()` method.

#### Scenario: Scope has registrations but not called
- **WHEN** types are registered with `scope: typeof(GameplayLifetimeScope)` but no `LifetimeScope.Configure()` calls `RegisterGameplay()`
- **THEN** analyzer produces warning ND103: "Scope 'Gameplay' has {N} registrations but no LifetimeScope calls RegisterGameplay()"

#### Scenario: Scope is called correctly
- **WHEN** at least one `LifetimeScope.Configure()` calls the scope's register method
- **THEN** no diagnostic
