# GEMINI.md - NhemDangFugBixs Tooling

## Project Overview
**Nhem Dang Fug Bixs Tooling** is a high-performance C# Source Generator and Roslyn Analyzer suite designed for **Unity** projects using **VContainer**. It automates dependency injection registration through type-safe attributes, reducing boilerplate and preventing runtime errors with compile-time validation.

### Main Technologies
- **C# / .NET Standard 2.0**: For compatibility with Unity's Roslyn version.
- **Roslyn Source Generators**: Incremental generators for high-performance code emission.
- **Roslyn Analyzers**: Custom diagnostics (ND001-ND103) for validating DI configurations.
- **VContainer**: The target DI framework for Unity.

### Architecture
- **`Source~`**: The "hidden" source directory (ignored by Unity) containing the C# projects.
  - `DangFugBixs.Generators~`: The core source generator logic.
  - `DangFugBixs.Analyzers~`: Custom Roslyn analyzers for DI validation.
  - `DangFugBixs.Runtime~`: Attributes and types used at runtime in Unity.
  - `DangFugBixs.Common~`: Shared models and utilities.
- **`Runtime/`**: Contains the compiled `NhemDangFugBixs.Runtime.dll` and its `.meta` files.
- **`Analyzers/`**: Contains the compiled `NhemDangFugBixs.Generators.dll` and `NhemDangFugBixs.Analyzers.dll`.
- **`Editor/`**: Unity-specific editor extensions (e.g., `SceneInjectedProcessor.cs`).
- **`GameFeelUnity-Test-Files/`**: Examples, migration guides, and test scenarios.

---

## Building and Running

### Development Workflow
1.  **Open Solution**: Open `Source~\NhemDangFugBixs.Tooling.sln` in an IDE (Rider or Visual Studio).
2.  **Build**: Building the solution will compile the DLLs.
3.  **Deployment**: 
    - The `Directory.Build.props` manages automatic deployment to a linked Unity project.
    - Set the `NHEM_UNITY_PROJECT_ROOT` environment variable or create a `Source~\LocalBuild.props` file to specify your Unity project path.
    - Alternatively, place a `.nhem-deploy-target` file in your Unity project's root.

### Usage in Unity
1.  Ensure **VContainer** is installed.
2.  The `Analyzers/` DLLs must be imported and marked as **Roslyn Analyzers** in the Unity Inspector.
3.  Reference `NhemDangFugBixs.Runtime.dll` in your `asmdef` files.
4.  The generator automatically produces `VContainerRegistration.g.cs` containing methods like `RegisterGame(builder)`.

---

## Development Conventions

### Registration Patterns (v3.0+)
- **Type-Safe Scopes**: Always prefer `[AutoRegisterIn<TScope>]` over the deprecated string-based `[AutoRegister]`.
- **Naming Conventions**: The generator strips "LifetimeScope" from the class name to create registration methods:
  - `GameLifetimeScope` → `VContainerRegistration.RegisterGame(builder)`
- **Aliases**: Use the automatically generated `NLifetime` alias for `NhemDangFugBixs.Attributes.Lifetime` to avoid conflicts with VContainer's native `Lifetime` enum.

### Coding Standards
- **Source Generators**: Implement `IIncrementalGenerator` for performance.
- **Diagnostics**: All analyzers should use the `NDxxx` prefix for error codes.
- **Multi-Assembly Support**: The generator uses assembly names as hint prefixes to prevent filename collisions in projects with multiple `asmdef` files.

### Testing
- Example implementations and migration guides are located in `GameFeelUnity-Test-Files/`.
- Use `DangFugBixs.Tests` (within the generators directory) for unit testing generator output.

---

## Key Files
- `package.json`: UPM package definition.
- `Source~\Directory.Build.props`: MSBuild logic for auto-deploying to Unity.
- `Source~\DangFugBixs.Generators~\DangFugBixs.Generators\VContainerAutoRegisterGenerator.cs`: Main source generator entry point.
- `README.md`: General usage and feature guide.
