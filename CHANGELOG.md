# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.2.0] - 2026-03-16

### ⭐ Added - Robustness Pass

- **Indestructible C# Emission**: Generated code now uses `global::` prefixes and fully qualified names for all types to prevent naming collisions.
- **Partial Class Support**: All generated static classes are now marked as `partial` to allow merging and prevent CS0260 errors.
- **Isolated Global Usings**: `global using NLifetime` is now emitted once per assembly in a dedicated `GlobalUsings.g.cs` file, resolving CS1537 errors.
- **Improved Sanitization**: Robust assembly name sanitization logic prevents namespace collisions in complex project structures.
- **Service Deduplication**: Automatic deduplication of services during emission prevents "already contains a definition" errors.

### 🔒 Breaking Changes

- **Enum Renaming**: Renamed internal `Lifetime` enum to `NhemLifetime` to eliminate ambiguity with `VContainer.Lifetime`. (Alias `NLifetime` remains unchanged).

## [3.1.0] - 2026-03-15

### ⭐ Added - Cross-Layer Discovery

- **Cross-Layer Support**: Register services from low-level assemblies (e.g., `Core.asmdef`) into high-level scopes (e.g., `Main.asmdef`) without circular dependencies.
- **Identity Types**: Support for lightweight marker types to decouple services from concrete `LifetimeScope` implementations.
- **`[LifetimeScopeFor(typeof(TIdentity))]` Attribute**: Maps a `LifetimeScope` to an Identity Type for cross-assembly discovery.
- **Unified Master Registration**: Single `VContainerRegistration.RegisterAll(builder)` call in the high-level scope registers discovered services from all layers.
- **Automatic Metadata Scanning**: Source Generator now scans all referenced assemblies for registration attributes.

### 🔒 Breaking Changes

- **Non-Generic Attributes**: Switched from `[AutoRegisterIn<T>]` to `[AutoRegisterIn(typeof(T))]` for compatibility with Unity's C# version (generic attributes require C# 11.0).

## [3.0.0] - 2026-03-15
... (rest of the file)
