## MODIFIED Requirements

### Requirement: Intelligent Entry Point Interface Detection
The system SHALL identify a class as an "Entry Point" if it implements any well-known VContainer interface, regardless of whether the interface is referred to by its simple name or its fully qualified namespace.

#### Scenario: Detection during Semantic Error
- **WHEN** the compiler cannot resolve the interface symbol (Error type)
- **THEN** the system falls back to string-suffix matching to identify the entry point.

### Requirement: Comprehensive VContainer Interface Support
The system SHALL support all standard VContainer life-cycle interfaces, including `IAsyncStartable`.

#### Scenario: Verify Interface Coverage
- **WHEN** a class implements `IAsyncStartable`
- **THEN** it is identified as an entry point.
