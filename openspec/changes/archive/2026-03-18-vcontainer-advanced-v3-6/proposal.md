## Why

To make the NhemDangFugBixs Tooling a truly comprehensive Dependency Injection framework for Unity, we need to support advanced VContainer features. Specifically, handling global exceptions for Entry Points (`RegisterEntryPointExceptionHandler`) and executing logic immediately after the container is built (`RegisterBuildCallback`) are critical for production-ready games. Currently, users have to manually write boilerplate code to achieve this.

## What Changes

- **Global Exception Handler Support**: Introduce an `IEntryPointExceptionHandler` interface in the `Runtime` assembly. The generator will automatically detect classes implementing this interface and register them using `builder.RegisterEntryPointExceptionHandler(...)`.
- **Build Callback Support**: Introduce an `IBuildCallback` interface in the `Runtime` assembly. The generator will automatically detect classes implementing this interface and register them using `builder.RegisterBuildCallback(...)`.
- **Analyzer & Emitter Updates**: Update `ClassAnalyzer` to detect these new interfaces and `RegistrationEmitter` to generate the appropriate VContainer API calls.

## Capabilities

### New Capabilities
- `global-exception-handler`: Automatic wiring of VContainer's Entry Point Exception Handler via a dedicated interface.
- `build-callback-support`: Automatic wiring of VContainer's Build Callback via a dedicated interface.

### Modified Capabilities

## Impact

- **NhemDangFugBixs.Runtime**: Addition of two new interfaces (`IEntryPointExceptionHandler`, `IBuildCallback`).
- **NhemDangFugBixs.Generators**: Updates to `ClassAnalyzer` and `RegistrationEmitter`.
- **Unity Projects**: Users can now handle DI errors and execute post-build logic simply by implementing an interface and adding the `[AutoRegisterIn]` attribute.
