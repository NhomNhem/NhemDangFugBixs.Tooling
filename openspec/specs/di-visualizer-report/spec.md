# Capability: di-visualizer-report

## Purpose
Provide a human-readable and machine-readable registration report that reflects the exact generated DI graph.

## Requirements
### Requirement: Generate registration report artifacts
The system SHALL generate report output from the same semantic graph used for code generation.

#### Scenario: Markdown report output
- **WHEN** generation succeeds
- **THEN** the system SHALL produce a Markdown report that lists registrations, scopes, lifetimes, installers, and special callback markers

#### Scenario: CSV report output
- **WHEN** structured export is enabled
- **THEN** the system SHALL produce CSV output for the same registration dataset

### Requirement: Keep report and emitted registration graph in sync
Report content SHALL match emitted registration behavior without drift.

#### Scenario: Scope grouping and properties are consistent
- **WHEN** services are grouped and emitted by scope
- **THEN** Markdown and CSV rows SHALL preserve the same scope grouping, lifetimes, installer order, and registration flags
