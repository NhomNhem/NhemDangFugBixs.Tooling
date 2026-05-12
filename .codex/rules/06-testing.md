# 06 — Testing Rules

## General rule

Every generator/analyzer behavior should have tests.

Do not add public attributes or diagnostics without tests.

## Required test categories

### Generator tests

Test:

- Basic `[AutoRegisterIn]`.
- Generic `[AutoRegisterIn<TScope>]`.
- `[LifetimeScopeFor]` marker mapping.
- Cross-assembly service discovery.
- Scope-owner aggregation.
- Safe deduplication.
- `[As<T>]` and `[BindAs]`.
- `[EntryPoint]` generation.
- `[SceneComponent]` generation.
- `RegisterInHierarchy` legacy option.
- Build callback generation.
- Installer ordering.
- MessagePipe broker generation if supported.

### Analyzer tests

Test:

- Missing scope mapping.
- Duplicate scope mapping.
- Public `[Inject]` field.
- MonoBehaviour constructor injection.
- Async `[Inject]` method.
- Too many constructor dependencies.
- Singleton depends on scoped.
- `IObjectResolver` misuse.
- Public R3 `Subject<T>`.

### CLI tests

Test:

- `di-smoke preflight`.
- `di-smoke graph`.
- `di-smoke report`.
- JSON output stability.
- Markdown output stability.

### Unity package tests

Test:

- Runtime asmdef compiles.
- Editor asmdef compiles.
- Samples import.
- Editor diagnostics window opens.

## Commands

Default validation:

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```

Run more targeted tests when appropriate.

## Test style

Use small focused source snippets for generator/analyzer tests.
Keep expected generated output readable.
Prefer snapshot tests only when output is stable and intentional.
