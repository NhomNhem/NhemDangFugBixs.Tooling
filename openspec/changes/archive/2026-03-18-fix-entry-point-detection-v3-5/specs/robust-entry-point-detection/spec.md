## ADDED Requirements

### Requirement: Intelligent Entry Point Interface Detection
The system SHALL identify a class as an "Entry Point" if it implements any well-known VContainer interface, regardless of whether the interface is referred to by its simple name or its fully qualified namespace.

#### Scenario: Detection by Simple Name
- **WHEN** a class implements `IInitializable`
- **THEN** the system marks it as an Entry Point.

#### Scenario: Detection by Full Namespace
- **WHEN** a class implements `VContainer.Unity.ITickable`
- **THEN** the system marks it as an Entry Point.

#### Scenario: Detection during Semantic Error
- **WHEN** the compiler cannot resolve the interface symbol (Error type)
- **THEN** the system falls back to string-suffix matching to identify the entry point.

### Requirement: Comprehensive VContainer Interface Support
The system SHALL support all standard VContainer life-cycle interfaces, including but not limited to: `IInitializable`, `IPostInitializable`, `IStartable`, `IPostStartable`, `IFixedTickable`, `IPostFixedTickable`, `ITickable`, `IPostTickable`, `ILateTickable`, `IPostLateTickable`, `IAsyncStartable`, and `IDisposable`.

#### Scenario: Support for AsyncStartable
- **WHEN** a class implements `IAsyncStartable`
- **THEN** the system correctly identifies it as an Entry Point.
