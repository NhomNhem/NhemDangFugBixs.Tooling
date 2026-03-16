# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
