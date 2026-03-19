## ADDED Requirements

### Requirement: Registration report generation
The system MUST generate a report that summarizes discovered registrations, scopes, and lifetimes.

#### Scenario: Markdown report is produced
- **WHEN** the generator runs successfully
- **THEN** it SHALL produce a Markdown report (`RegistrationReport.md`) containing the registration map with scopes, services, lifetimes, and installers

#### Scenario: CSV export is available
- **WHEN** the CLI smoke validation tool is invoked with `--format csv`
- **THEN** the system SHALL produce a CSV representation of the same registration graph

### Requirement: Report content matches generated graph
The report MUST be derived from the same semantic data used to generate code.

#### Scenario: Report reflects the emitted scope graph
- **WHEN** services are grouped into scopes for code generation
- **THEN** the report SHALL show the same scope grouping, lifetime, installer order, and special registration flags (EntryPoint, BuildCallback, etc.)

### Requirement: Report includes metadata for validation
The report SHALL include fields needed for smoke test validation.

#### Scenario: Service and scope counts are tracked
- **WHEN** the report is generated
- **THEN** it SHALL include `ServiceCount` and `ScopeCount` const fields for validation

#### Scenario: MessagePipe broker metadata is included
- **WHEN** a service is a MessagePipe broker
- **THEN** the report SHALL include the broker message type for reachability validation
