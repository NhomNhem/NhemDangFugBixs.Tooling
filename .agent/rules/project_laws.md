# NhemDangFugBixs Project Technical Laws

These laws are MANDATORY for all development within this project.

## 1. File & Directory Structure
- All source code, solutions, and build-time logic MUST live in the `Source~` directory.
- This ensures Unity ignores these files completely, preventing `ReflectionTypeLoadException` and domain reloading issues.
- Never place `.cs` files directly in `Runtime/` or `Editor/` folders; use the build pipeline to deploy DLLs there.

## 2. Namespace & Naming
- All code MUST use the `NhemDangFugBixs.*` namespace hierarchy.
- Assembly names MUST follow `NhemDangFugBixs.Component.dll`.

## 3. Common Library Usage
- The `NhemDangFugBixs.Common` project (found in `Source~/DangFugBixs.Common~`) is the single source of truth for shared models.
- If a model (like `ServiceInfo`) is used by both the Generator and another component, it MUST reside here.
- Always check for existing models in `Common` before creating new ones.

## 4. Encapsulation
- Internal models used for build-time processes should be marked as `internal` to avoid polluting the public API.

## 5. Metadata (.meta) Integrity
- DLL `.meta` files for Analyzers/Generators MUST have all platforms DISABLED.
- Use the templates in `Source~/MetaTemplates/` as the primary source of truth.
- The build process must automatically copy these validated `.meta` files to ensure they are not overwritten by Unity.
