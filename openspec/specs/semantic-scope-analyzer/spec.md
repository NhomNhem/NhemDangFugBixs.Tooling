# Capability: semantic-scope-analyzer

## Purpose
Enforce DI scope boundaries with symbol-accurate analysis and report invalid cross-scope dependencies.

## Requirements
### Requirement: Report unreachable cross-scope dependencies
The analyzer SHALL report `ND006` when a dependency target is outside reachable scope boundaries.

#### Scenario: Invalid dependency across unreachable scope
- **WHEN** a service depends on another service registered in an unreachable scope
- **THEN** the analyzer SHALL report `ND006`

#### Scenario: Valid identity scope mapping suppresses diagnostic
- **WHEN** a dependency path is bridged by declared identity scope or mapping rules
- **THEN** the analyzer SHALL NOT report `ND006`

### Requirement: Use symbol-based scope and API resolution
The analyzer SHALL resolve symbols before scope validation and SHALL not rely on name-only matching.

#### Scenario: Name collision with unsupported symbol
- **WHEN** a method or type has a matching name but does not match supported symbol identity
- **THEN** the analyzer SHALL ignore that symbol for scope validation
