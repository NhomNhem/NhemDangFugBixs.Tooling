## 1. Runtime Updates

- [x] 1.1 Create `IEntryPointExceptionHandler.cs` in `NhemDangFugBixs.Runtime/Attributes`.
- [x] 1.2 Create `IBuildCallback.cs` in `NhemDangFugBixs.Runtime/Attributes`.

## 2. Model Updates

- [x] 2.1 Update `ServiceInfo.cs` in `NhemDangFugBixs.Common/Models` to include `IsExceptionHandler` and `IsBuildCallback` boolean properties.

## 3. Analyzer Updates

- [x] 3.1 Update `ClassAnalyzer.cs` to check for `IEntryPointExceptionHandler` and set `isExceptionHandler = true`.
- [x] 3.2 Update `ClassAnalyzer.cs` to check for `IBuildCallback` and set `isBuildCallback = true`.
- [x] 3.3 Ensure `ReferencedAssemblyScanner.cs` also correctly sets these new flags during cross-assembly discovery.

## 4. Emitter Updates

- [x] 4.1 Update `RegistrationEmitter.cs` to generate `builder.RegisterEntryPointExceptionHandler(ex => ...)` when `isExceptionHandler` is true.
- [x] 4.2 Update `RegistrationEmitter.cs` to generate `builder.RegisterBuildCallback(container => ...)` when `isBuildCallback` is true.
- [x] 4.3 Ensure the base registration (e.g., `builder.Register<T>`) is still emitted for these classes so their instances can be resolved in the callbacks.

## 5. Verification & Documentation

- [x] 5.1 Verify that an implementation of `IEntryPointExceptionHandler` correctly generates the exception handler registration code.
- [x] 5.2 Verify that an implementation of `IBuildCallback` correctly generates the build callback registration code.
- [x] 5.3 Update `README.md` to document these new advanced features.
- [x] 5.4 Update `CHANGELOG.md` with version details for v3.6.0.
