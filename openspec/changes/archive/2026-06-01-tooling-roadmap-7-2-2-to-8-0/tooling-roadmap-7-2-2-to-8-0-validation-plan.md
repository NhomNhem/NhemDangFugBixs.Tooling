# Validation Plan: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Overview

This validation plan defines the validation strategy for each phase of the roadmap from 7.2.2 through 8.0.0.

## Phase-by-Phase Validation

### Phase 7.2.2 — Import and Package Diagnostics

#### Validation Focus
Unity Editor integration and package diagnostics

#### Validation Steps

**VV-7.2.2-1: Unity Editor Window Test**
- [ ] Verify Tools/Nhem/Tooling Diagnostics menu item exists
- [ ] Verify window opens when clicked
- [ ] Verify package version displayed correctly
- [ ] Verify assembly versions displayed correctly
- [ ] Verify analyzer DLL path displayed correctly
- [ ] Verify package path displayed correctly
- [ ] Verify VContainer reference status displayed correctly
- [ ] Verify version mismatch warning works
- [ ] Verify Console output button works

**VV-7.2.2-2: Version Detection Test**
- [ ] Verify package.json version read correctly
- [ ] Verify Attributes assembly loaded correctly
- [ ] Verify Runtime assembly loaded correctly
- [ ] Verify version comparison works correctly
- [ ] Verify mismatch detection works correctly

**VV-7.2.2-3: Troubleshooting Documentation**
- [ ] Verify README.md has Troubleshooting section
- [ ] Verify PackageCache stale package documented
- [ ] Verify packages-lock.json stale commit documented
- [ ] Verify regenerate project files documented
- [ ] Verify branch/tag mismatch documented
- [ ] Verify local file package mismatch documented

**VV-7.2.2-4: Release Gate Integration**
- [ ] Verify Unity Editor window test in release gate
- [ ] Verify version detection test in release gate
- [ ] Verify tests pass in CI/CD

---

### Phase 7.3.0 — Analyzer Maturity

#### Validation Focus
New analyzer diagnostics and analyzer philosophy

#### Validation Steps

**VV-7.3.0-1: NHEM_DI_061 Validation**
- [ ] Verify diagnostic ID added to DiagnosticIds.cs
- [ ] Verify diagnostic descriptor added to DiagnosticCatalog.cs
- [ ] Verify diagnostic emitted for duplicate contracts
- [ ] Verify diagnostic not emitted for different contracts
- [ ] Verify test coverage for NHEM_DI_061

**VV-7.3.0-2: NHEM_DI_062 Validation**
- [ ] Verify diagnostic ID added to DiagnosticIds.cs
- [ ] Verify diagnostic descriptor added to DiagnosticCatalog.cs
- [ ] Verify diagnostic emitted for internal type across asmdef boundary
- [ ] Verify diagnostic not emitted for public type
- [ ] Verify no false positives on service-only assemblies
- [ ] Verify test coverage for NHEM_DI_062

**VV-7.3.0-3: NHEM_DI_063 Validation**
- [ ] Verify diagnostic ID added to DiagnosticIds.cs
- [ ] Verify diagnostic descriptor added to DiagnosticCatalog.cs
- [ ] Verify diagnostic emitted for EntryPoint in service-only assembly
- [ ] Verify diagnostic not emitted when VContainer.Unity referenced
- [ ] Verify test coverage for NHEM_DI_063

**VV-7.3.0-4: NHEM_DI_066 Validation**
- [ ] Verify diagnostic ID added to DiagnosticIds.cs
- [ ] Verify diagnostic descriptor added to DiagnosticCatalog.cs
- [ ] Verify diagnostic emitted for RegisterComponentInHierarchy on non-MonoBehaviour
- [ ] Verify diagnostic not emitted for actual MonoBehaviour
- [ ] Verify test coverage for NHEM_DI_066

**VV-7.3.0-5: NHEM_DI_067 Validation**
- [ ] Verify diagnostic ID added to DiagnosticIds.cs
- [ ] Verify diagnostic descriptor added to DiagnosticCatalog.cs
- [ ] Verify diagnostic emitted for EntryPoint without lifecycle interface
- [ ] Verify diagnostic not emitted when lifecycle interface implemented
- [ ] Verify test coverage for NHEM_DI_067

