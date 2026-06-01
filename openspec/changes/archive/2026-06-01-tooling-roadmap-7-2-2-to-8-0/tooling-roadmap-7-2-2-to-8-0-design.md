# Design: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Overview

This design document details the technical implementation for each phase of the roadmap from 7.2.2 through 8.0.0.

## Phase 7.2.2 — Import and Package Diagnostics

### Architecture

#### Unity Editor Integration
- New menu item: `Tools/Nhem/Tooling Diagnostics`
- Window class: `ToolingDiagnosticsWindow`
- Command-line accessible via Unity batch mode

#### Data Collection
```csharp
public class PackageDiagnostics
{
    public string PackagePath { get; }
    public string PackageJsonVersion { get; }
    public Assembly AttributesAssembly { get; }
    public Assembly RuntimeAssembly { get; }
    public string AnalyzerDllPath { get; }
    public bool VContainerReferenced { get; }
    public List<string> Warnings { get; }
}
```

#### Version Detection
- Read `package.json` version from package manifest
- Load assemblies via `Assembly.Load()` to get actual versions
- Compare semantic versions
- Warn if major/minor version mismatch

### Implementation Details

#### File Locations
- Window: `Source~/DangFugBixs.Editor~/Editor/ToolingDiagnosticsWindow.cs`
- Diagnostics: `Source~/DangFugBixs.Editor~/Editor/PackageDiagnostics.cs`

#### Troubleshooting Docs
Add to README.md:
- PackageCache stale package
- packages-lock.json stale commit
- Regenerate project files
- Branch/tag mismatch
- Local file package mismatch

## Phase 7.3.0 — Analyzer Maturity

### New Diagnostics

#### NHEM_DI_061: Duplicate Explicit Contract Exposure
```csharp
// Triggers:
[As(typeof(IService))]
[As(typeof(IService))]  // Duplicate

// Should emit NHEM_DI_061
```

**Implementation**: Detect duplicate contract types in `GetExplicitContracts()` and report.

#### NHEM_DI_062: Cross-Assembly Auto-Registered Must Be Public
```csharp
// Triggers:
// Service asmdef A (public type IContract)
// Service asmdef B (internal type Service implementing IContract)
[AutoRegisterIn<IScope>(AsImplementedInterfaces = true)]
internal class Service : IContract { }  // Should be public
```

**Implementation**: Check visibility of auto-registered types and contracts across asmdef boundaries.

#### NHEM_DI_063: EntryPoint in Service-Only Assembly
```csharp
// Triggers:
// Service asmdef (no VContainer reference)
[AutoRegisterIn<IScope>]
[EntryPoint]
public class EntryPoint : IStartable { }  // May need VContainer.Unity
```

**Implementation**: Check if assembly has VContainer.Unity reference when EntryPoint is used.

#### NHEM_DI_064: Missing RegisterGeneratedFor Call
```csharp
// Triggers:
// Has [LifetimeScopeFor<IScope>]
// But Configure() doesn't call builder.RegisterGeneratedFor<IScope>()
```

**Implementation**: This requires project-level analysis, not in Roslyn analyzer. Move to di-smoke.

#### NHEM_DI_066: RegisterComponentInHierarchy on Non-MonoBehaviour
```csharp
// Triggers:
[RegisterComponentInHierarchy]
public class NotAMonoBehaviour { }  // Not MonoBehaviour
```

**Implementation**: Check base type is MonoBehaviour.

#### NHEM_DI_067: EntryPoint Without Lifecycle Interface
```csharp
// Triggers:
[AutoRegisterIn<IScope>]
[EntryPoint]
public class EntryPoint { }  // No IStartable, ITickable, etc.
```

**Implementation**: Check type implements known VContainer lifecycle interfaces.

### Analyzer Philosophy

