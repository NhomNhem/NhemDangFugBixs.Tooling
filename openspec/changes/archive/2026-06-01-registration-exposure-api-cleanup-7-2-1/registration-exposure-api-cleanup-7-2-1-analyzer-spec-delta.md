# Spec Delta: Analyzer Diagnostics for Registration Exposure API Cleanup 7.2.1

## Overview

This spec delta describes the new analyzer diagnostic NHEM_DI_060 for detecting mixed registration exposure style in version 7.2.1.

## New Diagnostic

### NHEM_DI_060: Mixed Registration Exposure Style

**ID**: `NHEM_DI_060`

**Title**: Mixed registration exposure style

**Severity**: Warning

**Category**: Usage

**Message**: Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.

**Default Enabled**: Yes

## Trigger Conditions

The diagnostic is triggered when a type:
1. Has `[AutoRegisterIn]` attribute
2. Has explicit exposure attributes `[As]` or `[AsSelf]`
3. Has legacy exposure flags explicitly set in `AutoRegisterIn`:
   - `AsImplementedInterfaces` is explicitly set to `true` or `false`
   - `AsSelf` is explicitly set to `true` or `false`

## Non-Trigger Conditions

The diagnostic is NOT triggered when:
1. **Pure explicit style**: Type has `[As]` or `[AsSelf]` but no legacy flags explicitly set
2. **Pure legacy style**: Type has legacy flags but no explicit `[As]` or `[AsSelf]` attributes
3. **Scope-only style**: Type has `AutoRegisterIn` with only scope and lifetime (no exposure flags, no explicit attributes)

## Implementation Details

### Detection Logic

**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`

```csharp
private static bool HasMixedExposureStyle(INamedTypeSymbol type, AttributeData autoRegister) {
    bool hasExplicitContracts = GetExplicitContracts(type).Count > 0;
    bool hasExplicitAsSelf = HasAttribute(type, "AsSelfAttribute");
    
    // Check if legacy flags are explicitly set (not default values)
    bool hasLegacyAsImplementedInterfaces = TryGetBoolNamedArgument(autoRegister, "AsImplementedInterfaces") == true;
    bool hasLegacyAsSelf = TryGetBoolNamedArgument(autoRegister, "AsSelf") == true;
    
    // If user explicitly set legacy flags AND has explicit attributes, warn
    if ((hasExplicitContracts || hasExplicitAsSelf) && (hasLegacyAsImplementedInterfaces || hasLegacyAsSelf)) {
        return true;
    }
    
    return false;
}
```

### Key Implementation Notes

1. **Default values are not "explicitly set"**: 
   - `AsImplementedInterfaces = true` is the default, so not setting it does NOT count as explicit
   - `AsSelf = true` is the default, so not setting it does NOT count as explicit
   - Only when user writes `AsImplementedInterfaces = true/false` or `AsSelf = true/false` explicitly

2. **`TryGetBoolNamedArgument` returns null for unset properties**:
   - If property is not set in attribute, returns `null`
   - If property is set to `true`, returns `true`
   - If property is set to `false`, returns `false`
   - We only warn when it returns `true` (explicitly set to true)

3. **Rationale for only warning on `true`**:
   - Setting flags to `false` explicitly is often intentional to disable auto-detection
   - Setting flags to `true` explicitly when also using explicit attributes is redundant/confusing
   - This keeps the diagnostic focused on the most problematic cases

## Example Scenarios

### Triggers NHEM_DI_060

**Scenario 1: Explicit [As] with AsImplementedInterfaces = true**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true)]
[As<IService>]
public sealed class Service : IService { }
```
**Diagnostic**: Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.

**Scenario 2: Explicit [AsSelf] with AsSelf = true**
```csharp
[AutoRegisterIn<IGameplayScope>(AsSelf = true)]
[AsSelf]
public sealed class Service { }
```
**Diagnostic**: Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.

**Scenario 3: Explicit [As] with AsSelf = true**
```csharp
[AutoRegisterIn<IGameplayScope>(AsSelf = true)]
[As<IService>]
public sealed class Service : IService { }
```
**Diagnostic**: Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime.

### Does NOT Trigger NHEM_DI_060

**Scenario 4: Pure explicit style (canonical)**
```csharp
[AutoRegisterIn<IGameplayScope>]
[As<IService>]
public sealed class Service : IService { }
```
**No diagnostic**: This is the canonical style.

**Scenario 5: Pure legacy style (backward compatible)**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = true)]
public sealed class Service : IService { }
```
**No diagnostic**: Legacy style remains supported.

**Scenario 6: Scope-only style**
```csharp
[AutoRegisterIn<IGameplayScope>]
public sealed class Service { }
```
**No diagnostic**: This triggers NHEM_DI_003 (no exposure intent), not NHEM_DI_060.

**Scenario 7: Explicit [As] with AsImplementedInterfaces = false (intentional)**
```csharp
[AutoRegisterIn<IGameplayScope>(AsImplementedInterfaces = false)]
[As<IService>]
public sealed class Service : IService, IOtherService { }
```
**No diagnostic**: Setting `AsImplementedInterfaces = false` is intentional to disable auto-detection while using explicit attributes.

## Diagnostic Integration

### SupportedDiagnostics Array

**Location**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`

```csharp
public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
    DiagnosticCatalog.InvalidContract,
    DiagnosticCatalog.InvalidScopeMarker,
    DiagnosticCatalog.MissingExposureIntent,
    DiagnosticCatalog.InvalidEntryPoint,
    DiagnosticCatalog.MixedExposureStyle);
```

### Analysis Flow

