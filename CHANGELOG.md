# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [6.0.5] - 2026-03-24

### Fixed - Unity package import cleanliness

- Removed precompiled assembly wrapper `.asmdef` files that conflicted with shipped DLL filenames in `Runtime/`, `Runtime/Attributes/`, and `Analyzers/`.
- Kept the source package release-ready for OpenUPM by limiting Unity package assets to the actual runtime, analyzer, generator, editor, and manifest files.
- Tightened deploy/readiness validation so docs, website, scripts, and contributor-only files do not belong in Unity package outputs.

## [6.0.1] - 2026-03-23

### ⭐ Fixed - Generator duplicate emission & stability

- Added DeclaringAssembly metadata to discovered services and filter discovered services by the target assembly to prevent cross-assembly leakage.
- Deduplicated emission within a single generated file so each service is emitted only once (first-assigned wins).
- Ensured RegisterAll() only calls registration methods that are actually emitted in the file to avoid dangling method calls and compile errors.
- Removed legacy v3 generated files from the repository to prevent historical duplication and confusion.
- Improved diagnostics and generation report for easier troubleshooting when building in Unity.

### 🔧 Notes

- The generator cannot automatically resolve cases where the same FullName exists in multiple assemblies (duplicate type definitions). Those must be fixed in source.
- Building Unity projects to regenerate .g.cs typically requires running the Unity Editor or restoring NuGet packages for editor projects; see docs for guidance.

## [6.0.0] - 2026-03-21

### ⭐ Added - Performance & Developer Experience

