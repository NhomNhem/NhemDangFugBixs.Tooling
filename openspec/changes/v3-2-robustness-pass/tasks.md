## 1. Runtime Refactoring

- [x] 1.1 Rename `Lifetime` enum to `NhemLifetime` in `NhemDangFugBixs.Runtime`.
- [x] 1.2 Update all attributes (`AutoRegister`, `AutoRegisterIn`, etc.) to use `NhemLifetime`.
- [x] 1.3 Update `NLifetime.cs` if it exists, or add it to provide the `global using` alias.

## 2. Generator Logic - Core Emission

- [x] 2.1 Update `RegistrationEmitter.cs` to add `partial` keyword to all generated classes.
- [x] 2.2 Update `RegistrationEmitter.cs` to use fully qualified names (`global::VContainer...`) for all types.
- [x] 2.3 Remove `global using` from `RegistrationEmitter.cs` header.
- [x] 2.4 Update `VContainerAutoRegisterGenerator.cs` to generate `NhemDangFugBixs.GlobalUsings.g.cs` as a separate, single file per assembly.

## 3. Namespace & Path Robustness

- [x] 3.1 Implement improved assembly name sanitization in `RegistrationEmitter`.
- [x] 3.2 Ensure `hintName` in `AddSource` is unique and collision-free.

## 4. Verification & Documentation

- [x] 4.1 Build and verify `Sandbox` project resolves all CS1537, CS0260, CS0102 errors.
- [x] 4.2 Update `CHANGELOG.md` with v3.2.0 details.
- [x] 4.3 Update `README.md` to reflect the robustness changes.