1. `Initialize()` registers symbol action for `SymbolKind.NamedType`
2. `AnalyzeType()` is called for each class
3. Check for `[AutoRegisterIn]` attribute
4. Run existing checks (scope marker, contracts, entry point)
5. **NEW**: Call `HasMixedExposureStyle()` to detect mixed usage
6. Report diagnostic if mixed style detected

### Diagnostic Location

The diagnostic is reported on the type declaration (first location of the type symbol).

```csharp
context.ReportDiagnostic(Diagnostic.Create(
    DiagnosticCatalog.MixedExposureStyle,
    type.Locations.FirstOrDefault()));
```

## Relationship to Other Diagnostics

### NHEM_DI_003: Missing Exposure Intent

- **NHEM_DI_003**: Warns when type has no exposure at all (no explicit attributes, no legacy flags)
- **NHEM_DI_060**: Warns when type has both explicit attributes AND legacy flags
- **Mutually exclusive**: A type cannot trigger both diagnostics simultaneously

### NHEM_DI_001: Invalid Contract

- **NHEM_DI_001**: Warns when `[As]` specifies a contract not implemented by the type
- **NHEM_DI_060**: Warns about mixing exposure mechanisms
- **Can trigger together**: A type can have both invalid contracts AND mixed exposure style

### NHEM_DI_002: Invalid Scope Marker

- **NHEM_DI_002**: Warns when scope marker doesn't implement `IScopeMarker`
- **NHEM_DI_060**: Warns about mixing exposure mechanisms
- **Independent**: These check different aspects

### NHEM_DI_040: Invalid Entry Point

- **NHEM_DI_040**: Warns when `[EntryPoint]` is used without VContainer lifecycle interface
- **NHEM_DI_060**: Warns about mixing exposure mechanisms
- **Independent**: These check different aspects

## Test Coverage Requirements

### Positive Tests (Should Trigger)

1. **Explicit [As] with AsImplementedInterfaces = true**
   - Input: `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)] [As<IContract>]`
   - Expected: NHEM_DI_060 emitted

2. **Explicit [AsSelf] with AsSelf = true**
   - Input: `[AutoRegisterIn<Scope>(AsSelf = true)] [AsSelf]`
   - Expected: NHEM_DI_060 emitted

3. **Explicit [As] with AsSelf = true**
   - Input: `[AutoRegisterIn<Scope>(AsSelf = true)] [As<IContract>]`
   - Expected: NHEM_DI_060 emitted

4. **Multiple explicit [As] with legacy flags**
   - Input: `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)] [As<I1>] [As<I2>]`
   - Expected: NHEM_DI_060 emitted

### Negative Tests (Should NOT Trigger)

1. **Pure explicit style**
   - Input: `[AutoRegisterIn<Scope>] [As<IContract>]`
   - Expected: No NHEM_DI_060

2. **Pure legacy style**
   - Input: `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)]`
   - Expected: No NHEM_DI_060

3. **Scope-only style**
   - Input: `[AutoRegisterIn<Scope>]`
   - Expected: No NHEM_DI_060 (but NHEM_DI_003)

4. **Explicit [As] with AsImplementedInterfaces = false**
   - Input: `[AutoRegisterIn<Scope>(AsImplementedInterfaces = false)] [As<IContract>]`
   - Expected: No NHEM_DI_060 (intentional disabling)

5. **Explicit [AsSelf] with AsSelf = false**
   - Input: `[AutoRegisterIn<Scope>(AsSelf = false)] [AsSelf]`
   - Expected: No NHEM_DI_060 (intentional disabling)

6. **No [AutoRegisterIn]**
   - Input: `[As<IContract>]` (no AutoRegisterIn)
   - Expected: No NHEM_DI_060 (not applicable)

### Edge Case Tests

1. **Generic scope with mixed style**
   - Input: `[AutoRegisterIn<IScope>(AsImplementedInterfaces = true)] [As<IContract>]`
   - Expected: NHEM_DI_060 emitted

2. **TypeOf scope with mixed style**
   - Input: `[AutoRegisterIn(typeof(Scope), AsImplementedInterfaces = true)] [As<IContract>]`
   - Expected: NHEM_DI_060 emitted

3. **Multiple [AutoRegisterIn] (unusual but possible)**
   - Input: Two `[AutoRegisterIn]` attributes with different scopes
   - Expected: Diagnostics for each that has mixed style

4. **Inherited attributes**
   - Input: Base class has `[AutoRegisterIn]`, derived has `[As]`
   - Expected: No NHEM_DI_060 (attribute usage is per-type)

## Performance Considerations

1. **Minimal overhead**: Additional check is simple boolean logic
2. **No additional symbol resolution**: Uses existing `GetExplicitContracts()` and `HasAttribute()` methods
3. **O(1) complexity**: Check is constant time per type
4. **No impact on compilation**: Analyzer runs in parallel with compilation

## User Experience

### Severity Justification

**Warning** (not Error) because:
- Mixed style is confusing but not functionally incorrect
- Generator handles mixed style correctly (explicit takes precedence)
- Legacy style remains supported for backward compatibility
- Warning helps users adopt canonical style without breaking builds

### Code Fix Provider

**Not included in this version**. Future enhancement could provide:
- Remove legacy flags, keep explicit attributes
- Remove explicit attributes, keep legacy flags
- Convert to canonical style

### Suppression

Users can suppress the diagnostic if they prefer:
```csharp
#pragma warning disable NHEM_DI_060
[AutoRegisterIn<IScope>(AsImplementedInterfaces = true)]
[As<IContract>>
public sealed class Service : IContract { }
#pragma warning restore NHEM_DI_060
```

However, this is not recommended for new code.
