## Why

Nhem DI currently has overlapping interpretations of scopes, services, generated registrations, analyzer diagnostics, smoke validation, and reports. The mismatch shows up in real Unity asmdef projects where services live in feature assemblies, markers live in shared assemblies, and composition roots live in bootstrap assemblies.

Version 8.0 should make the package's core promise explicit: NhemDangFugBixs.Tooling does not force one architecture, but it lets a user's architecture become compile-time checked.

## What Changes

- Introduce a shared DI contract graph that models scope identities, composition roots, auto-registered services, manual registrations, generated installers, diagnostics evidence, and assembly provenance.
- Rework generator, analyzers, smoke validation, and report emission to consume the same contract graph instead of re-parsing attributes independently.
- Make composition roots deterministic: every `[LifetimeScopeFor<TScope>]` SHALL have a generated `RegisterGeneratedFor<TScope>()` entry point, including no-op scopes and scopes whose services are discovered from referenced assemblies.
- Improve cross-assembly Unity asmdef behavior for the common marker/shared, feature-service, bootstrap-composition layout.
- Preserve generated API compatibility for existing users while allowing 8.0 internals to replace the current scattered model.
- Remove active OpenSpec proposals from older version tracks from the active backlog; archived copies remain available for history.
- **BREAKING**: diagnostics that relied on incomplete per-compilation assumptions may be renamed, reclassified, or moved to smoke validation when they require project-wide evidence.

## Capabilities

### New Capabilities
- `di-contract-graph`: Defines the shared semantic contract graph used by generator, analyzers, smoke validation, and reports.
- `cross-assembly-composition-generation`: Defines deterministic generated registration behavior across Unity asmdef boundaries.
- `di-smoke-contract-validation`: Defines project-wide validation that cannot be proven inside a single Roslyn compilation.

### Modified Capabilities
- `semantic-scope-analyzer`: Scope diagnostics shall be based on the shared DI contract graph and must avoid false positives when mappings are only visible project-wide.
- `conflict-detection-analyzer`: Manual registration conflict detection shall understand generated/no-op registration paths and distinguish deliberate composition calls from duplicate service registration.
- `di-visualizer-report`: Reports shall be emitted from the same DI contract graph used for generation and validation.

## Impact

- Generator code under `Source~/DangFugBixs.Generators~`.
- Analyzer rules and tests under `Source~/DangFugBixs.Analyzers~`.
- Common models under `Source~/DangFugBixs.Common~`.
- Smoke validation and CLI report code under `Source~/DangFugBixs.Tools~`.
- Runtime attributes remain source-compatible unless a later task explicitly introduces a migration.
- Unity package metadata and release notes will move from `7.4.0` toward `8.0.0` after implementation passes validation.
