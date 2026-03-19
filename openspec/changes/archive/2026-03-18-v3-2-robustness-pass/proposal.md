## Why

The current version (v3.1) of the Source Generator encounters compilation errors in multi-assembly Unity projects due to:
1. Duplicate `global using NLifetime` directives.
2. Missing `partial` modifiers on generated classes.
3. Namespace/Class name collisions in complex assembly structures.
4. Ambiguity between `VContainer.Lifetime` and `NhemDangFugBixs.Attributes.Lifetime`.

This change aims to make the generated code robust and "indestructible" in any assembly configuration.

## What Changes

- **Namespace Isolation**: Use fully qualified names (with `global::` prefix) in generated code.
- **Partial Classes**: Apply the `partial` keyword to all generated static classes.
- **Global Using Isolation**: Move `global using NLifetime` to a dedicated file generated once per assembly or use standard usings.
- **Lifetime Enum Renaming**: Rename internal `Lifetime` enum to `NhemLifetime` to prevent naming conflicts with VContainer while keeping the `NLifetime` alias.
- **Assembly Sanitization**: Improve assembly name sanitization to prevent name collisions.

## Capabilities

### New Capabilities
- `robust-emission`: Generation of Indestructible C# code using `partial` and `global::`.
- `isolated-global-usings`: Automated management of global using directives across multiple assemblies.

### Modified Capabilities
- `vcontainer-registration`: Update emission logic to use robust naming patterns.

## Impact

- **NhemDangFugBixs.Runtime**: Breaking change in internal enum naming (Lifetime -> NhemLifetime).
- **NhemDangFugBixs.Generators**: Significant update to `RegistrationEmitter` and `VContainerAutoRegisterGenerator`.
- **Unity Projects**: Resolves all CS1537, CS0260, and CS0102 errors.
