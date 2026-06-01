# Design: Registration Exposure API Cleanup 7.2.1

## Overview

This design document details the implementation of the registration exposure API cleanup for version 7.2.1. The goal is to separate concerns between scope/lifetime declaration and contract exposure, eliminating confusing mixed usage patterns.

## Current State

### Existing Code Locations

1. **Attribute Definition**: `Source~/DangFugBixs.Attributes~/Attributes/AutoRegisterInAttribute.cs`
   - Contains `AsImplementedInterfaces` and `AsSelf` properties
   - Contains `AsTypes` property for explicit contracts

2. **Generator Analysis**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Analyzers/ClassAnalyzer.cs`
   - `ExtractInfoFromComponentAttribute()` - extracts registration info
   - `NormalizeExplicitContractBehavior()` - already handles some normalization
   - `ExtractExplicitContractTypes()` - extracts `[As]` attributes

3. **Generator Emission**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Emitters/RegistrationEmitter.cs`
   - `GetSmartSuffix()` - determines registration suffix
   - `GetSmartEntryPointSuffix()` - determines entry point suffix

4. **Analyzer**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
   - `AnalyzeType()` - analyzes types for diagnostics
   - `ShouldWarnMissingExposure()` - checks for missing exposure intent

## Design Changes

### 1. Generator Behavior Changes

#### ClassAnalyzer.cs - NormalizeExplicitContractBehavior Enhancement

The existing `NormalizeExplicitContractBehavior()` method currently:
- Sets `asImplementedInterfaces = false` if `asTypes` has explicit contracts
- Sets `asSelf = false` if `asTypes` has explicit contracts and no explicit `[AsSelf]` attribute

**New behavior**: We need to enhance this to also check for explicit `[AsSelf]` attribute independently.

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
        if (hasExplicitContracts && !hasExplicitAsSelf && !HasNamedProperty(autoRegisterAttribute, "AsSelf")) {
            asSelf = false;
        }
    }
}
```

**Key change**: Check for `[AsSelf]` attribute independently, not just as part of `asTypes`.

#### RegistrationEmitter.cs - GetSmartSuffix Enhancement

The existing `GetSmartSuffix()` method already prioritizes `AsTypes` over legacy flags:

```csharp
private static string GetSmartSuffix(ServiceInfo svc) {
    if (svc.AsTypes != null && svc.AsTypes.Length > 0) {
        return string.Concat(svc.AsTypes.Select(t => $".As<global::{t}>()")) + (svc.AsSelf ? ".AsSelf()" : string.Empty);
    }

    var suffix = string.Empty;
    bool shouldBindImplementedInterfaces = svc.AsImplementedInterfaces || (svc.IsComponent && svc.IsEntryPoint);
    if (shouldBindImplementedInterfaces) suffix += ".AsImplementedInterfaces()";
    if (svc.AsSelf) suffix += ".AsSelf()";
    return suffix;
}
```

**New behavior**: Ensure that when `AsTypes` is present, we only use explicit contracts and `AsSelf` if explicitly set. The current implementation already does this correctly, but we need to ensure:
1. `AsTypes` are sorted deterministically
2. No duplicate `.As<T>()` calls are generated

#### Deterministic Contract Ordering

Add sorting to ensure deterministic output:

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

Apply the same sorting to `GetSmartEntryPointSuffix()`.

### 2. Analyzer Changes

#### Add New Diagnostic

**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/DiagnosticIds.cs`

```csharp
internal static class DiagnosticIds {
    // ... existing IDs ...
    public const string MixedExposureStyle = "NHEM_DI_060";
}
```

**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/DiagnosticCatalog.cs`

```csharp
internal static class DiagnosticCatalog {
    // ... existing descriptors ...
    
