# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
