## Why

The current generator and analyzers can compile successfully while the real DI graph still fails at runtime in Unity. A dedicated smoke validation command is needed to verify generated injectors, registrations, and scope wiring before entering Play mode.

## What Changes

- Add a DI smoke validation command that builds the project and runs a reflection-based validator over generated injector output.
- Fail fast when generated registrations are missing, duplicated, or inconsistent with declared scope metadata.
- Surface validation results outside Unity so problems can be caught in CI and local builds.

## Capabilities

### New Capabilities
- `di-smoke-validation`: Command and validator that inspect generated DI output and report invalid registration graphs before Unity Play.

### Modified Capabilities
- `conflict-detection-analyzer`: Validation results should align with existing analyzer conflict rules and surface duplicate registrations consistently.
- `smart-lifecycle-filtering`: Validation must respect existing scope and entry-point semantics when evaluating generated injectors.

## Impact

Affected areas include the generator output pipeline, build tooling, analyzer-facing diagnostics, and test projects that need to exercise the validation command against generated code. The command should integrate with the existing .NET build flow and remain usable outside Unity.
