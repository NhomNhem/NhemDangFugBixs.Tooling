## MODIFIED Requirements

### Requirement: Detect manual registration conflicts with auto-registration
The analyzer SHALL report `ND005` when a manually registered type is already decorated with `[AutoRegisterIn]` and the shared DI contract graph shows that the manual call creates a duplicate service registration.

#### Scenario: Auto-registered type is manually registered
- **WHEN** a `LifetimeScope` calls a supported registration API such as `Register<T>`, `RegisterEntryPoint<T>`, `RegisterComponent<T>`, `RegisterFactory<T>`, `RegisterComponentOnNewGameObject<T>`, or `RegisterComponentInHierarchy<T>`
- **AND** generic argument `T` is decorated with `[AutoRegisterIn]`
- **AND** the call is not the generated registration route for the mapped scope
- **THEN** the analyzer SHALL report `ND005` as an error on the manual registration call

#### Scenario: Manually registered type has no auto-registration attribute
- **WHEN** a `LifetimeScope` calls a supported manual registration API for type `T`
- **AND** `T` is not decorated with `[AutoRegisterIn]`
- **THEN** the analyzer SHALL NOT report `ND005`

#### Scenario: Generated route is not a duplicate manual registration
- **WHEN** a `LifetimeScope` calls `builder.RegisterGeneratedFor<TScope>()`
- **THEN** the analyzer SHALL NOT report `ND005` for registrations emitted behind that generated route

### Requirement: Provide actionable conflict diagnostic content
The analyzer SHALL include enough graph-backed context in `ND005` for users to resolve ambiguity.

#### Scenario: Conflict message points to both resolution paths
- **WHEN** `ND005` is reported
- **THEN** the message SHALL identify the conflicting type and advise either removing manual registration or removing `[AutoRegisterIn]`

#### Scenario: Conflict message includes scope context
- **WHEN** `ND005` is reported for a type-safe scoped service
- **THEN** the message SHALL include the scope marker when graph evidence is available
