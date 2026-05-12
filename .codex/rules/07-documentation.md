# 07 — Documentation Rules

## Documentation is part of the feature

When adding a public feature, update documentation.

Examples of public features:

- New attribute.
- New analyzer diagnostic.
- New CLI command.
- New generated API.
- New Unity Editor workflow.

## Required docs updates

For new attributes, update:

```txt
Documentation~/attributes.md
README.md if it is core usage
Samples~ if useful
```

For new diagnostics, update:

```txt
Documentation~/diagnostics.md
```

For new CLI commands, update:

```txt
Documentation~/cli.md
```

For scope marker changes, update:

```txt
Documentation~/scope-marker-pattern.md
```

## Documentation style

Use practical examples.
Show both input and generated intent.

Example:

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

## Avoid misleading claims

Do not claim the package replaces VContainer.
Use this phrasing:

```txt
This package improves VContainer workflow. It does not replace VContainer.
```