- Per-compilation analyzers validate only what they can see reliably
- Do not force service-only assemblies to see LifetimeScopeFor
- Do not perform whole-project cross-asmdef validation inside Roslyn analyzer
- Move project-level checks to di-smoke

## Phase 7.4.0 — DI Smoke Project-Level Validation

### Architecture

#### CLI Tool
```
nhem di-smoke --project <UnityProjectRoot>
```

#### Unity Menu
`Tools/Nhem/DI Smoke Validate`

### Checks

#### Service-Only Asmdef Validation
- Should not reference VContainer
- Should not reference VContainer.Unity
- Should not have VContainer types in compiled output

#### Composition Asmdef Validation
- Should reference VContainer
- Should reference service assemblies they compose
- Should have Configure() calling RegisterGeneratedFor<TScope>()

#### Scope Validation
- No duplicate LifetimeScopeFor for same scope
- Scopes with services should have composition target

#### Package Validation
- Package version matches loaded assembly versions
- Runtime.Testing not in shipped package surface

#### Generated Code Validation
- Unity sample generated code appears only in composition target

### Output Format

#### Console Report
```
DI Smoke Report
IGameplayScope: PASS
GlassRefrain.Locomotion: PASS, no VContainer reference
GlassRefrain.Composition: PASS, owns registration

Warnings:
- DebugOverlayService has no [As] or [AsSelf]
- IGameplayScope has duplicate LifetimeScopeFor
```

#### JSON Report
```json
{
  "scopes": [
    {
      "scopeName": "IGameplayScope",
      "status": "PASS",
      "compositionAssembly": "GlassRefrain.Composition",
      "serviceAssemblies": ["GlassRefrain.Locomotion"],
      "warnings": []
    }
  ],
  "asmdefs": [
    {
      "name": "GlassRefrain.Locomotion",
      "type": "service",
      "referencesVContainer": false,
      "status": "PASS"
    }
  ]
}
```

### Implementation Details

#### File Locations
- CLI: `Source~/DangFugBixs.CLI~/Commands/DiSmokeCommand.cs`
- Unity Menu: `Source~/DangFugBixs.Editor~/Editor/DiSmokeMenu.cs`
- Validation Logic: `Source~/DangFugBixs.Common~/Validation/DiSmokeValidator.cs`

#### Asmdef Parsing
- Parse .asmdef files in Unity project
- Extract references
- Determine assembly type (service vs composition)

#### Assembly Inspection
- Load compiled assemblies
- Check for VContainer references
- Validate generated code location

## Phase 7.5.0 — DI Report and Graph Viewer

### Architecture

#### Report Generation
- Output: `Library/NhemDangFugBixs/di-report.json`
- Generated after compilation
- Incremental update when code changes

#### Report Schema
```json
{
  "version": "7.5.0",
  "generatedAt": "2026-05-17T12:00:00Z",
  "scopes": [
    {
      "name": "IGameplayScope",
      "compositionAssembly": "GlassRefrain.Composition",
      "services": [
        {
          "name": "CombatService",
          "sourceAssembly": "GlassRefrain.Locomotion",
          "lifetime": "Scoped",
          "registrationKind": "Register",
          "contracts": ["ICombatService"],
          "hasAsSelf": true
        }
      ],
      "components": [
        {
          "name": "PlayerView",
          "sourceAssembly": "GlassRefrain.Locomotion",
          "registrationKind": "RegisterComponentInHierarchy",
          "contracts": ["IPlayerView"]
        }
      ],
      "entryPoints": [
        {
          "name": "GameplayLoopEntryPoint",
          "sourceAssembly": "GlassRefrain.Composition",
          "registrationKind": "RegisterEntryPoint",
          "lifecycleInterface": "IStartable"
        }
      ],
      "warnings": []
    }
  ]
}
```

#### Unity Editor UI
- Window: `Tools/Nhem/DI Report`
- Tree view of scopes, services, components, entry points
- Filter by scope, assembly, registration kind
- Click to navigate to source

