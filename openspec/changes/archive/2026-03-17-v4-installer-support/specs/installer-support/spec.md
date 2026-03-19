## ADDED Requirements

### Requirement: Installer Interface Definition
The system SHALL provide an `IVContainerInstaller` interface in the `NhemDangFugBixs.Attributes` namespace that requires an `Install(IContainerBuilder builder)` method.

#### Scenario: Interface Usage
- **WHEN** a user creates a configuration class
- **THEN** they can implement `IVContainerInstaller` to gain access to the `IContainerBuilder`.

### Requirement: Automatic Installer Detection
The system SHALL detect classes that implement the `IVContainerInstaller` interface (or a fully qualified equivalent) and have the `[AutoRegisterIn]` attribute, marking them as installers for that scope.

#### Scenario: Detect Installer Class
- **WHEN** a class implements `IVContainerInstaller` and is decorated with `[AutoRegisterIn(typeof(GameScope))]`
- **THEN** the generator recognizes it as an installer meant for the `GameScope`.
