# BasicAutoRegister

Demonstrates the smallest setup:

1. Define a scope marker (`IGameplayScope`).
2. Mark a service with `[AutoRegisterIn<IGameplayScope>]` and `[As<T>]`.
3. Map marker to `LifetimeScope` via `[LifetimeScopeFor<IGameplayScope>]`.
4. Call `builder.RegisterGeneratedFor<IGameplayScope>()`.
