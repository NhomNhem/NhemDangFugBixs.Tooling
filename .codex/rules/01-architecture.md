# 01 — Architecture Rules

## Repository layers

Expected source project layout:

```txt
Source~/
├── NhemDangFugBixs.Attributes
├── NhemDangFugBixs.Common
├── NhemDangFugBixs.Generators
├── NhemDangFugBixs.Analyzers
├── NhemDangFugBixs.Cli
├── NhemDangFugBixs.Generators.Tests
├── NhemDangFugBixs.Analyzers.Tests
├── NhemDangFugBixs.Cli.Tests
└── NhemDangFugBixs.Benchmarks
```

Expected Unity package layout:

```txt
Runtime/
Editor/
Analyzers/
Tests/
Samples~/
Documentation~/
```

## Dependency direction

Allowed:

```txt
Generators -> Common
Analyzers  -> Common
Cli        -> Common
Runtime    -> Attributes / lightweight runtime models
Editor     -> Runtime + UnityEditor APIs
```

Avoid:

```txt
Runtime -> Editor
Runtime -> UnityEditor
Attributes -> VContainer, UnityEditor, heavy dependencies
Application layer samples -> Composition layer
```

## Runtime assembly rule

The Unity Runtime assembly should stay lightweight.
It may contain:

- Public attributes.
- Marker interfaces/types.
- Small runtime models.
- Optional runtime helpers.

It should not contain:

- Generator implementation.
- Analyzer implementation.
- CLI logic.
- UnityEditor code.
- Heavy reflection-based runtime registration.

## Generator rule

Generated code should look like normal VContainer code.

Prefer output like:

```csharp
builder.Register<PhaseStateMachine>(Lifetime.Scoped)
    .As<IPhaseStateMachine>();
```

Avoid generated code that relies on broad runtime reflection.

## Analyzer rule

Analyzers should protect architecture, not annoy users.

Use severity carefully:

```txt
Error   — dangerous or definitely invalid
Warning — likely architecture mistake
Info    — style/convention guidance
```
