# Tasks: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Phase 7.2.2 — Import and Package Diagnostics

### Task 1.1: Create ToolingDiagnosticsWindow
**File**: `Source~/DangFugBixs.Editor~/Editor/ToolingDiagnosticsWindow.cs`
- Create Unity Editor window class
- Add menu item: Tools/Nhem/Tooling Diagnostics
- Implement OnGUI to display diagnostics
- Add button to print to Console

### Task 1.2: Create PackageDiagnostics Class
**File**: `Source~/DangFugBixs.Editor~/Editor/PackageDiagnostics.cs`
- Create data class for package diagnostics
- Implement package.json version reading
- Implement assembly version detection
- Implement VContainer reference check
- Implement version mismatch detection

### Task 1.3: Add Troubleshooting Documentation
**File**: `README.md`
- Add "Troubleshooting" section
- Document PackageCache stale package issue
- Document packages-lock.json stale commit issue
- Document regenerate project files
- Document branch/tag mismatch
- Document local file package mismatch

### Task 1.4: Add Release Gate Test
**File**: `scripts/release-gate.ps1`
- Add Unity Editor window test
- Verify diagnostics menu opens
- Verify version detection works
- Verify warnings display correctly

### Task 1.5: Update Version Numbers
- Bump package.json to 7.2.2
- Bump all project files to 7.2.2
- Update CHANGELOG.md

## Phase 7.3.0 — Analyzer Maturity

### Task 2.1: Implement NHEM_DI_061
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add diagnostic ID to DiagnosticIds.cs
- Add diagnostic descriptor to DiagnosticCatalog.cs
- Implement duplicate contract detection
- Add to SupportedDiagnostics

### Task 2.2: Implement NHEM_DI_062
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add diagnostic ID to DiagnosticIds.cs
- Add diagnostic descriptor to DiagnosticCatalog.cs
- Implement cross-assembly visibility check
- Add to SupportedDiagnostics

### Task 2.3: Implement NHEM_DI_063
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add diagnostic ID to DiagnosticIds.cs
- Add diagnostic descriptor to DiagnosticCatalog.cs
- Implement EntryPoint in service-only assembly check
- Add to SupportedDiagnostics

### Task 2.4: Implement NHEM_DI_066
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add diagnostic ID to DiagnosticIds.cs
- Add diagnostic descriptor to DiagnosticCatalog.cs
- Implement RegisterComponentInHierarchy on non-MonoBehaviour check
- Add to SupportedDiagnostics

### Task 2.5: Implement NHEM_DI_067
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add diagnostic ID to DiagnosticIds.cs
- Add diagnostic descriptor to DiagnosticCatalog.cs
- Implement EntryPoint without lifecycle interface check
- Add to SupportedDiagnostics

### Task 2.6: Write Analyzer Tests
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/OpenSpecAnalyzerMvpTests.cs`
- Add tests for NHEM_DI_061
- Add tests for NHEM_DI_062
- Add tests for NHEM_DI_063
- Add tests for NHEM_DI_066
- Add tests for NHEM_DI_067
- Verify no false positives on service-only assemblies

### Task 2.7: Update Documentation
**File**: `README.md`
- Document all new diagnostics
- Add examples for each diagnostic
- Document analyzer philosophy

### Task 2.8: Update Version Numbers
- Bump package.json to 7.3.0
- Bump all project files to 7.3.0
- Update CHANGELOG.md

## Phase 7.4.0 — DI Smoke Project-Level Validation

### Task 3.1: Create DiSmokeValidator Class
**File**: `Source~/DangFugBixs.Common~/Validation/DiSmokeValidator.cs`
- Create validation logic class
- Implement service-only asmdef validation
- Implement composition asmdef validation
- Implement scope validation
- Implement package validation

### Task 3.2: Create Asmdef Parser
**File**: `Source~/DangFugBixs.Common~/Parsing/AsmdefParser.cs`
- Create asmdef file parser
- Extract assembly references
- Determine assembly type (service vs composition)

### Task 3.3: Create CLI Command
**File**: `Source~/DangFugBixs.CLI~/Commands/DiSmokeCommand.cs`
- Create CLI command: nhem di-smoke
- Add --project argument
- Implement console report output
- Implement JSON report output

### Task 3.4: Create Unity Menu
**File**: `Source~/DangFugBixs.Editor~/Editor/DiSmokeMenu.cs`
- Add menu item: Tools/Nhem/DI Smoke Validate
- Implement validation trigger
- Display results in Unity Console

### Task 3.5: Write Validation Tests
**File**: `Source~/DangFugBixs.Tests~/Validation/DiSmokeValidatorTests.cs`
- Test service-only asmdef validation
- Test composition asmdef validation
- Test scope validation
- Test package validation

### Task 3.6: Add to Release Gate
**File**: `scripts/release-gate.ps1`
- Add di-smoke execution
- Run on Unity sample project
- Fail if validation fails

### Task 3.7: Update Version Numbers
- Bump package.json to 7.4.0
- Bump all project files to 7.4.0
- Update CHANGELOG.md

## Phase 7.5.0 — DI Report and Graph Viewer

### Task 4.1: Create Report Schema
**File**: `Source~/DangFugBixs.Common~/Models/DiReportSchema.cs`
- Define report data structures
- Define scope, service, component, entry point models

### Task 4.2: Create Report Generator
**File**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Reporting/DiReportGenerator.cs`
- Implement report generation
- Parse generated registration code
- Extract service information
- Write JSON to Library folder

