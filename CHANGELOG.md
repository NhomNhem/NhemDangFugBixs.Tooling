# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2026-03-15

### ⭐ Added - Type-Safe Scope Registration

- **`[AutoRegisterIn<TScope>]` Generic Attribute**: Type-safe scope references with full IntelliSense support. No more string typos!
- **Convention-Based Naming**: Automatic registration method names by stripping "LifetimeScope" suffix (e.g., `GameplayLifetimeScope` → `RegisterGameplay()`).
- **`[ScopeName("Custom")]` Override**: Custom registration method names when convention doesn't fit.
- **Compile-Time Validation**: Roslyn analyzers catch scope errors before they become runtime failures.

### 🔒 Breaking Changes

- **String-based scopes deprecated**: `[AutoRegister(scope: "Gameplay")]` is now marked `[Obsolete]` and produces compiler warnings.
- **Migration required**: Update to `[AutoRegisterIn<GameplayLifetimeScope>]` syntax.
- **See Migration Guide**: Refer to README.md for step-by-step migration instructions.

### 🛡️ New Diagnostic Rules

| Code | Severity | Description |
|------|----------|-------------|
| ND001 | Error | Scope type not found |
| ND002 | Error | Type must inherit from LifetimeScope |
| ND003 | Error | Circular scope dependency detected |
| ND004 | Warning | Parent scope cannot depend on child scope |
| ND103 | Warning | Scope has registrations but no LifetimeScope calls Register method |

### 📚 Documentation

- Updated README.md with v3.0 usage examples
- Added migration guide (v2.x → v3.0)
- Documented all diagnostic codes
- Added parent-child scope injection best practices

### 🧪 Testing

- Added `TypeSafeScopeTests.cs` with comprehensive generator tests
- Tests for convention-based naming
- Tests for parent-child injection patterns

### 🏗️ Architecture

- `ServiceInfo` model extended with `ScopeTypeName` and `UsesTypeSafeScope` properties
- `ClassAnalyzer` refactored to support both generic and legacy attributes
- `RegistrationEmitter` updated with convention-based naming logic

---

## [2.0.1] - 2026-03-04

### Fixed

- **Duplicated Contract Types**: Fixed "Conflict implementation type" error in VContainer by removing redundant `.AsImplementedInterfaces()` calls for EntryPoints.

## [2.0.0] - 2026-03-04

### Added

- **Dynamic Binding**: Support for `AsTypes` in `[AutoRegister]` to manually specify interfaces.
- **Auto EntryPoints**: Intelligent detection and registration of VContainer lifecycle interfaces (`ITickable`, `IInitializable`, etc.).
- **Factory Registration**: New `[AutoRegisterFactory]` attribute for automated `Func<T>` registration.
- **Assembly-Safe Generation**: Unique hint names for generated files to prevent cross-assembly collisions.
- **Professional Documentation**: Comprehensive English `README.md` with installation guides and quick starts.

### Changed

- **Internal Model**: Enhanced `ServiceInfo` and `ClassAnalyzer` to support advanced registration logic.
- **Improved Emitter**: Generator now emits optimized registration code for both components and pure C# classes.

## [1.1.2] - 2026-02-25

### Added

- **`[AutoRegisterScene]` attribute**: Automated registration of Scene-based MonoBehaviours.
- **Improved discovery**: Source Generator now intelligently finds and registers components existing in the Scene hierarchy.

### Fixed

- **Ambiguous reference (CS0104)**: Resolved conflict between `UnityEngine.Object` and `System.Object` in generated code.
- **Non-Unity build support**: Added preprocessor guards to allow generator and sandbox projects to compile outside of Unity environment.

## [1.0.1] - 2026-02-09

### Added

- **GitHub Issue Templates**: Structured forms for Bug Reports and Feature Requests.
- **AI Agent Workflows**: Standardized slash commands for `/release`, `/build-and-test`, and `/design-plan`.

### Changed

- **Infrastructure Consolidation**: Merged technical laws and AI rules into a single `.agent` directory.
- **Git Metadata Support**: Fixed `.gitignore` to correctly track `.meta` files for UPM distribution.

## [1.0.0] - 2026-02-09

### Added

- **VContainer Auto-Registration**: Source Generator for automatic dependency injection mapping.
- **Optimized Stat System**: High-performance character stat calculation (HP, Attack, etc.) with Zero-GC logic.
- **Automatic Interface Mapping**: Intelligent detection and registration of primary interfaces.
- **Infrastructure**: Isolated source code in `Source~` to prevent Unity domain reload issues.
- **Professional Packaging**: Standard UPM structure for seamless Git import.
- **Roslyn Analyzers**: Real-time validation rules for VContainer best practices.
- **Documentation**: Comprehensive README, Release Guide, and Technical Laws.

### Support

Developed by NhomNhem
