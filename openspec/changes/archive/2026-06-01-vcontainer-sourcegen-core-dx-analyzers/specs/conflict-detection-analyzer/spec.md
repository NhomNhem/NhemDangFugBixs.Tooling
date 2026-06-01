## MODIFIED Requirements

### Requirement: Detect manual registration conflicts with auto-registration
The analyzer SHALL report `ND005` when a manually registered type is already decorated with `[AutoRegisterIn]` and SHALL additionally detect duplicate generated invocation hazards.

#### Scenario: Auto-registered type is manually registered
- **WHEN** a `LifetimeScope` calls a supported registration API such as `Register<T>`, `RegisterEntryPoint<T>`, `RegisterComponent<T>`, `RegisterFactory<T>`, `RegisterComponentOnNewGameObject<T>`, or `RegisterComponentInHierarchy<T>`
- **AND** generic argument `T` is decorated with `[AutoRegisterIn]`
- **THEN** the analyzer SHALL report `ND005` as an error on the manual registration call

#### Scenario: Manually registered type has no auto-registration attribute
- **WHEN** a `LifetimeScope` calls a supported manual registration API for type `T`
- **AND** `T` is not decorated with `[AutoRegisterIn]`
- **THEN** the analyzer SHALL NOT report `ND005`

#### Scenario: Same implementation registered multiple times in one scope
- **WHEN** one implementation appears multiple times in resolved registrations for the same scope
- **THEN** analyzer SHALL report `NHEM_DI_020` as Error

#### Scenario: Same contract has multiple implementations without explicit keyed-or-collection intent
- **WHEN** multiple implementations bind the same contract in one scope without keyed/collection/explicit allow intent
- **THEN** analyzer SHALL report `NHEM_DI_021` as Warning

#### Scenario: Generated scope registrations invoked more than once
- **WHEN** code invokes both `RegisterGeneratedFor<IGameplayScope>()` and generated static installer for the same scope
- **THEN** analyzer SHALL report `NHEM_DI_022` as Error

### Requirement: Provide actionable conflict diagnostic content
The analyzer SHALL include enough context in `ND005` for users to resolve ambiguity.

#### Scenario: Conflict message points to both resolution paths
- **WHEN** `ND005` is reported
- **THEN** the message SHALL identify the conflicting type and advise either removing manual registration or removing `[AutoRegisterIn]`
