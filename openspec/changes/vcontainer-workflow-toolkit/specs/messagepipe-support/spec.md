## MODIFIED Requirements

### Requirement: Generate MessagePipe broker wiring by declared scope
The generator SHALL emit MessagePipe broker registration in scope methods matching declared broker targets after marker-based scope remapping.

#### Scenario: Broker registration in target scope
- **WHEN** a broker-marked type targets a specific marker or alias that resolves to a concrete scope
- **THEN** generated code SHALL emit broker registration in that scope registration method

#### Scenario: Multiple declared target scopes
- **WHEN** a broker-marked type declares multiple target scopes
- **THEN** generated code SHALL preserve and emit registration in each declared target scope

#### Scenario: Type-safe scope remapping
- **WHEN** services are remapped into marker-based scopes
- **THEN** broker metadata and effective target scope SHALL remain consistent after remapping

### Requirement: Use symbol identity for MessagePipe API detection
The discovery pipeline SHALL resolve supported APIs by symbol identity, not by name only.

#### Scenario: Name-collision false positive guard
- **WHEN** an unrelated API has MessagePipe-like method names
- **THEN** MessagePipe behavior SHALL NOT be triggered unless supported symbols are matched

### Requirement: Keep attribute target contract consistent with discovery behavior
Declared attribute target types SHALL match implemented discovery support.

#### Scenario: Struct target consistency
- **WHEN** attribute declaration allows `struct` targets
- **THEN** discovery SHALL scan structs; otherwise the attribute contract SHALL NOT advertise `struct`

## ADDED Requirements

### Requirement: Record MessagePipe consumer metadata for tooling outputs
The semantic graph SHALL record publisher and subscriber consumption so reports and diagnostics can show how events are used.

#### Scenario: Subscriber dependency is recorded
- **WHEN** a service depends on `ISubscriber<TMessage>`
- **THEN** generated metadata SHALL record that service as a subscriber of `TMessage` in its effective scope

#### Scenario: Publisher dependency is recorded
- **WHEN** a service depends on `IPublisher<TMessage>`
- **THEN** generated metadata SHALL record that service as a publisher of `TMessage` in its effective scope
