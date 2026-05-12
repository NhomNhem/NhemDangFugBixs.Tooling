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
```

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
