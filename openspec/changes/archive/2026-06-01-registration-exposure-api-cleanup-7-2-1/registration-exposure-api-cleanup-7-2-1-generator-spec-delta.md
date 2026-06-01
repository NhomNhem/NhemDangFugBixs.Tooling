# Spec Delta: Generator Behavior for Registration Exposure API Cleanup 7.2.1

## Overview

This spec delta describes the changes to the VContainer registration generator behavior in version 7.2.1 to support canonical explicit exposure attributes.

## Current Behavior

### NormalizeExplicitContractBehavior (ClassAnalyzer.cs)

The current implementation:
- Sets `asImplementedInterfaces = false` if `asTypes` has explicit contracts
- Sets `asSelf = false` if `asTypes` has explicit contracts and no explicit `[AsSelf]` attribute
- Does not independently check for `[AsSelf]` attribute without explicit contracts

### GetSmartSuffix (RegistrationEmitter.cs)

The current implementation:
- Prioritizes `AsTypes` over legacy flags when `AsTypes` is present
- Falls back to legacy flags when `AsTypes` is empty
- Does not guarantee deterministic ordering of `AsTypes`

### GetSmartEntryPointSuffix (RegistrationEmitter.cs)

The current implementation:
- Similar logic to `GetSmartSuffix` for entry points
- Does not guarantee deterministic ordering

## New Behavior

### NormalizeExplicitContractBehavior Enhancement

**Location**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Analyzers/ClassAnalyzer.cs`

**Change**: Enhance to check for explicit `[AsSelf]` attribute independently.

```csharp
private static void NormalizeExplicitContractBehavior(
    AttributeSyntax autoRegisterAttribute,
    TypeDeclarationSyntax typeDecl,
    ref bool asImplementedInterfaces,
    ref bool asSelf,
    ref string[] asTypes)
{
    bool hasExplicitContracts = asTypes.Length > 0;
    bool hasExplicitAsSelf = HasAttribute(typeDecl, "AsSelf");
    
    // If explicit exposure attributes exist, ignore legacy flags
    if (hasExplicitContracts || hasExplicitAsSelf) {
        asImplementedInterfaces = false;
        
        // Only disable AsSelf if we have explicit contracts but no explicit AsSelf attribute
        // and no explicit AsSelf named property in AutoRegisterIn
        if (hasExplicitContracts && !hasExplicitAsSelf && !HasNamedProperty(autoRegisterAttribute, "AsSelf")) {
            asSelf = false;
        }
    }
}
```

**Behavior Rules**:
1. If `[As(...)]` attributes present: ignore `AsImplementedInterfaces` flag
2. If `[AsSelf]` attribute present: ignore `AsSelf` flag
3. If `[As(...)]` present but `[AsSelf]` not present and no explicit `AsSelf` property: disable `AsSelf`
4. If no explicit attributes: preserve legacy flag behavior

### GetSmartSuffix Enhancement

**Location**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Emitters/RegistrationEmitter.cs`

**Change**: Add deterministic sorting for `AsTypes`.

```csharp
private static string GetSmartSuffix(ServiceInfo svc) {
    if (svc.AsTypes != null && svc.AsTypes.Length > 0) {
        var sortedTypes = svc.AsTypes.OrderBy(t => t, StringComparer.Ordinal).ToArray();
        return string.Concat(sortedTypes.Select(t => $".As<global::{t}>()")) + (svc.AsSelf ? ".AsSelf()" : string.Empty);
    }

    var suffix = string.Empty;
    bool shouldBindImplementedInterfaces = svc.AsImplementedInterfaces || (svc.IsComponent && svc.IsEntryPoint);
    if (shouldBindImplementedInterfaces) suffix += ".AsImplementedInterfaces()";
    if (svc.AsSelf) suffix += ".AsSelf()";
    return suffix;
}
```

**Behavior Rules**:
1. `AsTypes` are sorted alphabetically using `StringComparer.Ordinal`
2. Sorting ensures deterministic generated output
3. `AsSelf` is appended after all `As<TContract>()` calls if enabled

### GetSmartEntryPointSuffix Enhancement

**Location**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Emitters/RegistrationEmitter.cs`

**Change**: Add deterministic sorting for `AsTypes`.

```csharp
private static string GetSmartEntryPointSuffix(ServiceInfo svc) {
    if (svc.AsTypes != null && svc.AsTypes.Length > 0) {
        var sortedTypes = svc.AsTypes.OrderBy(t => t, StringComparer.Ordinal).ToArray();
        return string.Concat(sortedTypes.Select(t => $".As<global::{t}>()")) + (svc.AsSelf ? ".AsSelf()" : string.Empty);
    }

    var customInterfaces = svc.InterfaceNames
        .Where(i => !InterfaceUtils.IsVContainerEntryPoint(i))
        .OrderBy(i => i, StringComparer.Ordinal)
        .ToList();

    if (customInterfaces.Count > 0) {
        var suffix = string.Concat(customInterfaces.Select(i => $".As<global::{i}>()"));
        if (svc.AsSelf) {
            suffix += ".AsSelf()";
        }
        return suffix;
    }

    return svc.AsSelf ? ".AsSelf()" : string.Empty;
}
```

**Behavior Rules**:
1. `AsTypes` are sorted alphabetically using `StringComparer.Ordinal`
2. Fallback interface names are also sorted for consistency
3. `AsSelf` is appended after all interface registrations if enabled

## Generated Output Examples

### Example 1: Explicit [As] Only

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IService>]
public sealed class Service : IService { }
```

