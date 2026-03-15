## Context

Multi-assembly Unity projects with `asmdef` files create isolated compilation contexts. The Source Generator (SG) must emit code that is valid within each assembly while avoiding collisions when multiple assemblies are referenced. The current implementation uses `global using` in header files and lacks consistent `partial` modifiers, leading to CS1537 and CS0260 errors.

## Goals / Non-Goals

**Goals:**
- Eliminate all duplicate definition and alias errors (CS1537, CS0260, CS0102).
- Prevent naming collisions between VContainer and NhemDangFugBixs types.
- Ensure generated code is valid regardless of the user's `using` directives.

**Non-Goals:**
- Removing the `NLifetime` alias feature.
- Changing the public API of the registration methods.

## Decisions

### 1. Rename `Lifetime` to `NhemLifetime`
Rename the internal enum in `NhemDangFugBixs.Attributes`.
- **Rationale**: `Lifetime` is a very common name (VContainer has one, too). Renaming it to `NhemLifetime` eliminates ambiguity.
- **Alias**: We will keep `NLifetime` as a global alias to `NhemLifetime`.

### 2. Dedicated Global Usings File
Move the `global using NLifetime = global::NhemDangFugBixs.Attributes.NhemLifetime;` to a single file per assembly named `NhemDangFugBixs.GlobalUsings.g.cs`.
- **Rationale**: Roslyn allows only one declaration of a global alias per assembly. By using a specific hint name and only adding it once in the SG's `Execute` loop, we prevent duplicates.

### 3. Fully Qualified Names with `global::`
All types in generated code (e.g., `IContainerBuilder`, `Lifetime`, `NLifetime`) will be emitted using their fully qualified names.
- **Rationale**: Prevents conflicts with user-defined types or local usings. Example: `global::VContainer.IContainerBuilder`.

### 4. Mandatory `partial` on All Generated Classes
Always use `public static partial class`.
- **Rationale**: Allows multiple generated files or manual user code to extend the same class without conflict.

## Risks / Trade-offs

- **[Risk] Migration Effort** → Users might have used `Lifetime` directly in their code.
  - **Mitigation**: The `NLifetime` alias remains unchanged, and we'll keep the `Lifetime` enum name as a deprecated alias if possible, but preferably just rename it and let the alias handle it.
- **[Risk] Code Verbosity** → Fully qualified names make the generated code harder to read.
  - **Mitigation**: This code is machine-generated and not intended for manual editing. Robustness is more important than readability here.
