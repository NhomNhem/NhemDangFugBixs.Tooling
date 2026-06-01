## MODIFIED Requirements

### Requirement: Report unreachable cross-scope dependencies
The analyzer SHALL report `ND006` when shared DI contract graph evidence proves that a dependency target is outside reachable scope boundaries in the current compilation.

#### Scenario: Invalid dependency across unreachable scope
- **WHEN** a service depends on another service registered in an unreachable scope
- **AND** the shared DI contract graph has enough current-compilation evidence to prove the boundary violation
- **THEN** the analyzer SHALL report `ND006`

#### Scenario: Valid identity scope mapping suppresses diagnostic
- **WHEN** a dependency path is bridged by declared identity scope or mapping rules
- **THEN** the analyzer SHALL NOT report `ND006`

#### Scenario: Project-wide evidence is required
- **WHEN** the current compilation cannot see enough assemblies to prove whether the scope path is reachable
- **THEN** the analyzer SHALL NOT report `ND006` and SHALL leave the condition for smoke validation

### Requirement: Use symbol-based scope and API resolution
The analyzer SHALL resolve symbols into the shared DI contract graph before scope validation and SHALL not rely on name-only matching.

#### Scenario: Name collision with unsupported symbol
- **WHEN** a method or type has a matching name but does not match supported symbol identity
- **THEN** the analyzer SHALL ignore that symbol for scope validation

#### Scenario: Graph-backed scope resolution
- **WHEN** scope and service facts are available through the shared DI contract graph
- **THEN** the analyzer SHALL use graph identity rather than rule-local attribute parsing
