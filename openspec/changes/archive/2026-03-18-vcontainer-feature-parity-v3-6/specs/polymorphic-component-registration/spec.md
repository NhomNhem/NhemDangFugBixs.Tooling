## ADDED Requirements

### Requirement: Polymorphic Component Registration
The system SHALL correctly handle classes that are both Unity Components (MonoBehaviours) and VContainer Entry Points. These MUST be registered as Components with interface bindings.

#### Scenario: Register MonoBehaviour Entry Point
- **WHEN** a class is a MonoBehaviour AND implements `IInitializable`
- **THEN** the system generates `builder.RegisterComponentOnNewGameObject<T>(lifetime).AsImplementedInterfaces();`.

### Requirement: Prevent Loss of Interface Bindings
The system SHALL ensure that `.AsImplementedInterfaces()` is applied to Component registrations if the component implements life-cycle interfaces, even if it is not registered via `RegisterEntryPoint`.

#### Scenario: Verify Interface Suffix
- **WHEN** a MonoBehaviour implements `ITickable`
- **THEN** the registration code includes `.AsImplementedInterfaces()`.
