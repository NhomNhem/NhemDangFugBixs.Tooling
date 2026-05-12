# 09 — Do Not Rules

## Do not hard-code one game's architecture

Do not assume every project has:

```txt
ProjectLifetimeScope
GameplayLifetimeScope
MainMenuLifetimeScope
```

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

- Factory
- Spawner
- Bootstrapper
- LifetimeScope
- Explicitly configured exceptions

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