### Task 4.3: Hook into Compilation Pipeline
**File**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/DangFugBixsGenerators.cs`
- Add post-compilation callback
- Trigger report generation
- Implement incremental update logic

### Task 4.4: Create Viewer Window
**File**: `Source~/DangFugBixs.Editor~/Editor/DiReportViewerWindow.cs`
- Create Unity Editor window class
- Add menu item: Tools/Nhem/DI Report
- Implement tree view display
- Implement search and filtering
- Implement navigation to source

### Task 4.5: Write Report Tests
**File**: `Source~/DangFugBixs.Tests~/Reporting/DiReportGeneratorTests.cs`
- Test report generation
- Test report schema
- Test incremental updates

### Task 4.6: Add to Release Gate
**File**: `scripts/release-gate.ps1`
- Add DI report verification
- Verify report generation
- Verify report schema validity

### Task 4.7: Update Version Numbers
- Bump package.json to 7.5.0
- Bump all project files to 7.5.0
- Update CHANGELOG.md

## Phase 7.6.0 — Sample Suite

### Task 5.1: Create Sample 01 - Basic Service
**Directory**: `Samples~/NhemDangFugBixs/01-basic-service/`
- Create service asmdef
- Create composition asmdef
- Implement basic service with [As]
- Write README.md
- Write validation.md

### Task 5.2: Create Sample 02 - Composition-Only Asmdef
**Directory**: `Samples~/NhemDangFugBixs/02-composition-only-asmdef/`
- Create service asmdef (no VContainer reference)
- Create composition asmdef (with VContainer reference)
- Implement service with [As]
- Write README.md
- Write validation.md

### Task 5.3: Create Sample 03 - EntryPoint Composition Adapter
**Directory**: `Samples~/NhemDangFugBixs/03-entrypoint-composition-adapter/`
- Create service asmdef
- Create composition asmdef
- Implement EntryPoint in composition assembly
- Write README.md
- Write validation.md

### Task 5.4: Create Sample 04 - Component in Hierarchy
**Directory**: `Samples~/NhemDangFugBixs/04-component-in-hierarchy/`
- Create service asmdef
- Create composition asmdef
- Implement MonoBehaviour with [RegisterComponentInHierarchy]
- Write README.md
- Write validation.md

### Task 5.5: Create Sample 05 - Multi-Scope
**Directory**: `Samples~/NhemDangFugBixs/05-multi-scope/`
- Create service asmdef
- Create composition asmdef
- Implement multiple scopes
- Implement services in different scopes
- Write README.md
- Write validation.md

### Task 5.6: Create Sample 06 - Invalid Diagnostics
**Directory**: `Samples~/NhemDangFugBixs/06-invalid-diagnostics/`
- Create service asmdef
- Create composition asmdef
- Implement services that trigger diagnostics
- Document expected diagnostics
- Write README.md
- Write validation.md

### Task 5.7: Create Validation Scripts
**File**: `scripts/validate-samples.ps1`
- Script to validate all samples
- Run di-smoke on each sample
- Verify analyzer diagnostics
- Verify generated output

### Task 5.8: Add to Release Gate
**File**: `scripts/release-gate.ps1`
- Add sample validation
- Run validation on at least main composition-only sample
- Fail if validation fails

### Task 5.9: Update Documentation
**File**: `README.md`
- Link each sample to concept it teaches
- Add samples section to README

### Task 5.10: Update Version Numbers
- Bump package.json to 7.6.0
- Bump all project files to 7.6.0
- Update CHANGELOG.md

## Phase 7.7.0 — Performance and Scalability Gates

### Task 6.1: Create Benchmark Projects
**Directory**: `Benchmarks/`
- Create small project (10 services, 1 scope, 3 asmdefs)
- Create medium project (100 services, 3 scopes, 8 asmdefs)
- Create large project (500 services, 6 scopes, 20 asmdefs)

### Task 6.2: Create Benchmark Runner
**File**: `Source~/DangFugBixs.Benchmarks~/BenchmarkRunner.cs`
- Create benchmark runner script
- Measure generator execution time
- Measure analyzer execution time
- Measure generated source size
- Measure generated file count

### Task 6.3: Create Performance Report Generator
**File**: `Source~/DangFugBixs.Benchmarks~/PerformanceReportGenerator.cs`
- Generate performance report
- Compare against baseline
- Detect regressions

### Task 6.4: Establish Baseline Metrics
- Run benchmarks on current version
- Document baseline metrics
- Define regression thresholds

### Task 6.5: Add to CI/CD
**File**: `.github/workflows/benchmark.yml`
- Add benchmark workflow
- Run benchmarks on PR
- Emit performance report as artifact

### Task 6.6: Add to Release Gate
**File**: `scripts/release-gate.ps1`
- Add performance check
- Verify performance thresholds
- Fail if regression detected

### Task 6.7: Update Documentation
**File**: `README.md`
- Document performance rules
- Document regression thresholds

### Task 6.8: Update Version Numbers
- Bump package.json to 7.7.0
- Bump all project files to 7.7.0
- Update CHANGELOG.md

## Phase 7.8.0 — Migration Assistant

### Task 7.1: Create Pattern Detector
**File**: `Source~/DangFugBixs.Common~/Migration/PatternDetector.cs`
- Implement manual registration pattern detection
- Parse builder.Register<T>().As<TContract>() patterns
- Parse builder.Register<T>().AsImplementedInterfaces() patterns
- Parse builder.Register<T>().AsSelf() patterns
- Parse builder.RegisterEntryPoint<T>() patterns
- Parse builder.RegisterComponentInHierarchy<T>() patterns

### Task 7.2: Create Suggestion Generator
**File**: `Source~/DangFugBixs.Common~/Migration/SuggestionGenerator.cs`
- Convert detected patterns to attribute suggestions
- Generate [AutoRegisterIn] attribute
- Generate [As] attributes
- Generate [AsSelf] attribute
- Generate [EntryPoint] attribute

### Task 7.3: Create Migration Window
**File**: `Source~/DangFugBixs.Editor~/Editor/MigrationAssistantWindow.cs`
- Create Unity Editor window class
- Add menu item: Tools/Nhem/Migration Assistant
- Display detected patterns
- Display suggestions
- Export report

### Task 7.4: Create CLI Command
**File**: `Source~/DangFugBixs.CLI~/Commands/MigrateReportCommand.cs`
- Create CLI command: nhem migrate-report
- Add --project argument
- Implement report output

### Task 7.5: Write Migration Tests
**File**: `Source~/DangFugBixs.Tests~/Migration/PatternDetectorTests.cs`
- Test pattern detection
- Test suggestion generation
- Test on sample project

### Task 7.6: Update Documentation
**File**: `README.md`
- Document migration assistant
- Document supported patterns
- Document unsupported patterns

### Task 7.7: Update Version Numbers
- Bump package.json to 7.8.0
- Bump all project files to 7.8.0
- Update CHANGELOG.md

## Phase 8.0.0 — Clean API Breaking Release

### Task 8.1: Mark Legacy Flags as Obsolete
**File**: `Source~/DangFugBixs.Attributes~/Attributes/AutoRegisterInAttribute.cs`
- Add [Obsolete] attribute to AsImplementedInterfaces
- Add [Obsolete] attribute to AsSelf
- Update obsolescence message to point to migration guide

### Task 8.2: Strengthen NHEM_DI_060
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Change NHEM_DI_060 severity from Warning to Error
- Add diagnostic for obsolete flag usage

### Task 8.3: Add Code Fix for Obsolete Flags
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/CodeFixes/LegacyFlagCodeFixProvider.cs`
- Create code fix provider
- Convert legacy flags to explicit attributes
- Verify conversion correctness

