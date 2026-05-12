# Codex Rules for NhemDangFugBixs.Tooling

This document contains the recommended Codex/agent rule files for the package repository.

Recommended files:

```txt
<repo-root>
├── AGENTS.md
└── .codex
    └── rules
        ├── 00-project.md
        ├── 01-architecture.md
        ├── 02-vcontainer-generator.md
        ├── 03-scope-marker-pattern.md
        ├── 04-analyzers.md
        ├── 05-unity-package.md
        ├── 06-testing.md
        ├── 07-documentation.md
        ├── 08-release.md
        └── 09-do-not.md
```

---

# File: AGENTS.md

````md
# AGENTS.md — NhemDangFugBixs.Tooling

## Purpose

This repository builds `NhemDangFugBixs.Tooling`, a Unity package that improves VContainer workflows through:

- Source-generated VContainer registrations.
- Roslyn analyzers and code fixes.
- Scope marker mapping across Unity asmdef/layer boundaries.
- CLI and Editor diagnostics.
- AI-friendly DI reports and dependency graphs.

The package must remain reusable outside a single game project.

## Read these rules first

Before changing code, read the rule files in:

```txt
.codex/rules/
````

Suggested reading order:

1. `.codex/rules/00-project.md`
2. `.codex/rules/01-architecture.md`
3. `.codex/rules/02-vcontainer-generator.md`
4. `.codex/rules/03-scope-marker-pattern.md`
5. `.codex/rules/04-analyzers.md`
6. `.codex/rules/05-unity-package.md`
7. `.codex/rules/06-testing.md`
8. `.codex/rules/07-documentation.md`
9. `.codex/rules/08-release.md`
10. `.codex/rules/09-do-not.md`

## Core principle

Do not force one project architecture.

The package should provide generic primitives:

* `[AutoRegisterIn]`
* `[AutoRegisterIn<TScope>]`
* `[LifetimeScopeFor]`
* `[As<T>]`
* `[BindAs]`
* `[EntryPoint]`
* `[SceneComponent]`
* Scope marker mapping
* Diagnostics
* Generated reports

Convenience presets like `[ProjectService]`, `[GameplayService]`, and `[MainMenuService]` may exist, but they must be optional syntax sugar, not hard-coded foundations.

## Work style

When implementing a feature:

1. Update or add tests first when possible.
2. Keep public API small and explicit.
3. Avoid breaking existing attributes unless there is a migration path.
4. Prefer compile-time safety over runtime magic.
5. Generate readable VContainer code.
6. Keep Unity Runtime assembly lightweight.
7. Keep Editor-only code out of Runtime.
8. Document new public attributes or diagnostics.

## Validation

Before finishing a task, run relevant checks when available:

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```

If Unity-specific code changed, also validate package import in Unity when possible.

## Commit expectation

If running inside a Codex environment that requires commits, leave the worktree clean and commit the completed change.

## Safety

Do not introduce runtime service locator patterns.
Do not add hidden dependencies on a specific game project.
Do not make Application/Domain layers depend on Unity Composition assemblies.

````

---

# File: .codex/rules/00-project.md

```md
# 00 — Project Rules

## Package identity

This repository is for a reusable Unity package:

```txt
NhemDangFugBixs.Tooling
````

Package goal:

```txt
A compile-time VContainer workflow toolkit for Unity.
```

It provides:

* Attribute-driven registration.
* Scope marker architecture.
* Source-generated installers.
* DI analyzers.
* CLI preflight.
* Unity diagnostics window.
* AI-friendly dependency reports.

## Main users

* Unity developers using VContainer.
* Indie teams with asmdef-separated architecture.
* Developers using AI coding agents.
* Projects that want compile-time DI validation.

## Non-goals

Do not turn this package into:

* A replacement for VContainer.
* A runtime service locator.
* A Solar Phobia-only helper.
* A package that forces fixed scope names.
* A package that auto-registers everything without explicit opt-in.

## Design principle

The package should not decide the user's architecture.
It should make the user's chosen architecture safer.

Use this wording when in doubt:

```txt
NhemDangFugBixs.Tooling does not force your architecture.
It lets your architecture become compile-time checked.
```

## Public API stability

Public attributes and diagnostics should be treated as API.
Before renaming or removing an attribute:

1. Add backward compatibility if possible.
2. Add an analyzer warning if migration is needed.
3. Document the migration path.
4. Update samples and docs.

````

---

# File: .codex/rules/01-architecture.md

```md
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
````

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

* Public attributes.
* Marker interfaces/types.
* Small runtime models.
* Optional runtime helpers.

It should not contain:

* Generator implementation.
* Analyzer implementation.
* CLI logic.
* UnityEditor code.
* Heavy reflection-based runtime registration.

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

````

---

# File: .codex/rules/02-vcontainer-generator.md

```md
# 02 — VContainer Generator Rules

## Purpose

The source generator reads attributes and emits VContainer registration code.

Main attributes:

