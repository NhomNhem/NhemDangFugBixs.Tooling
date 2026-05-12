# 04 — Analyzer Rules

## Purpose

Analyzers should catch VContainer workflow and architecture mistakes before Play Mode.

## Required diagnostics

### Scope mapping

```txt
NDFG010 Error — Missing Scope Mapping
NDFG011 Error — Duplicate Scope Mapping
NDFG012 Error — Missing Scope Alias
NDFG013 Info  — Marker Declared In Composition Assembly
```

### Injection style

```txt
NDF020 Error   — MonoBehaviour constructor injection
NDF021 Error   — [Inject] public field
NDF022 Warning — [Inject] method is public
NDF023 Error   — [Inject] async method
NDF024 Info    — Method named Constructs instead of Construct
```

### Lifetime safety

```txt
NDF030 Error   — Singleton depends on Scoped service
NDF031 Warning — Runtime namespace registered as Singleton
NDF032 Warning — Disposable Transient service
NDF033 Warning — Gameplay service omitted explicit Lifetime
```

### EntryPoint

```txt
NDF040 Warning — Implements lifecycle interface but missing [EntryPoint]
NDF041 Error   — [EntryPoint] class implements no lifecycle interface
NDF042 Error   — Multiple IEntryPointExceptionHandler in same scope
```

### Architecture

```txt
NDF050 Warning — Too many constructor dependencies
NDF051 Warning — Too many MonoBehaviour injected dependencies
NDF052 Warning — IObjectResolver used outside factory/spawner/bootstrapper
NDF053 Warning — Service depends directly on LifetimeScope
NDF054 Warning — Application layer references Composition namespace
```

### R3 / event stream

```txt
NDF070 Error   — Public Subject<T>
NDF071 Warning — Subject owner does not implement IDisposable
```

## Severity rules

Use Error when code is definitely wrong or will generate invalid output.
Use Warning for likely architecture mistakes.
Use Info for naming/style suggestions.

## Code fix rules

Prefer code fixes for common problems:

- Convert public field injection to private Construct injection.
- Add `[EntryPoint]` to lifecycle service.
- Add `[As<T>]` for implemented interface.
- Add explicit `Lifetime = NhemLifetime.Scoped` to gameplay service.
- Generate missing scope marker mapping stub when safe.

## Analyzer config

Rules should be configurable through `.nhem-di.json` when possible.

Example:

```json
{
  "rules": {
    "maxConstructorDependencies": 6,
    "maxMonoBehaviourDependencies": 4,
    "forbidPublicFieldInjection": true,
    "forbidMonoBehaviourConstructorInjection": true,
    "warnRuntimeSingleton": true
  }
}
```

## Do not overreach

Do not report noisy diagnostics for code outside the package's concern.
Do not require every class to use `[AutoRegisterIn]`.
Only analyze DI-related classes or configured namespaces.
