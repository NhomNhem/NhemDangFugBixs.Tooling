## Context

NhemDangFugBixs.VContainer.SourceGenerator currently has drift between docs, package metadata, generator output, and runtime DX surface. Consumers hit import issues (missing package dependency metadata), unresolved extension usage (`builder.RegisterGeneratedFor<TScope>()`), and inconsistent generated output/versioning. The package must become production-safe for Unity package consumers while preserving the core philosophy: marker-first explicit primitives, user-owned `LifetimeScope`, and no forced architecture.

Current constraints:
- Unity package must remain reusable across projects with different architecture slices.
- Generator must emit stateless, explicit code that looks hand-written by senior Unity/VContainer developers.
- Analyzer diagnostics must prevent ambiguous/unsafe registration patterns without requiring magic runtime behavior.
- VContainer API baseline targets `IContainerBuilder`, `Lifetime`, `Register*`, `RegisterEntryPoint*`, `RegisterComponentInHierarchy*`, keyed registration where available.

Stakeholders:
- Package maintainers (stability, release quality)
- Game teams consuming package (DX, migration safety)
- CI/release owners (deterministic gates)

## Goals / Non-Goals

**Goals:**
- Define stable public attribute surface for registration intent (`AutoRegisterIn`, `As`, `AsSelf`, `LifetimeScopeFor`, `EntryPoint`, `Keyed`, `RegisterComponentInHierarchy`).
- Emit deterministic generated artifacts:
  - static per-scope installers
  - single extension-dispatch method `RegisterGeneratedFor<TScope>()`
  - optional report metadata
- Enforce marker-first mapping with user-owned `LifetimeScope` classes.
- Introduce analyzer MVP (`NHEM_DI_001`..`NHEM_DI_050`) with clear severity and remediation.
- Add release gate criteria that validate tests, package metadata, API/docs parity, and no mutable static runtime state.
- Provide migration strategy from manual registration to generated registration with incremental strictness.

**Non-Goals:**
- Generate Unity `LifetimeScope` MonoBehaviours by default.
- Partial-inject generated code into user `LifetimeScope` or other user MonoBehaviours by default.
- Introduce runtime resolver caches/service-locator state inside generated code.
- Implement Addressables/pooling/prefab runtime orchestration in this change.
- Force any single project architecture beyond explicit scope-marker primitives.

## Decisions

### 1) Keep user-owned LifetimeScope, generate installer + DX extension only
- Decision: generator emits static installers and extension dispatch; user keeps `Configure` body ownership.
- Rationale: preserves architecture freedom and avoids magic scene object side effects.
- Alternatives considered:
  - Auto-generate `LifetimeScope` classes: rejected (architecture forcing, Unity scene complexity).
  - Partial injection into user scope: rejected by default (hidden behavior and merge ambiguity).

### 2) Use a canonical intermediate registration model
- Decision: normalize all discovered registration declarations into `RegistrationModel` before emit.
- Rationale: enables consistent diagnostics, dedupe, and deterministic emission.
- Alternatives considered:
  - Emit directly from syntax nodes: rejected (harder diagnostics/duplicate handling).

### 3) Dispatch extension is DX sugar over static installers
- Decision: `builder.RegisterGeneratedFor<TScope>()` only dispatches to generated static installer and throws explicit error when no mapping exists.
- Rationale: ergonomic API, avoids duplicate registration logic.
- Alternatives considered:
  - Direct static calls only: less DX, inconsistent with docs intent.

### 4) Analyzer-first guardrails for ambiguity
- Decision: treat invalid contract/scope mapping and duplicate generated invocation as errors; misuse patterns as warnings in MVP.
- Rationale: fail-fast for incorrect codegen contract, warn-first for migration concerns.
- Alternatives considered:
  - Warning-only everywhere: rejected (too easy to ship broken mapping).

### 5) Deterministic generated layout and naming
- Decision: emit only known partial generated classes:
  - `NhemGeneratedVContainerExtensions`
  - `NhemGenerated{ScopeName}ScopeInstaller` (or `{ScopeName}Installer` by naming policy)
  - optional diagnostics/report classes
- Rationale: predictable compile shape and simpler test snapshots.

### 6) Release gates become contractual
- Decision: release blocked unless test matrix + package validation + docs/API parity + version sync all pass.
- Rationale: existing failures came from drift rather than isolated runtime bug.

## Risks / Trade-offs

- [Risk] Analyzer strictness increases upgrade friction for legacy projects.
  - Mitigation: phased migration mode (warnings first, strict mode later).

- [Risk] Keyed support depends on VContainer version/API availability.
  - Mitigation: document API baseline, gate by compile tests against target package version.

- [Risk] Duplicate detection can false-positive in advanced manual installer compositions.
  - Mitigation: explicit allow pattern (future optional primitives) and precise diagnostic messages.

- [Risk] Large multi-capability change may sprawl across generators/analyzers/docs.
  - Mitigation: split into small tasks with independent verification checkpoints.

- [Risk] Version drift reappears across package/generator/docs.
  - Mitigation: single-source version policy + CI check.

## Migration Plan

1. Keep existing manual `LifetimeScope` classes unchanged.
2. Introduce marker interfaces (`IScopeMarker` + concrete scope markers).
3. Annotate low-risk services with `AutoRegisterIn` and contracts.
4. Add `builder.RegisterGeneratedFor<TScope>()` in mapped `LifetimeScope`.
5. Enable analyzers in warning mode and fix surfaced issues.
6. Turn on strict diagnostics for duplicate invocation and invalid contract/mapping.
7. Validate with sample compile + generator/analyzer test matrix before release.

Rollback strategy:
- Disable generated invocation call and keep/manual registrations intact.
- Downgrade analyzer severity profile to warnings-only during emergency rollback.

## Open Questions

- Should `Keyed` accept only compile-time constants in MVP, or include broader key expression forms?
- Should `RegisterComponentInHierarchy` be a boolean flag, explicit attribute, or both long-term?
- Do we include `ManualInstallerFor<TScope>` hook in this release or explicitly defer to next change?
- What exact naming policy should be final for installer classes (`ScopeInstaller` suffix always vs condensed names)?
- Should `NHEM_DI_021` allow explicit suppression/allow-list metadata in MVP or next increment?
