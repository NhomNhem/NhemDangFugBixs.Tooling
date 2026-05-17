# Per-Version Acceptance Criteria: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Overview

This document defines the acceptance criteria for each phase of the roadmap from 7.2.2 through 8.0.0.

## Phase 7.2.2 — Import and Package Diagnostics

### Functional Requirements

#### FR-7.2.2-1: Diagnostics Menu
- [ ] Unity Editor menu item "Tools/Nhem/Tooling Diagnostics" exists
- [ ] Clicking menu item opens diagnostics window
- [ ] Window displays package version from package.json
- [ ] Window displays loaded Attributes assembly version
- [ ] Window displays loaded Runtime assembly version
- [ ] Window displays analyzer DLL path
- [ ] Window displays package path
- [ ] Window displays VContainer reference status

#### FR-7.2.2-2: Version Mismatch Detection
- [ ] Window warns if package.json version differs from loaded assembly versions
- [ ] Warning is displayed prominently
- [ ] Warning includes both versions for comparison
- [ ] Warning message is clear and actionable

#### FR-7.2.2-3: Console Output
- [ ] Window has button to print diagnostics to Console
- [ ] Clicking button prints all diagnostics to Unity Console
- [ ] Console output is readable and formatted

#### FR-7.2.2-4: Troubleshooting Documentation
- [ ] README.md has "Troubleshooting" section
- [ ] Documents PackageCache stale package issue
- [ ] Documents packages-lock.json stale commit issue
- [ ] Documents regenerate project files
- [ ] Documents branch/tag mismatch
- [ ] Documents local file package mismatch

### Non-Functional Requirements

#### NFR-7.2.2-1: Performance
- [ ] Diagnostics window opens in under 1 second
- [ ] Version detection completes in under 100ms
- [ ] Console output completes in under 50ms

#### NFR-7.2.2-2: Reliability
- [ ] Diagnostics window handles missing package.json gracefully
- [ ] Diagnostics window handles missing assemblies gracefully
- [ ] No exceptions thrown during normal operation

### Release Gate Requirements

#### RG-7.2.2-1: Unity Editor Test
- [ ] Release gate includes Unity Editor window test
- [ ] Test verifies diagnostics menu opens
- [ ] Test verifies version detection works
- [ ] Test verifies warnings display correctly

#### RG-7.2.2-2: Existing Tests
- [ ] All existing release gate checks pass
- [ ] No regressions in existing functionality

### Success Criteria

- [ ] Diagnostics menu opens in Unity
- [ ] Diagnostics can print to Console
- [ ] Diagnostics show package path and loaded assembly versions
- [ ] Docs include "stale package cache" troubleshooting section
- [ ] Existing release gate passes

---

## Phase 7.3.0 — Analyzer Maturity

### Functional Requirements

#### FR-7.3.0-1: NHEM_DI_061 Duplicate Contract Exposure
- [ ] Diagnostic emitted when duplicate [As] attributes for same contract
- [ ] Diagnostic message is clear
- [ ] Diagnostic points to duplicate attribute location
- [ ] No false positives on legitimate multiple [As] for different contracts

#### FR-7.3.0-2: NHEM_DI_062 Cross-Assembly Visibility
- [ ] Diagnostic emitted when auto-registered type is internal across asmdef boundary
- [ ] Diagnostic emitted when contract is internal across asmdef boundary
- [ ] Diagnostic message explains visibility requirement
- [ ] No false positives on public types

#### FR-7.3.0-3: NHEM_DI_063 EntryPoint in Service-Only Assembly
- [ ] Diagnostic emitted when EntryPoint used in service-only assembly
- [ ] Diagnostic warns about potential VContainer.Unity requirement
- [ ] No false positives when assembly has VContainer.Unity reference