```csharp
[AutoRegisterIn]
[AutoRegisterIn<TScope>]
[LifetimeScopeFor]
[As<T>]
[BindAs]
[AsSelf]
[AsImplementedInterfaces]
[EntryPoint]
[SceneComponent]
````

## Required behavior

The generator must:

1. Discover attributed services in the current compilation.
2. Discover attributed services in referenced assemblies when needed.
3. Resolve scope marker mappings.
4. Emit registration methods per scope marker.
5. Deduplicate safely.
6. Report diagnostics for invalid mappings.
7. Generate readable and deterministic code.

## Scope-owner aggregation

When a LifetimeScope maps a marker:

```csharp
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope
{
}
```

that scope owner should aggregate services from referenced assemblies using:

```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
```

Do not only include services whose declaring assembly equals the current assembly.

Correct conceptual filter:

```csharp
var mappedIdentityTypes = scopeMappings
    .Select(m => m.IdentityTypeName)
    .ToHashSet();

var servicesForThisScopeOwner = discoveredServices
    .Where(s => s.ScopeTypeName != null &&
                mappedIdentityTypes.Contains(s.ScopeTypeName));
```

## Deduplication

Do not deduplicate only by service full name.

Use a key like:

```txt
ServiceFullName + ScopeTypeName + Lifetime + Contracts
```

Reason: one service may intentionally register into multiple scopes or expose different contracts.

## Generated API

Preferred generated API:

```csharp
builder.RegisterGeneratedFor<IGameplayScope>();
```

Fallback generated API:

```csharp
builder.RegisterGeneratedForIGameplayScope();
```

## Emission order

Recommended order:

1. Manual installers with negative order.
2. Generated normal services.
3. Generated components.
4. MessagePipe brokers.
5. Entry points.
6. Build callbacks.
7. Manual post-installers if supported.

## Entry points

If a type is marked `[EntryPoint]`, emit:

```csharp
builder.RegisterEntryPoint<T>(lifetime);
```

Do not emit normal `builder.Register<T>()` for entry points unless explicitly required by a separate attribute.

## Components

For `[SceneComponent<TScope>]`, emit:

```csharp
builder.RegisterComponentInHierarchy<T>();
```

For legacy:

```csharp
[AutoRegisterIn(typeof(IGameplayScope), RegisterInHierarchy = true)]
```

preserve existing behavior.

## MessagePipe

MessagePipe support must be optional and resilient.
If MessagePipe is not referenced, the generator should not crash.

## Failure handling

Generator failures should report diagnostics when possible.
Do not silently swallow important errors that prevent registration generation.

````

---

# File: .codex/rules/03-scope-marker-pattern.md

```md
# 03 — Scope Marker Pattern Rules

## Purpose

Scope markers allow services in lower layers to declare their target VContainer scope without referencing the real Unity `LifetimeScope` class.

## Correct pattern

Shared assembly:

```csharp
namespace MyGame.Shared.Composition;

public interface IScopeMarker { }
public interface IProjectScope : IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface IMainMenuScope : IScopeMarker { }
````

Application assembly:

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IPhaseStateMachine>]
public sealed class PhaseStateMachine : IPhaseStateMachine
{
}
```

Composition assembly:

```csharp
[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

## Dependency direction

Allowed:

```txt
Application    -> Shared
Infrastructure -> Shared
Presentation   -> Shared + Application
Composition    -> Shared + Application + Infrastructure + Presentation
```

Forbidden:

```txt
Application -> Composition
Domain      -> Composition
Shared      -> Composition
```

## Marker type choice

Both marker interfaces and marker classes are valid.

Recommended:

```txt
Project architecture markers: interfaces
Sample/test identity markers: classes
```

Examples:

```csharp
public interface IGameplayScope : IScopeMarker { }
```

or:

```csharp
public sealed class GameplayScopeId { }
```

## Diagnostics required

The analyzer/generator should report:

```txt
MissingScopeMapping
- A service uses [AutoRegisterIn(typeof(IMarker))]
- No [LifetimeScopeFor(typeof(IMarker))] exists

DuplicateScopeMapping
- The same marker maps to multiple LifetimeScopes

InvalidMarkerLayer
- Optional warning if marker is declared in Composition instead of Shared/Abstractions
```

## Do not hard-code presets

Do not make `[GameplayService]` require a hard-coded `GameplayLifetimeScope` type.

If presets exist, they must map through alias/config:

```txt
GameplayService = AutoRegisterInScope("Gameplay") + Scoped
```

and `Gameplay` must resolve to a marker or scope mapping.

````

---

# File: .codex/rules/04-analyzers.md

```md
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
````

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

* Convert public field injection to private Construct injection.
* Add `[EntryPoint]` to lifecycle service.
* Add `[As<T>]` for implemented interface.
* Add explicit `Lifetime = NhemLifetime.Scoped` to gameplay service.
* Generate missing scope marker mapping stub when safe.

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

````

---

# File: .codex/rules/05-unity-package.md

```md
# 05 — Unity Package Rules

## Package layout

Follow Unity package layout:

```txt
package.json
README.md
CHANGELOG.md
LICENSE.md
Third Party Notices.md
Runtime/
Editor/
Tests/
Samples~/
Documentation~/
Source~/
````

