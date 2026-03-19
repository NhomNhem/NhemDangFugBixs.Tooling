# Capability: messagepipe-support

## Purpose
Provide symbol-safe discovery and scope-aware generation for MessagePipe broker registration.

## Requirements
### Requirement: Generate MessagePipe broker wiring by declared scope
The generator SHALL emit MessagePipe broker registration in scope methods matching declared broker targets.

#### Scenario: Broker registration in target scope
- **WHEN** a broker-marked type targets a specific scope
- **THEN** generated code SHALL emit broker registration in that scope registration method

#### Scenario: Multiple declared target scopes
- **WHEN** a broker-marked type declares multiple target scopes
- **THEN** generated code SHALL preserve and emit registration in each declared target scope

#### Scenario: Type-safe scope remapping
- **WHEN** services are remapped into type-safe scopes
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
