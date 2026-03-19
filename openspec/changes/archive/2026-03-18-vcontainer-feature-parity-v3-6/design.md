## Context

Recent integration tests in Unity have revealed that classes that are both MonoBehaviours and VContainer Entry Points (like `InfiniteChunkManager` implementing `ITickable`) are not correctly handled. Additionally, the generated code often misses `using VContainer.Unity;`, causing extension methods like `RegisterEntryPoint` to be invisible to the compiler.

## Goals / Non-Goals

**Goals:**
- Fix the missing `using` directives in generated code.
- Ensure MonoBehaviours that implement VContainer interfaces are registered as Components with interface bindings.
- Prevent CS1503 type conversion errors in generated registrations.
- Achieve reliable automatic registration for all standard VContainer patterns.

**Non-Goals:**
- Supporting custom VContainer extensions outside of the standard library.

## Decisions

### 1. Mandatory Namespace Imports
The `RegistrationEmitter` will be updated to always emit:
```csharp
using VContainer;
using VContainer.Unity;
```
This ensures that all builder extension methods are available.

### 2. Differentiated Registration Strategy
We will implement a clearer hierarchy in `RegistrationEmitter`:
- **IF IsComponent**: Use `RegisterComponentInHierarchy` or `RegisterComponentOnNewGameObject`.
  - **IF IsEntryPoint**: Append `.AsImplementedInterfaces()` to bind life-cycle methods.
- **ELSE IF IsEntryPoint**: Use `RegisterEntryPoint<T>`.
- **ELSE**: Use standard `Register<T>`.

### 3. Fully Qualified Enum References
To prevent CS1503 (ambiguity between `Lifetime` enum and a class name), we will strictly use `global::VContainer.Lifetime.[Name]` in all generated calls.

### 4. Component Interface Binding
The check `if (svc.AsImplementedInterfaces && !svc.IsEntryPoint)` will be removed. MonoBehaviours that are entry points MUST bind their interfaces to work, so `.AsImplementedInterfaces()` will be forced for such cases if not already requested.

## Risks / Trade-offs

- **[Risk]** Forcing `.AsImplementedInterfaces()` might bind more interfaces than intended if the class implements many interfaces.
  - **[Mitigation]** This is standard VContainer behavior for entry points. Users can use `AsTypes` for fine-grained control if needed.
