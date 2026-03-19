## Context

Currently, the NhemDangFugBixs Tooling supports standard service and entry point registrations. However, production games often require global error handling for Entry Points (e.g., catching exceptions in `ITickable.Tick` to prevent the game loop from crashing) and post-build initialization logic (e.g., resolving instances immediately to wire up legacy systems). VContainer provides `RegisterEntryPointExceptionHandler` and `RegisterBuildCallback` for these purposes, but users currently have to write manual registration code to use them, breaking the "auto-register" paradigm.

## Goals / Non-Goals

**Goals:**
- Provide a clean, interface-based approach for users to define global exception handlers and build callbacks.
- Automatically generate the necessary VContainer registration code for these features.
- Ensure the generated code correctly resolves the instances from the container.

**Non-Goals:**
- Supporting multiple exception handlers per scope (VContainer only supports one).
- Handling exceptions outside of VContainer's Entry Point system.

## Decisions

### 1. Interface-Based Design
We will introduce two new interfaces in `NhemDangFugBixs.Runtime`:
- `IEntryPointExceptionHandler`: Contains a single method `void OnException(System.Exception ex)`.
- `IBuildCallback`: Contains a single method `void OnBuild(global::VContainer.IObjectResolver container)`.
This approach is type-safe and easier to detect via Roslyn than attribute-based static methods.

### 2. Detection Logic
`ClassAnalyzer` will be updated to check if a class implements either of these interfaces. We will add boolean flags `isExceptionHandler` and `isBuildCallback` to `ServiceInfo`.

### 3. Registration Logic
`RegistrationEmitter` will be updated to handle these special cases:
- **Exception Handler:** If a class implements `IEntryPointExceptionHandler`, we emit a standard `Register` call for the class, AND emit `builder.RegisterEntryPointExceptionHandler(ex => { ... })` which resolves the class and calls `OnException`.
- **Build Callback:** If a class implements `IBuildCallback`, we emit a standard `Register` call for the class, AND emit `builder.RegisterBuildCallback(container => { ... })` which resolves the class and calls `OnBuild`.

### 4. Handling Multiple Handlers (Diagnostic)
If the analyzer detects multiple `IEntryPointExceptionHandler` implementations for the same scope, it should ideally issue a warning or take the first one, as VContainer expects a single handler. For this initial version, we will process all of them, but VContainer will likely only honor the last one registered.

## Risks / Trade-offs

- **[Risk]** Users might implement the interface but forget the `[AutoRegisterIn]` attribute.
  - **[Mitigation]** This is a general risk with the library. We rely on the attribute as the primary opt-in mechanism.
- **[Trade-off]** Resolving the handler instance inside the callback might have a slight performance cost during the first exception/build event.
  - **[Decision]** This cost is negligible and ensures the handler itself can participate in DI (e.g., injecting a Logger into the ExceptionHandler).
