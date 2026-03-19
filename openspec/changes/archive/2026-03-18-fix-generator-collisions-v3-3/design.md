## Context

The v3.2.1 release consolidated generated code into a single file per assembly, but stale generated files from v3.2.0 (like `GlobalUsings.g.cs`) often persist in Unity's internal cache. These stale files contain outdated references (e.g., to `Attributes.Lifetime`) and cause duplicate definition errors because they aren't overwritten by the new consolidated file (which has a different hint name).

## Goals / Non-Goals

**Goals:**
- Ensure all generated classes (`VContainerRegistration`, `SceneInjectionBlueprint`) are marked as `partial`.
- Prevent `global using` alias collisions (CS1537) by moving usings inside the namespace.
- Fix all incorrect references to the `NhemLifetime` enum.
- Standardize on a single, deterministic hint name per assembly that minimizes collision with old files.

**Non-Goals:**
- Removing files from the user's file system (Source Generators cannot delete files).
- Changing the public API of the attributes.

## Decisions

### 1. Move `global using` to Local `using` alias
Instead of `global using NLifetime = ...`, we will use `using NLifetime = ...` inside the generated namespace block. This ensures that if a stale `GlobalUsings.g.cs` exists in the assembly, the new file won't conflict with its global alias, and the local alias will take precedence within the generated code.

### 2. Universal `partial` for All Generated Containers
Every generated class will explicitly use `public static partial`. This allows the code to compile even if stale versions of these classes exist (as long as they are also partial).

### 3. Stable Hint Naming
We will use `{AssemblyName}.g.cs` as the hint name. This is clean and unlikely to conflict with the specific `GlobalUsings.g.cs` or `VContainerRegistration.g.cs` names used in the past, but it provides a single point of truth for the current version.

### 4. ComponentTypes Deduplication
We will use `HashSet<string>` or `GroupBy` to ensure `ComponentTypes` array content is unique within a single emission.

## Risks / Trade-offs

- **[Risk]** Stale files might still cause some errors if they define the exact same non-partial symbols.
  - **[Mitigation]** Instruct users to clear Unity's cache if errors persist, but the use of `partial` and local usings should solve most common cases.