**VV-7.3.0-6: Analyzer Philosophy Validation**
- [ ] Verify per-compilation analyzers validate only what they can see reliably
- [ ] Verify service-only assemblies not forced to see LifetimeScopeFor
- [ ] Verify no whole-project cross-asmdef validation inside Roslyn analyzer
- [ ] Verify project-level checks moved to di-smoke

**VV-7.3.0-7: Existing Analyzer Tests**
- [ ] Verify all existing analyzer tests pass
- [ ] Verify no regressions in existing diagnostics

---

### Phase 7.4.0 — DI Smoke Project-Level Validation

#### Validation Focus
CLI tool and project-level validation

#### Validation Steps

**VV-7.4.0-1: CLI Command Validation**
- [ ] Verify `nhem di-smoke` command exists
- [ ] Verify command accepts --project argument
- [ ] Verify command validates Unity project structure
- [ ] Verify command outputs human-readable console report
- [ ] Verify command outputs JSON report with --json flag

**VV-7.4.0-2: Unity Menu Validation**
- [ ] Verify Tools/Nhem/DI Smoke Validate menu item exists
- [ ] Verify menu item runs validation
- [ ] Verify results displayed in Unity Console
- [ ] Verify results include pass/fail status

**VV-7.4.0-3: Service-Only Asmdef Validation**
- [ ] Verify detection of service-only asmdefs referencing VContainer
- [ ] Verify detection of service-only asmdefs referencing VContainer.Unity
- [ ] Verify violations reported clearly
- [ ] Verify no false positives on legitimate VContainer references

**VV-7.4.0-4: Composition Asmdef Validation**
- [ ] Verify detection of composition asmdefs not referencing VContainer
- [ ] Verify detection of composition asmdefs not referencing service assemblies
- [ ] Verify detection of composition asmdefs without RegisterGeneratedFor<TScope>()
- [ ] Verify violations reported clearly

**VV-7.4.0-5: Scope Validation**
- [ ] Verify detection of duplicate LifetimeScopeFor for same scope
- [ ] Verify detection of scopes with services but no composition target
- [ ] Verify violations reported clearly

**VV-7.4.0-6: Package Validation**
- [ ] Verify detection of package version mismatch
- [ ] Verify detection of Runtime.Testing leakage
- [ ] Verify violations reported clearly

**VV-7.4.0-7: Generated Code Validation**
- [ ] Verify Unity sample generated code appears only in composition target
- [ ] Verify violations reported clearly

**VV-7.4.0-8: Performance Validation**
- [ ] Verify di-smoke runs in under 5 seconds for medium projects
- [ ] Verify di-smoke runs in under 10 seconds for large projects
- [ ] Verify no full loaded Unity assembly scan

**VV-7.4.0-9: Release Gate Integration**
- [ ] Verify di-smoke runs against Unity sample project
- [ ] Verify di-smoke catches missing composition target
- [ ] Verify di-smoke catches service asmdef referencing VContainer
- [ ] Verify di-smoke catches version mismatch
- [ ] Verify release gate can include di-smoke as optional or required step

---

### Phase 7.5.0 — DI Report and Graph Viewer

#### Validation Focus
Report generation and viewer UI

#### Validation Steps

**VV-7.5.0-1: Report Generation Validation**
- [ ] Verify report generated at Library/NhemDangFugBixs/di-report.json
- [ ] Verify report includes scopes
- [ ] Verify report includes composition assembly per scope
- [ ] Verify report includes registered services
- [ ] Verify report includes contracts
- [ ] Verify report includes lifetimes
- [ ] Verify report includes registration kind
- [ ] Verify report includes source assembly
- [ ] Verify report includes warnings

**VV-7.5.0-2: Report Determinism Validation**
- [ ] Verify report generated deterministically
- [ ] Verify same input produces identical output
- [ ] Verify report sorted for consistency

**VV-7.5.0-3: Unity Editor UI Validation**
- [ ] Verify Tools/Nhem/DI Report menu item exists
- [ ] Verify viewer window opens when clicked
- [ ] Verify viewer shows scopes
- [ ] Verify viewer shows services
- [ ] Verify viewer shows components
- [ ] Verify viewer shows entry points
- [ ] Verify viewer shows warnings
- [ ] Verify viewer shows source assembly
- [ ] Verify viewer shows lifetime

