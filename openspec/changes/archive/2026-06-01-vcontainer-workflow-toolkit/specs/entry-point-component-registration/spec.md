## ADDED Requirements

### Requirement: Generate entry point registration from explicit entry point intent
The system SHALL register entry point services with VContainer entry point APIs instead of normal service registration APIs when entry point intent is declared.

#### Scenario: Lifecycle service marked as entry point
- **WHEN** a service is annotated with `[EntryPoint]` and implements supported VContainer lifecycle interfaces
- **THEN** generated code SHALL emit `RegisterEntryPoint<T>(...)` for that service in its target scope

#### Scenario: Entry point marker without lifecycle interface
- **WHEN** a class is annotated with `[EntryPoint]` but implements no supported VContainer lifecycle interface
- **THEN** the analyzer SHALL report an entry-point contract diagnostic

### Requirement: Generate scene and hierarchy component registrations for MonoBehaviour types
The system SHALL support component-specific registration modes for existing scene objects and generated GameObject hosts.

#### Scenario: Scene component registration
- **WHEN** a `MonoBehaviour` is annotated with `[SceneComponent<IGameplayScope>]`
- **THEN** generated code SHALL emit hierarchy-based component registration for the mapped gameplay scope

#### Scenario: New GameObject component registration
- **WHEN** a `MonoBehaviour` is annotated with `[NewGameObjectComponent<IGameplayScope>("BulletPool")]`
- **THEN** generated code SHALL emit registration on a new GameObject with the provided name in the mapped gameplay scope

### Requirement: Execute installers and build callbacks in generated scope order
The system SHALL support generated execution ordering for manual installers and build callbacks that participate in a mapped scope.

#### Scenario: Installer ordering runs before generated services by default
- **WHEN** a scope contains installer types with declared order metadata
- **THEN** generated code SHALL execute installers in order before generated service registrations unless configured otherwise

#### Scenario: Build callback runs after scope registrations
- **WHEN** a scope contains a build callback registration
- **THEN** generated code SHALL invoke the callback after normal services, components, brokers, and entry points have been registered
