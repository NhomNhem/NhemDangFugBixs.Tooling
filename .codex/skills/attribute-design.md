# Skill — Attribute Design

## Purpose

Use this skill when adding or changing public attributes.

Examples:

- `[As<T>]`
- `[BindAs]`
- `[EntryPoint]`
- `[SceneComponent<TScope>]`
- `[Factory]`
- `[Keyed]`
- `[ContributesToCollection]`
- `[AutoRegisterInScope]`

## Required reading

```txt
AGENTS.md
.codex/rules/00-project.md
.codex/rules/01-architecture.md
.codex/rules/03-scope-marker-pattern.md
.codex/rules/07-documentation.md
```

## Design checklist

Before adding an attribute, answer:

```txt
1. What VContainer behavior does it express?
2. Is it generic or project-specific?
3. Does it work across asmdef boundaries?
4. Does it require VContainer as a runtime dependency?
5. Can misuse be diagnosed?
6. What generated code should it produce?
7. Is there a simpler existing attribute that already solves this?
```

## Output

A completed attribute addition should include:

```txt
- Attribute class in Runtime/Attributes or Attributes project.
- Generator extraction support.
- Model update if needed.
- Emitter update.
- Analyzer misuse rules if needed.
- Generator/analyzer tests.
- Documentation.
- Sample if user-facing.
```

## API rules

Attributes should be:

```txt
- Small.
- Explicit.
- Stable.
- Architecture-neutral.
- Safe across asmdef boundaries.
```

Prefer:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPhaseStateMachine>]
```

Avoid attributes that force one architecture:

```csharp
[GameplayService]
```

unless they are optional alias-based syntax sugar.

## Backward compatibility

If replacing an existing attribute/property:

1. Keep the old API if possible.
2. Add analyzer warning for migration.
3. Update docs.
4. Update CHANGELOG.
