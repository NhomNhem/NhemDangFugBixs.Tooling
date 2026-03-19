## ADDED Requirements

### Requirement: Indestructible C# Emission
The system SHALL emit code using `partial` classes and fully qualified names with the `global::` prefix to prevent collisions and ambiguity.

#### Scenario: Generate Registration Code
- **WHEN** the generator emits code for a service
- **THEN** it uses `global::NhemDangFugBixs.Attributes.NhemLifetime.Singleton` and `public static partial class VContainerRegistration`

### Requirement: Unique Namespace Generation
The system SHALL use a robust sanitization logic for assembly names to ensure unique namespaces for generated code.

#### Scenario: Collision-Free Namespaces
- **WHEN** two assemblies have similar names (e.g., `Core.API` and `Core-API`)
- **THEN** the generator produces distinct namespaces for each.
