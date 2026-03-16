# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
