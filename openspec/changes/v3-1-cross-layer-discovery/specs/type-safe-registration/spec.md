## ADDED Requirements

### Requirement: Enhanced Generic Auto-Registration
The `[AutoRegisterIn(typeof(T))]` attribute SHALL support any type `T` as its generic argument to represent a registration target.

#### Scenario: Register to Identity
- **WHEN** `[AutoRegisterIn(typeof(GameScope))]` is used on a class
- **THEN** it is registered into the scope that maps to `GameScope`.

### Requirement: Compatibility Mode
The system SHALL continue to support `[AutoRegisterIn(typeof(TScope))]` where `TScope` is a direct `LifetimeScope` type for backward compatibility.

#### Scenario: Direct Scope Reference
- **WHEN** `[AutoRegisterIn(typeof(GameLifetimeScope))]` is used in the same assembly
- **THEN** it functions as it did in v3.0.
