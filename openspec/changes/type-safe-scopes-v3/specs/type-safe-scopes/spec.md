## ADDED Requirements

### Requirement: Generic scope attribute syntax
The system SHALL provide a generic attribute `[AutoRegisterIn<TScope>]` for type-safe scope registration where `TScope` must inherit from `VContainer.Unity.LifetimeScope`.

#### Scenario: User applies attribute with valid scope type
- **WHEN** user writes `[AutoRegisterIn<GameplayLifetimeScope>]` on a class
- **THEN** the generator registers the class with the `GameplayLifetimeScope` registration method

#### Scenario: User applies attribute with invalid type (not LifetimeScope)
- **WHEN** user writes `[AutoRegisterIn<string>]` on a class
- **THEN** the analyzer produces error ND002: "Type must inherit from LifetimeScope"

#### Scenario: User applies attribute with non-existent type
- **WHEN** user writes `[AutoRegisterIn<NonExistentScope>]` on a class
- **THEN** the analyzer produces error ND001: "Type 'NonExistentScope' not found"

### Requirement: Fallback typeof() syntax support
The system SHALL support `[AutoRegister(scope: typeof(TScope))]` as an alternative syntax for dynamic or complex scenarios.

#### Scenario: User uses typeof() syntax
- **WHEN** user writes `[AutoRegister(scope: typeof(GameplayLifetimeScope))]`
- **THEN** the generator treats it identically to `[AutoRegisterIn<GameplayLifetimeScope>]`

### Requirement: Scope type validation
The system SHALL validate at compile-time that the specified scope type inherits from `VContainer.Unity.LifetimeScope`.

#### Scenario: Scope type is valid LifetimeScope subclass
- **WHEN** scope type inherits from `LifetimeScope`
- **THEN** no diagnostic is produced

#### Scenario: Scope type is not a LifetimeScope
- **WHEN** scope type does not inherit from `LifetimeScope`
- **THEN** analyzer produces error ND002