**Enhanced Diagnostics:**
- All 15 diagnostic messages now include documentation links (https://docs.nhemdangfugbixs.com/diagnostics/NDXXX)
- Enhanced error messages with concrete remediation hints and "Fix:" instructions
- Added detailed descriptions to all diagnostics for better IDE integration
- Improved message clarity for ND001, ND002, ND003, ND005, ND008, ND009, ND105, ND106, ND107, ND108, ND110

**Additional Code Fix Providers:**
- **ND005**: Conflict detection - removes duplicate manual registration
- **ND006**: Cross-scope dependency - adds bridge scope or moves registration
- **ND111**: Missing contract - enables AsImplementedInterfaces
- **ND112**: Duplicate contract - removes duplicate registration
- **ND113**: Scene view binding - adds [AutoRegisterIn] or manual registration
- Total: 9 Code Fix Providers (up from 3 in v5.1.0)

**Developer Documentation:**
- **CONTRIBUTING.md**: Complete guide for analyzer + generator contributions
- **GitHub Issue Templates**: Bug report, performance regression, false positive, feature request
- Improved development workflow documentation

### 🔧 Changed

- Diagnostic messages follow consistent format: Problem + Fix + Docs + Description
- All diagnostics now have `description` parameter for IDE tooltip integration

### 📊 Statistics

- **Total Tests**: 46 (all passing)
- **Diagnostic Codes**: ND001-ND113 (15 active diagnostics, all with docs links)
- **Code Fix Providers**: 9 (ND001, ND005, ND006, ND008, ND009, ND110, ND111, ND112, ND113)
- **CLI Tools**: 1 (`dotnet di-smoke`)

### ⚠️ BREAKING: None

v6.0.0 is fully backward compatible with v5.x. No code changes required.

## [5.1.0] - 2026-03-18

### ⭐ Added - Developer Experience & Runtime Validation

**Code Fix Providers (IDE Auto-Fix):**
- **ND008**: MessagePipe Broker registration - offers snippet to create broker class or manual registration
- **ND009**: ILogger Root infrastructure - offers LoggerFactory registration snippet or navigation to root scope
- **ND110**: View Component registration - offers `[AutoRegisterIn]` attribute or `RegisterComponentInHierarchy()` snippet

**Preflight CLI (`dotnet di-smoke`):**
- New `DangFugBixs.Cli` project with dotnet global tool support
- `preflight` command: clean + build + validate orchestration
- `validate` command: direct assembly validation
- Options: `--format`, `--output`, `--clean`, `--resolve-smoke`
- Usage:
  ```bash
  dotnet di-smoke preflight MyProject.csproj
  dotnet di-smoke preflight MyProject.csproj --resolve-smoke
  dotnet di-smoke validate bin/Debug/MyProject.dll
  ```

**Runtime Resolve Smoke (Phase 1):**
- `ResolveSmokeValidator` validates DI container resolution via reflection
- Headless validation (no Unity Editor required)
- EntryPoint resolution validation (ITickable, IInitializable, etc.)
- Coverage statistics (Total, Resolved, Skipped)
- Reports resolution errors with detailed messages

### 🔧 Changed

- `DiSmokeValidation` uses reflection for VContainer resolution (no direct dependency)
- `ResolveValidationResult` provides detailed human-readable output

### 📊 Statistics

- **Total Tests**: 46 (all passing)
- **Diagnostic Codes**: ND001-ND110 (16 active diagnostics)
- **CLI Tools**: 1 (`dotnet di-smoke`)
- **Code Fix Providers**: 3 (ND008, ND009, ND110)

## [5.0.0] - 2026-03-18

### ⭐ Added - Cysharp Integration & DI Visualizer

**MessagePipe Support:**
- Generator now emits `builder.RegisterMessageBroker<T>()` for types marked with `[AutoRegisterMessageBrokerIn(typeof(TScope))]`
- `MessagePipeType` enum (Publisher, Subscriber, Handler) for broker classification
- Symbol-based MessagePipe detection using Roslyn semantic analysis
- Multiple broker attributes supported on same type

**DI Visualizer Report:**
- Generated `RegistrationReport.g.cs` with metadata fields: `ServiceCount`, `ScopeCount`, `Scopes[]`, `Entries[]`, `Consumers[]`, `LoggerRoots[]`, `LoggerConsumers[]`
- Markdown report generation for human-readable inspection
- Report includes MessagePipe broker metadata and logger consumer tracking

**Cross-Scope Analyzer (ND006):**
- New diagnostic ND006 "Inaccessible Dependency Scope" for invalid cross-scope dependencies
- Scope graph analysis from `[AutoRegisterIn]` and `[LifetimeScopeFor]` attributes
- Parent→child scope validity detection (VContainer hierarchy)
- Identity mapping suppression via `[LifetimeScopeFor]` bridge

### 🔧 Changed

- `SemanticScopeUtils.TryGetMessagePipeDependency()` now returns `MessagePipeType` enum instead of string role
- `ServiceInfo` struct extended with `MessagePipeKind` field
- Common project updated to netstandard2.1 for Runtime compatibility
- Generator and Analyzers projects include Runtime source files for shared types

### 📊 Statistics

- **Total Tests**: 46 (all passing)
- **Diagnostic Codes**: ND001-ND107 (14 active diagnostics including ND006)
- **CLI Tools**: 1 (DI Smoke Validation)

## [4.2.0] - 2026-03-18

### ⭐ Added - DI Guardrails & Smoke Testing

- **MessagePipe Broker Guard (ND008)**: New analyzer that validates MessagePipe `IPublisher<T>` and `ISubscriber<T>` dependencies. Warns when a type injects MessagePipe services but no corresponding `[AutoRegisterMessageBrokerIn]` registration exists in a reachable scope.
- **ILogger Root Guard (ND009)**: New analyzer that validates `ILogger<T>` constructor dependencies. Warns when types inject loggers but the root scope doesn't have both `ILoggerFactory` and `ILogger<>` registrations.
- **DI Smoke Test Tool**: New CLI tool (`DangFugBixs.DiSmokeValidation`) for fail-fast validation before Unity Play mode. Uses reflection to validate generated injectors, MessagePipe broker reachability, and ILogger root infrastructure.
  - Usage: `dotnet run --project DangFugBixs.DiSmokeValidation -- <assembly.dll> [--format json|text]`

### 🧪 Testing

- **MessagePipe Guard Tests**: 4 tests covering same-scope, root-scope, missing broker, and non-generated consumer scenarios.
- **ILogger Guard Tests**: 3 tests covering valid setup, missing factory, and missing adapter scenarios.
- **Smoke Validation Tests**: 15 tests covering complete graphs, missing components, and JSON output formatting.

### 📊 Statistics

- **Total Tests**: 44 (all passing)
- **Diagnostic Codes**: ND001-ND107 (13 active diagnostics)
- **CLI Tools**: 1 (DI Smoke Validation)

## [4.1.0] - 2026-03-18

### ⭐ Added - Resiliency & Safety Guards

- **Conflict Detection Analyzer (ND005)**: New analyzer that prevents "Double Registration" by flagging manual `builder.Register<T>` calls for types already marked with `[AutoRegisterIn]`.
- **Smart EntryPoint Registration**: The generator now intelligently filters out VContainer lifecycle interfaces (ITickable, IStartable, etc.) when emitting `RegisterEntryPoint`, preventing redundant lifecycle method calls.
- **Global Registration Deduplication**: Added a strict deduplication pass to the generator to ensure each type is only registered once, even if discovered across multiple assemblies.

### ⚙️ Improved

- **Version Alignment**: Standardized all internal version markers and statistics to v4.1.0.

## [4.0.0] - 2026-03-17

### ⭐ Added - Module/Installer Pattern

- **VContainer Installers**: Introduced `IVContainerInstaller` interface. The generator now automatically detects, instantiates, and executes these installers within their respective scopes. This allows for decoupled, modular registration logic.
- **Installer Ordering**: Added `[InstallerOrder(int)]` attribute to define execution priority among multiple installers. Lower numbers execute first.
- **Strict Validation**: Added new Roslyn Analyzers (`ND105-ND107`) to ensure installers are public, have parameterless constructors, and are not components (MonoBehaviours).
- **Unified Diagnostics**: Standardized all diagnostic codes to the `NDxxx` prefix for a cleaner development experience.
- **Improved Registration Flow**: Re-structured the generated code to execute installers *before* other services, ensuring foundational dependencies are established early.

### ⚠️ BREAKING CHANGES

- **Legacy Attributes Removed**: Completely removed the deprecated `[AutoRegister(string)]` and `[AutoRegisterFactory]` attributes.
- **Stricter Access Control**: Installers used with the generator MUST now be `public` to ensure accessibility from the generated code.
- **Parameterless Constructor Requirement**: Installers MUST provide a public parameterless constructor.

## [3.6.0] - 2026-03-16

### ⭐ Added - Advanced VContainer Features

- **Global Exception Handlers**: The generator now automatically detects classes implementing `IEntryPointExceptionHandler` and registers them using `builder.RegisterEntryPointExceptionHandler`. This allows for centralized error handling for all entry point ticks/initializations.
- **Build Callbacks**: Classes implementing `IBuildCallback` are now automatically registered with `builder.RegisterBuildCallback`, allowing for complex post-build setup logic.
- **Mandatory VContainer Imports**: The generator now safely includes `using VContainer;` and `using VContainer.Unity;` in the generated file headers to ensure all extension methods are available, preventing CS1061 errors.
- **Polymorphic Component Registration**: Fixed a critical bug where MonoBehaviours implementing entry point interfaces (e.g., `ITickable`) were missing their interface bindings. They are now correctly registered using `RegisterComponent...` combined with `.AsImplementedInterfaces()`.

## [3.5.0] - 2026-03-16

### ⭐ Added - Robust Entry Point Detection

- **Intelligent Interface Matching**: Upgraded the detection logic for VContainer life-cycle interfaces (IInitializable, ITickable, etc.). It now supports fully qualified names and correctly handles cases where the semantic model fails to resolve the interface symbol.
- **Expanded Interface Support**: Added explicit support for `IAsyncStartable`.
- **Centralized Detection Logic**: Moved entry point detection to a shared utility used by both the local class analyzer and the referenced assembly scanner for perfect consistency.

## [3.4.0] - 2026-03-16

### ⭐ Added - Resilient Scanning & Traceability

- **Resilient Assembly Scanning**: The generator now gracefully handles unresolved assembly references (CS0234/AssemblyResolutionException). Instead of crashing, it skips the problematic assembly and reports a warning.
- **Traceability Headers**: Generated files now include metadata headers with version, timestamp, service count, and any scan warnings for easier troubleshooting.
- **Diagnostic ND104**: Added a new warning diagnostic that appears in the Unity console when an assembly scan fails, providing the assembly name and error message.
- **Improved Resiliency**: Added defensive try-catch blocks around all external symbol member access during discovery.

## [3.3.0] - 2026-03-16

### ⭐ Added - Final Robustness & Collision Fixes

- **Direct NhemLifetime Usage**: Removed `NLifetime` alias entirely. Users should now use `NhemLifetime` (or `NhemDangFugBixs.Attributes.NhemLifetime`) directly. This eliminates all duplicate alias (CS1537) and stale reference (CS0234) issues.
- **Universal Partial Modifiers**: Ensured all generated container classes (`VContainerRegistration`, `SceneInjectionBlueprint`) are marked as `partial` (CS0260).
- **Strict Single File per Assembly**: Standardized generated output to a single `{AssemblyName}.g.cs` file to ensure reliable overwriting of old artifacts.
- **Scene Injection Deduplication**: Improved deduplication logic for `ComponentTypes` array to prevent duplicate definition errors (CS0102).
- **Type Reference Alignment**: Fixed remaining incorrect references to the internal `NhemLifetime` enum (CS0234).

### ⚠️ IMPORTANT: Cache Clearing
While this update aims to resolve collisions automatically, if you still encounter errors, please **manually delete the `Generated` folder** in your IDE's source generator cache or restart Unity to clear any persistent v3.1/v3.2 artifacts.

## [3.2.1] - 2026-03-16

### ⭐ Added - Generator Robustness Fix

- **Consolidated Global Usings**: Moved `global using NLifetime` into the main registration file, eliminating duplicate alias errors (CS1537).
- **Stable Hint Names**: Improved hint naming logic to ensure files overwrite previous versions correctly, preventing duplicate definition errors (CS0102).
- **Type Reference Fix**: Fixed an issue where the emitter was using an outdated internal name for the `NhemLifetime` enum.
- **Improved Sanitization**: Enhanced assembly name sanitization while preserving stability for multi-assembly projects.

### ⚠️ IMPORTANT: Migration Cleanup
Due to changes in how files are named, please **manually delete the `Generated` folder** in your Unity project's `Temp` or `Library` directory (or wherever your IDE/Unity stores Source Generator output) to clear stale v3.1 files that might cause "Missing partial modifier" or "Duplicate definition" errors.

## [3.2.0] - 2026-03-16
... (rest of the file)