**VV-7.5.0-4: Viewer Performance Validation**
- [ ] Verify viewer reads report/cache rather than doing heavy scan on every repaint
- [ ] Verify viewer window opens in under 1 second
- [ ] Verify filtering completes in under 100ms
- [ ] Verify navigation to source is fast

**VV-7.5.0-5: Report Schema Validation**
- [ ] Verify report schema is valid JSON
- [ ] Verify report schema matches specification
- [ ] Verify report can be parsed correctly

**VV-7.5.0-6: Release Gate Integration**
- [ ] Verify release gate verifies report generation
- [ ] Verify release gate verifies report schema validity
- [ ] Verify report works for Unity sample

---

### Phase 7.6.0 — Sample Suite

#### Validation Focus
Sample projects and documentation

#### Validation Steps

**VV-7.6.0-1: Sample 01 - Basic Service Validation**
- [ ] Verify sample implements basic service with [As]
- [ ] Verify sample has README.md
- [ ] Verify sample has asmdef graph explanation
- [ ] Verify sample has expected generated output
- [ ] Verify sample has expected diagnostics
- [ ] Verify sample has validation command
- [ ] Verify sample compiles

**VV-7.6.0-2: Sample 02 - Composition-Only Asmdef Validation**
- [ ] Verify sample has service asmdef (no VContainer reference)
- [ ] Verify sample has composition asmdef (with VContainer reference)
- [ ] Verify sample implements service with [As]
- [ ] Verify sample has README.md
- [ ] Verify sample has validation command
- [ ] Verify sample compiles

**VV-7.6.0-3: Sample 03 - EntryPoint Composition Adapter Validation**
- [ ] Verify sample has service asmdef
- [ ] Verify sample has composition asmdef
- [ ] Verify sample implements EntryPoint in composition assembly
- [ ] Verify sample has README.md
- [ ] Verify sample has validation command
- [ ] Verify sample compiles

**VV-7.6.0-4: Sample 04 - Component in Hierarchy Validation**
- [ ] Verify sample has service asmdef
- [ ] Verify sample has composition asmdef
- [ ] Verify sample implements MonoBehaviour with [RegisterComponentInHierarchy]
- [ ] Verify sample has README.md
- [ ] Verify sample has validation command
- [ ] Verify sample compiles

**VV-7.6.0-5: Sample 05 - Multi-Scope Validation**
- [ ] Verify sample has service asmdef
- [ ] Verify sample has composition asmdef
- [ ] Verify sample implements multiple scopes
- [ ] Verify sample implements services in different scopes
- [ ] Verify sample has README.md
- [ ] Verify sample has validation command
- [ ] Verify sample compiles

**VV-7.6.0-6: Sample 06 - Invalid Diagnostics Validation**
- [ ] Verify sample has service asmdef
- [ ] Verify sample has composition asmdef
- [ ] Verify sample implements services that trigger diagnostics
- [ ] Verify sample documents expected diagnostics
- [ ] Verify sample has README.md
- [ ] Verify sample has validation command
- [ ] Verify sample compiles

**VV-7.6.0-7: Sample Compilation Validation**
- [ ] Verify all samples compile
- [ ] Verify all samples have no unexpected errors
- [ ] Verify all samples have expected diagnostics

**VV-7.6.0-8: Validation Script Validation**
- [ ] Verify validation script exists
- [ ] Verify script validates all samples
- [ ] Verify script verifies expected diagnostics
- [ ] Verify script verifies generated output

**VV-7.6.0-9: Documentation Validation**
- [ ] Verify each sample README is clear and concise
- [ ] Verify each sample README explains the concept it teaches
- [ ] Verify each sample has validation instructions

**VV-7.6.0-10: Release Gate Integration**
- [ ] Verify release gate can validate at least main composition-only sample
- [ ] Verify release gate validates all samples compile
- [ ] Verify release gate validates expected diagnostics

---

### Phase 7.7.0 — Performance and Scalability Gates

#### Validation Focus
Performance benchmarks and thresholds

#### Validation Steps

**VV-7.7.0-1: Benchmark Projects Validation**
- [ ] Verify small project created (10 services, 1 scope, 3 asmdefs)
- [ ] Verify medium project created (100 services, 3 scopes, 8 asmdefs)
- [ ] Verify large project created (500 services, 6 scopes, 20 asmdefs)

