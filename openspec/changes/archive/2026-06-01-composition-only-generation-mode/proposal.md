# Proposal: Composition-Only Generation Mode

## Summary

Move `NhemDangFugBixs.VContainer.SourceGenerator` from per-assembly VContainer code emission to composition-only generation mode.

In the new canonical model, service assemblies declare DI intent with attributes such as `[AutoRegisterIn]`, `[As]`, `[EntryPoint]`, and `[RegisterComponentInHierarchy]`, but they do not receive generated VContainer registration code and do not need to reference `VContainer`.

Assemblies that contain `[LifetimeScopeFor<TScope>]` become composition targets. Only composition targets emit:

- `NhemGeneratedVContainerExtensions.RegisterGeneratedFor<TScope>()`
- generated static installers per scope marker
- VContainer registration code that binds discovered services into `IContainerBuilder`

For the MVP, composition targets discover registrations from:

1. the current compilation
2. directly referenced assemblies

This keeps VContainer ownership in composition while preserving attribute-driven DI intent in lower-level assemblies.

## Problem Statement

The current generator emits `.g.cs` files into the same compilation that contains `[AutoRegisterIn]` services. Those generated files reference VContainer APIs such as:

- `IContainerBuilder`
- `Lifetime`
- `Register<T>()`
- `RegisterEntryPoint<T>()`
- `RegisterComponentInHierarchy<T>()`

That behavior forces low-level gameplay, application, and infrastructure assemblies to reference `VContainer` even when they only declare DI intent and should remain composition-agnostic.

This creates several problems:

- low-level asmdefs take on an unnecessary DI framework dependency
- architectural boundaries are weakened because service assemblies must know about VContainer indirectly through generated output
- real Unity projects with separate asmdefs fail to compile when service asmdefs do not reference VContainer
- the generated output location no longer matches the ownership model that senior Unity teams expect: services define intent, composition owns container wiring

## Goals

- Emit VContainer registration code only into composition target assemblies.
- Let service-only assemblies compile without a VContainer reference.
- Preserve local Roslyn validation in service-only assemblies.
- Discover local and referenced `[AutoRegisterIn]` registrations from composition targets.
- Emit one stateless installer per scope marker into the composition target assembly.
- Keep `RegisterGeneratedFor<TScope>()` as composition-side DX sugar only.
- Avoid duplicate discovered registration within a composition target.
- Keep generated code readable and equivalent to hand-written VContainer registration.
- Keep runtime generated code reflection-free and stateless.
- Keep discovery bounded to the current compilation and directly referenced assemblies.
- Keep generator work incremental, deterministic, and scalable for larger service counts.
- Update Unity sample and release gate to validate separated asmdefs.

## Non-Goals

This change does not implement or expand:

- `RegisterInstance`
- `ManualInstallerFor`
- MessagePipe integration
- pooling integration
- Addressables integration
- prefab or new-GameObject factory generation
- generated `LifetimeScope` MonoBehaviours
- partial injection into user `LifetimeScope` classes
- transitive assembly discovery beyond direct references for the MVP
- scanning all loaded Unity assemblies for registrations
- project-wide duplicate validation purely in Roslyn where compilation boundaries prevent reliable detection

## User-Facing Outcome

### Before

A gameplay assembly that contains only this service:

```csharp
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IPlayerLocomotionService))]
public sealed class PlayerLocomotionService : IPlayerLocomotionService {}
```

still receives generated `.g.cs` files that reference `VContainer`, forcing the gameplay asmdef to reference `VContainer`.

### After

The gameplay assembly keeps only attribute intent and local validation.

A composition assembly that contains:

```csharp
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

becomes the only place where generated VContainer code is emitted.

That generated installer can then register both local composition services and services discovered from referenced assemblies, for example:

```csharp
builder.Register<GlassRefrain.Locomotion.PlayerLocomotionService>(Lifetime.Scoped)
    .As<GlassRefrain.Locomotion.IPlayerLocomotionService>();
```

## Performance Requirements

1. Runtime generated code must remain reflection-free and stateless.
2. No generated code may call `Resolve<T>()` during registration.
3. Composition-only discovery must not scan all loaded Unity assemblies.
4. MVP discovery should inspect only the current compilation and directly referenced assemblies.
5. The generator must use incremental generator patterns and avoid semantic analysis for non-candidate syntax nodes.
6. Duplicate detection must use dictionary or grouping strategies, not O(n²) scans.
7. Generated output must be deterministic and sorted.
8. Add generator performance smoke tests for 10, 100, and 500 services.
9. Release gate should report Unity sample compile duration.

## Scope of Change

### Generator

- detect whether the current compilation is a composition target by scanning for `[LifetimeScopeFor]`
- emit no VContainer registration code when no composition target exists in the compilation
- discover registration metadata from the current compilation and directly referenced assemblies when a composition target exists
- deduplicate discovered registrations deterministically
- emit installers and `RegisterGeneratedFor<TScope>()` only into composition target assemblies
- preserve incremental performance patterns and deterministic ordering

### Analyzer

- keep local correctness diagnostics in service-only assemblies
- stop treating missing `LifetimeScopeFor` in a service-only assembly as a local error
- add composition-target diagnostics for visibility/reference issues and duplicate target issues where detectable
- move broader cross-asmdef checks toward `di-smoke`

### Release Gate

- restore the Unity sample to separated asmdefs
- verify that service asmdefs compile without a VContainer reference
- verify that composition asmdef compiles with VContainer and generated installers
- report Unity sample compile duration when Unity compile is executed

## Risks

- Roslyn metadata discovery across referenced assemblies must be modeled carefully to avoid over-discovery or unstable ordering.
- Direct-reference MVP may surprise users expecting transitive discovery.
- Legacy projects that relied on per-assembly emission may need migration guidance or a temporary compatibility switch.
- Some duplicate-composition scenarios cannot be proven inside one compilation and need `di-smoke` coverage.
- Performance regressions can hide inside referenced-assembly discovery if candidate filtering is too loose.

## Migration Strategy

- Treat composition-only generation as the canonical path.
- Consider current per-assembly generation legacy behavior.
- Provide migration guidance for teams moving from single-asmdef or prototype setups to separated service/composition asmdefs.
- Update docs and Unity sample to demonstrate the new boundary clearly.

## Acceptance Criteria

- Service-only assemblies with `[AutoRegisterIn]` compile without referencing VContainer.
- Service-only assemblies do not emit VContainer installers or `RegisterGeneratedFor<TScope>()`.
- Service-only assemblies still report local attribute correctness diagnostics.
- Composition assemblies emit `RegisterGeneratedFor<TScope>()` and one installer per scope marker.
- Composition assemblies discover services from the current compilation and directly referenced assemblies.
- Unreferenced service assemblies are not discovered.
- Generated installers remain stateless and never call `Resolve<T>()` during registration.
- Discovery remains bounded to the current compilation and directly referenced assemblies.
- Generated output is deterministic and sorted.
- Generator performance smoke tests cover 10, 100, and 500 services.
- Unity sample compile gate passes with separate `Shared`, `Gameplay/Locomotion`, and `Composition` asmdefs and reports compile duration.
