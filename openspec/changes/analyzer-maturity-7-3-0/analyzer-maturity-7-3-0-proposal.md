# Proposal: Analyzer Maturity (7.3.0)

## Context

Versions 7.2.1 and 7.2.2 stabilized the explicit registration API and package import diagnostics.

Current product principle:
- Services declare intent.
- Composition owns registration.
- Diagnostics protect architecture.

## Goal

Improve analyzer feedback for common misuse of explicit registration attributes introduced around 7.2.x.

## Scope

Implement only the following diagnostics in 7.3.0:

### 1. NHEM_DI_061 — Duplicate explicit contract exposure

**Trigger:**
A type declares duplicate `[As(...)]` attributes for the same contract type.

**Example:**
```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IPlayerView))]
[As(typeof(IPlayerView))]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

**Severity:** Warning

**Message:**
Duplicate contract exposure. Remove duplicate [As] declaration for the same contract.

**Requirements:**
- Compare contract symbols semantically where possible.
- Do not warn for different contracts.
- Do not warn for legacy AutoRegisterIn.AsImplementedInterfaces behavior.
- Do not duplicate existing NHEM_DI_060 mixed-style warning.

### 2. NHEM_DI_066 — RegisterComponentInHierarchy on non-MonoBehaviour

**Trigger:**
A type uses `[RegisterComponentInHierarchy]` but does not inherit from `UnityEngine.MonoBehaviour`.

**Example:**
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class PlayerViewService
{
}
```

**Severity:** Error or Warning. Prefer Error if the generator would produce invalid VContainer component registration.

**Message:**
RegisterComponentInHierarchy can only be used on MonoBehaviour types.

**Requirements:**
- Use semantic inheritance checks.
- Handle missing UnityEngine references gracefully.
- Do not crash analyzer if MonoBehaviour symbol cannot be resolved.
- Do not warn for valid MonoBehaviour subclasses.

### 3. NHEM_DI_067 — EntryPoint without known lifecycle contract

**Trigger:**
A type uses `[EntryPoint]` but does not implement a known VContainer lifecycle interface.

**Known lifecycle interfaces:**
- VContainer.Unity.IInitializable
- VContainer.Unity.IStartable
- VContainer.Unity.IPostInitializable
- VContainer.Unity.ITickable
- VContainer.Unity.IFixedTickable
- VContainer.Unity.ILateTickable
- System.IDisposable
- System.IAsyncDisposable if supported by target framework

**Example:**
```csharp
[EntryPoint]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
public sealed class GameplayLoopEntryPoint
{
}
```

**Severity:** Warning

**Message:**
EntryPoint should implement a known lifecycle interface such as IStartable, ITickable, IInitializable, or IDisposable.

**Requirements:**
- Use semantic interface checks.
- Handle missing VContainer.Unity references gracefully.
- Do not force service-only assemblies to reference VContainer.Unity.
- Only analyze types that actually use [EntryPoint].
- Do not warn if the type implements one known lifecycle interface.

## Non-goals

- Do not implement project-level asmdef validation in 7.3.0.
- Do not implement di-smoke.
- Do not inspect all Unity assemblies.
- Do not add RegisterInstance generation.
- Do not add Addressables, prefab factory, pooling, or MessagePipe automation.
- Do not change generator registration behavior unless needed to avoid invalid output from diagnostics.
- Do not break 7.2.x compatibility.

## Documentation

Add diagnostics documentation for NHEM_DI_061, NHEM_DI_066, NHEM_DI_067:
- Include bad/good examples.
- Explain that some project-level validations are deferred to 7.4.0 di-smoke.

## Tests

Add analyzer tests:
- NHEM_DI_061 emitted for duplicate [As] same contract.
- NHEM_DI_061 not emitted for [As(typeof(IFoo))] + [As(typeof(IBar))].
- NHEM_DI_066 emitted for [RegisterComponentInHierarchy] on non-MonoBehaviour.
- NHEM_DI_066 not emitted for MonoBehaviour subclass.
- NHEM_DI_067 emitted for [EntryPoint] without lifecycle interface.
- NHEM_DI_067 not emitted for IStartable.
- NHEM_DI_067 not emitted for ITickable.
- NHEM_DI_067 not emitted for IDisposable.
- Existing analyzer tests still pass.

## Version

- Bump package.json to 7.3.0.
- Bump DangFugBixs.Generators.csproj to 7.3.0 if version drift policy requires it.
- Update CHANGELOG.md.
- Rebuild shipped analyzer/runtime DLL payloads if changed.

## Release Gate

- Generator tests PASS.
- Analyzer tests PASS.
- DiSmokeValidation tests PASS.
- Version drift PASS.
- Docs check PASS.
- Unity sample dotnet build PASS.
- Unity sample compile should be attempted but can be marked SKIPPED/INCONCLUSIVE only if Unity Editor hangs again, with transparent release note.
