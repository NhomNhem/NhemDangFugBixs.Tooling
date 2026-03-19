## Why

The v3.2.1 release of the Source Generator still encounters compilation errors in some Unity environments (specifically multi-assembly projects) due to:
1. **Stale Files**: Unity's Source Generator cache may retain old generated files (e.g., `GlobalUsings.g.cs`) alongside the new consolidated files, causing duplicate alias (CS1537) and duplicate definition (CS0102) errors.
2. **Missing Partial Modifiers**: `SceneInjectionBlueprint` and `VContainerRegistration` are missing `partial` modifiers in some emission paths, causing CS0260.
3. **Invalid Type References**: Some parts of the emitter still attempt to reference `NhemDangFugBixs.Attributes.Lifetime` which was renamed to `NhemLifetime`.

## What Changes

- **Strict File Consolidation**: Ensure all generated code for a single assembly is emitted into exactly ONE file with a consistent hint name to overwrite stale v3.2.0 files.
- **Universal Partial Modifiers**: Apply `public static partial class` to all generated container classes.
- **Refined Type Mapping**: Verify and fix all references to the `NhemLifetime` enum.
- **Deduplication Logic**: Improve internal deduplication of `ComponentTypes` to prevent CS0102 if the same service is discovered multiple times.

## Capabilities

### New Capabilities
- `strict-single-file-emission`: Enforcement of a single output file per assembly to prevent collision with stale artifacts.

### Modified Capabilities
- `robust-emission`: Update to ensure `partial` is used everywhere and type references are correct.

## Impact

- **NhemDangFugBixs.Generators**: Updates to `RegistrationEmitter` and `VContainerAutoRegisterGenerator`.
- **Unity Projects**: Resolves remaining CS1537, CS0260, CS0102, and CS0234 errors without requiring manual cache clearing in most cases.
