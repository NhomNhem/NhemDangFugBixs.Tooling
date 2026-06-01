## 1. Contract Graph Foundation

- [x] 1.1 Add DI contract graph models for scope identities, service registrations, composition roots, manual registrations, generated installers, and diagnostic evidence.
- [x] 1.2 Add deterministic graph query APIs for services by scope, composition roots by marker, manual registrations by implementation, and generated installers by scope.
- [x] 1.3 Add unit tests for graph identity, stable ordering, duplicate implementation resolution, and assembly provenance.
- [x] 1.4 Add regression fixtures that model shared marker, feature service, and bootstrap composition assemblies.

## 2. Generator Migration

- [x] 2.1 Refactor current generator extraction to populate the DI contract graph without changing public generated APIs.
- [x] 2.2 Emit generated registration routes for every `[LifetimeScopeFor<TScope>]` mapping, including no-op installers.
- [x] 2.3 Emit referenced-assembly services through the composition assembly output when the assembly reference path makes them discoverable.
- [x] 2.4 Preserve compatibility methods: `RegisterGeneratedFor<TScope>()`, marker-specific generated helpers, and legacy named registration methods.
- [x] 2.5 Update generator snapshot and cross-assembly tests for no-op scopes and `afterimage`-style asmdef layout.

## 3. Analyzer Migration

- [x] 3.1 Migrate semantic scope analyzer rule logic to consume graph-backed evidence.
- [x] 3.2 Ensure per-compilation analyzer diagnostics do not report project-wide missing mapping conditions without enough evidence.
- [x] 3.3 Migrate manual registration conflict detection to distinguish generated route calls from duplicate manual service registrations.
- [x] 3.4 Update `NHEM_DI_011` tests so missing generated calls are reported only when the composition root and expected marker are proven.
- [x] 3.5 Document any diagnostic severity or ownership changes between analyzers and smoke validation.

## 4. Smoke Validation and Reports

- [x] 4.1 Migrate smoke validation to build or consume a project-wide DI contract graph.
- [x] 4.2 Add smoke diagnostics for composition assemblies that cannot discover service assemblies for a mapped scope.
- [x] 4.3 Add machine-readable smoke output with scope marker, registration, composition root, and assembly reference path evidence.
- [x] 4.4 Migrate Markdown, CSV, and JSON reports to use the same graph data used by generator and smoke validation.
- [x] 4.5 Add report coverage for no-op mapped scopes and referenced-assembly registrations.

## 5. Integration and Release Readiness

- [x] 5.1 Run existing generator, analyzer, CLI, and editor smoke tests and fix regressions.
- [x] 5.2 Validate the 8.0 package against an `afterimage`-style Unity project and remove any manual generated-registration bridge workaround.
- [x] 5.3 Update README, CHANGELOG, and migration notes for 8.0 DI architecture behavior.
- [x] 5.4 Update package metadata from `7.4.0` to `8.0.0` after validation passes.
- [x] 5.5 Run final release validation, including strict OpenSpec validation and package build verification.
