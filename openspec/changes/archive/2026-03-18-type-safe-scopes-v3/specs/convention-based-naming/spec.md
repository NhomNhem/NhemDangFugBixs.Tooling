## ADDED Requirements

### Requirement: Automatic method name generation
The generator SHALL create registration method names by stripping the "LifetimeScope" suffix from the scope type name.

#### Scenario: Standard LifetimeScope naming
- **WHEN** scope type is `GameplayLifetimeScope`
- **THEN** generated method is `RegisterGameplay()`

#### Scenario: Short LifetimeScope naming
- **WHEN** scope type is `GameLifetimeScope`
- **THEN** generated method is `RegisterGame()`

#### Scenario: No LifetimeScope suffix
- **WHEN** scope type is `GameScope` (no "Lifetime" prefix)
- **THEN** generated method is `RegisterGameScope()` (no stripping)

### Requirement: Custom scope name override
The system SHALL provide `[ScopeName("CustomName")]` attribute to override convention-based naming.

#### Scenario: User applies custom name
- **WHEN** user writes `[ScopeName("UI")]` on `UserInterfaceLifetimeScope`
- **THEN** generated method is `RegisterUI()` (not `RegisterUserInterface`)

#### Scenario: Custom name with special characters
- **WHEN** user writes `[ScopeName("Game_V2")]`
- **THEN** generated method is `RegisterGame_V2()`

### Requirement: Namespace-safe method generation
The generator SHALL handle scope types from different namespaces without naming conflicts.

#### Scenario: Same class name, different namespaces
- **WHEN** `Game.GameLifetimeScope` and `UI.GameLifetimeScope` both exist
- **THEN** generated methods are uniquely named (e.g., `RegisterGame_1()`, `RegisterGame_2()` or include namespace in naming)
