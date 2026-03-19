# Capability: conflict-detection-analyzer (ND005)

## Purpose
Prevent ambiguous DI registration by rejecting manual VContainer registration for types already marked with `[AutoRegisterIn]`.

## Requirements
### Requirement: Detect manual registration conflicts with auto-registration
The analyzer SHALL report `ND005` when a manually registered type is already decorated with `[AutoRegisterIn]`.

#### Scenario: Auto-registered type is manually registered
- **WHEN** a `LifetimeScope` calls a supported registration API such as `Register<T>`, `RegisterEntryPoint<T>`, `RegisterComponent<T>`, `RegisterFactory<T>`, `RegisterComponentOnNewGameObject<T>`, or `RegisterComponentInHierarchy<T>`
- **AND** generic argument `T` is decorated with `[AutoRegisterIn]`
- **THEN** the analyzer SHALL report `ND005` as an error on the manual registration call

#### Scenario: Manually registered type has no auto-registration attribute
- **WHEN** a `LifetimeScope` calls a supported manual registration API for type `T`
- **AND** `T` is not decorated with `[AutoRegisterIn]`
- **THEN** the analyzer SHALL NOT report `ND005`

### Requirement: Provide actionable conflict diagnostic content
The analyzer SHALL include enough context in `ND005` for users to resolve ambiguity.

#### Scenario: Conflict message points to both resolution paths
- **WHEN** `ND005` is reported
- **THEN** the message SHALL identify the conflicting type and advise either removing manual registration or removing `[AutoRegisterIn]`
