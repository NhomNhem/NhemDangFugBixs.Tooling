## ADDED Requirements

### Requirement: Registration report generation
The system MUST generate a report that summarizes discovered registrations, scopes, and lifetimes.

#### Scenario: Markdown report is produced
- **WHEN** the generator runs successfully
- **THEN** it SHALL produce a Markdown report containing the registration map

#### Scenario: CSV export is available
- **WHEN** structured export is enabled
- **THEN** the system SHALL produce a CSV representation of the same registration graph

### Requirement: Report content matches generated graph
The report MUST be derived from the same semantic data used to generate code.

#### Scenario: Report reflects the emitted scope graph
- **WHEN** services are grouped into scopes for code generation
- **THEN** the report SHALL show the same scope grouping, lifetime, installer order, and special registration flags
