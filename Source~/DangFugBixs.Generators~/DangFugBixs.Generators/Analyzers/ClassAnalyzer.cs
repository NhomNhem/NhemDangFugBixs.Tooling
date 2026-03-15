using NhemDangFugBixs.Common.Models;
using NhemDangFugBixs.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Generators.Analyzers;

internal class ClassAnalyzer {
    private const string ExpectedAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterAttribute";
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute";
    private const string AutoRegisterInGenericAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1";
    private const string LifetimeScopeForAttributeName = "NhemDangFugBixs.Attributes.LifetimeScopeForAttribute";
    private const string LifetimeScopeForGenericAttributeName = "NhemDangFugBixs.Attributes.LifetimeScopeForAttribute`1";
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

    private static bool IsAttributeMatch(AttributeSyntax attr, string simpleName) {
        string fullName = attr.Name.ToString();
        string name = fullName;

        // If it's a generic name or qualified name, extract the simple name part
        if (attr.Name is GenericNameSyntax genericName) {
            name = genericName.Identifier.Text;
        } else if (attr.Name is QualifiedNameSyntax qualifiedName) {
            // Find the rightmost part which might be a generic name
            var current = qualifiedName.Right;
            if (current is GenericNameSyntax gn) {
                name = gn.Identifier.Text;
            } else {
                name = current.Identifier.Text;
            }
        } else if (fullName.Contains("<")) {
            // Fallback for other syntactic forms
            int index = fullName.IndexOf('<');
            name = fullName.Substring(0, index);
            if (name.Contains(".")) {
                name = name.Split('.').Last();
            }
        } else if (fullName.Contains(".")) {
            name = fullName.Split('.').Last();
        }

        return name == simpleName || name == $"{simpleName}Attribute";
    }

    public static ScopeMappingInfo? ExtractScopeMapping(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        try {
            var classDecl = (ClassDeclarationSyntax)context.Node;
            AttributeSyntax? mappingAttr = null;
            AttributeSyntax? nameAttr = null;

            foreach (var attrList in classDecl.AttributeLists) {
                foreach (var attr in attrList.Attributes) {
                    if (IsAttributeMatch(attr, "LifetimeScopeFor")) {
                        mappingAttr = attr;
                    } else if (IsAttributeMatch(attr, "ScopeName")) {
                        nameAttr = attr;
                    }
                }
            }

            if (mappingAttr == null) return null;

            string? identityTypeName = null;

            // 1. [LifetimeScopeFor<T>] (C# 11.0+)
            if (mappingAttr.Name is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0) {
                var identityTypeArg = genericName.TypeArgumentList.Arguments[0];
                var identityTypeSymbol = context.SemanticModel.GetTypeInfo(identityTypeArg, cancellationToken).Type;
                if (identityTypeSymbol != null) {
                    identityTypeName = identityTypeSymbol.ToDisplayString();
                }
            } 
            // 2. [Attributes.LifetimeScopeFor<T>] (C# 11.0+)
            else if (mappingAttr.Name is QualifiedNameSyntax qn && qn.Right is GenericNameSyntax gn && gn.TypeArgumentList.Arguments.Count > 0) {
                var identityTypeArg = gn.TypeArgumentList.Arguments[0];
                var identityTypeSymbol = context.SemanticModel.GetTypeInfo(identityTypeArg, cancellationToken).Type;
                if (identityTypeSymbol != null) {
                    identityTypeName = identityTypeSymbol.ToDisplayString();
                }
            }
            // 3. [LifetimeScopeFor(typeof(T))] (Compatible with all versions)
            else if (mappingAttr.ArgumentList != null && mappingAttr.ArgumentList.Arguments.Count > 0) {
                var arg = mappingAttr.ArgumentList.Arguments[0].Expression;
                if (arg is TypeOfExpressionSyntax typeOfExpr) {
                    var identityTypeSymbol = context.SemanticModel.GetTypeInfo(typeOfExpr.Type, cancellationToken).Type;
                    if (identityTypeSymbol != null) {
                        identityTypeName = identityTypeSymbol.ToDisplayString();
                    }
                }
            }

            if (string.IsNullOrEmpty(identityTypeName)) return null;

            // Extract custom name if present
            string className = classDecl.Identifier.Text;
            if (nameAttr != null && nameAttr.ArgumentList != null && nameAttr.ArgumentList.Arguments.Count > 0) {
                var nameExpr = nameAttr.ArgumentList.Arguments[0].Expression;
                var constantValue = context.SemanticModel.GetConstantValue(nameExpr, cancellationToken);
                if (constantValue.HasValue && constantValue.Value is string customName) {
                    className = $"{customName}LifetimeScope"; // Add suffix so RegistrationEmitter handles it normally
                }
            }

            var ns = classDecl.GetNamespace();
            return new ScopeMappingInfo(ns, className, identityTypeName!);
        } catch {
            return null;
        }
    }

