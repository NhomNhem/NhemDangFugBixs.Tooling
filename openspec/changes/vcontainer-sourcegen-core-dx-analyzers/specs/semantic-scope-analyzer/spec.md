## MODIFIED Requirements

### Requirement: Report unreachable cross-scope dependencies
The analyzer SHALL report `ND006` when a dependency target is outside reachable scope boundaries and SHALL additionally report scope-marker mapping guardrail diagnostics for generated registration integration.

#### Scenario: Invalid dependency across unreachable scope
- **WHEN** a service depends on another service registered in an unreachable scope
- **THEN** the analyzer SHALL report `ND006`

#### Scenario: Valid identity scope mapping suppresses diagnostic
- **WHEN** a dependency path is bridged by declared identity scope or mapping rules
- **THEN** the analyzer SHALL NOT report `ND006`

#### Scenario: Scope has registrations but no mapping
- **WHEN** a scope marker has generated registrations but no `LifetimeScopeFor<TScope>` mapping
- **THEN** the analyzer SHALL report `NHEM_DI_010` as Warning

#### Scenario: Mapping exists but Configure omits generated call
- **WHEN** `LifetimeScopeFor<TScope>` exists and `Configure` does not invoke `builder.RegisterGeneratedFor<TScope>()`
- **THEN** the analyzer SHALL report `NHEM_DI_011` as Warning

#### Scenario: Mapping exists but wrong scope invoked
- **WHEN** `LifetimeScopeFor<TScope>` maps one marker but `Configure` invokes `RegisterGeneratedFor<TOtherScope>()`
- **THEN** the analyzer SHALL report `NHEM_DI_012` as Error

### Requirement: Use symbol-based scope and API resolution
The analyzer SHALL resolve symbols before scope validation and SHALL not rely on name-only matching.

#### Scenario: Name collision with unsupported symbol
- **WHEN** a method or type has a matching name but does not match supported symbol identity
- **THEN** the analyzer SHALL ignore that symbol for scope validation
