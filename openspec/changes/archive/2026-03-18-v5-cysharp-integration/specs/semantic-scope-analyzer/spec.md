## ADDED Requirements

### Requirement: Cross-scope dependency validation
The system MUST detect invalid dependencies that cross scope boundaries without an approved bridge or mapping.

#### Scenario: Service depends on an out-of-scope registration
- **WHEN** a service resolves another service that is not registered in the same scope and has no valid scope bridge
- **THEN** the analyzer SHALL report ND006

#### Scenario: Allowed identity mapping suppresses ND006
- **WHEN** a service is linked through a declared identity scope or mapping attribute
- **THEN** the analyzer SHALL treat the dependency as valid and SHALL NOT report ND006

### Requirement: Scope analysis uses symbols
The analyzer MUST resolve symbols for services, scopes, and registrations before evaluating scope validity.

#### Scenario: Name-only matches do not pass validation
- **WHEN** a method or type name matches a known DI API but the resolved symbol does not belong to the supported surface
- **THEN** the analyzer SHALL ignore it for ND006 evaluation
