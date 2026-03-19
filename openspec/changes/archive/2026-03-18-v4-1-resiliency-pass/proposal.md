## Why

The current v4.0 release, while functionally complete, suffers from "over-enthusiastic" registration logic and a lack of safeguards against manual developer errors. Specifically, the "Double Registration" bug in classes like `BulletPresenter` (registering as both an EntryPoint and via manual code) leads to runtime side-effects like multiple `Tick()` calls per frame. To achieve a "9.5/10" professional standard, the tool must move from "convenient automation" to "fail-safe automation."

## What Changes

- **Smart EntryPoint Registration**: The generator will now automatically filter out VContainer lifecycle interfaces (`ITickable`, `IStartable`, etc.) when using `RegisterEntryPoint<T>()`, as VContainer handles these natively.
- **Manual Conflict Safeguard (ND005)**: A new Roslyn Analyzer will detect and block manual `builder.Register<T>` calls in `LifetimeScope` files for types already marked with `[AutoRegisterIn]`.
- **Cross-Layer Registration Deduplication**: Implementation of a strict "Single Registration" rule in the generator to prevent duplicate entries from multiple assemblies or drift.
- **Version Synchronization**: Update all internal version strings to reflect the official `v4.1.0` status.

## Capabilities

### New Capabilities
- `conflict-detection-analyzer`: Detects and prevents double registration between Source Generator and manual code (ND005).
- `smart-lifecycle-filtering`: Automatically manages VContainer lifecycle interfaces to prevent redundant registration side-effects.

### Modified Capabilities
- `vcontainer-registration`: Enhanced with deduplication and normalized contract generation (.AsSelf, .AsImplementedInterfaces).

## Impact

- **NhemDangFugBixs.Generators**: Updates to `RegistrationEmitter`, `VContainerAutoRegisterGenerator`, and `ReferencedAssemblyScanner`.
- **NhemDangFugBixs.Analyzers**: Addition of `ConflictCheckRule (ND005)`.
- **Unity Projects**: Improved runtime stability and clearer compile-time errors for DI configuration.