#### FR-7.3.0-4: NHEM_DI_066 Non-MonoBehaviour Component
- [ ] Diagnostic emitted when RegisterComponentInHierarchy on non-MonoBehaviour
- [ ] Diagnostic message explains MonoBehaviour requirement
- [ ] No false positives on actual MonoBehaviours

#### FR-7.3.0-5: NHEM_DI_067 EntryPoint Without Lifecycle Interface
- [ ] Diagnostic emitted when EntryPoint without VContainer lifecycle interface
- [ ] Diagnostic lists known lifecycle interfaces
- [ ] No false positives when lifecycle interface implemented

#### FR-7.3.0-6: Analyzer Philosophy
- [ ] Per-compilation analyzers validate only what they can see reliably
- [ ] Service-only assemblies not forced to see LifetimeScopeFor
- [ ] No whole-project cross-asmdef validation inside Roslyn analyzer

### Non-Functional Requirements

#### NFR-7.3.0-1: Performance
- [ ] Analyzer execution time does not increase significantly
- [ ] No O(n²) algorithms added
- [ ] Analyzer remains fast for large projects

#### NFR-7.3.0-2: Accuracy
- [ ] Zero false positives on service-only assemblies
- [ ] All new diagnostics have unit tests
- [ ] Existing analyzer tests still pass

### Release Gate Requirements

#### RG-7.3.0-1: Analyzer Tests
- [ ] Analyzer tests cover all new diagnostics
- [ ] All analyzer tests pass
- [ ] No regressions in existing analyzer tests

#### RG-7.3.0-2: Documentation
- [ ] Docs list all new diagnostics
- [ ] Docs include examples for each diagnostic
- [ ] Docs include fixes for each diagnostic

### Success Criteria

- [ ] Analyzer tests cover all new diagnostics
- [ ] No false positive on service-only assemblies
- [ ] Existing analyzer tests pass
- [ ] Docs list diagnostics with examples and fixes

---

## Phase 7.4.0 — DI Smoke Project-Level Validation

### Functional Requirements

#### FR-7.4.0-1: CLI Command
- [ ] CLI command `nhem di-smoke --project <UnityProjectRoot>` exists
- [ ] Command accepts project path argument
- [ ] Command validates Unity project structure
- [ ] Command outputs human-readable console report
- [ ] Command outputs optional JSON report with `--json` flag

#### FR-7.4.0-2: Unity Menu
- [ ] Unity Editor menu item "Tools/Nhem/DI Smoke Validate" exists
- [ ] Clicking menu item runs validation
- [ ] Results displayed in Unity Console
- [ ] Results include pass/fail status per scope and asmdef

#### FR-7.4.0-3: Service-Only Asmdef Validation
- [ ] Detects service-only asmdefs referencing VContainer
- [ ] Detects service-only asmdefs referencing VContainer.Unity
- [ ] Reports violations clearly
- [ ] No false positives on legitimate VContainer references in composition asmdefs

#### FR-7.4.0-4: Composition Asmdef Validation
- [ ] Detects composition asmdefs not referencing VContainer
- [ ] Detects composition asmdefs not referencing service assemblies
- [ ] Detects composition asmdefs without Configure() calling RegisterGeneratedFor<TScope>()
- [ ] Reports violations clearly

#### FR-7.4.0-5: Scope Validation
- [ ] Detects duplicate LifetimeScopeFor for same scope
- [ ] Detects scopes with services but no composition target
- [ ] Reports violations clearly

#### FR-7.4.0-6: Package Validation
- [ ] Detects package version mismatch with loaded assembly versions
- [ ] Detects Runtime.Testing leaking into shipped package surface
- [ ] Reports violations clearly

#### FR-7.4.0-7: Generated Code Validation
- [ ] Verifies Unity sample generated code appears only in composition target
- [ ] Reports violations clearly

### Non-Functional Requirements

#### NFR-7.4.0-1: Performance
- [ ] di-smoke runs in under 5 seconds for medium projects (100 services)
- [ ] di-smoke runs in under 10 seconds for large projects (500 services)
- [ ] No full loaded Unity assembly scan