### Implementation Details

#### Report Generator
- Hook into Unity compilation pipeline
- Parse generated registration code
- Extract service information
- Write JSON to Library folder

#### Viewer Window
- Read JSON report (not regenerate on every repaint)
- Use Unity's IMGUI or UI Toolkit
- Display hierarchical tree view
- Support search and filtering

## Phase 7.6.0 — Sample Suite

### Sample Structure

```
Samples~/NhemDangFugBixs/
├── 01-basic-service/
│   ├── README.md
│   ├── Service/
│   ├── Composition/
│   └── validation.md
├── 02-composition-only-asmdef/
│   ├── README.md
│   ├── Service/
│   ├── Composition/
│   └── validation.md
├── 03-entrypoint-composition-adapter/
│   ├── README.md
│   ├── Service/
│   ├── Composition/
│   └── validation.md
├── 04-component-in-hierarchy/
│   ├── README.md
│   ├── Service/
│   ├── Composition/
│   └── validation.md
├── 05-multi-scope/
│   ├── README.md
│   ├── Service/
│   ├── Composition/
│   └── validation.md
└── 06-invalid-diagnostics/
    ├── README.md
    ├── Service/
    ├── Composition/
    └── validation.md
```

### Sample Content

#### README.md
- Concept description
- Asmdef graph explanation
- Expected generated output
- Expected diagnostics
- Validation command

#### validation.md
- Automated validation steps
- Expected analyzer diagnostics
- Expected generated code snippets
- Release gate integration

### Implementation Details

#### Validation Scripts
- Script to run di-smoke on each sample
- Script to verify analyzer diagnostics
- Script to verify generated output
- Integration with release gate

## Phase 7.7.0 — Performance and Scalability Gates

### Benchmark Matrix

#### Small Project
- 10 services
- 1 scope
- 3 asmdefs

#### Medium Project
- 100 services
- 3 scopes
- 8 asmdefs

#### Large Project
- 500 services
- 6 scopes
- 20 asmdefs

### Measurements

#### Generator Metrics
- Execution time (ms)
- Generated source size (KB)
- Generated file count
- Memory usage (MB)

#### Analyzer Metrics
- Execution time (ms)
- Memory usage (MB)
- Diagnostic count

#### Unity Compile Metrics
- Compilation duration (s)
- Incremental compile duration (s)

### Performance Rules

- Direct referenced assemblies only
- Deterministic sorted output
- No O(n²) duplicate scanning
- No full loaded Unity assembly scan
- No runtime reflection
- No runtime per-frame work
- No Resolve<T>() during registration

### Implementation Details

#### Benchmark Infrastructure
- Create synthetic test projects for each size
- Benchmark runner script
- Performance report generator
- CI/CD integration

#### Regression Thresholds
- Generator time: < 500ms for small, < 2s for medium, < 5s for large
- Analyzer time: < 100ms per 100 services
- Generated size: Linear with service count

## Phase 7.8.0 — Migration Assistant

### Architecture

#### Pattern Detection
Parse manual VContainer registration code:
```csharp
builder.Register<MemoryStateService>(Lifetime.Scoped)
    .As<IMemoryStateService>();
```

#### Suggestion Generation
Suggest attribute-driven composition:
```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IMemoryStateService))]
public sealed class MemoryStateService : IMemoryStateService
{
}
```

### Implementation Details

#### Unity Editor Menu
- Window: `Tools/Nhem/Migration Assistant`
- Scan project for manual registrations
- Display suggestions
- Export report

#### CLI Tool
```
nhem migrate-report --project <UnityProjectRoot>
```

#### Supported Patterns
- `Register<T>(Lifetime).As<TContract>()`
- `Register<T>().AsImplementedInterfaces()`
- `Register<T>().AsSelf()`
- `RegisterEntryPoint<T>()`
- `RegisterComponentInHierarchy<T>()`

