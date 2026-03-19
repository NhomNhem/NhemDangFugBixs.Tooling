## 1. Emission Header Updates

- [x] 1.1 Update `RegistrationEmitter.cs` to always emit `using VContainer;` and `using VContainer.Unity;` in the file header.

## 2. Registration Logic Refactoring

- [x] 2.1 Update `RegistrationEmitter.cs` to prioritize Component registration even if it's an Entry Point.
- [x] 2.2 Fix logic to ensure `.AsImplementedInterfaces()` is added to Components that are also Entry Points.
- [x] 2.3 Use `global::VContainer.Lifetime.[Name]` for all lifetime arguments to prevent type conversion errors (CS1503).
- [x] 2.4 Ensure `RegisterEntryPoint` is only called for non-Component classes.

## 3. Verification & Documentation

- [x] 3.1 Verify that `InfiniteChunkManager` (Component + ITickable) is correctly registered as a Component with interface bindings.
- [x] 3.2 Verify that no CS1061 (missing extension method) or CS1503 (type mismatch) errors occur in Unity.
- [x] 3.3 Update `CHANGELOG.md` with version `3.6.0` detailing the VContainer feature parity improvements.
- [x] 3.4 Increment version to `3.6.0` in `package.json`.
