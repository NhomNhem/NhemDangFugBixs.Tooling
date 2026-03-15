## ADDED Requirements

### Requirement: Identity Type Marker
The system SHALL support the use of any class or interface as an Identity Type for a LifetimeScope, provided it is accessible to both the service and the scope.

#### Scenario: Define Identity in Base Assembly
- **WHEN** a developer creates `public class GameScope {}` in a base assembly
- **THEN** this type can be used in `[AutoRegisterIn<GameScope>]` and `[LifetimeScopeFor<GameScope>]`

### Requirement: Identity-to-Scope Mapping
The system SHALL allow mapping an Identity Type to a specific `LifetimeScope` class using the `[LifetimeScopeFor<T>]` attribute.

#### Scenario: Map Identity to LifetimeScope
- **WHEN** `[LifetimeScopeFor<GameScope>]` is applied to `GameLifetimeScope`
- **THEN** any service registered to `GameScope` is automatically registered to `GameLifetimeScope`
