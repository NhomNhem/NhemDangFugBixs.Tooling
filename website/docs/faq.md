---
sidebar_position: 4
---

# FAQ & Troubleshooting (v6.0)

## Generated code not visible in Unity?
- Ensure DLLs are in `Assets/Plugins/Analyzers`.
- Mark DLLs as Roslyn Analyzer in Inspector.
- Rebuild Unity project if needed.

## Attribute added but not registered?
- Use `[AutoRegisterIn(typeof(Scope))]` (not legacy string-based attributes).
- Check for typos and correct namespace usage.

## Build error with Roslyn version?
- Target framework must be `netstandard2.0`.
- Use the documented Roslyn version for compatibility.

## CLI/Analyzer not working?
- Run `dotnet di-smoke validate` for diagnostics.
- Check analyzer warnings in IDE output.
