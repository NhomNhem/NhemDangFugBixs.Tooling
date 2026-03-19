## 1. Analyzer Improvements (Conflict Guard)

- [x] 1.1 Create `ConflictCheckRule.cs` in `DangFugBixs.Analyzers`.
- [x] 1.2 Implement `ND005` logic to detect manual `builder.Register<T>` calls for types with `[AutoRegisterIn]`.
- [x] 1.3 Add Unit Tests for `ND005` (Success and Conflict cases).
- [x] 1.4 Register `ND005` in `AutoRegisterRules.cs` supported diagnostics.

## 2. Generator Improvements (Smart Filtering & Dedupe)

- [x] 2.1 Update `RegistrationEmitter.cs` to implement smart interface filtering for `RegisterEntryPoint`.
- [x] 2.2 Modify `VContainerAutoRegisterGenerator.Execute` to include a deduplication pass (GroupBy FullName).
- [x] 2.3 Refactor `EmitRegistration` to use the new filtered interface logic instead of a blind `.AsImplementedInterfaces()`.
- [x] 2.4 Fix `GenerationStats` to correctly report `v4.1.0` in the generated file header.


## 3. General Cleanup & Versioning

- [x] 3.1 Update `package.json` to version `4.1.0`.
- [x] 3.2 Update `CHANGELOG.md` with version `4.1.0` and the new Resiliency features.
- [x] 3.3 Run `Sandbox` project tests to verify the new generated code structure.