#### NFR-7.4.0-2: Reliability
- [ ] di-smoke handles missing asmdef files gracefully
- [ ] di-smoke handles missing assemblies gracefully
- [ ] No exceptions thrown during normal operation

### Release Gate Requirements

#### RG-7.4.0-1: Integration Test
- [ ] di-smoke runs against Unity sample project
- [ ] di-smoke catches missing composition target
- [ ] di-smoke catches service asmdef referencing VContainer
- [ ] di-smoke catches version mismatch

#### RG-7.4.0-2: Release Gate Integration
- [ ] release-gate.ps1 can include di-smoke as optional or required step
- [ ] di-smoke can be run in CI/CD
- [ ] di-smoke output is parseable by release gate

### Success Criteria

- [ ] di-smoke runs against Unity sample project
- [ ] di-smoke catches missing composition target
- [ ] di-smoke catches service asmdef referencing VContainer
- [ ] di-smoke catches version mismatch
- [ ] release gate can include di-smoke as optional or required step

---

## Phase 7.5.0 — DI Report and Graph Viewer

### Functional Requirements

#### FR-7.5.0-1: Report Generation
- [ ] Report generated at Library/NhemDangFugBixs/di-report.json
- [ ] Report includes scopes
- [ ] Report includes composition assembly per scope
- [ ] Report includes registered services
- [ ] Report includes contracts
- [ ] Report includes lifetimes
- [ ] Report includes registration kind (Register, RegisterComponentInHierarchy, RegisterEntryPoint)
- [ ] Report includes source assembly
- [ ] Report includes warnings

#### FR-7.5.0-2: Report Determinism
- [ ] Report generated deterministically
- [ ] Same input produces identical output
- [ ] Report sorted for consistency

#### FR-7.5.0-3: Unity Editor UI
- [ ] Unity Editor menu item "Tools/Nhem/DI Report" exists
- [ ] Clicking menu item opens report viewer window
- [ ] Viewer shows scopes
- [ ] Viewer shows services
- [ ] Viewer shows components
- [ ] Viewer shows entry points
- [ ] Viewer shows warnings
- [ ] Viewer shows source assembly
- [ ] Viewer shows lifetime

#### FR-7.5.0-4: Viewer Performance
- [ ] Viewer reads report/cache rather than doing heavy scan on every repaint
- [ ] Viewer window opens in under 1 second
- [ ] Filtering completes in under 100ms
- [ ] Navigation to source is fast

### Non-Functional Requirements

#### NFR-7.5.0-1: Performance
- [ ] Report generation completes in under 2 seconds
- [ ] Report generation is incremental (only regenerates when code changes)
- [ ] Report size is reasonable (< 1MB for typical projects)

#### NFR-7.5.0-2: Reliability
- [ ] Report generation handles missing generated code gracefully
- [ ] Viewer handles missing report file gracefully
- [ ] No exceptions thrown during normal operation

### Release Gate Requirements

#### RG-7.5.0-1: Report Verification
- [ ] Release gate verifies report generation
- [ ] Release gate verifies report schema validity
- [ ] Report works for Unity sample

### Success Criteria

- [ ] Report generated deterministically
- [ ] Editor window reads report/cache rather than doing heavy scan on every repaint
- [ ] Report works for Unity sample
- [ ] Docs include report example

---

## Phase 7.6.0 — Sample Suite

### Functional Requirements

#### FR-7.6.0-1: Sample 01 - Basic Service
- [ ] Sample implements basic service with [As]
- [ ] Sample has README.md
- [ ] Sample has asmdef graph explanation
- [ ] Sample has expected generated output
- [ ] Sample has expected diagnostics
- [ ] Sample has validation command

#### FR-7.6.0-2: Sample 02 - Composition-Only Asmdef
- [ ] Sample has service asmdef (no VContainer reference)
- [ ] Sample has composition asmdef (with VContainer reference)
- [ ] Sample implements service with [As]
- [ ] Sample has README.md
- [ ] Sample has validation command

