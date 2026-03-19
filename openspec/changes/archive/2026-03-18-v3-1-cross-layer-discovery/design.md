## Context

The current v3.0 implementation of `NhemDangFugBixs.Tooling` relies on direct references to `LifetimeScope` types within the `[AutoRegisterIn(typeof(T))]` attribute. In Unity, projects are often divided into multiple assemblies (`asmdef`). High-level assemblies (containing the `LifetimeScope`) reference low-level assemblies (containing services). If a service in a low-level assembly tries to register itself into a high-level scope, it must reference that assembly, creating a circular dependency because the high-level assembly also needs to call the generated registration code in the low-level assembly.

## Goals / Non-Goals

**Goals:**
- Decouple services from their concrete `LifetimeScope` implementation using Identity Types.
- Enable type-safe cross-assembly discovery of registrations.
- Provide a unified registration API at the high-level assembly.
- Maintain backward compatibility with the v3.0 local registration pattern.

**Non-Goals:**
- Removing string-based registration support (kept for legacy).
- Automatic discovery of assemblies that *do not* reference the Runtime DLL.
- Support for complex generic scope resolution (beyond simple type markers).

## Decisions

### 1. Identity Types as Type Markers
Instead of referencing a `LifetimeScope`, we use an arbitrary type (class/interface) as a marker.
- **Rationale**: This allows placing the "Identity" in a low-level shared assembly that both the Service and the Scope can reference without circularity.
- **Alternative**: Using strings. **Rejected** because we want to maintain the "Type-Safe" benefit of v3.0 (IntelliSense, refactoring safety).

### 2. [LifetimeScopeFor(typeof(T))] Attribute
A new attribute used to map the Identity Type to the actual `LifetimeScope`.
- **Rationale**: Provides the missing link for the Source Generator to know which `LifetimeScope` should host which registrations.

### 3. Cross-Assembly Metadata Scan in High-Level Assembly
The Source Generator running in the assembly with `[LifetimeScopeFor(typeof(T))]` will scan all referenced assemblies via `Compilation.SourceModule.ReferencedAssemblySymbols`.
- **Rationale**: This is the only way for the high-level assembly to "see" registrations in low-level assemblies and generate the unified code.
- **Alternative**: Each low-level assembly generating its own code. **Rejected** because it still requires manual "collection" calls in the high-level scope, which is boilerplate.

### 4. Unified Master Registration Method
Generate a single `VContainerRegistration.RegisterAll(builder)` in the high-level assembly.
- **Rationale**: Dramatically simplifies the user experience. One call handles everything discovered across all layers.

## Risks / Trade-offs

- **[Risk] Performance Overhead** → Scanning all referenced assembly symbols in a massive project might slow down compilation.
  - **Mitigation**: Only perform the full scan if `[LifetimeScopeFor]` is present in the current compilation. Use Incremental Generator caching.
- **[Risk] Namespace Collisions** → Two different high-level assemblies might try to claim the same Identity.
  - **Mitigation**: Analyzer error if multiple `[LifetimeScopeFor(typeof(T))]` exist for the same `T` across the project (or at least within the reference graph).
- **[Risk] Metadata Resolution** → Attributes in referenced DLLs might not have full type information.
  - **Mitigation**: Use `ToDisplayString()` for identity mapping and ensure types are fully qualified.
