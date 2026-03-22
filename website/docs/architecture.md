---
sidebar_position: 2
---

# Architecture Overview (v6.0)

NhemDangFugBixs Tooling is built for large Unity projects using VContainer. It consists of:
- **Source Generator**: Scans for `[AutoRegisterIn]` and emits registration code per scope.
- **Analyzer**: Roslyn analyzers (ND001–ND110) for compile-time DI validation and code fixes.
- **Runtime**: Attribute and enum definitions for use in Unity code.

## Generator Pipeline
1. **Input**: Analyzes syntax trees for attributed classes/installers.
2. **Transform**: Builds a model of services, scopes, and installers.
3. **Emit**: Generates `VContainerRegistration.g.cs` with registration methods per scope.

## Key Patterns
- **Module/Installer**: Implement `IVContainerInstaller` for complex DI logic.
- **Cross-Layer**: Use identity types to bridge assemblies without circular refs.
- **Type-Safe Scopes**: Register to specific LifetimeScope types for full safety.

## Directory Structure
- `Source~`: C# source (not imported by Unity)
- `Runtime/`, `Analyzers/`: Built DLLs for Unity
- `Editor/`: Unity Editor helpers
