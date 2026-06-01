# Design: Analyzer Maturity (7.3.0)

## Architecture

### New Diagnostics

#### NHEM_DI_061 — Duplicate explicit contract exposure

**Implementation location:**
`Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Diagnostics.cs`

**Logic:**
```csharp
private static void CheckDuplicateContractExposure(INamedTypeSymbol typeSymbol, List<AttributeData> asAttributes, DiagnosticReporter reporter)
{
    var contractTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    
    foreach (var asAttr in asAttributes)
    {
        if (asAttr.AttributeClass is INamedTypeSymbol attrClass && 
            attrClass.Name == "AsAttribute" &&
            asAttr.ConstructorArguments.Length > 0)
        {
            var contractType = asAttr.ConstructorArguments[0].Value as INamedTypeSymbol;
            if (contractType != null)
            {
                if (contractTypes.Contains(contractType))
                {
                    reporter.Report(
                        DiagnosticDescriptors.DuplicateContractExposure,
                        typeSymbol.Locations[0],
                        typeSymbol.Name
                    );
                    return; // Report once per type
                }
                contractTypes.Add(contractType);
            }
        }
    }
}
```

**Diagnostic descriptor:**
```csharp
internal static DiagnosticDescriptor DuplicateContractExposure = new(
    id: "NHEM_DI_061",
    title: "Duplicate explicit contract exposure",
    messageFormat: "Duplicate contract exposure. Remove duplicate [As] declaration for the same contract.",
    category: "NhemDangFugBixs",
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true,
    description: "A type declares duplicate [As(...)] attributes for the same contract type."
);
```

#### NHEM_DI_066 — RegisterComponentInHierarchy on non-MonoBehaviour

**Implementation location:**
`Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Diagnostics.cs`

**Logic:**
```csharp
private static void CheckRegisterComponentInHierarchyUsage(INamedTypeSymbol typeSymbol, List<AttributeData> attributes, DiagnosticReporter reporter, Compilation compilation)
{
    var hasRegisterComponentInHierarchy = attributes.Any(a => 
        a.AttributeClass is INamedTypeSymbol attrClass && 
        attrClass.Name == "RegisterComponentInHierarchyAttribute");
    
    if (!hasRegisterComponentInHierarchy)
        return;
    
    // Check if type inherits from MonoBehaviour
    var monoBehaviourSymbol = compilation.GetTypeByMetadataName("UnityEngine.MonoBehaviour");
    if (monoBehaviourSymbol == null)
        return; // Gracefully handle missing UnityEngine reference
    
    var inheritsFromMonoBehaviour = typeSymbol.InheritsFrom(monoBehaviourSymbol);
    
    if (!inheritsFromMonoBehaviour)
    {
        reporter.Report(
            DiagnosticDescriptors.RegisterComponentInHierarchyOnNonMonoBehaviour,
            typeSymbol.Locations[0],
            typeSymbol.Name
        );
    }
}

private static bool InheritsFrom(this INamedTypeSymbol typeSymbol, INamedTypeSymbol baseType)
{
    var current = typeSymbol.BaseType;
    while (current != null)
    {
        if (SymbolEqualityComparer.Default.Equals(current, baseType))
            return true;
        current = current.BaseType;
    }
    return false;
}
```

**Diagnostic descriptor:**
```csharp
internal static DiagnosticDescriptor RegisterComponentInHierarchyOnNonMonoBehaviour = new(
    id: "NHEM_DI_066",
    title: "RegisterComponentInHierarchy on non-MonoBehaviour",
    messageFormat: "RegisterComponentInHierarchy can only be used on MonoBehaviour types.",
    category: "NhemDangFugBixs",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true,
    description: "A type uses [RegisterComponentInHierarchy] but does not inherit from UnityEngine.MonoBehaviour."
);
```

#### NHEM_DI_067 — EntryPoint without known lifecycle contract

**Implementation location:**
`Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Diagnostics.cs`

**Logic:**
```csharp
private static void CheckEntryPointLifecycleInterface(INamedTypeSymbol typeSymbol, List<AttributeData> attributes, DiagnosticReporter reporter, Compilation compilation)
{
    var hasEntryPoint = attributes.Any(a => 
        a.AttributeClass is INamedTypeSymbol attrClass && 
        attrClass.Name == "EntryPointAttribute");
    
    if (!hasEntryPoint)
        return;
    
    // Known lifecycle interfaces
    var lifecycleInterfaces = new[]
    {
        "VContainer.Unity.IInitializable",
        "VContainer.Unity.IStartable",
        "VContainer.Unity.IPostInitializable",
        "VContainer.Unity.ITickable",
        "VContainer.Unity.IFixedTickable",
        "VContainer.Unity.ILateTickable",
        "System.IDisposable"
    };
    
    // Check if type implements any lifecycle interface
    var implementsLifecycle = false;
    foreach (var interfaceName in lifecycleInterfaces)
    {
        var interfaceSymbol = compilation.GetTypeByMetadataName(interfaceName);
        if (interfaceSymbol != null && typeSymbol.AllInterfaces.Contains(interfaceSymbol, SymbolEqualityComparer.Default))
        {
            implementsLifecycle = true;
            break;
        }
    }
    
    if (!implementsLifecycle)
    {
        reporter.Report(
            DiagnosticDescriptors.EntryPointWithoutLifecycleInterface,
            typeSymbol.Locations[0],
            typeSymbol.Name
        );
    }
}
```

**Diagnostic descriptor:**
```csharp
internal static DiagnosticDescriptor EntryPointWithoutLifecycleInterface = new(
    id: "NHEM_DI_067",
    title: "EntryPoint without known lifecycle contract",
    messageFormat: "EntryPoint should implement a known lifecycle interface such as IStartable, ITickable, IInitializable, or IDisposable.",
    category: "NhemDangFugBixs",
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true,
    description: "A type uses [EntryPoint] but does not implement a known VContainer lifecycle interface."
);
```

## Integration Points

### Existing Diagnostic Flow

The new diagnostics should be integrated into the existing diagnostic analysis flow in `Diagnostics.cs`:

```csharp
private static void AnalyzeTypeForDiagnostics(INamedTypeSymbol typeSymbol, AttributeData autoRegisterAttr, DiagnosticReporter reporter, Compilation compilation)
{
    // Existing checks...
    
    // New checks
    var allAttributes = typeSymbol.GetAttributes().ToList();
    CheckDuplicateContractExposure(typeSymbol, allAttributes, reporter);
    CheckRegisterComponentInHierarchyUsage(typeSymbol, allAttributes, reporter, compilation);
    CheckEntryPointLifecycleInterface(typeSymbol, allAttributes, reporter, compilation);
}
```

## Dependencies

- Microsoft.CodeAnalysis (for Roslyn analysis)
- SymbolEqualityComparer.Default (for semantic type comparison)
- Compilation.GetTypeByMetadataName (for resolving type symbols)

## Error Handling

- All type resolution wrapped in null checks
- Missing UnityEngine or VContainer.Unity references handled gracefully
- Analyzer should not crash if symbols cannot be resolved
- Warnings only emitted when sufficient information is available

## Performance

- Analysis only on types with [AutoRegisterIn] attributes
- No assembly scanning beyond the compilation
- Symbol comparison uses semantic equality for accuracy
- Early returns for types without relevant attributes

## Security

- No external dependencies
- No network calls
- No file system operations
- No user input processing
