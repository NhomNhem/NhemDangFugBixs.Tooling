## ADDED Requirements

### Requirement: Generate registration entry points for every composition root
The generator SHALL emit a `RegisterGeneratedFor<TScope>()` route for every `[LifetimeScopeFor<TScope>]` mapping visible in the composition assembly.

#### Scenario: Scope has discovered services
- **WHEN** a composition assembly maps `IGameplayScope` and references assemblies containing services registered for `IGameplayScope`
- **THEN** generated code SHALL register those services through `builder.RegisterGeneratedFor<IGameplayScope>()`

#### Scenario: Scope has no services
- **WHEN** a composition assembly maps `IProjectScope` but no matching services are discovered
- **THEN** generated code SHALL still provide a no-op `builder.RegisterGeneratedFor<IProjectScope>()` route

### Requirement: Preserve generated API compatibility
Generated composition APIs SHALL preserve existing entry points for generic and legacy registration calls.

#### Scenario: Generic entry point remains available
- **WHEN** generated code is emitted for a scope marker
- **THEN** `NhemGeneratedVContainerExtensions.RegisterGeneratedFor<TScope>()` SHALL remain available

#### Scenario: Legacy named entry point remains available
- **WHEN** generated code is emitted for a mapped scope
- **THEN** the legacy named registration method for that scope SHALL remain available

### Requirement: Use referenced assembly registrations in composition output
The generator SHALL include eligible auto-registered services from referenced assemblies in the composition assembly output.

#### Scenario: Shared marker and feature service assembly
- **WHEN** a shared assembly declares `IGameplayScope`, a feature assembly declares `[AutoRegisterIn<IGameplayScope>]`, and a bootstrap assembly declares `[LifetimeScopeFor<IGameplayScope>]`
- **THEN** the bootstrap generated output SHALL include the feature service registration