#### Unsupported Patterns
- `RegisterInstance()` - too risky
- Config-based registrations - context-dependent
- Scene-based registrations - context-dependent

### Report Format
```json
{
  "registrations": [
    {
      "type": "MemoryStateService",
      "contracts": ["IMemoryStateService"],
      "lifetime": "Scoped",
      "suggestion": "[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]\n[As(typeof(IMemoryStateService))]"
    }
  ]
}
```

## Phase 8.0.0 — Clean API Breaking Release

### API Cleanup

#### AutoRegisterIn Changes
- Mark `AsImplementedInterfaces` as `[Obsolete]`
- Mark `AsSelf` as `[Obsolete]`
- Remove in future major version (9.0.0)
- Update XML docs to emphasize canonical usage

#### Analyzer Strengthening
- NHEM_DI_060 becomes error (from warning)
- Add diagnostic for obsolete flag usage
- Provide code fix to convert to canonical style

#### Canonical API Enforcement
```csharp
// Canonical 8.0 API:
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IMemoryStateService))]
public sealed class MemoryStateService : IMemoryStateService
{
}

[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[AsSelf]
public sealed class MemoryStateCache
{
}

[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[EntryPoint]
public sealed class GameplayLoopEntryPoint : IStartable
{
}
```

### Implementation Details

#### Obsolescence Path
- 8.0.0: Mark flags as obsolete with warning
- 9.0.0: Remove flags entirely
- Provide migration guide from 7.x to 8.0

#### Migration Assistant Enhancement
- Add auto-fix for obsolete flags
- Convert legacy flags to explicit attributes
- Verify conversion correctness

#### Sample Updates
- Update all samples to canonical API
- Remove legacy flag examples
- Update documentation

## Cross-Phase Considerations

### Shared Infrastructure

#### Common Utilities
- Version detection (used by 7.2.2, 7.4.0, 7.8.0)
- Asmdef parsing (used by 7.4.0, 7.5.0, 7.6.0)
- Assembly inspection (used by 7.4.0, 7.5.0, 7.8.0)
- Report generation (used by 7.4.0, 7.5.0, 7.8.0)

#### Code Organization
- `Source~/DangFugBixs.Common~/` - Shared logic
- `Source~/DangFugBixs.Editor~/` - Unity Editor integration
- `Source~/DangFugBixs.CLI~/` - CLI commands
- `Source~/DangFugBixs.Analyzers~/` - Roslyn analyzers
- `Source~/DangFugBixs.Generators~/` - Source generators

### Performance Considerations

#### Incremental Updates
- DI report cached in Library folder
- Only regenerate when code changes
- Use file timestamps for invalidation

#### Lazy Loading
- Load assemblies only when needed
- Parse asmdef files on demand
- Cache parsed results

### Testing Strategy

#### Unit Tests
- Each diagnostic has unit tests
- Each validation rule has unit tests
- Report generation has unit tests

#### Integration Tests
- di-smoke runs against sample project
- Migration assistant runs against sample project
- Performance benchmarks run in CI

#### Regression Tests
- Existing functionality must continue working
- Performance must not degrade
- Analyzer false positive rate must stay zero

## Release Gate Impact

Each phase requires release gate updates:

#### 7.2.2
- Add Unity Editor test for diagnostics window
- Verify version detection works

#### 7.3.0
- Add analyzer tests for new diagnostics
- Verify no false positives

#### 7.4.0
- Add di-smoke to release gate
- Run di-smoke on sample project

#### 7.5.0
- Add DI report verification
- Verify report generation

#### 7.6.0
- Add sample validation to release gate
- Verify all samples compile

#### 7.7.0
- Add performance benchmarks to release gate
- Verify performance thresholds

#### 7.8.0
- Add migration assistant tests
- Verify pattern detection accuracy

#### 8.0.0
- Add migration guide verification
- Verify canonical API enforcement
- Run full migration assistant on sample project