#### FR-7.6.0-3: Sample 03 - EntryPoint Composition Adapter
- [ ] Sample has service asmdef
- [ ] Sample has composition asmdef
- [ ] Sample implements EntryPoint in composition assembly
- [ ] Sample has README.md
- [ ] Sample has validation command

#### FR-7.6.0-4: Sample 04 - Component in Hierarchy
- [ ] Sample has service asmdef
- [ ] Sample has composition asmdef
- [ ] Sample implements MonoBehaviour with [RegisterComponentInHierarchy]
- [ ] Sample has README.md
- [ ] Sample has validation command

#### FR-7.6.0-5: Sample 05 - Multi-Scope
- [ ] Sample has service asmdef
- [ ] Sample has composition asmdef
- [ ] Sample implements multiple scopes
- [ ] Sample implements services in different scopes
- [ ] Sample has README.md
- [ ] Sample has validation command

#### FR-7.6.0-6: Sample 06 - Invalid Diagnostics
- [ ] Sample has service asmdef
- [ ] Sample has composition asmdef
- [ ] Sample implements services that trigger diagnostics
- [ ] Sample documents expected diagnostics
- [ ] Sample has README.md
- [ ] Sample has validation command

#### FR-7.6.0-7: Sample Compilation
- [ ] All samples compile
- [ ] All samples have no unexpected errors
- [ ] All samples have expected diagnostics

### Non-Functional Requirements

#### NFR-7.6.0-1: Documentation
- [ ] Each sample README is clear and concise
- [ ] Each sample README explains the concept it teaches
- [ ] Each sample has validation instructions

#### NFR-7.6.0-2: Maintenance
- [ ] Samples are easy to update
- [ ] Samples have automated validation
- [ ] Samples are versioned with package

### Release Gate Requirements

#### RG-7.6.0-1: Sample Validation
- [ ] Release gate can validate at least the main composition-only sample
- [ ] Release gate validates all samples compile
- [ ] Release gate validates expected diagnostics

#### RG-7.6.0-2: Documentation
- [ ] Docs link each sample to the concept it teaches
- [ ] README.md has samples section

### Success Criteria

- [ ] All samples compile
- [ ] Release gate can validate at least the main composition-only sample
- [ ] Docs link each sample to the concept it teaches

---

## Phase 7.7.0 — Performance and Scalability Gates

### Functional Requirements

#### FR-7.7.0-1: Benchmark Matrix
- [ ] Small project created (10 services, 1 scope, 3 asmdefs)
- [ ] Medium project created (100 services, 3 scopes, 8 asmdefs)
- [ ] Large project created (500 services, 6 scopes, 20 asmdefs)

#### FR-7.7.0-2: Measurements
- [ ] Generator execution time measured
- [ ] Analyzer execution time measured
- [ ] Generated source size measured
- [ ] Generated file count measured
- [ ] Unity sample compile duration measured

#### FR-7.7.0-3: Performance Rules
- [ ] Direct referenced assemblies only
- [ ] Deterministic sorted output
- [ ] No O(n²) duplicate scanning
- [ ] No full loaded Unity assembly scan
- [ ] No runtime reflection
- [ ] No runtime per-frame work
- [ ] No Resolve<T>() during registration

#### FR-7.7.0-4: Performance Report
- [ ] Performance report generated
- [ ] Report includes baseline metrics
- [ ] Report includes current metrics
- [ ] Report includes comparison
- [ ] Report detects regressions

### Non-Functional Requirements

#### NFR-7.7.0-1: Performance Thresholds
- [ ] Generator time: < 500ms for small, < 2s for medium, < 5s for large
- [ ] Analyzer time: < 100ms per 100 services
- [ ] Generated size: Linear with service count

