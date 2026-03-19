## Why

The current version (v3.4.0) of the Source Generator fails to correctly register classes that implement VContainer Entry Point interfaces (like `IInitializable`, `ITickable`, `IFixedTickable`, `IAsyncStartable`, etc.) when they also use the `[AutoRegisterIn]` attribute. This is due to rigid string matching in the interface detection logic, which misses interfaces when they are referred to by their full namespace or when the semantic model returns an error. This forcing users to manually register these essential services.

## What Changes

- **Intelligent Interface Detection**: Upgrade the detection logic to use fuzzy/suffix matching for well-known VContainer interfaces.
- **Expanded Entry Point Support**: Add support for all modern VContainer interfaces, including `IAsyncStartable`.
- **Metadata Alignment**: Ensure the `ReferencedAssemblyScanner` uses the same robust detection logic as the `ClassAnalyzer`.
- **Validation**: Enable automatic registration for common services like `InfiniteChunkManager` that were previously skipped.

## Capabilities

### New Capabilities
- `robust-entry-point-detection`: Advanced logic for identifying VContainer entry point interfaces regardless of namespace qualification.

### Modified Capabilities
- `vcontainer-registration`: Update emission logic to handle the newly detected entry points correctly using `builder.RegisterEntryPoint<T>`.

## Impact

- **NhemDangFugBixs.Generators**: Updates to `ClassAnalyzer`, `ReferencedAssemblyScanner`, and `RegistrationEmitter`.
- **Unity Projects**: Users can stop manually registering services that implement `IInitializable`, `ITickable`, etc.; the generator will now handle them automatically.