**VV-7.7.0-2: Benchmark Runner Validation**
- [ ] Verify generator execution time measured
- [ ] Verify analyzer execution time measured
- [ ] Verify generated source size measured
- [ ] Verify generated file count measured
- [ ] Verify Unity sample compile duration measured

**VV-7.7.0-3: Performance Rules Validation**
- [ ] Verify direct referenced assemblies only
- [ ] Verify deterministic sorted output
- [ ] Verify no O(n²) duplicate scanning
- [ ] Verify no full loaded Unity assembly scan
- [ ] Verify no runtime reflection
- [ ] Verify no runtime per-frame work
- [ ] Verify no Resolve<T>() during registration

**VV-7.7.0-4: Performance Report Validation**
- [ ] Verify performance report generated
- [ ] Verify report includes baseline metrics
- [ ] Verify report includes current metrics
- [ ] Verify report includes comparison
- [ ] Verify report detects regressions

**VV-7.7.0-5: Performance Thresholds Validation**
- [ ] Verify generator time: < 500ms for small
- [ ] Verify generator time: < 2s for medium
- [ ] Verify generator time: < 5s for large
- [ ] Verify analyzer time: < 100ms per 100 services
- [ ] Verify generated size: Linear with service count

**VV-7.7.0-6: Regression Detection Validation**
- [ ] Verify performance regressions detected
- [ ] Verify regression thresholds documented
- [ ] Verify regression detection automated in CI/CD

**VV-7.7.0-7: Release Gate Integration**
- [ ] Verify performance smoke tests run in CI or release gate
- [ ] Verify performance report emitted as artifact
- [ ] Verify regression thresholds documented

---

### Phase 7.8.0 — Migration Assistant

#### Validation Focus
Pattern detection and suggestion generation

#### Validation Steps

**VV-7.8.0-1: Pattern Detection Validation**
- [ ] Verify Register<T>(Lifetime).As<TContract>() pattern detected
- [ ] Verify Register<T>().AsImplementedInterfaces() pattern detected
- [ ] Verify Register<T>().AsSelf() pattern detected
- [ ] Verify RegisterEntryPoint<T>() pattern detected
- [ ] Verify RegisterComponentInHierarchy<T>() pattern detected

**VV-7.8.0-2: Suggestion Generation Validation**
- [ ] Verify detected patterns converted to attribute suggestions
- [ ] Verify [AutoRegisterIn] attribute generated
- [ ] Verify [As] attributes generated
- [ ] Verify [AsSelf] attribute generated
- [ ] Verify [EntryPoint] attribute generated

**VV-7.8.0-3: Unity Editor Integration Validation**
- [ ] Verify Tools/Nhem/Migration Assistant menu item exists
- [ ] Verify window displays detected patterns
- [ ] Verify window displays suggestions
- [ ] Verify window can export report

**VV-7.8.0-4: CLI Integration Validation**
- [ ] Verify `nhem migrate-report` command exists
- [ ] Verify command accepts --project argument
- [ ] Verify command outputs suggestions
- [ ] Verify command outputs JSON report with --json flag

**VV-7.8.0-5: Unsupported Patterns Validation**
- [ ] Verify RegisterInstance() not attempted (too risky)
- [ ] Verify config-based registrations not attempted (context-dependent)
- [ ] Verify scene-based registrations not attempted (context-dependent)

**VV-7.8.0-6: Accuracy Validation**
- [ ] Verify pattern detection is accurate
- [ ] Verify suggestion generation is correct
- [ ] Verify no false positives on complex patterns
- [ ] Verify tool does not modify files (report-only)

**VV-7.8.0-7: Performance Validation**
- [ ] Verify migration assistant runs in under 10 seconds for typical projects
- [ ] Verify no performance degradation

**VV-7.8.0-8: Documentation Validation**
- [ ] Verify docs explain migration assistant
- [ ] Verify docs explain supported patterns
- [ ] Verify docs explain unsupported patterns

---

### Phase 8.0.0 — Clean API Breaking Release

#### Validation Focus
API cleanup and migration

#### Validation Steps

