## 1. Generator Core Refactoring

- [x] 1.1 Update `RegistrationEmitter.cs` to move the `NLifetime` alias from `global using` to a local `using` inside the namespace block.
- [x] 1.2 Update `RegistrationEmitter.cs` to ensure `SceneInjectionBlueprint` is declared as `public static partial class`.
- [x] 1.3 Update `RegistrationEmitter.cs` to ensure `VContainerRegistration` is declared as `public static partial class`.
- [x] 1.4 Update `RegistrationEmitter.cs` to ensure `ComponentTypes` array in `SceneInjectionBlueprint` is deduplicated using `GroupBy` or `HashSet`.
- [x] 1.5 Update `RegistrationEmitter.cs` to use `global::NhemDangFugBixs.Attributes.NhemLifetime` for all registrations.

## 2. Source Generation Pipeline

- [x] 2.1 Update `VContainerAutoRegisterGenerator.cs` to use `{sanitizedHint}.g.cs` as the deterministic hint name for all emissions.
- [x] 2.2 Remove any logic that still attempts to emit separate `GlobalUsings.g.cs` files if any exists.

## 3. Verification & Cleanup

- [x] 3.1 Verify that the generated code compiles correctly in a multi-assembly scenario.
- [x] 3.2 Update `GEMINI.md` to fix the outdated `NLifetime` alias documentation.
- [x] 3.3 Update `CHANGELOG.md` with version v3.3.0 detailing the robustness fixes.
- [x] 3.4 Increment version to `3.3.0` in `package.json`.