### Task 8.4: Update All Samples
- Update all sample README.md to use canonical API
- Remove legacy flag examples from all samples
- Verify all samples compile

### Task 8.5: Update Documentation
**File**: `README.md`
- Remove legacy flag examples
- Emphasize canonical API
- Update all examples to canonical style

### Task 8.6: Update AutoRegisterInAttribute XML Docs
**File**: `Source~/DangFugBixs.Attributes~/Attributes/AutoRegisterInAttribute.cs`
- Remove legacy flag examples from XML docs
- Update all examples to canonical style
- Document obsolescence

### Task 8.7: Create Migration Guide 7.x to 8.0
**File**: `docs/migration-7-to-8.md`
- Document breaking changes
- Document obsolescence timeline
- Provide migration steps
- Provide examples

### Task 8.8: Enhance Migration Assistant
**File**: `Source~/DangFugBixs.Common~/Migration/SuggestionGenerator.cs`
- Add auto-fix for obsolete flags
- Convert legacy flags to explicit attributes
- Verify conversion correctness

### Task 8.9: Write Migration Tests
**File**: `Source~/DangFugBixs.Tests~/Migration/Migration8Tests.cs`
- Test migration from 7.x to 8.0
- Test code fix provider
- Verify conversion correctness

### Task 8.10: Add to Release Gate
**File**: `scripts/release-gate.ps1`
- Add migration guide verification
- Run migration assistant on sample project
- Verify canonical API enforcement

