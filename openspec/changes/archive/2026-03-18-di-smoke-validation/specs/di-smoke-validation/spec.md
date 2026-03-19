## ADDED Requirements

### Requirement: Smoke validation command
The system MUST provide a command that builds the project and runs a reflection-based DI validator against generated injector output.

#### Scenario: Validation succeeds on a consistent graph
- **WHEN** the generated DI graph is complete and internally consistent
- **THEN** the command SHALL complete successfully

#### Scenario: Validation fails on a broken graph
- **WHEN** the generated DI graph contains missing, duplicate, or invalid registrations
- **THEN** the command SHALL fail with a diagnostic summary

### Requirement: Smoke validation is build-time only
The system MUST run the validator without requiring Unity Play mode.

#### Scenario: CI can run the validator headlessly
- **WHEN** the command is executed in a non-Unity environment
- **THEN** it SHALL validate the generated injector output from build artifacts only
