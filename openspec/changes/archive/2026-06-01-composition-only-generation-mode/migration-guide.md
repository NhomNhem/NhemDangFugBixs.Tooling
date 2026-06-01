# Migration Guide: Composition-Only Generation Mode

## Why this change exists

The old prototype behavior emitted VContainer `.g.cs` files into every compilation that contained `[AutoRegisterIn]`. That made low-level service asmdefs depend on VContainer indirectly.

Composition-only generation restores the intended ownership model:

- services declare intent
- composition owns VContainer registration

## What changes for existing projects

### Service-only asmdefs

Service asmdefs can keep using:

- `[AutoRegisterIn]`
- `[As]`
- `[AsSelf]`
- `[EntryPoint]`
- `[RegisterComponentInHierarchy]`

But they should no longer need to reference `VContainer` just because generated code exists.

### Composition asmdefs

Composition asmdefs become the place that must:

- reference `VContainer`
- reference each service asmdef they want to compose
- declare `[LifetimeScopeFor]`
- call `builder.RegisterGeneratedFor<TScope>()`

## Recommended migration steps

1. Identify service-only asmdefs that currently reference `VContainer` only because of generated code.
2. Keep DI intent attributes on services in those asmdefs.
3. Remove unnecessary `VContainer` references from service-only asmdefs.
4. Ensure composition asmdefs reference every service asmdef they should compose.
5. Move or confirm `[LifetimeScopeFor]` declarations in composition asmdefs.
6. Keep user-owned `LifetimeScope` classes and call `builder.RegisterGeneratedFor<TScope>()` there.
7. Run generator tests, analyzer tests, and the Unity sample compile gate.

## Behavioral differences to expect

- Service-only assemblies stop receiving VContainer installer `.g.cs` files.
- Composition assemblies become the only place where installers are emitted.
- Unreferenced service asmdefs are intentionally invisible to a composition target.
- Direct references are the discovery boundary for the MVP.

## Not included in this migration

This change does not add or expand:

- `RegisterInstance`
- MessagePipe
- pooling
- Addressables
- prefab factory support
- generated `LifetimeScope` MonoBehaviours
- partial injection into user `LifetimeScope` classes

## Rollback consideration

If a team currently relies on per-assembly emission, migration risk should be evaluated before removing compatibility behavior entirely. That decision belongs to implementation planning and release notes.
