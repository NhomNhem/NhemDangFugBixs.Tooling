## MODIFIED Requirements

### Requirement: Strict Single File per Assembly
The system SHALL emit exactly one generated source file for each assembly, containing all registrations, global usings, and emission metadata, to prevent collisions and improve traceability.

#### Scenario: Metadata Included in Single File
- **WHEN** an assembly contains registrations
- **THEN** it produces a single file with the registered services AND a metadata header.