    public static readonly DiagnosticDescriptor MixedExposureStyle = new(
        DiagnosticIds.MixedExposureStyle,
        "Mixed registration exposure style",
        "Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
```

**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`

Add the new diagnostic to `SupportedDiagnostics`:

```csharp
public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
    DiagnosticCatalog.InvalidContract,
    DiagnosticCatalog.InvalidScopeMarker,
    DiagnosticCatalog.MissingExposureIntent,
    DiagnosticCatalog.InvalidEntryPoint,
    DiagnosticCatalog.MixedExposureStyle);
```

Add detection logic in `AnalyzeType()`:

```csharp
private static void AnalyzeType(SymbolAnalysisContext context) {
    var type = (INamedTypeSymbol)context.Symbol;
    if (type.TypeKind != TypeKind.Class) {
        return;
    }

    var autoRegister = FindAttribute(type, "AutoRegisterInAttribute");
    if (autoRegister == null) {
        return;
    }

    // ... existing checks ...

    // New: Check for mixed exposure style
    if (HasMixedExposureStyle(type, autoRegister)) {
        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticCatalog.MixedExposureStyle,
            type.Locations.FirstOrDefault()));
    }
}

private static bool HasMixedExposureStyle(INamedTypeSymbol type, AttributeData autoRegister) {
    bool hasExplicitContracts = GetExplicitContracts(type).Count > 0;
    bool hasExplicitAsSelf = HasAttribute(type, "AsSelfAttribute");
    
    // Check if legacy flags are explicitly set (not default)
    bool hasLegacyAsImplementedInterfaces = TryGetBoolNamedArgument(autoRegister, "AsImplementedInterfaces") == true;
    bool hasLegacyAsSelf = TryGetBoolNamedArgument(autoRegister, "AsSelf") == true;
    
    // If user explicitly set legacy flags AND has explicit attributes, warn
    if ((hasExplicitContracts || hasExplicitAsSelf) && (hasLegacyAsImplementedInterfaces || hasLegacyAsSelf)) {
        return true;
    }
    
    return false;
}
```

**Important**: Only warn when legacy flags are **explicitly set** (not just default values). This means:
- `AsImplementedInterfaces = true` is the default, so don't warn if not explicitly set
- `AsSelf = true` is the default, so don't warn if not explicitly set
- Only warn if user explicitly sets `AsImplementedInterfaces = false/true` or `AsSelf = false/true` AND has explicit `[As]` or `[AsSelf]` attributes

### 3. Documentation Changes

#### AutoRegisterInAttribute.cs XML Documentation

Update the XML docs to emphasize canonical usage:

```csharp
/// <summary>
/// Registers a class with VContainer using type-safe scope reference.
/// 
/// Canonical usage: Use [As] and [AsSelf] attributes for contract exposure.
/// AutoRegisterIn should only declare scope and lifetime.
/// 
/// The scope type should inherit from VContainer.Unity.LifetimeScope, OR it can be an
/// Identity Type mapped via [LifetimeScopeFor] for cross-assembly discovery.
///
/// Note: Uses NhemDangFugBixs.Attributes.NhemLifetime which has the same values as VContainer.Lifetime.
/// In your Unity project files, use the fully qualified name or an alias to avoid ambiguity.
/// </summary>
/// <example>
/// <code>
/// // Canonical - Explicit contract exposure
/// [AutoRegisterIn(typeof(GameScope), Lifetime = NhemLifetime.Scoped)]
/// [As(typeof(IGameService))]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;(Lifetime.Scoped).As&lt;IGameService&gt;();
///
/// // Legacy - Flag-based exposure (still supported for compatibility)
/// [AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = true)]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;().AsImplementedInterfaces().AsSelf();
///
/// // Multiple explicit contracts
/// [AutoRegisterIn(typeof(GameScope), Lifetime = NhemLifetime.Scoped)]
/// [As(typeof(IGameService))]
/// [As(typeof(ITickable))]
/// public class GameService : IGameService, ITickable { }
/// // Generates: builder.Register&lt;GameService&gt;(Lifetime.Scoped).As&lt;IGameService&gt;().As&lt;ITickable&gt;();
/// </code>
/// </example>
```

#### README.md Updates

Update the canonical usage section to show explicit attributes first:

```markdown
## Minimal Usage Example

Shared marker:

```csharp
public interface IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
```

Service asmdef (canonical explicit style):

```csharp
using NhemDangFugBixs.Attributes;

public interface ICombatCoreService { }

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<ICombatCoreService>]
public sealed class CombatCoreService : ICombatCoreService
{
}
```

Legacy flag-style (still supported):

```csharp
[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = true)]
public sealed class CombatCoreService : ICombatCoreService
{
}
```
```

Add migration note section:

```markdown
## Migration Guide for 7.2.1

### Registration Exposure API

Version 7.2.1 introduces canonical explicit exposure attributes.

**Before (legacy flag-style):**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true, AsSelf = false)]
public sealed class Service : IService { }
```

**After (canonical explicit style):**
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IService>]
public sealed class Service : IService { }
```

**Migration notes:**
- Legacy flag-style remains fully supported for backward compatibility
- New diagnostic NHEM_DI_060 warns when mixing explicit attributes with legacy flags
- Explicit attributes provide clearer intent and better IDE support
- No breaking changes to existing code
```

### 4. Test Changes

#### Generator Tests

