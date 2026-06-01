# Changelog

## 8.0.0 - 2026-06-02

### Added
- Shared DI contract graph for generator, analyzers, smoke validation, and report output.
- Deterministic generated registration routes for every `[LifetimeScopeFor<TScope>]`, including no-op mapped scopes.
- Referenced-assembly registration emission for Unity asmdef layouts where markers live in shared assemblies, services live in feature assemblies, and composition roots live in bootstrap assemblies.
- `ND006` warning for cross-scope dependencies when the current compilation proves both sides and the target scope is unreachable.
- Machine-readable graph evidence in smoke validation and report output, including scope marker, service, composition root, source assembly, composition assembly, and reference path facts.

### Changed
- Analyzer checks now use graph-backed evidence and avoid reporting project-wide missing mapping conditions from incomplete per-compilation data.
- `ND005` duplicate manual registration detection now ignores generated installer routes and no-op generated registration calls.
- Project-wide missing mapping and assembly-reference gaps are owned by `di-smoke` validation instead of speculative per-compilation analyzer warnings.

### Migration
- Add `using NhemDangFugBixs.VContainer;` in composition files that call `builder.RegisterGeneratedFor<TScope>()`.
- Remove manual generated-registration bridge workarounds; 8.0 emits the generated route from the composition assembly.
- Refresh Unity generated `.csproj` files or reopen Unity after package cache/hash changes so analyzer paths point at the current PackageCache folder.

### Validation
- Generator, analyzer, common graph, CLI/smoke validation tests, solution build, and an `afterimage`-style bootstrap compile were used as release gates.

## 7.4.0 - 2026-05-28

### Added
- Composition-only generation mode: only assemblies containing `[LifetimeScopeFor<T>]` emit generated installers.
- Cross-asmdef validation via `di-smoke` CLI tool.
- Diagnostics NDFG005 (duplicate composition target), NDFG006 (duplicate discovered registration), NDFG007 (missing VContainer reference).
- `SmokeValidationOptions.AssemblyPaths` supports multiple assembly paths.
- Release-gate cross-asmdef validation with exit code check.

### Changed
- `RegisterEntryPoint<T>()` generator output now includes `Lifetime` parameter: `RegisterEntryPoint<T>(Lifetime.Scoped)`.

### Fixed
- CI build failures from stale generated files in Sandbox.
- Missing Unity asmdef files for Runtime, Attributes, and Analyzers.

## 7.3.1 - 2026-05-18

### Fixed
- Added the missing Unity `.meta` file for the Tooling Diagnostics editor menu asset.
- Ensured deploy output includes `docs/assets.meta` with the shipped documentation assets.
- Removed analyzer `.pdb` files from deploy output to avoid immutable UPM import warnings.

## 7.3.0 - 2026-05-17

### Added
- NHEM_DI_061: Duplicate explicit contract exposure warning. Detects duplicate `[As(...)]` declarations for the same contract type.
- NHEM_DI_066: RegisterComponentInHierarchy on non-MonoBehaviour error. Ensures `[RegisterComponentInHierarchy]` is only used on MonoBehaviour types.
- NHEM_DI_067: EntryPoint without known lifecycle contract warning. Ensures `[EntryPoint]` types implement a known VContainer lifecycle interface (IStartable, ITickable, IInitializable, IDisposable, etc.).
- Analyzer tests for all new diagnostics including positive and negative cases.

### Changed
- Analyzer source layout recovered from v7.1.0 tag and restored to `Source~/DangFugBixs.Analyzers~/`.
- Analyzer csproj PostBuild target made conditional on `$(FinalPluginsPath)` for test-only builds.

### Validation
- 34 analyzer tests pass (8 new + 26 existing).

## 7.2.2 - 2026-05-17

### Added
- Unity Editor diagnostics menu (Tools/Nhem/Tooling Diagnostics/Print Diagnostics) to detect stale package imports, version mismatches, and missing analyzer DLLs.
- Troubleshooting documentation for stale Unity PackageCache issues.

## 7.2.1 - 2026-05-17

### Changed
- Prefer explicit exposure attributes over implicit registration exposure detection.

## 7.2.0 - 2026-05-17

### Changed
- Removed runtime testing fixtures from shipped runtime surface to prevent Unity package analyzer noise.
- Added release gate Unity sample dotnet preflight build and stricter Unity compile failure handling.
- Updated composition-only sandbox fixtures to keep cross-layer coverage in test-only code.

## 7.1.0 - 2026-05-17

### Changed
- Deploy workflow now copies the package metadata files alongside the main Unity package folders.
- Package version aligned with the generator assembly version.

## 6.1.0 - 2026-05-13

### Added
- Marker-first VContainer workflow direction across runtime attributes, generator output, and diagnostics.
- Scope-marker mapping support with `LifetimeScopeFor` bridging marker identities to concrete `LifetimeScope` owners.
- Generated installer entry points for marker scopes (`RegisterGeneratedFor<TScopeMarker>` and non-generic variants).
- Expanded generator tests for scope mapping, bindings, entry points, components, and cross-assembly discovery.
- Expanded analyzer test suites for scope mapping, injection style, lifetime diagnostics, and resolver misuse patterns.
- Unity Editor diagnostics smoke tests for window creation and render path stability.
- Package samples for:
  - Basic registration
  - Scope marker architecture
  - MessagePipe integration
  - Scene component registration

### Changed
- Documentation now emphasizes marker-based architecture and generated installer usage as the default workflow.
- Diagnostics and validation messaging aligned with architecture guardrails and preflight usage.

### Validation
- Generator tests, analyzer tests, and CLI validation tests executed as implementation signoff gates.
