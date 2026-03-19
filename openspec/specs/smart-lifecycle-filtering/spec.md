# Capability: smart-lifecycle-filtering

## Requirement

- **Goal**: Prevent redundant lifecycle registrations when using `RegisterEntryPoint<T>()`.
- **Logic**:
  - Identify all interfaces implemented by the target class `T`.
  - Filter out interfaces that are natively managed by VContainer's `RegisterEntryPoint`.
- **Managed Interfaces**:
  - `IInitializable`, `IPostInitializable`
  - `IStartable`, `IPostStartable`, `IAsyncStartable`
  - `ITickable`, `IPostTickable`, `IFixedTickable`, `IPostFixedTickable`, `ILateTickable`, `IPostLateTickable`
  - `IDisposable`
- **Output**:
  - If a class only implements these managed interfaces, emit: `builder.RegisterEntryPoint<T>(lifetime).AsSelf();`.
  - If a class implements additional interfaces (e.g., `IBullet`), emit: `builder.RegisterEntryPoint<T>(lifetime).As<global::IBullet>().AsSelf();`.
  - NEVER emit `.AsImplementedInterfaces()` alongside `RegisterEntryPoint` unless all interfaces are explicitly filtered.

## Verification

- **Case A**: `BulletPresenter : ITickable, IBullet`.
  - Expected output: `builder.RegisterEntryPoint<global::BulletPresenter>(...).As<global::IBullet>().AsSelf();`.
- **Case B**: `SimpleService : IInitializable`.
  - Expected output: `builder.RegisterEntryPoint<global::SimpleService>(...).AsSelf();`.
