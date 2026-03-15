## ADDED Requirements

### Requirement: Master Registration Method
The Source Generator SHALL generate a single `RegisterAll(IContainerBuilder builder)` method in the high-level assembly that encompasses all discovered registrations.

#### Scenario: Single Master Call
- **WHEN** a developer calls `VContainerRegistration.RegisterAll(builder)` in a `LifetimeScope`
- **THEN** all services from all discovered assemblies are registered.

### Requirement: Grouped Registrations by Identity
The system SHALL group all discovered registrations by their mapped Identity Type and emit separate registration methods for each.

#### Scenario: Identity-Based Grouping
- **WHEN** multiple services are registered to `GameScope` and others to `UIScope`
- **THEN** SG emits `RegisterForGame(builder)` and `RegisterForUI(builder)` methods.
