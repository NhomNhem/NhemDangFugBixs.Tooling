## ADDED Requirements

### Requirement: Strict Single File per Assembly
The system SHALL emit exactly one generated source file for each assembly, containing all registrations and global usings, to prevent collisions with stale files from previous versions.

#### Scenario: Multi-Service Assembly
- **WHEN** an assembly contains multiple services
- **THEN** only one `<AssemblyName>.VContainerRegistration.g.cs` file is added to the compilation.

### Requirement: Deterministic Hint Names
The system SHALL use a deterministic and stable hint name for the generated file that is unique per assembly but consistent across builds.

#### Scenario: Hint Name Stability
- **WHEN** a project is rebuilt
- **THEN** the generator uses the exact same hint name as the previous build, allowing Unity to overwrite the old file.
