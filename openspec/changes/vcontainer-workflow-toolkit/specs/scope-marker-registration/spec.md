## ADDED Requirements

### Requirement: Register services by scope marker instead of concrete LifetimeScope type
The system SHALL allow registration attributes to target a scope marker type that is mapped to a concrete `LifetimeScope` by composition code.

#### Scenario: Service targets a mapped scope marker
- **WHEN** a service is annotated with `[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]` and composition declares `[LifetimeScopeFor(typeof(IGameplayScope))]`
- **THEN** the generator SHALL emit the registration into the mapped `GameplayLifetimeScope` output

#### Scenario: Lower-layer assembly avoids composition dependency
- **WHEN** an application or infrastructure asmdef references only a shared marker interface
- **THEN** the registration SHALL compile without requiring a reference to the composition asmdef that owns the concrete `LifetimeScope`

### Requirement: Support alias-based scope registration without hard-coding package scope names
The system SHALL support alias-based registration and optional preset attributes that resolve through declared aliases or configuration.

#### Scenario: Alias resolves to mapped marker type
- **WHEN** a service uses `[AutoRegisterInScope("Gameplay")]` and a scope owner declares `[RegisterScopeAlias("Gameplay")]` for a mapped marker
- **THEN** the generator SHALL treat the service as targeting that marker's mapped `LifetimeScope`

#### Scenario: Preset requires alias
- **WHEN** a service uses a preset attribute such as `[GameplayService]` without a matching `Gameplay` alias
- **THEN** the analyzer SHALL report a missing-alias diagnostic instead of silently guessing a scope

### Requirement: Emit generated installer entry points for mapped markers
The generator SHALL emit explicit VContainer installer entry points for each mapped scope marker.

#### Scenario: Generic generated installer entry point
- **WHEN** a scope marker has one mapped `LifetimeScope`
- **THEN** generated code SHALL expose `RegisterGeneratedFor<TScopeMarker>(this IContainerBuilder builder)` for that marker

#### Scenario: Non-generic generated installer entry point
- **WHEN** a scope marker has one mapped `LifetimeScope`
- **THEN** generated code SHALL also expose a non-generic helper method named after the marker or mapped scope for callers that cannot use the generic entry point
