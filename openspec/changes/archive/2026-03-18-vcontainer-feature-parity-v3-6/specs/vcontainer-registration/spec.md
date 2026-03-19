## MODIFIED Requirements

### Requirement: VContainer registration method generation
The generator SHALL generate registration methods for each scope, using the most appropriate VContainer method based on the service's nature (Class vs Component) and its life-cycle interfaces.

**Previous Behavior:**
- Misidentified MonoBehaviours as simple classes for `RegisterEntryPoint`, leading to CS1503 errors.
- Missed `.AsImplementedInterfaces()` for Components that implement life-cycle interfaces.

**New Behavior:**
- If a class is a Component: Use `RegisterComponent...` methods. If it's also an Entry Point, ALWAYS add `.AsImplementedInterfaces()`.
- If a class is NOT a Component but IS an Entry Point: Use `RegisterEntryPoint<T>`.
- Always use fully qualified namespaces for `VContainer.Lifetime` to avoid ambiguity.

#### Scenario: Register plain C# Entry Point
- **WHEN** class is NOT a MonoBehaviour but implements `IInitializable`
- **THEN** emits `builder.RegisterEntryPoint<global::MyClass>(global::VContainer.Lifetime.Singleton);`

#### Scenario: Register MonoBehaviour Entry Point
- **WHEN** class IS a MonoBehaviour and implements `IInitializable`
- **THEN** emits `builder.RegisterComponentOnNewGameObject<global::MyComponent>(global::VContainer.Lifetime.Singleton).AsImplementedInterfaces();`