**VV-8.0.0-1: Legacy Flag Obsolescence Validation**
- [ ] Verify AutoRegisterIn.AsImplementedInterfaces marked as [Obsolete]
- [ ] Verify AutoRegisterIn.AsSelf marked as [Obsolete]
- [ ] Verify obsolescence message points to migration guide
- [ ] Verify flags still function (not removed yet)

**VV-8.0.0-2: Diagnostic Strengthening Validation**
- [ ] Verify NHEM_DI_060 severity changed from Warning to Error
- [ ] Verify diagnostic for obsolete flag usage added
- [ ] Verify diagnostic message explains migration path

**VV-8.0.0-3: Code Fix Provider Validation**
- [ ] Verify code fix provider for obsolete flags exists
- [ ] Verify code fix converts legacy flags to explicit attributes
- [ ] Verify code fix verifies conversion correctness

**VV-8.0.0-4: Sample Updates Validation**
- [ ] Verify all samples use only canonical API
- [ ] Verify all samples compile
- [ ] Verify legacy flag examples removed from all samples

**VV-8.0.0-5: Documentation Updates Validation**
- [ ] Verify README.md removes legacy flag examples
- [ ] Verify README.md emphasizes canonical API
- [ ] Verify all examples updated to canonical style
- [ ] Verify AutoRegisterInAttribute XML docs updated

**VV-8.0.0-6: Migration Guide Validation**
- [ ] Verify migration guide from 7.x to 8.0 exists
- [ ] Verify guide documents breaking changes
- [ ] Verify guide documents obsolescence timeline
- [ ] Verify guide provides migration steps
- [ ] Verify guide provides examples

**VV-8.0.0-7: Migration Assistant Enhancement Validation**
- [ ] Verify migration assistant adds auto-fix for obsolete flags
- [ ] Verify migration assistant converts legacy flags to explicit attributes
- [ ] Verify migration assistant verifies conversion correctness

**VV-8.0.0-8: Canonical API Validation**
- [ ] Verify AutoRegisterIn only declares scope and lifetime
- [ ] Verify explicit [As] or [AsSelf] required for contract exposure
- [ ] Verify duplicate exposure diagnostics strengthened

**VV-8.0.0-9: Backward Compatibility Validation**
- [ ] Verify legacy flags still function (marked obsolete but not removed)
- [ ] Verify existing code still compiles
- [ ] Verify no breaking changes to generated output for legacy code

**VV-8.0.0-10: Release Gate Integration**
- [ ] Verify migration guide from 7.x to 8.0 exists
- [ ] Verify samples use only canonical API
- [ ] Verify analyzer explains how to migrate legacy flags
- [ ] Verify release gate passes
- [ ] Verify Unity sample compile passes
- [ ] Verify migration assistant runs on sample project

---

## Cross-Phase Validation

### CV-1: Documentation Validation
- [ ] Verify README.md updated after each phase
- [ ] Verify CHANGELOG.md updated after each phase
- [ ] Verify API documentation kept current
- [ ] Verify examples updated as needed

### CV-2: Release Gate Validation
- [ ] Verify release gate updated after each phase
- [ ] Verify new checks added as features are added
- [ ] Verify release gate remains fast
- [ ] Verify release gate passes for each phase

### CV-3: CI/CD Validation
- [ ] Verify CI/CD workflows updated after each phase
- [ ] Verify new test suites added
- [ ] Verify new benchmarks added
- [ ] Verify CI/CD passes for each phase

### CV-4: Testing Validation
- [ ] Verify all new features have unit tests
- [ ] Verify all new features have integration tests
- [ ] Verify no regressions in existing tests
- [ ] Verify test coverage maintained or improved

### CV-5: Performance Validation
- [ ] Verify no performance degradation
- [ ] Verify performance benchmarks pass
- [ ] Verify performance thresholds met
- [ ] Verify no O(n²) algorithms introduced

---

## Validation Metrics

### Phase-Specific Metrics

#### 7.2.2 Metrics
- Unity Editor window opens in under 1 second
- Version detection completes in under 100ms
- Console output completes in under 50ms

#### 7.3.0 Metrics
- Analyzer execution time does not increase significantly
- Zero false positives on service-only assemblies
- All new diagnostics have unit tests

#### 7.4.0 Metrics
- di-smoke runs in under 5 seconds for medium projects
- di-smoke runs in under 10 seconds for large projects
- No full loaded Unity assembly scan

