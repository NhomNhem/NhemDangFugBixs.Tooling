## Why

The v3.3.0 release improved robustness, but the generator still crashes when encountering unresolved assemblies in Unity (e.g., `AssemblyResolutionException` for missing dependencies). A single failure in a referenced assembly should not break the entire code generation process. Additionally, generated files lack metadata for traceability and troubleshooting.

## What Changes

- **Resilient Scanning**: Wrap assembly member access in try-catch blocks to gracefully handle unresolved references.
- **Emission Traceability**: Add metadata headers (version, timestamp, service count, warnings) to all generated `.g.cs` files.
- **Diagnostic Reporting**: Emit non-breaking warnings (Diagnostics) when an assembly scan fails, instead of throwing an exception.
- **Pre-flight Check**: Improve assembly name sanitization and unique hint name enforcement.

## Capabilities

### New Capabilities
- `traceable-emission`: Addition of metadata headers to generated files for easier troubleshooting.
- `non-breaking-diagnostics`: Reporting of scan failures as IDE warnings rather than compiler errors.

### Modified Capabilities
- `strict-single-file-emission`: Enhancement of the single-file emission logic to include resiliency checks.

## Impact

- **NhemDangFugBixs.Generators**: Updates to `ReferencedAssemblyScanner` and `RegistrationEmitter`.
- **Unity Projects**: Improved stability; the generator will produce registrations for available assemblies even if some references are unresolved.