#### NFR-7.7.0-2: Regression Detection
- [ ] Performance regressions detected
- [ ] Regression thresholds documented
- [ ] Regression detection automated in CI/CD

### Release Gate Requirements

#### RG-7.7.0-1: Performance Smoke Tests
- [ ] Performance smoke tests run in CI or release gate
- [ ] Performance report is emitted as artifact
- [ ] Regression thresholds are documented

#### RG-7.7.0-2: Performance Enforcement
- [ ] Release gate fails if performance regression detected
- [ ] Performance report reviewed before release

### Success Criteria

- [ ] Performance smoke tests run in CI or release gate
- [ ] Performance report is emitted as artifact
- [ ] Regression thresholds are documented

---

## Phase 7.8.0 — Migration Assistant

### Functional Requirements

#### FR-7.8.0-1: Pattern Detection
- [ ] Detects builder.Register<T>(Lifetime).As<TContract>() patterns
- [ ] Detects builder.Register<T>().AsImplementedInterfaces() patterns
- [ ] Detects builder.Register<T>().AsSelf() patterns
- [ ] Detects builder.RegisterEntryPoint<T>() patterns
- [ ] Detects builder.RegisterComponentInHierarchy<T>() patterns

#### FR-7.8.0-2: Suggestion Generation
- [ ] Converts detected patterns to attribute suggestions
- [ ] Generates [AutoRegisterIn] attribute
- [ ] Generates [As] attributes
- [ ] Generates [AsSelf] attribute
- [ ] Generates [EntryPoint] attribute

#### FR-7.8.0-3: Unity Editor Integration
- [ ] Unity Editor menu item "Tools/Nhem/Migration Assistant" exists
- [ ] Window displays detected patterns
- [ ] Window displays suggestions
- [ ] Window can export report

#### FR-7.8.0-4: CLI Integration
- [ ] CLI command `nhem migrate-report --project <UnityProjectRoot>` exists
- [ ] Command outputs suggestions
- [ ] Command outputs JSON report with `--json` flag

#### FR-7.8.0-5: Unsupported Patterns
- [ ] Does not attempt RegisterInstance() patterns (too risky)
- [ ] Does not attempt config-based registrations (context-dependent)
- [ ] Does not attempt scene-based registrations (context-dependent)

### Non-Functional Requirements

#### NFR-7.8.0-1: Accuracy
- [ ] Pattern detection is accurate
- [ ] Suggestion generation is correct
- [ ] No false positives on complex patterns
- [ ] Does not modify files (report-only)

#### NFR-7.8.0-2: Performance
- [ ] Migration assistant runs in under 10 seconds for typical projects
- [ ] No performance degradation

### Release Gate Requirements

#### RG-7.8.0-1: Migration Tests
- [ ] Migration assistant produces suggestions without modifying files
- [ ] Supports common Register<T>().As<TContract>() patterns
- [ ] Does not attempt risky instance/config/scene registrations
- [ ] Docs explain what should remain manual

#### RG-7.8.0-2: Documentation
- [ ] Docs explain migration assistant
- [ ] Docs explain supported patterns
- [ ] Docs explain unsupported patterns

### Success Criteria

- [ ] Migration assistant produces suggestions without modifying files
- [ ] Supports common Register<T>().As<TContract>() patterns
- [ ] Does not attempt risky instance/config/scene registrations
- [ ] Docs explain what should remain manual

---

## Phase 8.0.0 — Clean API Breaking Release

### Functional Requirements

#### FR-8.0.0-1: Legacy Flag Obsolescence
- [ ] AutoRegisterIn.AsImplementedInterfaces marked as [Obsolete]
- [ ] AutoRegisterIn.AsSelf marked as [Obsolete]
- [ ] Obsolescence message points to migration guide
- [ ] Flags still function (not removed yet)

#### FR-8.0.0-2: Diagnostic Strengthening
- [ ] NHEM_DI_060 severity changed from Warning to Error
- [ ] Diagnostic for obsolete flag usage added
- [ ] Diagnostic message explains migration path

