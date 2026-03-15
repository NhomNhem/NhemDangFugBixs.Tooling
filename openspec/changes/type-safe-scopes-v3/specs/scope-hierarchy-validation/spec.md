## ADDED Requirements

### Requirement: Explicit parent-child scope wiring
The system SHALL support explicit parent-child scope relationships where parent `LifetimeScope` manually calls child scope registration methods.

#### Scenario: Parent calls child registration
- **WHEN** `GameLifetimeScope.Configure()` calls `VContainerRegistration.RegisterGameplay(builder)`
- **THEN** child scope services are registered within parent's container hierarchy

#### Scenario: Child scope resolves parent service
- **WHEN** `[AutoRegisterIn<Gameplay>]` class injects `[AutoRegisterIn<Game>]` class
- **THEN** VContainer resolves the parent service successfully

### Requirement: Scope hierarchy validation
The analyzer SHALL validate that scope hierarchy is correctly wired and detect missing parent registrations.

#### Scenario: Parent forgets to call child registration
- **WHEN** types exist with `scope: typeof(GameplayLifetimeScope)` but `GameLifetimeScope.Configure()` doesn't call `RegisterGameplay()`
- **THEN** analyzer produces warning ND103

#### Scenario: Correct hierarchy wiring
- **WHEN** parent scope calls all child scope registration methods
- **THEN** no diagnostic

### Requirement: Cross-scope dependency rules
The system SHALL enforce that parent scopes cannot depend on child scopes (but child scopes can depend on parent scopes).

#### Scenario: Valid child→parent dependency
- **WHEN** child scope service has constructor parameter of parent scope service type
- **THEN** registration succeeds, no diagnostic

#### Scenario: Invalid parent→child dependency
- **WHEN** parent scope service has constructor parameter of child scope service type
- **THEN** analyzer produces error ND004

### Requirement: Sibling scope isolation
The system SHALL ensure sibling scopes (scopes with same parent) are isolated and cannot directly inject each other.

#### Scenario: Sibling scopes attempt injection
- **WHEN** `SceneALifetimeScope` service tries to inject `SceneBLifetimeScope` service
- **THEN** analyzer produces warning: "Sibling scopes cannot directly inject each other"

#### Scenario: Shared service via parent
- **WHEN** both sibling scopes inject a service from parent scope
- **THEN** registration succeeds (shared parent service)
