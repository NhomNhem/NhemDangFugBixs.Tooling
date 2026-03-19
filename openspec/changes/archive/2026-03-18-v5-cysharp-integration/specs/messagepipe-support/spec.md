## ADDED Requirements

### Requirement: MessagePipe registration support
The system MUST support discovering MessagePipe-related services and emitting scope-aware registration code for them.

#### Scenario: Message broker is registered in the target scope
- **WHEN** a service is annotated with MessagePipe support metadata
- **THEN** the generator SHALL emit the required broker or handler wiring into the matching registration method

#### Scenario: Registration follows VContainer scope mapping
- **WHEN** a service maps to a specific `LifetimeScope` or identity scope
- **THEN** the generated MessagePipe wiring SHALL be placed in the same scope-specific registration path

#### Scenario: Multiple target scopes are preserved
- **WHEN** a service is annotated for more than one MessagePipe target scope
- **THEN** the generator SHALL emit registration for each target scope

#### Scenario: Scope remapping preserves broker metadata
- **WHEN** a MessagePipe service is remapped into a type-safe scope
- **THEN** the remapped service SHALL keep its broker type and MessagePipe registration flags

### Requirement: MessagePipe diagnostics are symbol-based
The system MUST resolve the target APIs by symbol before treating them as MessagePipe registration points.

#### Scenario: Similar method names do not trigger MessagePipe behavior
- **WHEN** a project defines a non-MessagePipe method with the same name as a supported API
- **THEN** the generator or analyzer SHALL ignore it unless the containing type matches the supported MessagePipe surface

### Requirement: MessagePipe discovery matches declared targets
The system MUST only advertise attribute targets that the generator and analyzer can actually discover.

#### Scenario: Struct targets are either supported or disallowed consistently
- **WHEN** the attribute declaration allows `struct`
- **THEN** the discovery pipeline SHALL also scan structs, or the attribute SHALL not expose struct targets