## Runtime folder

Runtime may contain:

* Attributes.
* Lightweight models.
* Runtime helpers.
* Runtime asmdef.

Runtime must not contain:

* UnityEditor code.
* Roslyn generator implementation.
* CLI implementation.
* Editor windows.

## Editor folder

Editor may contain:

* Diagnostics window.
* Menu items.
* UnityEditor integrations.
* Editor asmdef.

Editor must not be referenced by Runtime.

## Analyzers folder

The Unity package should include prebuilt analyzer/generator DLLs when needed.

Do not place full source generator project directly in Runtime.

## Source~ folder

`Source~` is for development source:

* Generator projects.
* Analyzer projects.
* CLI projects.
* Tests.
* Benchmarks.

Unity should ignore this folder during package import.

## Samples~ folder

Samples should be importable through Unity Package Manager.

Required samples:

```txt
BasicAutoRegister
ScopeMarkerArchitecture
MessagePipeIntegration
SceneComponents
SolarPhobiaStyleArchitecture
```

## Documentation~ folder

Documentation should include:

```txt
index.md
getting-started.md
scope-marker-pattern.md
attributes.md
diagnostics.md
cli.md
editor-window.md
troubleshooting.md
migration-guide.md
```

## package.json

Ensure `package.json` includes:

* name
* version
* displayName
* description
* unity
* author
* documentationUrl if available
* changelogUrl if available
* licensesUrl if available
* dependencies where appropriate

VContainer should usually be treated as a peer dependency documented in README unless the package intentionally depends on it.

````

---

# File: .codex/rules/06-testing.md

```md
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
````

Run more targeted tests when appropriate.

## Test style

Use small focused source snippets for generator/analyzer tests.
Keep expected generated output readable.
Prefer snapshot tests only when output is stable and intentional.

````

---

# File: .codex/rules/07-documentation.md

```md
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
````

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

````

---

# File: .codex/rules/08-release.md

```md
# 08 — Release Rules

## Versioning

Use SemVer:

```txt
MAJOR.MINOR.PATCH
````

Recommended stages:

```txt
0.1.0 — experimental core generator
0.2.0 — analyzer rules
0.3.0 — CLI report
0.4.0 — Unity Editor diagnostics
1.0.0 — stable public API
```

## Release checklist

Before release:

1. Update `package.json` version.
2. Update `CHANGELOG.md`.
3. Update documentation.
4. Run build.
5. Run tests.
6. Validate Unity package import.
7. Validate samples.
8. Build deploy branch contents.
9. Tag release as `vX.Y.Z`.
10. Publish GitHub release.
11. Optional: publish/check OpenUPM readiness.

## Deploy branch

The `deploy` branch should be the minimal Unity Package Manager import surface.

It should contain:

```txt
package.json
README.md
CHANGELOG.md
LICENSE.md
Third Party Notices.md
Runtime/
Editor/
Analyzers/
Tests/
Samples~/
Documentation~/
```

It should not contain unnecessary development-only files unless intentionally included.

## Breaking changes

Breaking changes require:

* CHANGELOG entry.
* Migration guide.
* Analyzer/code fix if possible.
* Major version bump after 1.0.0.

````

---

# File: .codex/rules/09-do-not.md

```md
# 09 — Do Not Rules

## Do not hard-code one game's architecture

Do not assume every project has:

```txt
ProjectLifetimeScope
GameplayLifetimeScope
MainMenuLifetimeScope
````

These may exist as samples or optional presets only.

## Do not force presets

`[ProjectService]`, `[GameplayService]`, and `[MainMenuService]` must be optional syntax sugar.

Core API must remain:

```csharp
[AutoRegisterIn(typeof(SomeScopeOrMarker), Lifetime = NhemLifetime.Scoped)]
```

## Do not break layer boundaries

Do not require Application/Domain services to reference Composition LifetimeScope classes.

Use scope markers instead.

## Do not abuse IObjectResolver

Do not encourage this pattern:

```csharp
public sealed class SomeService
{
    private readonly IObjectResolver _resolver;

    public void DoSomething()
    {
        var service = _resolver.Resolve<IService>();
    }
}
```

Only allow `IObjectResolver` in:

* Factory
* Spawner
* Bootstrapper
* LifetimeScope
* Explicitly configured exceptions

## Do not hide generated behavior

Generated code should be understandable.
Avoid runtime reflection magic for normal registration.

## Do not put Editor code in Runtime

No `UnityEditor` references in Runtime.

## Do not make MessagePipe/R3 mandatory

MessagePipe and R3 support should be optional/configurable.
The package should still work without them.

## Do not auto-register everything by convention alone

Registration should be explicit opt-in through attributes or configuration.

## Do not swallow important generator errors

If generation fails in a way that affects output, report a diagnostic.

## Do not create noisy analyzers

Avoid warnings on unrelated code.
Analyze only DI-related types, configured namespaces, or types with relevant attributes.

```
```
