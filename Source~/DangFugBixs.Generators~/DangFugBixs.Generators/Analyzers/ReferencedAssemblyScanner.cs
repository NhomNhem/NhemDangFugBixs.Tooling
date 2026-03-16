using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NhemDangFugBixs.Common.Models;

namespace NhemDangFugBixs.Generators.Analyzers;

internal static class ReferencedAssemblyScanner {
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute";
    private const string AutoRegisterInGenericAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1";
    private const string InstallerOrderAttributeName = "NhemDangFugBixs.Attributes.InstallerOrderAttribute";

    public static (List<ServiceInfo> Services, List<string> Warnings) Scan(Compilation compilation) {
        var results = new List<ServiceInfo>();
        var warnings = new List<string>();

        // Get the attribute symbols from the compilation
        var attrSymbol = compilation.GetTypeByMetadataName(AutoRegisterInAttributeName);
        var genericAttrSymbol = compilation.GetTypeByMetadataName(AutoRegisterInGenericAttributeName);
        var orderAttrSymbol = compilation.GetTypeByMetadataName(InstallerOrderAttributeName);
        
        if (attrSymbol == null && genericAttrSymbol == null) return (results, warnings);

        // Scan all referenced assemblies
        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols) {
            try {
                ScanNamespace(assembly.GlobalNamespace, attrSymbol, genericAttrSymbol, orderAttrSymbol, results);
            } catch (System.Exception ex) {
                warnings.Add($"{assembly.Name}: {ex.Message}");
            }
        }

