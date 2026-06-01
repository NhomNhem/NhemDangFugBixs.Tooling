## MODIFIED Requirements

### Requirement: Generate registration report artifacts
The system SHALL generate report output from the same semantic graph used for code generation, including marker mappings and special registration metadata.

#### Scenario: Markdown report output
- **WHEN** generation succeeds
- **THEN** the system SHALL produce a Markdown report that lists scope markers, mapped scopes, registrations, lifetimes, installers, entry points, components, MessagePipe metadata, and special callback markers

#### Scenario: CSV report output
- **WHEN** structured export is enabled
- **THEN** the system SHALL produce CSV output for the same registration dataset with enough fields to reconstruct scope ownership and registration mode

### Requirement: Keep report and emitted registration graph in sync
Report content SHALL match emitted registration behavior without drift.

#### Scenario: Scope grouping and properties are consistent
- **WHEN** services are grouped and emitted by scope
- **THEN** Markdown and CSV rows SHALL preserve the same scope grouping, marker identity, lifetimes, installer order, and registration flags

## ADDED Requirements

### Requirement: Surface architecture warnings in report output
The report SHALL carry analyzer-adjacent warnings that help humans and automation inspect risky registrations.

#### Scenario: Warning summary for generated graph
- **WHEN** generated metadata includes warning-level findings such as runtime singletons or oversized constructors
- **THEN** the report SHALL include a warning summary grouped by affected scope or service
