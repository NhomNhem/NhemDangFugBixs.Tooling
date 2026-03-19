## ADDED Requirements

### Requirement: Installer Interface Definition
The system SHALL provide an `IVContainerInstaller` interface in the `NhemDangFugBixs.Attributes` namespace that requires an `Install(global::VContainer.IContainerBuilder builder)` method.

#### Scenario: Interface Usage
- **WHEN** a user creates a configuration class
- **THEN** they can implement `IVContainerInstaller` to gain access to the `IContainerBuilder`.

### Requirement: Automatic Installer Detection
The system SHALL detect classes that implement the `IVContainerInstaller` interface (or a fully qualified equivalent) and have the `[AutoRegisterIn]` attribute, marking them as installers for that scope.

#### Scenario: Detect Installer Class
- **WHEN** a class implements `IVContainerInstaller` and is decorated with `[AutoRegisterIn(typeof(GameScope))]`
- **THEN** the generator recognizes it as an installer meant for the `GameScope`.

### Requirement: Parameterless Constructor Validation
The system SHALL require any class implementing `IVContainerInstaller` to have a public parameterless constructor.

#### Scenario: Missing Parameterless Constructor
- **WHEN** an installer class has only a constructor that requires arguments
- **THEN** the generator SHALL emit a diagnostic `ND105` (Error) to alert the user.

### Requirement: Installer Accessibility Validation
The system SHALL require any class implementing `IVContainerInstaller` used with the generator to be `public`.

#### Scenario: Internal Installer Class
- **WHEN** an installer class is marked as `internal`
- **THEN** the generator SHALL emit a diagnostic `ND106` (Error) to prevent compilation failure in the generated assembly.

### Requirement: Installer Type Constraint Validation
The system SHALL NOT allow `IVContainerInstaller` implementations to inherit from `UnityEngine.Component` or `UnityEngine.MonoBehaviour`.

#### Scenario: MonoBehaviour Installer
- **WHEN** a class inherits from `MonoBehaviour` and implements `IVContainerInstaller`
- **THEN** the generator SHALL emit a diagnostic `ND107` (Error) because components cannot be instantiated via `new()`.
