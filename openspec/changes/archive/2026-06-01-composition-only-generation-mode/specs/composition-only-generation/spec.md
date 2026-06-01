# Capability: Composition-Only Generation

## ADDED Requirements

### Requirement: Composition-target-only VContainer emission

The generator SHALL emit `RegisterGeneratedFor<TScope>()` and generated VContainer installers only into compilations that contain at least one `[LifetimeScopeFor]` declaration.

#### Scenario: Service-only assembly does not emit VContainer code
- **WHEN** a compilation contains `[AutoRegisterIn]` services but no `[LifetimeScopeFor]`
- **THEN** the generator emits no `RegisterGeneratedFor<TScope>()`
- **AND** the generator emits no VContainer installer `.g.cs`
- **AND** the compilation does not need a VContainer reference to compile

#### Scenario: Composition assembly emits dispatcher and installer
- **WHEN** a compilation contains `[LifetimeScopeFor(typeof(IGameplayScope))]`
- **THEN** the generator emits `NhemGeneratedVContainerExtensions.RegisterGeneratedFor<IGameplayScope>()`
- **AND** emits one installer for `IGameplayScope`
- **AND** emits those files into the composition compilation only

### Requirement: Composition-target discovery uses explicit assembly references

The generator SHALL discover eligible `[AutoRegisterIn]` registrations from the current compilation and directly referenced assemblies only.

#### Scenario: Referenced service assembly is discovered
- **WHEN** a composition compilation references a service assembly that contains `[AutoRegisterIn(typeof(IGameplayScope))]`
- **THEN** that service registration is eligible for installer generation in the composition compilation

#### Scenario: Unreferenced service assembly is not discovered
- **WHEN** a service assembly is not directly referenced by the composition compilation
- **THEN** its registrations are not discovered
- **AND** no implicit or transitive discovery is required for the MVP

#### Scenario: Discovery does not scan all loaded Unity assemblies
- **WHEN** composition-only discovery executes
- **THEN** it inspects only the current compilation and directly referenced assemblies
- **AND** does not scan all loaded Unity editor assemblies

### Requirement: Generated installers remain stateless

The generator SHALL emit stateless installers that do not resolve services during registration.

#### Scenario: No Resolve call appears in generated installer
- **WHEN** installer code is generated for a composition target
- **THEN** the generated output contains no `Resolve<T>()` call in the registration path
- **AND** contains no mutable static runtime state
- **AND** remains reflection-free at runtime

### Requirement: Local and referenced registrations compose together

The generator SHALL include both local composition-assembly registrations and discovered referenced-assembly registrations when they match the mapped scope marker.

#### Scenario: Composition target registers local and referenced services
- **WHEN** the composition compilation contains a local gameplay service and references another gameplay service assembly
- **THEN** the generated gameplay installer contains registrations for both services
- **AND** uses valid VContainer API for each registration kind

### Requirement: Generated output is deterministic and scalable

The generator SHALL produce deterministic sorted output and use scalable discovery and duplicate-detection strategies.

#### Scenario: Deterministic sorted emission
- **WHEN** the same composition input is generated multiple times
- **THEN** the output ordering is stable and sorted
- **AND** duplicate handling is deterministic

#### Scenario: Performance smoke coverage exists
- **WHEN** release validation runs for generator tests
- **THEN** the suite includes composition-only performance smoke tests for 10, 100, and 500 services