### Task 8.11: Update Version Numbers
- Bump package.json to 8.0.0
- Bump all project files to 8.0.0
- Update CHANGELOG.md

## Cross-Phase Tasks

### Documentation Maintenance
- Review and update README.md after each phase
- Keep API documentation current
- Update examples as needed

### Release Gate Updates
- Update release gate after each phase
- Add new checks as features are added
- Ensure release gate remains fast

### CI/CD Updates
- Update CI/CD workflows after each phase
- Add new test suites
- Add new benchmarks

### Community Communication
- Announce each phase release
- Gather user feedback
- Adjust roadmap based on feedback

## Task Dependencies

### Phase 7.2.2
- No dependencies
- Can start immediately

### Phase 7.3.0
- Can proceed in parallel with 7.2.2
- No dependencies

### Phase 7.4.0
- Depends on 7.3.0 (uses analyzer diagnostics)
- Must complete 7.3.0 first

### Phase 7.5.0
- Depends on 7.4.0 (uses di-smoke data)
- Must complete 7.4.0 first

### Phase 7.6.0
- Can proceed in parallel with 7.4.0-7.5.0
- No dependencies

### Phase 7.7.0
- Can proceed in parallel with 7.4.0-7.6.0
- No dependencies

### Phase 7.8.0
- Can proceed in parallel with 7.4.0-7.6.0
- No dependencies

### Phase 8.0.0
- Depends on 7.3.0 (uses analyzer diagnostics)
- Depends on 7.8.0 (uses migration assistant)
- Should complete most phases before 8.0.0

## Estimated Effort

### Phase 7.2.2
- 8-12 hours

### Phase 7.3.0
- 16-24 hours

### Phase 7.4.0
- 16-24 hours

### Phase 7.5.0
- 16-24 hours

### Phase 7.6.0
- 16-24 hours

### Phase 7.7.0
- 8-12 hours

### Phase 7.8.0
- 16-24 hours

### Phase 8.0.0
- 24-32 hours

**Total**: 120-176 hours (approximately 3-4.5 months)
