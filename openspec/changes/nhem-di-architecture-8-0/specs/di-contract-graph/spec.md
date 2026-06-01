## ADDED Requirements

### Requirement: Represent DI facts in a shared contract graph
The system SHALL represent scopes, services, composition roots, manual registrations, generated installers, assembly provenance, and diagnostic evidence in a shared DI contract graph.

#### Scenario: Scope and service facts share one model
- **WHEN** generator, analyzer, smoke validation, or report code extracts DI facts
- **THEN** those facts SHALL be expressible through the shared DI contract graph model

#### Scenario: Assembly provenance is preserved
- **WHEN** a service or scope is discovered from a referenced assembly
- **THEN** the graph SHALL preserve the declaring assembly and discovery path

### Requirement: Provide deterministic graph queries
The system SHALL provide deterministic queries for services by scope, composition roots by marker, manual registrations by implementation, and generated installers by scope.

#### Scenario: Query services for a scope marker
- **WHEN** a caller queries services for a type-safe scope marker
- **THEN** the graph SHALL return matching services in stable ordering with duplicate implementation identities resolved predictably

#### Scenario: Query composition root for a marker
- **WHEN** a caller queries the composition root for a marker
- **THEN** the graph SHALL return the mapped lifetime scope facts when one exists

### Requirement: Distinguish local and project-wide evidence
The graph SHALL identify whether evidence came from the current compilation, a referenced assembly, or project-wide smoke validation.

#### Scenario: Per-compilation evidence is incomplete
- **WHEN** a service assembly cannot see the bootstrap composition root
- **THEN** the graph SHALL mark the scope mapping evidence as unavailable rather than inventing a missing mapping error
