## Why

In layered Unity projects (asmdef), low-level service assemblies (Core, Data) cannot reference high-level LifetimeScope assemblies (Main, Runtime), making it impossible to use type-safe `[AutoRegisterIn<TScope>]` without causing Circular Dependency errors. This change introduces a "Cross-Layer Bridge" using lightweight Identity Types to enable type-safe registration across any number of architectural layers.

## What Changes

- **Identity Types**: Support for lightweight POCO/marker classes in low-level assemblies to serve as scope identifiers.
- **[LifetimeScopeFor<T>] Attribute**: A new attribute to map a `LifetimeScope` to an Identity Type.
- **Enhanced [AutoRegisterIn<T>]**: Support for passing Identity Types as the generic argument.
- **Global Discovery**: Source Generator update to scan referenced assemblies and emit unified registration code in the high-level assembly.
- **Breaking Change**: Deprecation of direct `LifetimeScope` type references in `[AutoRegisterIn<T>]` in favor of Identity Types (v3.0 approach still supported but discouraged for layered projects). **BREAKING**

## Capabilities

### New Capabilities
- `identity-types`: Lightweight marker types for cross-assembly scope identification.
- `global-discovery`: Source Generator capability to scan all referenced assemblies for registration attributes.
- `unified-registration`: Generation of a single master registration method in the high-level assembly to eliminate manual boilerplate.

### Modified Capabilities
- `type-safe-registration`: Updated to support Identity Types instead of just direct `LifetimeScope` types.

## Impact

- **NhemDangFugBixs.Runtime**: Addition of `LifetimeScopeForAttribute`.
- **NhemDangFugBixs.Generators**: Major update to scan referenced assemblies (`Compilation.SourceModule.ReferencedAssemblySymbols`) and map identities to scopes.
- **Unity Projects**: Requires a one-time setup of Identity Types in a base assembly (e.g., `Shared.asmdef`).
