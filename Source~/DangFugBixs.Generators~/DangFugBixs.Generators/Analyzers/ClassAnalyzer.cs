using NhemDangFugBixs.Common.Models;
using NhemDangFugBixs.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Generators.Analyzers;

internal class ClassAnalyzer {
    private const string ExpectedAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterAttribute";
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1";
    private const string FactoryAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterFactoryAttribute";
    private const string SceneAttributeName = "NhemDangFugBixs.Attributes.AutoInjectSceneAttribute";
    private static readonly HashSet<string> ValidLifetimes = new() { "Singleton", "Transient", "Scoped" };
    private static readonly HashSet<string> EntryPointInterfaces = new() {
        "VContainer.Unity.IInitializable", "IInitializable",
        "VContainer.Unity.IPostInitializable", "IPostInitializable",
        "VContainer.Unity.IStartable", "IStartable",
        "VContainer.Unity.IPostStartable", "IPostStartable",
        "VContainer.Unity.IFixedTickable", "IFixedTickable",
        "VContainer.Unity.IPostFixedTickable", "IPostFixedTickable",
        "VContainer.Unity.ITickable", "ITickable",
        "VContainer.Unity.IPostTickable", "IPostTickable",
        "VContainer.Unity.ILateTickable", "ILateTickable",
        "VContainer.Unity.IPostLateTickable", "IPostLateTickable",
        "VContainer.Unity.IAsyncStartable", "IAsyncStartable",
        "System.IDisposable", "IDisposable"
    };

    public static ServiceInfo? ExtractInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        try {
            var classDecl = (ClassDeclarationSyntax)context.Node;

            // Check for AutoRegisterIn<TScope> generic attribute first
            var autoRegisterInAttr = classDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => {
                    string name = x.Name.ToString();
                    return name == "AutoRegisterIn" || name == "AutoRegisterInAttribute" ||
                           name.EndsWith(".AutoRegisterIn") || name.EndsWith(".AutoRegisterInAttribute");
                });

            if (autoRegisterInAttr != null) {
                return ExtractInfoFromGenericAttribute(context, classDecl, autoRegisterInAttr, cancellationToken);
            }

