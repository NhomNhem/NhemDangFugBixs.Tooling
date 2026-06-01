# Roadmap: NhemDangFugBixs.Tooling 7.2.2 to 8.0.0

## Change Name
`tooling-roadmap-7-2-2-to-8-0`

## Vision Statement

NhemDangFugBixs.Tooling should become a **compile-time DI architecture guardrail** for Unity + VContainer.

### Core Principle
- Services declare intent
- Composition owns registration
- Diagnostics protect architecture

### Product Positioning
The package should evolve into Unity DI architecture tooling, not a magic runtime framework.

## Current State

### Completed
- **7.2.0**: Introduced composition-only generation mode
- **7.2.1**: Cleaned up registration exposure style by making explicit `[As]` / `[AsSelf]` canonical

### Next Steps
This roadmap outlines the evolution from 7.2.2 through 8.0.0 to mature the tooling into a comprehensive DI architecture guardrail.

## Non-Goals

The following features are explicitly out of scope for this roadmap:

- Do not add RegisterInstance generation
- Do not add Addressables integration
- Do not add prefab factory generation
- Do not add pooling integration
- Do not generate LifetimeScope MonoBehaviours
- Do not partial-inject into user LifetimeScope
- Do not add scene auto setup
- Do not add MessagePipe auto-magic yet
- Do not introduce runtime reflection scanning
- Do not scan all loaded Unity assemblies

## Roadmap Phases

### Phase 7.2.2 — Import and Package Diagnostics
**Goal**: Prevent real Unity projects from accidentally using stale cached package DLLs or mismatched package payloads.

**Key Features**:
- Unity Editor diagnostics menu: Tools/Nhem/Tooling Diagnostics
- Display package version, loaded assembly versions, analyzer DLL path, package path
- Warn on version mismatches
- Troubleshooting documentation for stale package cache issues

**Impact**: Improves developer experience by catching configuration errors early.

### Phase 7.3.0 — Analyzer Maturity
**Goal**: Make analyzers catch common architecture and usage mistakes without false positives.

**Key Features**:
- NHEM_DI_061: Duplicate explicit contract exposure
- NHEM_DI_062: Cross-assembly auto-registered implementation or contract must be public
- NHEM_DI_063: EntryPoint placed in service-only assembly may require VContainer.Unity
- NHEM_DI_064: LifetimeScopeFor exists but Configure does not call RegisterGeneratedFor<TScope>()
- NHEM_DI_066: RegisterComponentInHierarchy used on non-MonoBehaviour
- NHEM_DI_067: EntryPoint does not implement known VContainer lifecycle interface

**Impact**: Catches architecture violations at compile time with reliable diagnostics.

### Phase 7.4.0 — DI Smoke Project-Level Validation
**Goal**: Provide project-level validation that can inspect Unity asmdef structure beyond Roslyn compilation boundaries.

**Key Features**:
- CLI command: `nhem di-smoke --project <UnityProjectRoot>`
- Unity menu: Tools/Nhem/DI Smoke Validate
- Checks: service-only asmdefs, composition asmdefs, duplicate scopes, missing composition targets
- Output: Human-readable console report and optional JSON report

**Impact**: Validates project structure and package integration at the project level.

### Phase 7.5.0 — DI Report and Graph Viewer
**Goal**: Make composition graph visible to developers.

**Key Features**:
- Output: Library/NhemDangFugBixs/di-report.json
- Report includes: scopes, composition assemblies, services, contracts, lifetimes, registration kind, source assembly, warnings
- Unity Editor UI: Tools/Nhem/DI Report

**Impact**: Provides visibility into DI composition structure for debugging and architecture review.

### Phase 7.6.0 — Sample Suite
**Goal**: Provide multiple small scenario-focused Unity samples.

**Key Features**:
- 01-basic-service
- 02-composition-only-asmdef
- 03-entrypoint-composition-adapter
- 04-component-in-hierarchy
- 05-multi-scope
- 06-invalid-diagnostics

Each sample includes README, asmdef graph explanation, expected generated output, expected diagnostics, validation command.

**Impact**: Teaches concepts through concrete, focused examples.

### Phase 7.7.0 — Performance and Scalability Gates
**Goal**: Ensure generator/analyzer performance remains acceptable as project size grows.

