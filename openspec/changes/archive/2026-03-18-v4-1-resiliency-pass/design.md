## Context

The current v4.0 release is too permissive. It blindly follows attributes without checking if the same types are registered manually in `LifetimeScope` files. Additionally, the generator is unaware of VContainer's native behaviors when using `RegisterEntryPoint<T>`, leading to redundant `.AsImplementedInterfaces()` calls that trigger multiple lifecycle events (e.g., `Tick()` running twice).

## Goals / Non-Goals

**Goals:**
- **Zero-Redundancy Registration**: Generator must only emit `.As()` for non-lifecycle interfaces when using `RegisterEntryPoint`.
- **Compile-Time Conflict Detection**: A Roslyn Analyzer must flag manual registrations that conflict with auto-registration attributes.
- **Strict Deduplication**: One type, one registration, one owner per scope.
- **Version Alignment**: Standardize all version markers to v4.1.0.

**Non-Goals:**
- Changing the existing `[AutoRegisterIn]` attribute syntax.
- Supporting complex registration logic (like `.WithParameter`) in the generator (keep it simple).

## Decisions

### 1. Smart Interface Filtering (Generator)
- **Problem**: `RegisterEntryPoint<T>` natively maps to `IInitializable`, `ITickable`, etc.
- **Decision**: Update `RegistrationEmitter.cs` to filter `svc.InterfaceNames` using `InterfaceUtils.IsVContainerEntryPoint`.
- **Rationale**: Prevents VContainer from creating multiple internal mappings for the same lifecycle event.

### 2. The ND005 Analyzer (Conflict Guard)
- **Problem**: SG cannot know if a dev also wrote `builder.Register<T>` in C# code.
- **Decision**: Use a Roslyn `SyntaxNodeAction` for `InvocationExpression` to intercept `Register<T>` and `RegisterEntryPoint<T>` calls. Check if `T` has the `[AutoRegisterIn]` attribute.
- **Rationale**: Shifting the error from "Runtime Side-Effect" to "Compile-Time Error" (Fail Fast).

### 3. Generator Deduplication (Post-Scan Pass)
- **Problem**: `ReferencedAssemblyScanner` might collect duplicate `ServiceInfo` from different assemblies.
- **Decision**: Add a `GroupBy(FullName)` pass in `VContainerAutoRegisterGenerator.Execute` before calling the emitter.
- **Rationale**: Ensures the final `.g.cs` file is clean and deterministic.

## Risks / Trade-offs

- **[Risk]**: Analyzer performance impact on large projects.
  - **Mitigation**: Scoping the analyzer specifically to `InvocationExpression` on `builder` methods and checking for the attribute presence early.
- **[Risk]**: Breaking changes for users who *wanted* double registration (rare).
  - **Mitigation**: This is considered a bug fix for v4.1.0; users should use manual registration OR auto-registration, not both.
