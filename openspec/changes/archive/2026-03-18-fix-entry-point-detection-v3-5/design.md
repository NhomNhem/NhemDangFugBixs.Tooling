## Context

The Source Generator identifies VContainer "Entry Points" to use the correct `builder.RegisterEntryPoint<T>` method. Currently, this detection is brittle because it relies on exact string matching against a static list of interface names. This fails when interfaces are fully qualified or when the semantic model fails to resolve them (returning an `Error` type).

## Goals / Non-Goals

**Goals:**
- Implement a robust `IsEntryPoint` detection logic that works even with qualified names.
- Ensure consistency between local class analysis and referenced assembly scanning.
- Support all modern VContainer life-cycle interfaces.

**Non-Goals:**
- Changing how the generator handles non-entry-point services.

## Decisions

### 1. Centralized Interface Utility
We will create a helper method `IsVContainerEntryPoint(string typeName)` that uses `EndsWith` or regex to match interfaces like `VContainer.Unity.IInitializable` or just `IInitializable`.

### 2. Improved Interface List
The list of interfaces will be updated to include:
- `IInitializable`, `IPostInitializable`
- `IStartable`, `IPostStartable`
- `ITickable`, `IPostTickable`
- `IFixedTickable`, `IPostFixedTickable`
- `ILateTickable`, `IPostLateTickable`
- `IAsyncStartable`
- `IDisposable`

### 3. Fallback Suffix Matching
When the semantic model returns a `TypeKind.Error`, we will perform a string-based check on the raw type name. If the name ends with any of the known interface names (prefixed with a dot or as the whole string), it will be flagged as an entry point.

### 4. Referenced Assembly Scanner Synchronization
The `ReferencedAssemblyScanner` will be updated to use the same logic as `ClassAnalyzer` to ensure services in external assemblies are correctly identified as entry points.

## Risks / Trade-offs

- **[Risk]** Potential for false positives if a user defines an interface with the same name but different namespace.
  - **[Mitigation]** Given the specific nature of these interface names (e.g., `IFixedTickable`), the risk is low. We will prefer full namespace matches when available.
