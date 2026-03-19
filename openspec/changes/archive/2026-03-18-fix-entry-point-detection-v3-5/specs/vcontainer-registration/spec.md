## MODIFIED Requirements

### Requirement: VContainer registration method generation
The generator SHALL generate registration methods for each scope, using `builder.RegisterEntryPoint<T>` for classes identified as VContainer Entry Points.

**Previous Behavior:**
- Entry point detection relied on exact string matches, often missing qualified names or new interfaces.

**New Behavior:**
- Uses robust interface detection to identify entry points.
- Automatically emits `builder.RegisterEntryPoint<T>(lifetime)` for classes that implement VContainer life-cycle interfaces.

#### Scenario: Generate entry point registration
- **WHEN** user writes `[AutoRegisterIn(typeof(GameScope))] public class GameManager : IInitializable { }`
- **THEN** generator emits `builder.RegisterEntryPoint<global::GameManager>(global::VContainer.Lifetime.Singleton);`
