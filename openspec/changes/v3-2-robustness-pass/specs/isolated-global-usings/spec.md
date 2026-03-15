## ADDED Requirements

### Requirement: Single Global Using per Assembly
The system SHALL ensure that `global using NLifetime` is only declared once per assembly in a dedicated `GlobalUsings.g.cs` file.

#### Scenario: Multi-File Assembly
- **WHEN** an assembly has multiple generated registration files
- **THEN** only one `GlobalUsings.g.cs` is added to the compilation.

### Requirement: Atomic Usings in Registration Files
The system SHALL NOT include `global using` directives inside the main registration files to prevent CS1537 errors.

#### Scenario: Clean Header
- **WHEN** the generator creates `VContainerRegistration.g.cs`
- **THEN** the header contains only standard `using` directives.
