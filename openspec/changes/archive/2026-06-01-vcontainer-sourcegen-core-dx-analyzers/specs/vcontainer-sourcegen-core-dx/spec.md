## ADDED Requirements

### Requirement: Generate marker-scoped static installers
The source generator SHALL emit one stateless static installer per scope marker that has resolved registrations.

#### Scenario: Marker has services
- **WHEN** one or more services are registered for `IGameplayScope`
- **THEN** the generator SHALL emit `NhemGeneratedGameplayScopeInstaller` with deterministic registration methods

### Requirement: Emit extension DX dispatch for generated installers
The source generator SHALL emit `NhemGeneratedVContainerExtensions.RegisterGeneratedFor<TScope>(this IContainerBuilder builder)` as DX sugar over static installers.

#### Scenario: Scope mapping exists
- **WHEN** `RegisterGeneratedFor<IGameplayScope>()` is called
- **THEN** extension code SHALL call the generated gameplay installer exactly once and return

#### Scenario: Scope mapping does not exist
- **WHEN** `RegisterGeneratedFor<TScope>()` is called for a marker with no generated installer
- **THEN** the extension SHALL throw `InvalidOperationException` with the marker full name

### Requirement: Keep user-owned LifetimeScope policy
The package SHALL require users to own and author their `LifetimeScope` classes and SHALL NOT generate `LifetimeScope` MonoBehaviour classes by default.

#### Scenario: Composition setup
- **WHEN** a project defines `[LifetimeScopeFor<IGameplayScope>]` on a user class derived from `LifetimeScope`
- **THEN** generated code SHALL only provide static installers and extension dispatch, without injecting/creating new scope MonoBehaviours
