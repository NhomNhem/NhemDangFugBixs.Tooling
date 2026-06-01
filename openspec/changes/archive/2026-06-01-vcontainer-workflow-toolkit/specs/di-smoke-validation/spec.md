## ADDED Requirements

### Requirement: Provide preflight validation for generated DI metadata
The system SHALL provide a CLI command that validates generated DI metadata without requiring Unity Play Mode.

#### Scenario: Preflight succeeds on a valid graph
- **WHEN** `di-smoke preflight` is run against a project with valid scope mappings and registrations
- **THEN** the command SHALL complete successfully with a validation summary

#### Scenario: Preflight fails on invalid mappings or registrations
- **WHEN** `di-smoke preflight` detects missing mappings, duplicates, or invalid lifetimes
- **THEN** the command SHALL fail and report the discovered diagnostics

### Requirement: Provide scope exploration commands
The system SHALL provide commands for listing services and exporting dependency views from generated metadata.

#### Scenario: Scope service listing
- **WHEN** `di-smoke list` is run with a specific scope marker or alias
- **THEN** the command SHALL output the services, lifetimes, and key registration flags for that scope

#### Scenario: Mermaid graph export
- **WHEN** `di-smoke graph` is run with Mermaid output selected
- **THEN** the command SHALL export a dependency graph for the selected scope using the generated metadata

### Requirement: Export report formats for CI and documentation
The system SHALL export report data in formats suited for human review and automation.

#### Scenario: Markdown report export
- **WHEN** `di-smoke report --format markdown` is executed
- **THEN** the command SHALL write a Markdown report describing scope mappings, services, warnings, and special registration metadata

#### Scenario: JSON report export
- **WHEN** `di-smoke report --format json` is executed
- **THEN** the command SHALL write machine-readable registration metadata for automation or downstream tooling
