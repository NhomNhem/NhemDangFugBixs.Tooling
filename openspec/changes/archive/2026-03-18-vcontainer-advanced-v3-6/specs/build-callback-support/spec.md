## ADDED Requirements

### Requirement: Build Callback Detection
The system SHALL detect classes that implement the `IBuildCallback` interface (or a fully qualified equivalent) and mark them as build callbacks.

#### Scenario: Implement Build Callback Interface
- **WHEN** a class implements `NhemDangFugBixs.Attributes.IBuildCallback` and has the `[AutoRegisterIn]` attribute
- **THEN** the generator recognizes it as a build callback for its specified scope.

### Requirement: Automatic Build Callback Registration
The system SHALL generate code to register the detected class as a service AND register a build callback using VContainer's `RegisterBuildCallback` builder method to invoke the class's `OnBuild` method.

#### Scenario: Generate Registration and Callback Code
- **WHEN** a build callback is found for a scope
- **THEN** the generator emits a standard `Register` call for the class AND a `builder.RegisterBuildCallback(container => { ... })` that resolves the instance and invokes `OnBuild`.
