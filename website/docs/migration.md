# Migration Guide (v3.x → v4.0)

v4.0 is a major breaking change that removes legacy attributes and introduces stricter validation.

## Breaking Changes
- Removed: [AutoRegister(string scope)]
- Removed: [AutoRegisterFactory]
- Stricter Validation: Installers must be public and have a parameterless constructor

## Migration Steps
1. Replace [AutoRegister("Name")] with [AutoRegisterIn(typeof(Scope))]
2. Convert [AutoRegisterFactory] logic into an IVContainerInstaller implementation
3. Ensure all services use NhemLifetime instead of deprecated Lifetime enum alias
