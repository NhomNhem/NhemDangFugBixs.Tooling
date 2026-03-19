## ADDED Requirements

### Requirement: Cross-scope dependency validation
The system MUST detect invalid dependencies that cross scope boundaries without an approved bridge or mapping.

#### Scenario: Service depends on an out-of-scope registration
- **WHEN** a service in scope A injects a dependency registered only in scope B
- **AND** no valid scope bridge (parent→child or identity mapping) exists
- **THEN** the analyzer SHALL report ND006 "Invalid cross-scope dependency"

#### Scenario: Allowed identity mapping suppresses ND006
- **WHEN** a service is linked through a declared identity scope (`[LifetimeScopeFor]`) or mapping attribute
- **THEN** the analyzer SHALL treat the dependency as valid and SHALL NOT report ND006

#### Scenario: Parent scope dependency is valid
- **WHEN** a child scope service depends on a parent scope registration
- **THEN** the analyzer SHALL treat the dependency as valid (VContainer hierarchy resolution)

### Requirement: Scope analysis uses symbols
The analyzer MUST resolve symbols for services, scopes, and registrations before evaluating scope validity.

#### Scenario: Name-only matches do not pass validation
- **WHEN** a method or type name matches a known DI API but the resolved symbol does not belong to the supported surface
- **THEN** the analyzer SHALL ignore it for ND006 evaluation

#### Scenario: Scope graph is built from attributes
- **WHEN** the analyzer processes `[AutoRegisterIn(typeof(TScope))]` and `[LifetimeScopeFor]` attributes
- **THEN** it SHALL construct a scope graph using Roslyn symbols, not string matching
