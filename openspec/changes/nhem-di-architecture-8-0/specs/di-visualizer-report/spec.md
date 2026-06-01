## MODIFIED Requirements

### Requirement: Generate registration report artifacts
The system SHALL generate report output from the same DI contract graph used for code generation and smoke validation.

#### Scenario: Markdown report output
- **WHEN** generation succeeds
- **THEN** the system SHALL produce a Markdown report that lists registrations, scopes, lifetimes, installers, and special callback markers

#### Scenario: CSV report output
- **WHEN** structured export is enabled
- **THEN** the system SHALL produce CSV output for the same registration dataset

#### Scenario: Graph evidence output
- **WHEN** machine-readable report output is enabled
- **THEN** the system SHALL include graph evidence for source assembly, discovered scope, composition root, and registration path

### Requirement: Keep report and emitted registration graph in sync
Report content SHALL match emitted registration behavior without drift.

#### Scenario: Scope grouping and properties are consistent
- **WHEN** services are grouped and emitted by scope
- **THEN** Markdown and CSV rows SHALL preserve the same scope grouping, lifetimes, installer order, and registration flags

#### Scenario: No-op scope is represented consistently
- **WHEN** a composition root emits a no-op installer for a mapped scope with no services
- **THEN** reports SHALL represent that scope consistently with generated behavior
