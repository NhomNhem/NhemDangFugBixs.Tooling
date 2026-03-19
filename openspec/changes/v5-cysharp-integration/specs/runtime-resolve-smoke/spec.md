## ADDED Requirements

### Requirement: Headless container resolution validation
The system SHALL validate DI container resolution without Unity Editor or Play Mode.

#### Scenario: Resolve all registered services
- **WHEN** preflight runs with `--resolve-smoke` flag
- **THEN** the tool SHALL:
  1. Load generated assembly via reflection
  2. Create VContainer `ContainerBuilder`
  3. Invoke `VContainerRegistration.RegisterAll(builder)`
  4. Build container (`builder.Build()`)
  5. Resolve all registered services
  6. Report any resolution failures

#### Scenario: EntryPoint resolution
- **WHEN** container contains EntryPoint types (ITickable, IInitializable)
- **THEN** the tool SHALL resolve all EntryPoints and verify:
  1. No circular dependencies
  2. All constructor parameters satisfied
  3. No VContainerException thrown

#### Scenario: MonoBehaviour exclusion
- **WHEN** a registered type inherits MonoBehaviour
- **THEN** the tool SHALL skip resolution (requires Unity runtime)
- **AND** log a warning: "Skipped MonoBehaviour: {TypeName}"

#### Scenario: Scope hierarchy validation
- **WHEN** multiple scopes are registered (Game, Gameplay, UI)
- **THEN** the tool SHALL:
  1. Build root container
  2. Create child containers (if scope hierarchy detected)
  3. Resolve services from each scope
  4. Verify parent→child resolution works

### Requirement: Pure C# type validation (Phase 1)
Phase 1 SHALL validate only pure C# types (no Unity dependencies).

#### Scenario: Filter Unity-dependent types
- **WHEN** a type has constructor parameters of Unity types (GameObject, Transform, Component)
- **THEN** the tool SHALL skip resolution
- **AND** log: "Skipped Unity-dependent: {TypeName} (requires Unity runtime)"

#### Scenario: Report coverage statistics
- **WHEN** smoke test completes
- **THEN** the tool SHALL display:
  1. Total services: N
  2. Validated (pure C#): M
  3. Skipped (Unity-dependent): K
  4. Failed: J
