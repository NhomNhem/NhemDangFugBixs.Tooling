# Skill — Generator Feature

## Purpose

Use this skill when implementing or changing source-generation behavior for VContainer registrations.

Examples:

- Add `[As<T>]` support.
- Add `[BindAs]` support.
- Add `[EntryPoint]` generation.
- Add `[SceneComponent]` generation.
- Add scope-owner aggregation.
- Add `RegisterGeneratedFor<TScopeMarker>()`.
- Fix deduplication.

## Required reading

Before starting:

```txt
AGENTS.md
.codex/rules/02-vcontainer-generator.md
.codex/rules/03-scope-marker-pattern.md
.codex/rules/06-testing.md
```

## Inputs

The task should specify:

```txt
Feature name
Target attribute(s)
Expected generated code
Affected model types
Affected emitter(s)
Affected tests
Backward compatibility concerns
```

## Output

A completed generator feature should include:

```txt
- Updated attribute extraction if needed.
- Updated ServiceInfo / model if needed.
- Updated emitter.
- Updated diagnostics if misuse is possible.
- Generator tests.
- Documentation example.
```

## Workflow

1. Add or update a generator test.
2. Define expected generated registration output.
3. Update syntax/semantic extraction.
4. Update common model if needed.
5. Update registration emitter.
6. Add or update diagnostics.
7. Run generator tests.
8. Update docs.

## Required checks

Run when possible:

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release --filter Generator
```

## Guardrails

Do not:

- Hard-code `ProjectLifetimeScope`, `GameplayLifetimeScope`, or `MainMenuLifetimeScope`.
- Emit unreadable runtime reflection code for normal registrations.
- Deduplicate only by class name.
- Require lower layers to reference Composition assemblies.
- Crash when optional packages are missing.

## Example

Input:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPhaseStateMachine>]
public sealed class PhaseStateMachine : IPhaseStateMachine
{
}
```

Expected generated intent:

```csharp
builder.Register<PhaseStateMachine>(Lifetime.Scoped)
    .As<IPhaseStateMachine>();
```
