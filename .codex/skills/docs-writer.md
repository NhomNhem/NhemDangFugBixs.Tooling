# Skill — Docs Writer

## Purpose

Use this skill when writing or updating package documentation.

## Required reading

```txt
AGENTS.md
.codex/rules/07-documentation.md
```

## Docs locations

```txt
README.md
Documentation~/
Samples~/
CHANGELOG.md
```

## Documentation style

Use practical examples.

Each feature should show:

1. Problem.
2. Recommended attribute usage.
3. Generated intent.
4. Common mistakes.
5. Migration notes if needed.

## Required phrasing

Use:

```txt
This package improves VContainer workflow. It does not replace VContainer.
```

Use:

```txt
NhemDangFugBixs.Tooling does not force your architecture.
It lets your architecture become compile-time checked.
```

## Avoid

Do not claim:

- The package replaces VContainer.
- Users must use Project/Gameplay/MainMenu scope names.
- MessagePipe or R3 is required.
- Runtime reflection is the main registration strategy.

## Example format

````md
## Example

Input:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPhaseStateMachine>]
public sealed class PhaseStateMachine : IPhaseStateMachine
{
}
```

Generated intent:

```csharp
builder.Register<PhaseStateMachine>(Lifetime.Scoped)
    .As<IPhaseStateMachine>();
```
````
