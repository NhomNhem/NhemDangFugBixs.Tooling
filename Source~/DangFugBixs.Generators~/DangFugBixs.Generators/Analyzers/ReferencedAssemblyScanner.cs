using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NhemDangFugBixs.Common.Models;

namespace NhemDangFugBixs.Generators.Analyzers;

internal static class ReferencedAssemblyScanner {
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1";

    public static List<ServiceInfo> Scan(Compilation compilation) {
        var results = new List<ServiceInfo>();

        // Get the attribute symbol from the compilation
        var attrSymbol = compilation.GetTypeByMetadataName(AutoRegisterInAttributeName);
        if (attrSymbol == null) return results;

        // Scan all referenced assemblies
        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols) {
            // Optimization: Only scan assemblies that reference our Runtime DLL
            // (In Unity, this is usually implied if they use our attributes)
            
            ScanNamespace(assembly.GlobalNamespace, attrSymbol, results);
        }

        return results;
    }

    private static void ScanNamespace(INamespaceSymbol ns, INamedTypeSymbol attrSymbol, List<ServiceInfo> results) {
        foreach (var member in ns.GetMembers()) {
            if (member is INamespaceSymbol nestedNs) {
                ScanNamespace(nestedNs, attrSymbol, results);
            } else if (member is INamedTypeSymbol type) {
                ScanType(type, attrSymbol, results);
            }
        }
    }

    private static void ScanType(INamedTypeSymbol type, INamedTypeSymbol attrSymbol, List<ServiceInfo> results) {
        // Scan nested types
        foreach (var nestedType in type.GetTypeMembers()) {
            ScanType(nestedType, attrSymbol, results);
        }

        // Check for AutoRegisterInAttribute
        var attr = type.GetAttributes().FirstOrDefault(a => 
            SymbolEqualityComparer.Default.Equals(a.AttributeClass?.OriginalDefinition, attrSymbol));

        if (attr != null) {
            var info = ExtractServiceInfo(type, attr);
            if (info.HasValue) {
                results.Add(info.Value);
            }
        }
    }

    private static ServiceInfo? ExtractServiceInfo(INamedTypeSymbol type, AttributeData attr) {
        // Extract Identity Type from generic argument
        var attrClass = attr.AttributeClass;
        if (attrClass == null || attrClass.TypeArguments.Length == 0) return null;

        var identityType = attrClass.TypeArguments[0];
        string scopeTypeName = identityType.ToDisplayString();

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
        
        // Simplified entry point check for metadata (can be improved)
        bool isEntryPoint = interfaceNames.Any(i => i.Contains("VContainer.Unity.I") && 
            (i.EndsWith("Initializable") || i.EndsWith("Startable") || i.EndsWith("Tickable") || i.EndsWith("Disposable")));

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
            true // usesTypeSafeScope
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
