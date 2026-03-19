# Capability: smart-lifecycle-filtering

## Purpose
Avoid redundant or unsafe interface exposure when generating `RegisterEntryPoint<T>()` registrations.

## Requirements
### Requirement: Filter VContainer-managed lifecycle interfaces from entry point exposure
The generator SHALL exclude interfaces that are already managed by `RegisterEntryPoint<T>()` from `.As<...>()` emission.

#### Scenario: Entry point implements lifecycle and domain interfaces
- **WHEN** `T` implements both lifecycle interfaces (for example `ITickable`) and domain interfaces (for example `IBullet`)
- **THEN** generated registration SHALL expose only non-lifecycle interfaces, for example `.As<global::IBullet>().AsSelf()`

#### Scenario: Entry point implements lifecycle interfaces only
- **WHEN** `T` implements only lifecycle interfaces managed by VContainer
- **THEN** generated registration SHALL be `RegisterEntryPoint<T>(...).AsSelf()` without extra `.As<...>()`

### Requirement: Do not emit broad interface exposure for entry points
The generator SHALL avoid `.AsImplementedInterfaces()` for `RegisterEntryPoint<T>()` paths.

#### Scenario: Entry point code generation
- **WHEN** generating registration for an entry point
- **THEN** output SHALL NOT use `.AsImplementedInterfaces()` and SHALL use explicit filtered interface emission instead
