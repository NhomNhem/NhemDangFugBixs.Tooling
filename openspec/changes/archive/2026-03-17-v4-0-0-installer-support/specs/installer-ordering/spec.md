## ADDED Requirements

### Requirement: Installer Order Attribute
The system SHALL provide an `InstallerOrderAttribute` allowing users to explicitly define the execution priority of their installers via an integer value.

#### Scenario: Define Execution Priority
- **WHEN** a user decorates an installer class with `[InstallerOrder(-100)]`
- **THEN** the system registers its execution order as `-100`.

### Requirement: Default Installer Order
The system SHALL assign a default execution order of `0` to any installer class that does not explicitly declare an `InstallerOrderAttribute`.

#### Scenario: Implicit Execution Priority
- **WHEN** a user creates an installer class without an `[InstallerOrder]` attribute
- **THEN** the system registers its execution order as `0`.