**Generated**:
```csharp
builder.Register<Service>(Lifetime.Singleton)
    .As<IService>();
```

### Example 2: Explicit [As] with [AsSelf]

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IService>]
[AsSelf]
public sealed class Service : IService { }
```

**Generated**:
```csharp
builder.Register<Service>(Lifetime.Singleton)
    .As<IService>()
    .AsSelf();
```

### Example 3: Multiple Explicit [As] (Sorted)

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IZService>]
[As<IAService>]
public sealed class Service : IZService, IAService { }
```

**Generated**:
```csharp
builder.Register<Service>(Lifetime.Singleton)
    .As<IAService>()
    .As<IZService>();
```

**Note**: Contracts are sorted alphabetically: `IAService` before `IZService`.

### Example 4: Explicit [As] with Legacy AsImplementedInterfaces=true (Ignored)

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true)]
[As<IService>]
public sealed class Service : IService, IOtherService { }
```

**Generated**:
```csharp
builder.Register<Service>(Lifetime.Singleton)
    .As<IService>();
```

**Note**: `AsImplementedInterfaces` flag is ignored because explicit `[As]` is present. Only `IService` is registered, not `IOtherService`.

### Example 5: Explicit [AsSelf] with Legacy AsSelf=true (No Duplicate)

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>(AsSelf = true)]
[AsSelf]
public sealed class Service { }
```

**Generated**:
```csharp
builder.Register<Service>(Lifetime.Singleton)
    .AsSelf();
```

**Note**: Only one `.AsSelf()` call is generated, not two.

### Example 6: Legacy Flag-Only (Backward Compatible)

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true, AsSelf = true)]
public sealed class Service : IService, IOtherService { }
```

**Generated**:
```csharp
builder.Register<Service>(Lifetime.Singleton)
    .AsImplementedInterfaces()
    .AsSelf();
```

**Note**: Legacy behavior is preserved when no explicit attributes are present.

### Example 7: Component in Hierarchy with Explicit [As]

**Input**:
```csharp
[RegisterComponentInHierarchy]
[AutoRegisterIn<IGameplayScope>]
[As<IPlayerView>]
public sealed class PlayerView : MonoBehaviour, IPlayerView { }
```

**Generated**:
```csharp
builder.RegisterComponentInHierarchy<PlayerView>()
    .As<IPlayerView>();
```

### Example 8: Entry Point with Explicit [As]

**Input**:
```csharp
[AutoRegisterIn<IGameplayScope>]
[EntryPoint]
[As<IGameplayLoop>]
public sealed class GameplayLoop : IStartable, IGameplayLoop { }
```

**Generated**:
```csharp
builder.RegisterEntryPoint<GameplayLoop>()
    .As<IGameplayLoop>();
```

**Note**: VContainer lifecycle interfaces (`IStartable`) are handled automatically, not explicitly registered.

## Backward Compatibility Guarantees

1. **Legacy flag-only behavior unchanged**: Code using only `AsImplementedInterfaces` and `AsSelf` flags continues to generate identical output.

2. **No duplicate registrations**: Explicit attributes prevent legacy flags from creating duplicate `.As<T>()` calls.

3. **Deterministic ordering**: New sorting ensures consistent output, but does not change the semantics of registration.

4. **Composition-only preserved**: Service assemblies without explicit VContainer references continue to emit no VContainer code.

5. **No Resolve<T>()**: No changes to resolution behavior.

6. **No RegisterInstance**: No new registration modes introduced.

## Test Coverage Requirements

### Unit Tests

1. **Explicit [As] only**: Verify generates one `.As<TContract>()`
2. **Explicit [As] with legacy flags**: Verify no duplicate `.As<TContract>()`
3. **Explicit [AsSelf] with legacy AsSelf**: Verify no duplicate `.AsSelf()`
4. **Legacy flag-only**: Verify unchanged behavior
5. **Deterministic sorting**: Verify contracts are sorted alphabetically
6. **Composition-only**: Verify no VContainer code in service assemblies
7. **Component registration**: Verify correct suffix for hierarchy components
8. **Entry point registration**: Verify correct suffix for entry points

### Integration Tests

1. **Full registration flow**: Verify end-to-end generation with mixed assemblies
2. **Cross-assembly discovery**: Verify explicit attributes work across asmdef boundaries
3. **Scope mapping**: Verify explicit attributes work with type-safe scopes
4. **MessagePipe integration**: Verify explicit attributes don't interfere with MessagePipe

## Performance Considerations

1. **Sorting overhead**: Minimal overhead from sorting small arrays (typically 1-3 contracts)
2. **Attribute lookup**: Additional `HasAttribute` call for `[AsSelf]` - negligible impact
3. **No additional passes**: Changes are in existing analysis phase, no new compilation passes

## Edge Cases

1. **Empty AsTypes array**: Should not occur, but handled by existing null/empty checks
2. **Duplicate contract types**: Deduplication already happens in `MergeContractTypes`
3. **Circular references**: No change to existing circular reference handling
4. **Generic contracts**: No special handling needed, sorting works on full type names
5. **Nested generics**: Full type names include nesting, sorting works correctly