    public static ServiceInfo? ExtractInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        try {
            var classDecl = (ClassDeclarationSyntax)context.Node;

            // Check for [AutoRegisterIn] attribute first (can be generic or typeof)
            var typeSafeAttr = classDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => IsAttributeMatch(x, "AutoRegisterIn"));

            if (typeSafeAttr != null) {
                return ExtractInfoFromTypeSafeAttribute(context, classDecl, typeSafeAttr, cancellationToken);
            }

            // Fall back to legacy AutoRegister attribute
            var attr = classDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => IsAttributeMatch(x, "AutoRegister") || IsAttributeMatch(x, "AutoRegisterFactory"));

            if (attr == null) return null;

            return ExtractInfoFromLegacyAttribute(context, classDecl, attr, cancellationToken);
        } catch {
            return null;
        }
    }

    private static ServiceInfo? ExtractInfoFromTypeSafeAttribute(
        GeneratorSyntaxContext context,
        ClassDeclarationSyntax classDecl,
        AttributeSyntax attr,
        CancellationToken cancellationToken)
    {
        var ns = classDecl.GetNamespace();

        // Extract scope type from generic argument OR positional argument
        string? scopeTypeName = null;
        bool usesTypeSafeScope = false;

        // 1. [AutoRegisterIn<T>] (C# 11.0+)
        if (attr.Name is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0) {
            var scopeTypeArg = genericName.TypeArgumentList.Arguments[0];
            var scopeTypeSymbol = context.SemanticModel.GetTypeInfo(scopeTypeArg, cancellationToken).Type;
            if (scopeTypeSymbol != null) {
                scopeTypeName = scopeTypeSymbol.ToDisplayString();
                usesTypeSafeScope = true;
            }
        } 
        // 2. [Attributes.AutoRegisterIn<T>] (C# 11.0+)
        else if (attr.Name is QualifiedNameSyntax qn && qn.Right is GenericNameSyntax gn && gn.TypeArgumentList.Arguments.Count > 0) {
            var scopeTypeArg = gn.TypeArgumentList.Arguments[0];
            var scopeTypeSymbol = context.SemanticModel.GetTypeInfo(scopeTypeArg, cancellationToken).Type;
            if (scopeTypeSymbol != null) {
                scopeTypeName = scopeTypeSymbol.ToDisplayString();
                usesTypeSafeScope = true;
            }
        }
        // 3. [AutoRegisterIn(typeof(T))] (Compatible with all versions)
        else if (attr.ArgumentList != null && attr.ArgumentList.Arguments.Count > 0) {
            var arg = attr.ArgumentList.Arguments[0].Expression;
            if (arg is TypeOfExpressionSyntax typeOfExpr) {
                var scopeTypeSymbol = context.SemanticModel.GetTypeInfo(typeOfExpr.Type, cancellationToken).Type;
                if (scopeTypeSymbol != null) {
                    scopeTypeName = scopeTypeSymbol.ToDisplayString();
                    usesTypeSafeScope = true;
                }
            }
        }

        // Extract lifetime from named argument
        string lifetime = "Singleton";
        if (attr.ArgumentList != null) {
            // Find named argument "Lifetime"
            var lifetimeArg = attr.ArgumentList.Arguments
                .FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == "Lifetime" || a.NameColon?.Name.Identifier.Text == "Lifetime");
            
            if (lifetimeArg != null) {
                var lifetimeExpr = lifetimeArg.Expression;
                if (lifetimeExpr is MemberAccessExpressionSyntax memberAccess) {
                    lifetime = memberAccess.Name.Identifier.Text;
                } else {
                    // Try to resolve constant if it's an enum
                    var symbolInfo = context.SemanticModel.GetSymbolInfo(lifetimeExpr, cancellationToken);
                    if (symbolInfo.Symbol is IFieldSymbol fieldSymbol) {
                        lifetime = fieldSymbol.Name;
                    }
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
            } else if (!IsAttributeMatch(attr, "AutoRegister")) {
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
                .FirstOrDefault(x => IsAttributeMatch(x, "AutoInjectScene"));

            if (attr == null) return null;

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