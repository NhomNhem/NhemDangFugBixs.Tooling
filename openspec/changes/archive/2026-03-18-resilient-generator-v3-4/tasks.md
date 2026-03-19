## 1. Diagnostics & Models

- [x] 1.1 Create `GenerationStats` model in `NhemDangFugBixs.Common.Models` (if applicable) or pass it locally.
- [x] 1.2 Add `ND104` DiagnosticDescriptor for unresolved assembly scan failures.

## 2. Generator Logic - Resilient Scan

- [x] 2.1 Update `ReferencedAssemblyScanner.cs` with try-catch logic in `Scan` method.
- [x] 2.2 Implement error collection during the scan to report back to the main generator.
- [x] 2.3 Ensure `ScanNamespace` handles resolution failures gracefully.

## 3. Emission Logic - Metadata Headers

- [x] 3.1 Update `RegistrationEmitter.cs` to accept metadata (version, date, service count, warnings).
- [x] 3.2 Update `GenerateSource` to emit the metadata header at the very top of the generated file.

## 4. Generator Pipeline - Wiring & Reporting

- [x] 4.1 Update `VContainerAutoRegisterGenerator.cs` to catch scanner errors and report them as `ND104` diagnostics.
- [x] 4.2 Pass generation statistics to the `RegistrationEmitter`.
- [x] 4.3 Standardize the tool version string to `v3.4.0`.

## 5. Verification & Documentation

- [x] 5.1 Verify that the generator no longer crashes when an assembly dependency is missing.
- [x] 5.2 Verify that `ND104` warnings appear in the Unity console.
- [x] 5.3 Update `CHANGELOG.md` with version `3.4.0` details.
- [x] 5.4 Increment version to `3.4.0` in `package.json`.
