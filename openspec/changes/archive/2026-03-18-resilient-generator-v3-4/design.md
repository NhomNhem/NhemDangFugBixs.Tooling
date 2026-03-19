## Context

The v3.3.0 release consolidated the generator output, but it remains vulnerable to external failures in Unity's assembly resolution (e.g., during cross-assembly discovery of services with `[AutoRegisterIn]`). When a referenced assembly fails to resolve, the generator currently crashes the entire compilation.

## Goals / Non-Goals

**Goals:**
- Make the generator "resilient" to external assembly resolution failures.
- Provide clear diagnostics (warnings) for failed assembly scans.
- Enhance troubleshooting with file metadata headers.
- Maintain a single-file output per assembly.

**Non-Goals:**
- Removing the `[AutoRegisterIn]` feature.
- Changing the primary registration logic.

## Decisions

### 1. Resilient Scanning in `ReferencedAssemblyScanner`
We will wrap the `foreach` loop over `compilation.SourceModule.ReferencedAssemblySymbols` and the `ScanNamespace` calls in try-catch blocks. If a `SymbolResolutionException` or similar occurs, we will catch it, log a warning diagnostic, and proceed with other assemblies.

### 2. Traceability Header in `RegistrationEmitter`
The `GenerateSource` method will be updated to accept a `GenerationStats` object (including warning list, service count, and tool version). This information will be emitted as a commented header block.

### 3. New Diagnostic Code (ND104)
A new diagnostic descriptor `ND104` will be added for "Unresolved Assembly Scan". This is a warning, not an error, so it won't break the build but will appear in the Unity console.

### 4. Timestamp Generation
We will use `DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")` for the generation timestamp to distinguish between build versions.

## Risks / Trade-offs

- **[Risk]** Partial registration might mask real issues if a service is missed but the build succeeds.
  - **[Mitigation]** The `ND104` warning ensures the user is aware of the missed scan.
- **[Trade-off]** Timestamped headers might cause Git noise if generated files are committed.
  - **[Decision]** Unity generated files are usually in `Temp/Generated` or `Library/`, so Git noise is minimal. For users who commit them, the benefit of traceability outweighs the noise.
