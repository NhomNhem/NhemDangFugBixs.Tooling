## ADDED Requirements

### Requirement: Graceful Assembly Scan Failure
The system SHALL catch exceptions during assembly scanning and emit a diagnostic warning instead of failing the entire compilation.

#### Scenario: Unresolved Assembly
- **WHEN** an assembly cannot be resolved by the generator
- **THEN** it emits an `ND104` warning in the Unity console with the name of the problematic assembly.

### Requirement: Partial Result Emission
The system SHALL continue to emit registration code for all successfully scanned assemblies even if one or more assembly scans fail.

#### Scenario: Partial Emission
- **WHEN** one assembly fails to scan but others are valid
- **THEN** the generator produces a file containing registrations for all valid services.
