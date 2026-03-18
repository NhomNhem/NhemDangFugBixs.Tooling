using NhemDangFugBixs.Common.Models;
using NhemDangFugBixs.Common.Utils;
using NhemDangFugBixs.Generators.Utils;
using NhemDangFugBixs.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace NhemDangFugBixs.Generators.Analyzers;

internal class ClassAnalyzer {
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute";
    private const string AutoRegisterInGenericAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute`1";
    private const string LifetimeScopeForAttributeName = "NhemDangFugBixs.Attributes.LifetimeScopeForAttribute";
    private const string LifetimeScopeForGenericAttributeName = "NhemDangFugBixs.Attributes.LifetimeScopeForAttribute`1";
    private const string SceneAttributeName = "NhemDangFugBixs.Attributes.AutoInjectSceneAttribute";
    private const string InstallerOrderAttributeName = "NhemDangFugBixs.Attributes.InstallerOrderAttribute";
    private static readonly HashSet<string> ValidLifetimes = new() { "Singleton", "Transient", "Scoped" };

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
            string originalClassName = classDecl.Identifier.Text;
            return new ScopeMappingInfo(ns, className, identityTypeName!, originalClassName);
        } catch {
            return null;
        }
    }

    private const string AutoRegisterMessageBrokerInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterMessageBrokerInAttribute";

    public static ImmutableArray<ServiceInfo> ExtractInfos(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var results = new List<ServiceInfo>();
        try {
            var typeDecl = (TypeDeclarationSyntax)context.Node;

            // 1. Process [AutoRegisterIn] attributes
            var typeSafeAttrs = typeDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .Where(x => IsAttributeMatch(x, "AutoRegisterIn"));

            foreach (var attr in typeSafeAttrs) {
                var info = ExtractInfoFromTypeSafeAttribute(context, typeDecl, attr, cancellationToken);
                if (info != null) results.Add(info.Value);
            }

            // 2. Process [AutoRegisterMessageBrokerIn] attributes
            var brokerAttrs = typeDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .Where(x => IsAttributeMatch(x, "AutoRegisterMessageBrokerIn"));

            foreach (var attr in brokerAttrs) {
                var info = ExtractMessageBrokerInfo(context, typeDecl, attr, cancellationToken);
                if (info != null) results.Add(info.Value);
            }

        } catch {
            // Log error if needed
        }
        return results.ToImmutableArray();
    }

    private static ServiceInfo? ExtractMessageBrokerInfo(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeDecl,
        AttributeSyntax attr,
        CancellationToken cancellationToken)
    {
        var ns = typeDecl.GetNamespace();
        string? scopeTypeName = null;
        bool usesTypeSafeScope = false;

        // Extract scope type (Generic or TypeOf)
        if (attr.Name is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0) {
            var scopeTypeArg = genericName.TypeArgumentList.Arguments[0];
            var scopeTypeSymbol = context.SemanticModel.GetTypeInfo(scopeTypeArg, cancellationToken).Type;
            if (scopeTypeSymbol != null) {
                scopeTypeName = scopeTypeSymbol.ToDisplayString();
                usesTypeSafeScope = true;
            }
        } else if (attr.ArgumentList != null && attr.ArgumentList.Arguments.Count > 0) {
            var arg = attr.ArgumentList.Arguments[0].Expression;
            if (arg is TypeOfExpressionSyntax typeOfExpr) {
                var scopeTypeSymbol = context.SemanticModel.GetTypeInfo(typeOfExpr.Type, cancellationToken).Type;
                if (scopeTypeSymbol != null) {
                    scopeTypeName = scopeTypeSymbol.ToDisplayString();
                    usesTypeSafeScope = true;
                }
            }
        }

        // Extract lifetime
        string lifetime = ExtractLifetime(context, attr, cancellationToken);

        // Detect MessagePipe kind from implemented interfaces
        var messagePipeKind = MessagePipeType.Publisher; // Default
        var messageType = typeDecl.Identifier.Text; // Default to type name

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, cancellationToken) as INamedTypeSymbol;
        if (typeSymbol != null) {
            foreach (var iface in typeSymbol.AllInterfaces) {
                if (iface.IsGenericType && iface.OriginalDefinition.ToDisplayString() == "MessagePipe.IPublisher<T>") {
                    messagePipeKind = MessagePipeType.Publisher;
                    if (iface.TypeArguments.Length > 0) {
                        messageType = iface.TypeArguments[0].ToDisplayString();
                    }
                    break;
                }
                else if (iface.IsGenericType && iface.OriginalDefinition.ToDisplayString() == "MessagePipe.ISubscriber<T>") {
                    messagePipeKind = MessagePipeType.Subscriber;
                    if (iface.TypeArguments.Length > 0) {
                        messageType = iface.TypeArguments[0].ToDisplayString();
                    }
                    break;
                }
            }
        }

        return new ServiceInfo(
            ns, typeDecl.Identifier.Text, lifetime, "Global",
            new string[0], false, false, false,
            false, new string[0], false, false,
            scopeTypeName, usesTypeSafeScope, false, false, false, 0,
            true, messageType, messagePipeKind);
    }

    private static string ExtractLifetime(GeneratorSyntaxContext context, AttributeSyntax attr, CancellationToken cancellationToken) {
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
        return lifetime;
    }

    private static ServiceInfo? ExtractInfoFromTypeSafeAttribute(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeDecl,
        AttributeSyntax attr,
        CancellationToken cancellationToken)
    {
        var ns = typeDecl.GetNamespace();

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
        string lifetime = ExtractLifetime(context, attr, cancellationToken);

        // Extract boolean properties
        bool asImplementedInterfaces = ExtractBooleanProperty(context, attr, "AsImplementedInterfaces", true, cancellationToken);
        bool asSelf = ExtractBooleanProperty(context, attr, "AsSelf", true, cancellationToken);
        bool registerInHierarchy = ExtractBooleanProperty(context, attr, "RegisterInHierarchy", false, cancellationToken);
        string[] asTypes = ExtractTypeArrayProperty(context, attr, "AsTypes", cancellationToken);

        // Extract interfaces and component info
        var (interfaceNames, isComponent, isEntryPoint, isExceptionHandler, isBuildCallback, isInstaller, installerOrder) = ExtractClassInfo(context, typeDecl, cancellationToken);
        var metadata = ExtractMessagePipeConsumerMetadata(context, typeDecl, cancellationToken);
        foreach (var pair in ExtractLoggerConsumerMetadata(context, typeDecl, cancellationToken)) {
            metadata[pair.Key] = pair.Value;
        }

        return new ServiceInfo(
            ns, typeDecl.Identifier.Text, lifetime, "Global",
            interfaceNames.ToArray(), isComponent, asImplementedInterfaces, asSelf,
            registerInHierarchy, asTypes, isEntryPoint, false,
            scopeTypeName, usesTypeSafeScope, isExceptionHandler, isBuildCallback, isInstaller, installerOrder,
            false, null, MessagePipeType.Publisher, metadata);
    }

    private static (List<string> interfaceNames, bool isComponent, bool isEntryPoint, bool isExceptionHandler, bool isBuildCallback, bool isInstaller, int installerOrder) ExtractClassInfo(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeDecl,
        CancellationToken cancellationToken)
    {
        var interfaceNames = new List<string>();
        bool isComponent = false;
        bool isEntryPoint = false;
        bool isExceptionHandler = false;
        bool isBuildCallback = false;
        bool isInstaller = false;
        int installerOrder = 0;

        if (typeDecl.BaseList != null) {
            foreach (var baseType in typeDecl.BaseList.Types) {
                var symbol = context.SemanticModel.GetTypeInfo(baseType.Type, cancellationToken).Type;
                if (symbol == null || symbol.TypeKind == TypeKind.Error) {
                    string rawName = baseType.Type.ToString();
                    if (rawName.StartsWith("I") && char.IsUpper(rawName.Length > 1 ? rawName[1] : 'a')) {
                        interfaceNames.Add(rawName);
                        if (InterfaceUtils.IsVContainerEntryPoint(rawName)) {
                            isEntryPoint = true;
                        }
                        if (rawName.EndsWith("IEntryPointExceptionHandler")) {
                            isExceptionHandler = true;
                        }
                        if (rawName.EndsWith("IBuildCallback")) {
                            isBuildCallback = true;
                        }
                        if (rawName.EndsWith("IVContainerInstaller")) {
                            isInstaller = true;
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
                    if (InterfaceUtils.IsVContainerEntryPoint(fullName)) {
                        isEntryPoint = true;
                    }
                    if (fullName.EndsWith("IEntryPointExceptionHandler")) {
                        isExceptionHandler = true;
                    }
                    if (fullName.EndsWith("IBuildCallback")) {
                        isBuildCallback = true;
                    }
                    if (fullName.EndsWith("IVContainerInstaller")) {
                        isInstaller = true;
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

        // If it's an installer, look for [InstallerOrder]
        if (isInstaller) {
            var orderAttr = typeDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => IsAttributeMatch(x, "InstallerOrder"));

            if (orderAttr != null && orderAttr.ArgumentList != null && orderAttr.ArgumentList.Arguments.Count > 0) {
                var orderExpr = orderAttr.ArgumentList.Arguments[0].Expression;
                var constantValue = context.SemanticModel.GetConstantValue(orderExpr, cancellationToken);
                if (constantValue.HasValue && constantValue.Value is int orderValue) {
                    installerOrder = orderValue;
                }
            }
        }

        return (interfaceNames, isComponent, isEntryPoint, isExceptionHandler, isBuildCallback, isInstaller, installerOrder);
    }

    private static Dictionary<string, string> ExtractMessagePipeConsumerMetadata(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeDecl,
        CancellationToken cancellationToken) {
        var metadata = new Dictionary<string, string>();
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, cancellationToken) as INamedTypeSymbol;
        if (typeSymbol == null) {
            return metadata;
        }

        var consumers = new List<string>();

        foreach (var ctor in typeSymbol.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)) {
            foreach (var parameter in ctor.Parameters) {
                if (SemanticScopeUtils.TryGetMessagePipeDependency(parameter.Type, out var messageType, out var messagePipeType) &&
                    messageType != null) {
                    consumers.Add($"{messagePipeType}:{messageType.ToDisplayString()}");
                }
            }
        }

        if (consumers.Count > 0) {
            metadata["MessagePipeConsumers"] = string.Join(";", consumers.Distinct());
        }

        return metadata;
    }

    private static Dictionary<string, string> ExtractLoggerConsumerMetadata(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeDecl,
        CancellationToken cancellationToken) {
        var metadata = new Dictionary<string, string>();
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, cancellationToken) as INamedTypeSymbol;
        if (typeSymbol == null) {
            return metadata;
        }

        var consumers = new List<string>();
        foreach (var ctor in typeSymbol.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)) {
            foreach (var parameter in ctor.Parameters) {
                if (SemanticScopeUtils.TryGetLoggerDependency(parameter.Type, out var categoryType) && categoryType != null) {
                    consumers.Add(categoryType.ToDisplayString());
                }
            }
        }

        if (consumers.Count > 0) {
            metadata["LoggerConsumers"] = string.Join(";", consumers.Distinct());
        }

        return metadata;
    }

    public static SceneInjectionInfo? ExtractSceneInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        
        try {
            var typeDecl = (TypeDeclarationSyntax)context.Node;

            var attr = typeDecl.AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(x => IsAttributeMatch(x, "AutoInjectScene"));

            if (attr == null) return null;

            var ns = typeDecl.GetNamespace();
            return new SceneInjectionInfo(ns, typeDecl.Identifier.Text);
        } catch {
            return null;
        }
    }

    public static RootLoggingInfo? ExtractRootLoggingInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        if (context.Node is not ClassDeclarationSyntax classDecl) {
            return null;
        }

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDecl, cancellationToken) as INamedTypeSymbol;
        if (typeSymbol == null || !InheritsLifetimeScope(typeSymbol) || !SemanticScopeUtils.IsRootScopeName(typeSymbol.Name)) {
            return null;
        }

        var hasLoggerFactory = false;
        var hasLoggerAdapter = false;

        foreach (var method in classDecl.Members.OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.Text == "Configure")) {
            foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                foreach (var referencedType in GetReferencedTypes(context.SemanticModel, invocation, cancellationToken)) {
                    if (!hasLoggerFactory && SemanticScopeUtils.IsLoggerFactoryType(referencedType)) {
                        hasLoggerFactory = true;
                    }

                    if (!hasLoggerAdapter && SemanticScopeUtils.IsLoggerAdapterType(referencedType)) {
                        hasLoggerAdapter = true;
                    }
                }
            }
        }

        return new RootLoggingInfo(typeSymbol.Name, hasLoggerFactory, hasLoggerAdapter);
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

        return defaultValue;
    }

    private static bool InheritsLifetimeScope(INamedTypeSymbol typeSymbol) {
        var current = typeSymbol.BaseType;
        while (current != null) {
            if (current.Name == "LifetimeScope" || current.ToDisplayString() == "VContainer.Unity.LifetimeScope") {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static IEnumerable<ITypeSymbol> GetReferencedTypes(
        SemanticModel semanticModel,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken) {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name is GenericNameSyntax genericName) {
            foreach (var typeArg in genericName.TypeArgumentList.Arguments) {
                var type = semanticModel.GetTypeInfo(typeArg, cancellationToken).Type;
                if (type != null) {
                    yield return type;
                }
            }
        }

        foreach (var argument in invocation.ArgumentList.Arguments) {
            if (argument.Expression is TypeOfExpressionSyntax typeOfExpr) {
                var type = semanticModel.GetTypeInfo(typeOfExpr.Type, cancellationToken).Type;
                if (type != null) {
                    yield return type;
                }
            }
        }
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