**Key Features**:
- Benchmark matrix: Small (10 services), Medium (100 services), Large (500 services)
- Measure: generator execution time, analyzer execution time, generated source size, Unity compile duration
- Performance rules: Direct referenced assemblies only, deterministic sorted output, no O(n²) scanning, no full assembly scan, no runtime reflection

**Impact**: Ensures tooling scales to large Unity projects without performance degradation.

### Phase 7.8.0 — Migration Assistant
**Goal**: Help users migrate from manual VContainer registration to attribute-driven composition intent.

**Key Features**:
- Report-only, no auto-fix required initially
- Detect manual registration patterns
- Unity Editor menu: Tools/Nhem/Migration Assistant
- Optional CLI: `nhem migrate-report --project <UnityProjectRoot>`

**Impact**: Lowers barrier to adoption for existing projects using manual VContainer registration.

### Phase 8.0.0 — Clean API Breaking Release
**Goal**: Remove or obsolete legacy exposure flags and finalize clean API.

**Key Features**:
- AutoRegisterIn should only declare scope and lifetime
- Remove or obsolete AutoRegisterIn.AsImplementedInterfaces
- Remove or obsolete AutoRegisterIn.AsSelf
- Require explicit [As] or [AsSelf] for contract exposure
- Strengthen duplicate exposure diagnostics
- Clean docs and samples to only use canonical style

**Impact**: Finalizes the canonical API, removing legacy patterns and improving clarity.

## Timeline

### Estimated Duration
- 7.2.2: 1-2 weeks
- 7.3.0: 2-3 weeks
- 7.4.0: 2-3 weeks
- 7.5.0: 2-3 weeks
- 7.6.0: 2-3 weeks
- 7.7.0: 1-2 weeks
- 7.8.0: 3-4 weeks

**Total**: 13-20 weeks (approximately 3-5 months)

### Dependencies
- 7.2.2: No dependencies
- 7.3.0: Can proceed in parallel with 7.2.2
- 7.4.0: Depends on 7.3.0 (uses analyzer diagnostics)
- 7.5.0: Depends on 7.4.0 (uses di-smoke data)
- 7.6.0: Can proceed in parallel with 7.4.0-7.5.0
- 7.7.0: Can proceed in parallel with 7.4.0-7.6.0
- 7.8.0: Depends on 7.3.0-7.8.0 (builds on all features)

## Success Metrics

### Technical Metrics
- All analyzer tests pass with zero false positives
- di-smoke validation runs in under 5 seconds for medium projects
- DI report generation completes in under 2 seconds
- Generator performance meets or exceeds baseline benchmarks
- Zero breaking changes before 8.0.0

### User Experience Metrics
- Stale package cache issues reduced by 90%
- Architecture violations caught at compile time vs runtime
- Time to debug DI issues reduced by 50%
- Sample suite adoption rate tracked
- Migration assistant usage feedback positive

## Risk Areas

### Technical Risks
- Performance degradation with large projects
- Analyzer false positives blocking valid code
- di-smoke tool complexity and maintenance burden
- Migration assistant accuracy on complex patterns

### Adoption Risks
- Breaking changes in 8.0.0 may cause user friction
- Users may not adopt canonical style before 8.0.0
- Sample suite maintenance overhead
- Documentation keeping pace with features

## Mitigation Strategies

### Technical Mitigations
- Performance benchmarking in CI/CD
- Analyzer test coverage and user feedback loops
- di-smoke tool modular design for maintainability
- Migration assistant report-only initially, auto-fix later

### Adoption Mitigations
- Clear migration guides and deprecation warnings
- Gradual obsolescence before hard removal in 8.0.0
- Sample suite automated validation in release gate
- Documentation reviewed with each release

## Release Strategy

### Minor Releases (7.2.2 - 7.8.0)
- Additive features only
- No breaking changes
- Backward compatibility maintained
- Incremental improvements to diagnostics and tooling

### Major Release (8.0.0)
- Breaking API changes
- Legacy flag removal
- Canonical API enforcement
- Comprehensive migration guide
- Long deprecation period for legacy patterns

## Communication Plan

### Pre-8.0.0
- Document canonical API in all samples and docs
- Add obsolescence warnings for legacy flags in 7.x
- Publish migration guide early
- Gather user feedback on migration path

### 8.0.0 Release
- Clear breaking change documentation
- Migration assistant enhancements
- Sample suite fully updated
- Release notes emphasize benefits of canonical API
