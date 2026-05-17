# Spec Deltas: NhemDangFugBixs.Tooling Roadmap 7.2.2 to 8.0.0

## Overview

This document describes the specification deltas for each phase of the roadmap from 7.2.2 through 8.0.0.

## Phase 7.2.2 — Import and Package Diagnostics

### New Specification

#### Unity Editor Diagnostics Menu
**Location**: `Source~/DangFugBixs.Editor~/Editor/`

**New API**:
```csharp
public class ToolingDiagnosticsWindow : EditorWindow
{
    [MenuItem("Tools/Nhem/Tooling Diagnostics")]
    public static void ShowWindow()
    
    void OnGUI()
}
```

**Behavior**:
- Opens Unity Editor window
- Displays package version from package.json
- Displays loaded assembly versions
- Displays analyzer DLL path
- Displays package path
- Displays VContainer reference status
- Warns on version mismatch

#### Package Diagnostics
**Location**: `Source~/DangFugBixs.Editor~/Editor/`

**New API**:
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

**Behavior**:
- Reads package.json version
- Loads assemblies to get actual versions
- Compares versions and warns on mismatch
- Checks VContainer reference status

### Spec Changes
- **Added**: Unity Editor diagnostics menu
- **Added**: Package diagnostics data class
- **Added**: Version detection logic
- **Added**: Troubleshooting documentation

---

## Phase 7.3.0 — Analyzer Maturity

### New Specifications

#### NHEM_DI_061: Duplicate Contract Exposure
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`

**Diagnostic**:
```csharp
public const string DuplicateContractExposure = "NHEM_DI_061";