#### 7.5.0 Metrics
- Report generation completes in under 2 seconds
- Viewer window opens in under 1 second
- Filtering completes in under 100ms

#### 7.6.0 Metrics
- All samples compile
- Validation script completes in under 30 seconds
- Sample documentation complete

#### 7.7.0 Metrics
- Generator time: < 500ms for small, < 2s for medium, < 5s for large
- Analyzer time: < 100ms per 100 services
- Generated size: Linear with service count

#### 7.8.0 Metrics
- Migration assistant runs in under 10 seconds
- Pattern detection accuracy > 95%
- Suggestion generation accuracy > 95%

#### 8.0.0 Metrics
- Migration guide comprehensive
- Code fix provider accuracy > 95%
- All samples use canonical API

### Overall Metrics
- All phases pass acceptance criteria
- All phases pass release gate
- No performance regressions
- No breaking changes before 8.0.0
- Documentation complete for all phases

## Validation Schedule

### Phase 7.2.2
- 1 week before release: Complete validation
- 3 days before release: Release gate validation
- 1 day before release: Final validation

### Phase 7.3.0
- 2 weeks before release: Complete validation
- 1 week before release: Release gate validation
- 3 days before release: Final validation

### Phase 7.4.0
- 2 weeks before release: Complete validation
- 1 week before release: Release gate validation
- 3 days before release: Final validation

### Phase 7.5.0
- 2 weeks before release: Complete validation
- 1 week before release: Release gate validation
- 3 days before release: Final validation

### Phase 7.6.0
- 2 weeks before release: Complete validation
- 1 week before release: Release gate validation
- 3 days before release: Final validation

### Phase 7.7.0
- 1 week before release: Complete validation
- 3 days before release: Release gate validation
- 1 day before release: Final validation

### Phase 7.8.0
- 2 weeks before release: Complete validation
- 1 week before release: Release gate validation
- 3 days before release: Final validation

### Phase 8.0.0
- 3 weeks before release: Complete validation
- 2 weeks before release: Release gate validation
- 1 week before release: Final validation

## Validation Tools

### Automated Validation
- Release gate script (release-gate.ps1)
- Unit tests (dotnet test)
- Benchmark runner
- di-smoke CLI tool
- Migration assistant CLI tool

### Manual Validation
- Code review
- Documentation review
- Sample review
- Performance review

### Validation Commands
```powershell
# Run release gate
.\scripts\release-gate.ps1

# Run generator tests
dotnet test Source~/DangFugBixs.Generators~/DangFugBixs.Tests/DangFugBixs.Tests.csproj

# Run analyzer tests
dotnet test Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/DangFugBixs.Analyzers.Tests.csproj

# Run di-smoke
nhem di-smoke --project <UnityProjectRoot>

# Run migration assistant
nhem migrate-report --project <UnityProjectRoot>

# Run benchmarks
dotnet run --project Source~/DangFugBixs.Benchmarks~/DangFugBixs.Benchmarks.csproj
```

## Validation Reporting

### Phase Validation Report Template
```markdown
# Validation Report for [Phase]

## Phase: [Phase Name]
## Version: [Version]

## Validation Results
- [Check Name]: PASS/FAIL
- [Check Name]: PASS/FAIL

## Metrics
- [Metric Name]: [Value]
- [Metric Name]: [Value]

## Issues
- List any issues found

## Remediation
- List any remediation steps

## Overall Status: PASS/FAIL
```

### Validation Artifacts
- Validation report per phase
- Test results
- Benchmark results
- Release gate output
- Performance reports

## Success Criteria

Each phase is considered successfully validated when:
- All phase-specific validation steps pass
- All acceptance criteria met
- Release gate passes
- No blocking issues identified
- Performance thresholds met
- Documentation complete
- Tests pass

## Failure Handling

### If Phase Validation Fails
1. Identify failing validation step
2. Determine root cause
3. Implement fix
4. Re-run validation
5. Document failure and fix

### Escalation
- If validation fails and cannot be fixed quickly: Escalate to Phase Lead
- If release gate fails: Block release until fixed
- If critical issue found: Consider delaying phase

## Conclusion

This validation plan ensures comprehensive validation for each phase of the roadmap from 7.2.2 through 8.0.0. By following this plan, we can ensure each phase is implemented correctly, tested thoroughly, documented completely, and ready for release.
