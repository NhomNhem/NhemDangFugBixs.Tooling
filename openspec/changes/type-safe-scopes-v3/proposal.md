## Why

The current string-based scope registration system (`scope: "Gameplay"`) is error-prone and lacks IDE support. As the project scales beyond 2 scopes, typos in scope names and forgotten registrations become critical risks. This change introduces type-safe scope references using C# generics, compile-time validation, and convention-based naming to prevent runtime DI failures.

## What Changes

- **Type-Safe Scope Attribute**: New `[AutoRegisterIn(typeof(TScope))]` generic attribute for clean, IDE-friendly scope selection
- **Convention-Based Naming**: Automatic method name generation by stripping "LifetimeScope" suffix (e.g., `GameplayLifetimeScope` → `RegisterGameplay()`)
- **Scope Validation Diagnostics**: 4 new Roslyn analyzer rules (ND001-ND004) catching scope errors at compile-time
- **Hierarchical Scope Support**: Explicit parent-child scope wiring with analyzer validation
- **Migration Path**: v3.0 is a **BREAKING** change—string-based scopes deprecated and removed

## Capabilities

### New Capabilities

- `type-safe-scopes`: Generic attribute syntax `[AutoRegisterIn(typeof(TScope))]` for type-safe scope registration
- `scope-diagnostics`: Roslyn analyzer rules validating scope types, hierarchy, and cross-scope dependencies
- `convention-based-naming`: Automatic registration method naming from type names
- `scope-hierarchy-validation`: Compile-time validation of parent-child scope relationships

### Modified Capabilities

- `vcontainer-registration`: Registration method generation now uses convention-based naming instead of string scope names

## Impact

- **Breaking Change**: Existing `[AutoRegister(scope: "string")]` syntax removed
- **Generator Changes**: `ClassAnalyzer`, `ServiceInfo`, `RegistrationEmitter` require updates
- **New Analyzer Rules**: 4 new diagnostics (ND001-ND004) added to `DangFugBixs.Analyzers`
- **Runtime Attributes**: New `AutoRegisterInAttribute` class added to `DangFugBixs.Runtime`
- **Documentation**: README and usage examples need updates for v3.0 syntax