public static readonly DiagnosticDescriptor DuplicateContractExposure = new(
    DiagnosticIds.DuplicateContractExposure,
    "Duplicate contract exposure",
    "Type '{0}' has duplicate [As] contract '{1}'.",
    "Usage",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
```

**Trigger**: When duplicate `[As]` attributes for same contract

**Behavior**: Warns about duplicate explicit contracts

#### NHEM_DI_062: Cross-Assembly Visibility
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`

**Diagnostic**:
```csharp
public const string CrossAssemblyVisibility = "NHEM_DI_062";

public static readonly DiagnosticDescriptor CrossAssemblyVisibility = new(
    DiagnosticIds.CrossAssemblyVisibility,
    "Cross-assembly type must be public",
    "Type '{0}' is auto-registered across asmdef boundary but is not public.",
    "Usage",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
```

**Trigger**: When auto-registered type is internal across asmdef boundary

**Behavior**: Warns about visibility requirements

#### NHEM_DI_063: EntryPoint in Service-Only Assembly
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`

**Diagnostic**:
```csharp
public const string EntryPointInServiceAssembly = "NHEM_DI_063";

public static readonly DiagnosticDescriptor EntryPointInServiceAssembly = new(
    DiagnosticIds.EntryPointInServiceAssembly,
    "EntryPoint in service-only assembly",
    "Type '{0}' uses [EntryPoint] but assembly may not reference VContainer.Unity.",
    "Usage",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
```

**Trigger**: When EntryPoint used in service-only assembly

**Behavior**: Warns about potential VContainer.Unity requirement

#### NHEM_DI_066: Non-MonoBehaviour Component
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`

**Diagnostic**:
```csharp
public const string NonMonoBehaviourComponent = "NHEM_DI_066";

public static readonly DiagnosticDescriptor NonMonoBehaviourComponent = new(
    DiagnosticIds.NonMonoBehaviourComponent,
    "RegisterComponentInHierarchy on non-MonoBehaviour",
    "Type '{0}' uses [RegisterComponentInHierarchy] but does not inherit from MonoBehaviour.",
    "Usage",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);
```

**Trigger**: When RegisterComponentInHierarchy on non-MonoBehaviour

**Behavior**: Errors on incorrect attribute usage

#### NHEM_DI_067: EntryPoint Without Lifecycle Interface
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`

**Diagnostic**:
```csharp
public const string EntryPointWithoutLifecycle = "NHEM_DI_067";

public static readonly DiagnosticDescriptor EntryPointWithoutLifecycle = new(
    DiagnosticIds.EntryPointWithoutLifecycle,
    "EntryPoint without lifecycle interface",
    "Type '{0}' uses [EntryPoint] but does not implement known VContainer lifecycle interface.",
    "Usage",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
```

**Trigger**: When EntryPoint without VContainer lifecycle interface

**Behavior**: Warns about missing lifecycle interface

### Spec Changes
- **Added**: NHEM_DI_061 diagnostic
- **Added**: NHEM_DI_062 diagnostic
- **Added**: NHEM_DI_063 diagnostic
- **Added**: NHEM_DI_066 diagnostic
- **Added**: NHEM_DI_067 diagnostic
- **Modified**: Analyzer philosophy to avoid project-level checks in Roslyn analyzer

---

## Phase 7.4.0 — DI Smoke Project-Level Validation

### New Specifications

#### CLI Command
**Location**: `Source~/DangFugBixs.CLI~/Commands/`

**New API**:
```csharp
public class DiSmokeCommand
{
    public static int Execute(string projectRoot, bool jsonOutput)
}
```

**Behavior**:
- Validates Unity project structure
- Checks service-only asmdefs
- Checks composition asmdefs
- Checks scope configuration
- Checks package integration
- Outputs human-readable report
- Outputs optional JSON report

#### Validation Rules
**Location**: `Source~/DangFugBixs.Common~/Validation/`

**New API**:
```csharp
public class DiSmokeValidator
{
    public DiSmokeReport Validate(string projectRoot)
}

public class DiSmokeReport
{
    public List<ScopeValidation> Scopes { get; }
    public List<AsmdefValidation> Asmdefs { get; }
    public List<string> Warnings { get; }
    public bool Success { get; }
}
```

**Behavior**:
- Service-only asmdefs should not reference VContainer
- Composition asmdefs should reference VContainer
- Composition asmdefs should reference service assemblies
- No duplicate LifetimeScopeFor for same scope
- Scopes with services should have composition target

### Spec Changes
- **Added**: CLI command `nhem di-smoke`
- **Added**: Unity menu `Tools/Nhem/DI Smoke Validate`
- **Added**: DiSmokeValidator class
- **Added**: DiSmokeReport data structure
- **Added**: Asmdef parser
- **Added**: Validation rules

---

## Phase 7.5.0 — DI Report and Graph Viewer

### New Specifications

#### Report Schema
**Location**: `Source~/DangFugBixs.Common~/Models/`

**New API**:
```csharp
public class DiReport
{
    public string Version { get; }
    public DateTime GeneratedAt { get; }
    public List<ScopeReport> Scopes { get; }
}

public class ScopeReport
{
    public string Name { get; }
    public string CompositionAssembly { get; }
    public List<ServiceReport> Services { get; }
    public List<ComponentReport> Components { get; }
    public List<EntryPointReport> EntryPoints { get; }
    public List<string> Warnings { get; }
}
```

**Behavior**:
- Generated deterministically
- Includes scopes, services, components, entry points
- Includes registration kind, lifetime, source assembly
- Includes warnings

#### Report Generator
**Location**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Reporting/`

**New API**:
```csharp
public class DiReportGenerator
{
    public void Generate(string outputPath)
}
```

**Behavior**:
- Parses generated registration code
- Extracts service information
- Writes JSON to Library folder
- Incremental update when code changes

#### Viewer Window
**Location**: `Source~/DangFugBixs.Editor~/Editor/`

**New API**:
```csharp
public class DiReportViewerWindow : EditorWindow
{
    [MenuItem("Tools/Nhem/DI Report")]
    public static void ShowWindow()
    
    void OnGUI()
}
```

**Behavior**:
- Reads report from Library folder
- Displays hierarchical tree view
- Supports search and filtering
- Navigates to source on click

### Spec Changes
- **Added**: DI report schema
- **Added**: DI report generator
- **Added**: DI report viewer window
- **Added**: Unity menu `Tools/Nhem/DI Report`

---

## Phase 7.6.0 — Sample Suite

### New Specifications

#### Sample Structure
**Location**: `Samples~/NhemDangFugBixs/`

**Samples**:
- `01-basic-service/`
- `02-composition-only-asmdef/`
- `03-entrypoint-composition-adapter/`
- `04-component-in-hierarchy/`
- `05-multi-scope/`
- `06-invalid-diagnostics/`

**Each Sample Contains**:
- README.md
- Service asmdef
- Composition asmdef
- Implementation
- validation.md

#### Validation Script
**Location**: `scripts/`

**New API**:
```powershell
function Test-SampleSuite
{
    # Validate all samples
}
```

**Behavior**:
- Validates all samples compile
- Validates expected diagnostics
- Validates expected generated output

### Spec Changes
- **Added**: 6 focused samples
- **Added**: Sample validation script
- **Added**: Sample documentation structure

---

## Phase 7.7.0 — Performance and Scalability Gates

### New Specifications

#### Benchmark Infrastructure
**Location**: `Source~/DangFugBixs.Benchmarks~/`

**New API**:
```csharp
public class BenchmarkRunner
{
    public BenchmarkReport Run()
}

public class BenchmarkReport
{
    public BenchmarkMetrics Small { get; }
    public BenchmarkMetrics Medium { get; }
    public BenchmarkMetrics Large { get; }
}

public class BenchmarkMetrics
{
    public TimeSpan GeneratorTime { get; }
    public TimeSpan AnalyzerTime { get; }
    public long GeneratedSourceSize { get; }
    public int GeneratedFileCount { get; }
}
```

**Behavior**:
- Runs benchmarks on small, medium, large projects
- Measures generator execution time
- Measures analyzer execution time
- Measures generated source size
- Detects performance regressions

#### Performance Rules
- Direct referenced assemblies only
- Deterministic sorted output
- No O(n²) duplicate scanning
- No full loaded Unity assembly scan
- No runtime reflection
- No runtime per-frame work
- No Resolve<T>() during registration

### Spec Changes
- **Added**: Benchmark runner
- **Added**: Benchmark report
- **Added**: Performance thresholds
- **Added**: CI/CD integration

---

## Phase 7.8.0 — Migration Assistant

### New Specifications

#### Pattern Detector
**Location**: `Source~/DangFugBixs.Common~/Migration/`

**New API**:
```csharp
public class PatternDetector
{
    public List<ManualRegistration> Detect(string projectRoot)
}

public class ManualRegistration
{
    public string Type { get; }
    public List<string> Contracts { get; }
    public string Lifetime { get; }
    public RegistrationKind Kind { get; }
}
```

**Behavior**:
- Detects manual VContainer registration patterns
- Supports Register<T>().As<TContract>()
- Supports Register<T>().AsImplementedInterfaces()
- Supports Register<T>().AsSelf()
- Supports RegisterEntryPoint<T>()
- Supports RegisterComponentInHierarchy<T>()

#### Suggestion Generator
**Location**: `Source~/DangFugBixs.Common~/Migration/`

**New API**:
```csharp
public class SuggestionGenerator
{
    public List<MigrationSuggestion> Generate(List<ManualRegistration> registrations)
}

public class MigrationSuggestion
{
    public string Type { get; }
    public string SuggestedAttributes { get; }
}
```

**Behavior**:
- Converts detected patterns to attribute suggestions
- Generates [AutoRegisterIn] attribute
- Generates [As] attributes
- Generates [AsSelf] attribute
- Generates [EntryPoint] attribute

#### Migration Window
**Location**: `Source~/DangFugBixs.Editor~/Editor/`

**New API**:
```csharp
public class MigrationAssistantWindow : EditorWindow
{
    [MenuItem("Tools/Nhem/Migration Assistant")]
    public static void ShowWindow()
    
    void OnGUI()
}
```

**Behavior**:
- Displays detected patterns
- Displays suggestions
- Can export report
- Report-only (no auto-fix in 7.8.0)

#### CLI Command
**Location**: `Source~/DangFugBixs.CLI~/Commands/`

**New API**:
```csharp
public class MigrateReportCommand
{
    public static int Execute(string projectRoot, bool jsonOutput)
}
```

**Behavior**:
- Outputs migration suggestions
- Outputs JSON report with `--json` flag

### Spec Changes
- **Added**: Pattern detector
- **Added**: Suggestion generator
- **Added**: Migration assistant window
- **Added**: CLI command `nhem migrate-report`
- **Added**: Report-only approach (no auto-fix yet)

---

## Phase 8.0.0 — Clean API Breaking Release

### New Specifications

#### Obsolete Attributes
**Location**: `Source~/DangFugBixs.Attributes~/Attributes/`

**Modified API**:
```csharp
public sealed class AutoRegisterInAttribute : Attribute
{
    [Obsolete("Use explicit [As] and [AsSelf] attributes instead. See migration guide for details.")]
    public bool AsImplementedInterfaces { get; set; } = true;
    
    [Obsolete("Use explicit [AsSelf] attribute instead. See migration guide for details.")]
    public bool AsSelf { get; set; } = true;
}
```

**Behavior**:
- Marks legacy flags as obsolete
- Obsolescence message points to migration guide
- Flags still function (not removed yet)

#### Strengthened Diagnostic
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/`

**Modified API**:
```csharp
public static readonly DiagnosticDescriptor MixedExposureStyle = new(
    DiagnosticIds.MixedExposureStyle,
    "Mixed registration exposure style",
    "Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.",
    "Usage",
    DiagnosticSeverity.Error,  // Changed from Warning
    isEnabledByDefault: true);
```

**Behavior**:
- NHEM_DI_060 severity changed from Warning to Error
- Blocks compilation on mixed style
- Enforces canonical API

#### Code Fix Provider
**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/CodeFixes/`

**New API**:
```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = "LegacyFlagCodeFixProvider")]
public class LegacyFlagCodeFixProvider : CodeFixProvider
{
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
}
```

**Behavior**:
- Provides code fix for obsolete flags
- Converts legacy flags to explicit attributes
- Verifies conversion correctness

#### Migration Guide
**Location**: `docs/migration-7-to-8.md`

**Content**:
- Breaking changes documentation
- Obsolescence timeline
- Migration steps
- Examples

### Spec Changes
- **Modified**: AutoRegisterIn.AsImplementedInterfaces marked obsolete
- **Modified**: AutoRegisterIn.AsSelf marked obsolete
- **Modified**: NHEM_DI_060 severity changed to Error
- **Added**: Code fix provider for legacy flags
- **Added**: Migration guide 7.x to 8.0
- **Modified**: All samples to use canonical API
- **Modified**: Documentation to emphasize canonical API

---

## Cross-Phase Spec Changes

### Shared Infrastructure
- **Added**: Version detection utility (7.2.2, 7.4.0, 7.8.0)
- **Added**: Asmdef parser (7.4.0, 7.5.0, 7.6.0)
- **Added**: Assembly inspection (7.4.0, 7.5.0, 7.8.0)
- **Added**: Report generation (7.4.0, 7.5.0, 7.8.0)

### Performance Requirements
- **Added**: Direct referenced assemblies only (all phases)
- **Added**: Deterministic sorted output (all phases)
- **Added**: No O(n²) algorithms (all phases)
- **Added**: No full assembly scan (all phases)
- **Added**: No runtime reflection (all phases)

### Non-Goals
- **Explicitly excluded**: RegisterInstance generation
- **Explicitly excluded**: Addressables integration
- **Explicitly excluded**: Prefab factory generation
- **Explicitly excluded**: Pooling integration
- **Explicitly excluded**: LifetimeScope generation
- **Explicitly excluded**: Partial injection into user LifetimeScope
- **Explicitly excluded**: Scene auto setup
- **Explicitly excluded**: MessagePipe auto-magic
- **Explicitly excluded**: Runtime reflection scanning

## Spec Delta Summary

### New APIs
- 7.2.2: ToolingDiagnosticsWindow, PackageDiagnostics
- 7.3.0: 5 new diagnostics (NHEM_DI_061, NHEM_DI_062, NHEM_DI_063, NHEM_DI_066, NHEM_DI_067)
- 7.4.0: DiSmokeCommand, DiSmokeValidator, AsmdefParser
- 7.5.0: DiReport, DiReportGenerator, DiReportViewerWindow
- 7.6.0: 6 sample projects, validation script
- 7.7.0: BenchmarkRunner, BenchmarkReport
- 7.8.0: PatternDetector, SuggestionGenerator, MigrationAssistantWindow
- 8.0.0: LegacyFlagCodeFixProvider

### Modified APIs
- 7.3.0: AttributeContractAnalyzer (new diagnostics added)
- 7.4.0: Release gate (di-smoke integration)
- 7.5.0: Release gate (DI report verification)
- 7.6.0: Release gate (sample validation)
- 7.7.0: Release gate (performance benchmarks)
- 8.0.0: AutoRegisterInAttribute (obsolete attributes), NHEM_DI_060 (severity change)

### Deprecated APIs
- 8.0.0: AutoRegisterIn.AsImplementedInterfaces (obsolete)
- 8.0.0: AutoRegisterIn.AsSelf (obsolete)

### Removed APIs
- None in 7.2.2-8.0.0 (deprecation in 8.0.0, removal in 9.0.0)
