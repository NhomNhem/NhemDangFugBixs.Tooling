## 1. Detection Logic Refinement

- [x] 1.1 Create or update a utility class/method to perform robust entry point interface detection (using `EndsWith` or similar).
- [x] 1.2 Expand the list of recognized VContainer interfaces to include `IAsyncStartable`.

## 2. Analyzer & Scanner Updates

- [x] 2.1 Update `ClassAnalyzer.cs` to use the new robust detection logic, specifically in the `ExtractClassInfo` method.
- [x] 2.2 Ensure the fallback logic for `TypeKind.Error` in `ClassAnalyzer.cs` correctly identifies entry points by their raw string names.
- [x] 2.3 Update `ReferencedAssemblyScanner.cs` to use the same robust logic for identifying entry points in external assemblies.

## 3. Verification & Documentation

- [x] 3.1 Verify that services like `InfiniteChunkManager` (which implement `ITickable`, etc.) are now correctly registered using `RegisterEntryPoint`.
- [x] 3.2 Update `CHANGELOG.md` with version `3.5.0` details.
- [x] 3.3 Increment version to `3.5.0` in `package.json`.