#### FR-8.0.0-3: Code Fix Provider
- [ ] Code fix provider for obsolete flags exists
- [ ] Code fix converts legacy flags to explicit attributes
- [ ] Code fix verifies conversion correctness

#### FR-8.0.0-4: Sample Updates
- [ ] All samples use only canonical API
- [ ] All samples compile
- [ ] Legacy flag examples removed from all samples

#### FR-8.0.0-5: Documentation Updates
- [ ] README.md removes legacy flag examples
- [ ] README.md emphasizes canonical API
- [ ] All examples updated to canonical style
- [ ] AutoRegisterInAttribute XML docs updated

#### FR-8.0.0-6: Migration Guide
- [ ] Migration guide from 7.x to 8.0 exists
- [ ] Guide documents breaking changes
- [ ] Guide documents obsolescence timeline
- [ ] Guide provides migration steps
- [ ] Guide provides examples

#### FR-8.0.0-7: Migration Assistant Enhancement
- [ ] Migration assistant adds auto-fix for obsolete flags
- [ ] Migration assistant converts legacy flags to explicit attributes
- [ ] Migration assistant verifies conversion correctness

#### FR-8.0.0-8: Canonical API
- [ ] AutoRegisterIn only declares scope and lifetime
- [ ] Explicit [As] or [AsSelf] required for contract exposure
- [ ] Duplicate exposure diagnostics strengthened

### Non-Functional Requirements

#### NFR-8.0.0-1: Backward Compatibility
- [ ] Legacy flags still function (marked obsolete but not removed)
- [ ] Existing code still compiles
- [ ] No breaking changes to generated output for legacy code

#### NFR-8.0.0-2: Migration Path
- [ ] Clear migration path documented
- [ ] Migration guide comprehensive
- [ ] Migration assistant helpful

### Release Gate Requirements

#### RG-8.0.0-1: Migration Guide Verification
- [ ] Migration guide from 7.x to 8.0 exists
- [ ] Migration guide is comprehensive
- [ ] Migration guide is accurate

#### RG-8.0.0-2: Sample Verification
- [ ] Samples use only canonical API
- [ ] Analyzer explains how to migrate legacy flags
- [ ] Release gate passes
- [ ] Unity sample compile passes

#### RG-8.0.0-3: Migration Assistant Verification
- [ ] Migration assistant runs on sample project
- [ ] Migration assistant handles legacy flags
- [ ] Migration assistant produces correct suggestions

### Success Criteria

- [ ] Migration guide from 7.x to 8.0 exists
- [ ] Obsolete warnings exist before hard removal where possible
- [ ] Samples use only canonical API
- [ ] Analyzer explains how to migrate legacy flags
- [ ] Release gate passes
- [ ] Unity sample compile passes

---

## Cross-Phase Acceptance Criteria

### Documentation
- [ ] README.md updated after each phase
- [ ] CHANGELOG.md updated after each phase
- [ ] API documentation kept current
- [ ] Examples updated as needed

### Release Gate
- [ ] Release gate updated after each phase
- [ ] New checks added as features are added
- [ ] Release gate remains fast
- [ ] Release gate passes for each phase

### CI/CD
- [ ] CI/CD workflows updated after each phase
- [ ] New test suites added
- [ ] New benchmarks added
- [ ] CI/CD passes for each phase

### Testing
- [ ] All new features have unit tests
- [ ] All new features have integration tests
- [ ] No regressions in existing tests
- [ ] Test coverage maintained or improved

### Performance
- [ ] No performance degradation
- [ ] Performance benchmarks pass
- [ ] Performance thresholds met
- [ ] No O(n²) algorithms introduced

### User Experience
- [ ] New features are discoverable
- [ ] Documentation is clear
- [ ] Error messages are actionable
- [ ] Migration path is clear
