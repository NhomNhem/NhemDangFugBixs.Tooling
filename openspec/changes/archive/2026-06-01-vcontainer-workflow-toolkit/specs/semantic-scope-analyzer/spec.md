## MODIFIED Requirements

### Requirement: Report unreachable cross-scope dependencies
The analyzer SHALL report `ND006` when a dependency target is outside reachable scope boundaries after resolving marker-to-scope mappings and scope hierarchy rules.

#### Scenario: Invalid dependency across unreachable marker scope
- **WHEN** a service depends on another service registered in a scope that is unreachable through the mapped marker hierarchy
- **THEN** the analyzer SHALL report `ND006`

#### Scenario: Valid identity scope mapping suppresses diagnostic
- **WHEN** a dependency path is bridged by declared identity scope or marker mapping rules
- **THEN** the analyzer SHALL NOT report `ND006`

### Requirement: Use symbol-based scope and API resolution
The analyzer SHALL resolve symbols before scope validation and SHALL not rely on name-only matching.

#### Scenario: Name collision with unsupported symbol
- **WHEN** a method or type has a matching name but does not match supported symbol identity
- **THEN** the analyzer SHALL ignore that symbol for scope validation

## ADDED Requirements

### Requirement: Report invalid marker mappings before dependency validation
The analyzer SHALL report configuration diagnostics when marker-based registrations cannot be mapped to exactly one concrete scope.

#### Scenario: Missing marker mapping
- **WHEN** a registration targets a scope marker with no matching `[LifetimeScopeFor]` declaration
- **THEN** the analyzer SHALL report a missing-mapping diagnostic for that marker

#### Scenario: Duplicate marker mapping
- **WHEN** more than one concrete `LifetimeScope` maps the same marker type
- **THEN** the analyzer SHALL report a duplicate-mapping diagnostic for that marker
