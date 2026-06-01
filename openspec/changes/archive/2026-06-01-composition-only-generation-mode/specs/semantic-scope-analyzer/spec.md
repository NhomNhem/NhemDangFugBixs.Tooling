# Capability: semantic-scope-analyzer

## MODIFIED Requirements

### Requirement: Validate scope markers for local DI intent without forcing local composition

The analyzer SHALL continue to validate local scope-marker correctness for `[AutoRegisterIn]` usage, even when the current compilation is service-only and contains no `[LifetimeScopeFor]`.

#### Scenario: Service-only assembly still reports invalid scope marker
- **WHEN** a service-only compilation uses `[AutoRegisterIn(typeof(NotAScopeMarker))]`
- **THEN** the analyzer reports the invalid scope marker diagnostic
- **AND** no local VContainer generation is required for that diagnostic to exist

### Requirement: Missing composition target is not a local service-assembly error

The analyzer SHALL NOT treat the absence of `[LifetimeScopeFor]` in a service-only compilation as a local failure.

#### Scenario: Service-only assembly has valid DI intent and no local LifetimeScopeFor
- **WHEN** a compilation contains valid `[AutoRegisterIn]` services and no `[LifetimeScopeFor]`
- **THEN** the analyzer does not report a missing local composition mapping error for that compilation
- **AND** cross-assembly composition validation may be handled by `di-smoke` instead