            // Fall back to legacy AutoRegister attribute
            var attr = classDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => {
                    string name = x.Name.ToString();
                    return name == "AutoRegister" || name == "AutoRegisterAttribute" ||
                           name.EndsWith(".AutoRegister") || name.EndsWith(".AutoRegisterAttribute") ||
                           name == "AutoRegisterFactory" || name == "AutoRegisterFactoryAttribute" ||
                           name.EndsWith(".AutoRegisterFactory") || name.EndsWith(".AutoRegisterFactoryAttribute");
                });

            if (attr == null) return null;

            return ExtractInfoFromLegacyAttribute(context, classDecl, attr, cancellationToken);
        } catch {
            return null;
        }
    }

    private static ServiceInfo? ExtractInfoFromGenericAttribute(
        GeneratorSyntaxContext context,
        ClassDeclarationSyntax classDecl,
        AttributeSyntax attr,
        CancellationToken cancellationToken)
    {
        var ns = classDecl.GetNamespace();

        // Extract scope type from generic argument
        string? scopeTypeName = null;
        bool usesTypeSafeScope = false;

        if (attr.Name is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0) {
            var scopeTypeArg = genericName.TypeArgumentList.Arguments[0];
            var scopeTypeSymbol = context.SemanticModel.GetTypeInfo(scopeTypeArg, cancellationToken).Type;
            if (scopeTypeSymbol != null) {
                scopeTypeName = scopeTypeSymbol.ToDisplayString();
                usesTypeSafeScope = true;
            }
        }

        // Extract lifetime from named argument
        string lifetime = "Singleton";
        if (attr.ArgumentList != null) {
            var lifetimeArg = attr.ArgumentList.Arguments
                .FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "Lifetime");
            if (lifetimeArg != null) {
                var lifetimeExpr = lifetimeArg.Expression;
                if (lifetimeExpr is MemberAccessExpressionSyntax memberAccess) {
                    lifetime = memberAccess.Name.Identifier.Text;
                }
            }
        }

        // Extract boolean properties
        bool asImplementedInterfaces = ExtractBooleanProperty(context, attr, "AsImplementedInterfaces", true, cancellationToken);
        bool asSelf = ExtractBooleanProperty(context, attr, "AsSelf", true, cancellationToken);
        bool registerInHierarchy = ExtractBooleanProperty(context, attr, "RegisterInHierarchy", false, cancellationToken);
        string[] asTypes = ExtractTypeArrayProperty(context, attr, "AsTypes", cancellationToken);

        // Extract interfaces and component info
        var (interfaceNames, isComponent, isEntryPoint) = ExtractClassInfo(context, classDecl, cancellationToken);

        return new ServiceInfo(
            ns, classDecl.Identifier.Text, lifetime, "Global",
            interfaceNames.ToArray(), isComponent, asImplementedInterfaces, asSelf,
            registerInHierarchy, asTypes, isEntryPoint, false,
            scopeTypeName, usesTypeSafeScope);
    }

    private static ServiceInfo? ExtractInfoFromLegacyAttribute(
        GeneratorSyntaxContext context,
        ClassDeclarationSyntax classDecl,
        AttributeSyntax attr,
        CancellationToken cancellationToken)
    {
        // Verify attribute type using semantic model (with fallback to name only)
        var attrSymbol = context.SemanticModel.GetSymbolInfo(attr, cancellationToken).Symbol?.ContainingType;
        var fullTypeName = attrSymbol?.ToDisplayString();

        bool isFactory = false;
        if (fullTypeName == FactoryAttributeName) {
            isFactory = true;
        } else if (fullTypeName != ExpectedAttributeName) {
            // Fallback: Check if the name matches exactly "AutoRegister" or "AutoRegisterAttribute"
            string attrName = attr.Name.ToString();
            if (attrName == "AutoRegisterFactory" || attrName == "AutoRegisterFactoryAttribute") {
                isFactory = true;
            } else if (attrName != "AutoRegister" && attrName != "AutoRegisterAttribute") {
                return null;
            }
        }

        var ns = classDecl.GetNamespace();

        // Extract lifetime using semantic model (handles named arguments)
        string lifetime = ExtractLifetime(context, attr, cancellationToken);

        // Extract scope using semantic model (handles named arguments)
        string scope = ExtractScope(context, attr, cancellationToken);

        // Extract boolean properties
        bool asImplementedInterfaces = ExtractBooleanProperty(context, attr, "AsImplementedInterfaces", true, cancellationToken);
        bool asSelf = ExtractBooleanProperty(context, attr, "AsSelf", true, cancellationToken);
        bool registerInHierarchy = ExtractBooleanProperty(context, attr, "RegisterInHierarchy", false, cancellationToken);
        string[] asTypes = ExtractTypeArrayProperty(context, attr, "AsTypes", cancellationToken);

        // Extract interfaces and component info
        var (interfaceNames, isComponent, isEntryPoint) = ExtractClassInfo(context, classDecl, cancellationToken);

        return new ServiceInfo(ns, classDecl.Identifier.Text, lifetime, scope, interfaceNames.ToArray(), isComponent, asImplementedInterfaces, asSelf, registerInHierarchy, asTypes, isEntryPoint, isFactory);
    }

    private static (List<string> interfaceNames, bool isComponent, bool isEntryPoint) ExtractClassInfo(
        GeneratorSyntaxContext context,
        ClassDeclarationSyntax classDecl,
        CancellationToken cancellationToken)
    {
        var interfaceNames = new List<string>();
        bool isComponent = false;
        bool isEntryPoint = false;

        if (classDecl.BaseList != null) {
            foreach (var baseType in classDecl.BaseList.Types) {
                var symbol = context.SemanticModel.GetTypeInfo(baseType.Type, cancellationToken).Type;
                if (symbol == null || symbol.TypeKind == TypeKind.Error) {
                    string rawName = baseType.Type.ToString();
                    if (rawName.StartsWith("I") && char.IsUpper(rawName.Length > 1 ? rawName[1] : 'a')) {
                        interfaceNames.Add(rawName);
                        if (EntryPointInterfaces.Contains(rawName)) {
                            isEntryPoint = true;
                        }
                    }
                    else if (rawName == "MonoBehaviour" || rawName == "Component" || rawName == "SerializedMonoBehaviour" || rawName == "NetworkBehaviour") {
                        isComponent = true;
                    }
                    continue;
                }

                if (symbol.TypeKind == TypeKind.Interface) {
                    string fullName = symbol.ToDisplayString();
                    interfaceNames.Add(fullName);
                    if (EntryPointInterfaces.Contains(fullName)) {
                        isEntryPoint = true;
                    }
                } else if (symbol.TypeKind == TypeKind.Class) {
                    var current = symbol;
                    while (current != null) {
                        if (current.ToDisplayString() == "UnityEngine.Component" || current.ToDisplayString() == "UnityEngine.MonoBehaviour") {
                            isComponent = true;
                            break;
                        }
                        current = current.BaseType;
                    }
                }
            }
        }

        return (interfaceNames, isComponent, isEntryPoint);
    }

    public static SceneInjectionInfo? ExtractSceneInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        
        try {
            var classDecl = (ClassDeclarationSyntax)context.Node;

            var attr = classDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => {
                    string name = x.Name.ToString();
                    return name == "AutoInjectScene" || name == "AutoInjectSceneAttribute" || 
                           name.EndsWith(".AutoInjectScene") || name.EndsWith(".AutoInjectSceneAttribute");
                });

            if (attr == null) return null;

            var attrSymbol = context.SemanticModel.GetSymbolInfo(attr, cancellationToken).Symbol?.ContainingType;
            if (attrSymbol?.ToDisplayString() != SceneAttributeName) {
                // Fallback: Check if the name matches exactly "AutoInjectScene" or "AutoInjectSceneAttribute"
                string attrName = attr.Name.ToString();
                if (attrName != "AutoInjectScene" && attrName != "AutoInjectSceneAttribute") {
                    return null;
                }
            }

            var ns = classDecl.GetNamespace();
            return new SceneInjectionInfo(ns, classDecl.Identifier.Text);
        } catch {
            return null;
        }
    }



    private static string ExtractLifetime(GeneratorSyntaxContext context, AttributeSyntax attr, CancellationToken cancellationToken) {
        if (attr.ArgumentList == null || attr.ArgumentList.Arguments.Count == 0) {
            return "Singleton"; // Default
        }

        // Find lifetime argument (either positional [0] or named "lifetime")
        AttributeArgumentSyntax? lifetimeArg = null;
        
        // First check for named argument
        lifetimeArg = attr.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "lifetime");
        
        // If not found, assume first positional argument is lifetime
        if (lifetimeArg == null && attr.ArgumentList.Arguments.Count > 0) {
            var firstArg = attr.ArgumentList.Arguments[0];
            // Only use it if it's NOT named "scope"
            if (firstArg.NameColon?.Name.Identifier.Text != "scope") {
                lifetimeArg = firstArg;
            }
        }

        if (lifetimeArg == null) {
            return "Singleton"; // Default if not found
        }

        var lifetimeExpr = lifetimeArg.Expression;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(lifetimeExpr, cancellationToken);
        
        // If it's an enum member, get its name
        if (symbolInfo.Symbol is IFieldSymbol fieldSymbol && fieldSymbol.ContainingType.TypeKind == TypeKind.Enum) {
            var lifetimeValue = fieldSymbol.Name;
            if (ValidLifetimes.Contains(lifetimeValue)) {
                return lifetimeValue;
            }
        }

        // Default fallback
        return "Singleton";
    }

    private static string ExtractScope(GeneratorSyntaxContext context, AttributeSyntax attr, CancellationToken cancellationToken) {
        if (attr.ArgumentList == null || attr.ArgumentList.Arguments.Count == 0) {
            return "Global"; // Default
        }

        // Find scope argument (either positional [1] or named "scope")
        AttributeArgumentSyntax? scopeArg = null;
        
        // First check for named argument
        scopeArg = attr.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "scope");
        
        // If not found, check if there's a second positional argument
        if (scopeArg == null && attr.ArgumentList.Arguments.Count > 1) {
            var secondArg = attr.ArgumentList.Arguments[1];
            // Only use it if it's NOT named "lifetime"
            if (secondArg.NameColon?.Name.Identifier.Text != "lifetime") {
                scopeArg = secondArg;
            }
        }

        if (scopeArg == null) {
            return "Global"; // Default if not found
        }

        var scopeExpr = scopeArg.Expression;
        
        // Use semantic model to get constant string value
        var constantValue = context.SemanticModel.GetConstantValue(scopeExpr, cancellationToken);
        
        if (constantValue.HasValue && constantValue.Value is string scopeStr) {
            return scopeStr;
        }

        // If not a constant, return default
        return "Global";
    }

    private static bool ExtractBooleanProperty(GeneratorSyntaxContext context, AttributeSyntax attr, string propertyName, bool defaultValue, CancellationToken cancellationToken) {
        if (attr.ArgumentList == null) return defaultValue;

        // Property assignments use NameEquals (e.g., AsSelf = false)
        var arg = attr.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == propertyName);

        if (arg == null) return defaultValue;

        var constantValue = context.SemanticModel.GetConstantValue(arg.Expression, cancellationToken);
        if (constantValue.HasValue && constantValue.Value is bool boolVal) {
            return boolVal;
        }

        // Fallback syntactic check
        if (arg.Expression is LiteralExpressionSyntax literal) {
            if (literal.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.TrueLiteralExpression)) return true;
            if (literal.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.FalseLiteralExpression)) return false;
        }

        return defaultValue;
    }

    private static string[] ExtractTypeArrayProperty(GeneratorSyntaxContext context, AttributeSyntax attr, string propertyName, CancellationToken cancellationToken) {
        if (attr.ArgumentList == null) return System.Array.Empty<string>();

        var arg = attr.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == propertyName);

        if (arg == null) return System.Array.Empty<string>();

        var types = new List<string>();

        // Handle new[] { typeof(T) }
        if (arg.Expression is ImplicitArrayCreationExpressionSyntax implicitArray) {
            foreach (var expr in implicitArray.Initializer.Expressions) {
                if (expr is TypeOfExpressionSyntax typeOfExpr) {
                    var typeSymbol = context.SemanticModel.GetSymbolInfo(typeOfExpr.Type, cancellationToken).Symbol as ITypeSymbol;
                    if (typeSymbol != null) {
                        types.Add(typeSymbol.ToDisplayString());
                    } else {
                        types.Add(typeOfExpr.Type.ToString()); // Fallback
                    }
                }
            }
        } 
        // Handle new Type[] { typeof(T) }
        else if (arg.Expression is ArrayCreationExpressionSyntax explicitArray) {
             if (explicitArray.Initializer != null) {
                 foreach (var expr in explicitArray.Initializer.Expressions) {
                     if (expr is TypeOfExpressionSyntax typeOfExpr) {
                        var typeSymbol = context.SemanticModel.GetSymbolInfo(typeOfExpr.Type, cancellationToken).Symbol as ITypeSymbol;
                        if (typeSymbol != null) {
                            types.Add(typeSymbol.ToDisplayString());
                        } else {
                            types.Add(typeOfExpr.Type.ToString()); // Fallback
                        }
                    }
                 }
             }
        }

        return types.ToArray();
    }
}