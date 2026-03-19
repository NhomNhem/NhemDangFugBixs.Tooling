## ADDED Requirements

### Requirement: Global Exception Handler Detection
The system SHALL detect classes that implement the `IEntryPointExceptionHandler` interface (or a fully qualified equivalent) and mark them as global exception handlers.

#### Scenario: Implement Exception Handler Interface
- **WHEN** a class implements `NhemDangFugBixs.Attributes.IEntryPointExceptionHandler` and has the `[AutoRegisterIn]` attribute
- **THEN** the generator recognizes it as an exception handler for its specified scope.

### Requirement: Automatic Exception Handler Registration
The system SHALL generate code to register the detected exception handler using VContainer's `RegisterEntryPointExceptionHandler` builder method.

#### Scenario: Generate Registration Code
- **WHEN** an exception handler is found for a scope
- **THEN** the generator emits `builder.RegisterEntryPointExceptionHandler(ex => { ... })` which resolves the handler instance and invokes its `OnException` method.