        return (results, warnings);
    }

    private static void ScanNamespace(INamespaceSymbol ns, INamedTypeSymbol? attrSymbol, INamedTypeSymbol? genericAttrSymbol, INamedTypeSymbol? orderAttrSymbol, List<ServiceInfo> results) {
        try {
            foreach (var member in ns.GetMembers()) {
                if (member is INamespaceSymbol nestedNs) {
                    ScanNamespace(nestedNs, attrSymbol, genericAttrSymbol, orderAttrSymbol, results);
                } else if (member is INamedTypeSymbol type) {
                    ScanType(type, attrSymbol, genericAttrSymbol, orderAttrSymbol, results);
                }
            }
        } catch {
            // Gracefully ignore namespace resolution failures
        }
    }

    private static void ScanType(INamedTypeSymbol type, INamedTypeSymbol? attrSymbol, INamedTypeSymbol? genericAttrSymbol, INamedTypeSymbol? orderAttrSymbol, List<ServiceInfo> results) {
        // Scan nested types
        foreach (var nestedType in type.GetTypeMembers()) {
            ScanType(nestedType, attrSymbol, genericAttrSymbol, orderAttrSymbol, results);
        }

        // Check for AutoRegisterInAttribute (either version)
        var attr = type.GetAttributes().FirstOrDefault(a => 
            (attrSymbol != null && SymbolEqualityComparer.Default.Equals(a.AttributeClass, attrSymbol)) ||
            (genericAttrSymbol != null && SymbolEqualityComparer.Default.Equals(a.AttributeClass?.OriginalDefinition, genericAttrSymbol)));

        if (attr != null) {
            var info = ExtractServiceInfo(type, attr, orderAttrSymbol);
            if (info.HasValue) {
                results.Add(info.Value);
            }
        }
    }

    private static ServiceInfo? ExtractServiceInfo(INamedTypeSymbol type, AttributeData attr, INamedTypeSymbol? orderAttrSymbol) {
        string? scopeTypeName = null;
        bool usesTypeSafeScope = false;

        // 1. Extract from generic argument if it's the generic version
        if (attr.AttributeClass != null && attr.AttributeClass.TypeArguments.Length > 0) {
            var identityType = attr.AttributeClass.TypeArguments[0];
            scopeTypeName = identityType.ToDisplayString();
            usesTypeSafeScope = true;
        } 
        // 2. Extract from positional constructor argument if it's the non-generic version
        else if (attr.ConstructorArguments.Length > 0) {
            var arg = attr.ConstructorArguments[0];
            if (arg.Kind == TypedConstantKind.Type && arg.Value is ITypeSymbol typeSymbol) {
                scopeTypeName = typeSymbol.ToDisplayString();
                usesTypeSafeScope = true;
            }
        }

        if (string.IsNullOrEmpty(scopeTypeName)) return null;

        // Extract properties (Lifetime, etc.)
        string lifetime = "Singleton";
        bool asImplementedInterfaces = true;
        bool asSelf = true;
        bool registerInHierarchy = false;
        List<string> asTypes = new List<string>();

        foreach (var namedArg in attr.NamedArguments) {
            switch (namedArg.Key) {
                case "Lifetime":
                    lifetime = namedArg.Value.Value?.ToString() ?? "Singleton";
                    // Lifetime is often an enum, we need the name
                    if (namedArg.Value.Value is int enumValue) {
                        lifetime = enumValue switch {
                            0 => "Singleton",
                            1 => "Transient",
                            2 => "Scoped",
                            _ => "Singleton"
                        };
                    }
                    break;
                case "AsImplementedInterfaces":
                    asImplementedInterfaces = (bool)(namedArg.Value.Value ?? true);
                    break;
                case "AsSelf":
                    asSelf = (bool)(namedArg.Value.Value ?? true);
                    break;
                case "RegisterInHierarchy":
                    registerInHierarchy = (bool)(namedArg.Value.Value ?? false);
                    break;
                case "AsTypes":
                    if (namedArg.Value.Kind == TypedConstantKind.Array) {
                        foreach (var val in namedArg.Value.Values) {
                            if (val.Value is ITypeSymbol typeSym) {
                                asTypes.Add(typeSym.ToDisplayString());
                            }
                        }
                    }
                    break;
            }
        }

        // Extract interfaces and component info
        var interfaceNames = type.AllInterfaces.Select(i => i.ToDisplayString()).ToArray();
        
        bool isComponent = IsSubclassOf(type, "UnityEngine.Component") || IsSubclassOf(type, "UnityEngine.MonoBehaviour");
        
        // v3.5: Use robust entry point detection
        bool isEntryPoint = interfaceNames.Any(i => NhemDangFugBixs.Generators.Utils.InterfaceUtils.IsVContainerEntryPoint(i));
        bool isExceptionHandler = interfaceNames.Any(i => i.EndsWith("IEntryPointExceptionHandler"));
        bool isBuildCallback = interfaceNames.Any(i => i.EndsWith("IBuildCallback"));
        bool isInstaller = interfaceNames.Any(i => i.EndsWith("IVContainerInstaller"));

        int installerOrder = 0;
        if (isInstaller && orderAttrSymbol != null) {
            var orderAttr = type.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, orderAttrSymbol));
            if (orderAttr != null && orderAttr.ConstructorArguments.Length > 0) {
                var arg = orderAttr.ConstructorArguments[0];
                if (arg.Value is int val) {
                    installerOrder = val;
                }
            }
        }

        return new ServiceInfo(
            type.ContainingNamespace?.ToDisplayString() ?? "",
            type.Name,
            lifetime,
            "Global", // Default scope name for mapping
            interfaceNames,
            isComponent,
            asImplementedInterfaces,
            asSelf,
            registerInHierarchy,
            asTypes.ToArray(),
            isEntryPoint,
            false, // isFactory - can add support if needed
            scopeTypeName,
            usesTypeSafeScope,
            isExceptionHandler,
            isBuildCallback,
            isInstaller,
            installerOrder
        );
    }

    private static bool IsSubclassOf(INamedTypeSymbol type, string baseTypeName) {
        var current = type.BaseType;
        while (current != null) {
            if (current.ToDisplayString() == baseTypeName) return true;
            current = current.BaseType;
        }
        return false;
    }
}