Add new tests in `DangFugBixs.Tests/BindingGenerationTests.cs`:

```csharp
[Test]
public void ExplicitAsAttribute_WithLegacyAsImplementedInterfacesTrue_DoesNotDuplicate()
{
    const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
public interface IService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped, AsImplementedInterfaces = true)]
[As(typeof(IService))]
public sealed class Service : IService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

    var (_, generated) = GeneratorTestHost.Run(source);
    Assert.That(generated, Does.Contain(".As<global::IService>()"));
    Assert.That(generated, Does.Not.Contain(".AsImplementedInterfaces()"));
}

[Test]
public void ExplicitAsSelfAttribute_WithLegacyAsSelfTrue_DoesNotDuplicate()
{
    const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped, AsSelf = true)]
[AsSelf]
public sealed class Service { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

    var (_, generated) = GeneratorTestHost.Run(source);
    // Should only have one .AsSelf() call
    var count = generated.Split(".AsSelf()").Length - 1;
    Assert.That(count, Is.EqualTo(1));
}

[Test]
public void MultipleExplicitAsAttributes_SortedDeterministically()
{
    const string source = """
using NhemDangFugBixs.Attributes;
public interface IGameplayScope { }
public interface IZService { }
public interface IAService { }
[AutoRegisterIn(typeof(IGameplayScope), Lifetime = NhemLifetime.Scoped)]
[As(typeof(IZService))]
[As(typeof(IAService))]
public sealed class Service : IZService, IAService { }
[LifetimeScopeFor(typeof(IGameplayScope))]
public sealed class GameplayLifetimeScope { }
""";

    var (_, generated) = GeneratorTestHost.Run(source);
    // Contracts should be sorted alphabetically
    var indexOfA = generated.IndexOf(".As<global::IAService>()");
    var indexOfZ = generated.IndexOf(".As<global::IZService>()");
    Assert.That(indexOfA, Is.LessThan(indexOfZ));
}
```

#### Analyzer Tests

Add new tests in `DangFugBixs.Analyzers.Tests/OpenSpecAnalyzerMvpTests.cs`:

```csharp
[Fact]
public void MixedExposureStyle_ReportsNhemDi060()
{
    const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IService {}
[AutoRegisterIn(typeof(IGameplayScope), AsImplementedInterfaces = true)]
[As(typeof(IService))]
public sealed class CombatCore : IService {}
namespace NhemDangFixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { 
    public AutoRegisterInAttribute(System.Type t) {}
    public bool AsImplementedInterfaces { get; set; }
  }
  public class AsAttribute : System.Attribute { 
    public AsAttribute(System.Type t) {}
  }
}
""";

    var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
    Assert.Contains(diagnostics, d => d.Id == "NHEM_DI_060");
}

[Fact]
public void PureExplicitStyle_DoesNotReportNhemDi060()
{
    const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IService {}
[AutoRegisterIn(typeof(IGameplayScope))]
[As(typeof(IService))]
public sealed class CombatCore : IService {}
namespace NhemDangFixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { 
    public AutoRegisterInAttribute(System.Type t) {}
  }
  public class AsAttribute : System.Attribute { 
    public AsAttribute(System.Type t) {}
  }
}
""";

    var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
    Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_060");
}

[Fact]
public void PureLegacyStyle_DoesNotReportNhemDi060()
{
    const string source = """
using NhemDangFugBixs.Attributes;
public interface IScopeMarker {}
public interface IGameplayScope : IScopeMarker {}
public interface IService {}
[AutoRegisterIn(typeof(IGameplayScope), AsImplementedInterfaces = true)]
public sealed class CombatCore : IService {}
namespace NhemDangFixs.Attributes {
  public class AutoRegisterInAttribute : System.Attribute { 
    public AutoRegisterInAttribute(System.Type t) {}
    public bool AsImplementedInterfaces { get; set; }
  }
}
""";

    var diagnostics = AnalyzerTestHost.Run(source, new AttributeContractAnalyzer());
    Assert.DoesNotContain(diagnostics, d => d.Id == "NHEM_DI_060");
}
```

## Implementation Order

1. Add diagnostic ID and descriptor
2. Implement analyzer detection logic
3. Enhance generator normalization logic
4. Add deterministic sorting
5. Write generator tests
6. Write analyzer tests
7. Update documentation
8. Update samples

## Backward Compatibility

All changes are backward compatible:
- Legacy flag-style continues to work exactly as before
- New diagnostic is a warning, not an error
- No changes to generated output for legacy-style code
- Explicit attributes are additive, not replacing
