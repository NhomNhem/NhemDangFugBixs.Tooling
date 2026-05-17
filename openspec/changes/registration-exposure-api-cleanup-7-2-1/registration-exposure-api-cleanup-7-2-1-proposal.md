# Proposal: Registration Exposure API Cleanup 7.2.1

## Change Name
`registration-exposure-api-cleanup-7-2-1`

## Version
7.2.1

## Context

Version 7.2.0 introduced composition-only generation mode for VContainer registration. The canonical architecture is:

- Services declare DI intent
- Composition owns VContainer registration
- Diagnostics protect architecture

## Problem Statement

The registration exposure API has duplicate responsibilities.

`AutoRegisterIn` currently supports legacy exposure flags:
- `AsImplementedInterfaces`
- `AsSelf`

But the package also supports explicit exposure attributes:
- `[As(typeof(TContract))]`
- `[AsSelf]`

This creates confusing mixed usage such as:

```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = false, AsSelf = false)]
[As(typeof(IPlayerView))]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

Users are unclear which mechanism to use, and the flags can conflict with explicit attributes.

## Desired Canonical API

Separation of concerns:

- **AutoRegisterIn**: Declares only scope and lifetime
- **As / AsSelf**: Declare contract exposure
- **EntryPoint**: Declares VContainer lifecycle registration
- **RegisterComponentInHierarchy**: Declares Unity component registration kind

## Canonical Example

```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IPlayerView))]
public sealed class PlayerView : MonoBehaviour, IPlayerView
{
}
```

## Behavior Requirements

1. **Explicit [As(...)] and [AsSelf] are canonical**: These should be the preferred way to declare contract exposure.

2. **Explicit attributes take precedence**: If a type has explicit `[As(...)]` or `[AsSelf]`, the generator must use only those explicit exposure attributes.

3. **No duplicate exposure**: If explicit exposure attributes exist, legacy `AutoRegisterIn.AsImplementedInterfaces` and `AutoRegisterIn.AsSelf` flags must not create duplicate generated exposure.

4. **Backward compatibility**: If a type has no explicit `[As(...)]` or `[AsSelf]`, preserve legacy flag-only behavior for backward compatibility.

5. **Prevent duplicate generated calls**: Prevent duplicate generated `.As<TContract>()` calls.

6. **Deterministic ordering**: Ensure deterministic contract ordering in generated output.

7. **Preserve composition-only behavior**: Do not change composition-only behavior.

8. **No VContainer dependency in service assemblies**: Do not reintroduce VContainer dependency into service-only assemblies.

9. **No Resolve<T>()**: Do not generate `Resolve<T>()`.

10. **No RegisterInstance support**: Do not add RegisterInstance support.

## Analyzer Requirements

Add new diagnostic:

**ID**: `NHEM_DI_060`

**Severity**: Warning

**Title**: Mixed registration exposure style

**Message**: Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.

**Trigger**: Warn when a type mixes explicit exposure attributes `[As]` or `[AsSelf]` with legacy `AutoRegisterIn` exposure flags `AsImplementedInterfaces` or `AsSelf`.

**Do not trigger**:
- Pure explicit style (only `[As]` / `[AsSelf]`)
- Pure legacy style (only `AutoRegisterIn` flags)
- `AutoRegisterIn` with only scope and lifetime (no exposure flags)

## Generator Tests

- Explicit `[As]` only generates one `.As<TContract>()`
- Explicit `[As]` plus legacy `AsImplementedInterfaces=true` does not duplicate `.As<TContract>()`
- Explicit `[AsSelf]` plus legacy `AsSelf=true` does not duplicate self registration
- Legacy flag-only behavior still works
- Contract output is deterministic and sorted
- Composition-only service assemblies still emit no VContainer code

## Analyzer Tests

- `NHEM_DI_060` is emitted for mixed explicit + legacy exposure style
- `NHEM_DI_060` is not emitted for pure explicit style
- `NHEM_DI_060` is not emitted for pure legacy style
- Existing analyzer tests still pass

## Docs Requirements

- Update README canonical usage
- Update `AutoRegisterInAttribute` docs to avoid legacy-flag-first examples
- Add migration note: Old flag-style remains supported for compatibility, but explicit `[As]` / `[AsSelf]` is preferred
- Remove examples that use `AsImplementedInterfaces = false` and `AsSelf = false` together with `[As]` or `[AsSelf]`
- Document the rule:
  - AutoRegisterIn decides where and how long
  - As / AsSelf decide as what
  - EntryPoint / RegisterComponentInHierarchy decide registration kind

## Sample Requirements

- Update samples to use canonical explicit exposure style
- EntryPoint examples that implement VContainer.Unity interfaces should live in Composition assemblies
- Service-only asmdef samples must not reference VContainer or VContainer.Unity

## Versioning

- Bump package.json to 7.2.1
- Bump DangFugBixs.Generators.csproj version to 7.2.1
- Update CHANGELOG.md with 7.2.1 section
- Rebuild shipped DLL payloads:
  - Runtime/NhemDangFugBixs.Attributes.dll
  - Runtime/NhemDangFugBixs.Runtime.dll
  - Analyzers/NhemDangFugBixs.Generators.dll
  - Analyzers/NhemDangFugBixs.Analyzers.dll

## Release Gate

- Generator tests must pass
- Analyzer tests must pass
- Version drift check must pass
- Docs check must pass
- Unity sample dotnet build must pass when NHEM_UNITY_PROJECT_ROOT is set
- Unity sample compile must pass when UNITY_EXE is set
- release-gate.ps1 must fail if Unity returns non-zero

## Deliverables

- proposal.md
- design.md
- tasks.md
- migration-guide.md
- spec deltas for generator behavior
- spec deltas for analyzer diagnostics
- docs update plan
- test matrix
- release checklist
