using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NhemDangFugBixs.Common.Utils;

internal static class SemanticScopeUtils {
    private const string AutoRegisterInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute";
    private const string AutoRegisterInGenericAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterInAttribute<TScope>";
    private const string AutoRegisterMessageBrokerInAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterMessageBrokerInAttribute";
    private const string AutoRegisterMessageBrokerInGenericAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterMessageBrokerInAttribute<TScope>";
    private const string LoggerFactoryTypeName = "Microsoft.Extensions.Logging.ILoggerFactory";
    private const string LoggerGenericTypeName = "Microsoft.Extensions.Logging.ILogger<TCategoryName>";
    private static readonly HashSet<string> RootScopeNames = new(StringComparer.Ordinal) {
        "LifetimeScope",
        "ProjectLifetimeScope",
        "GlobalLifetimeScope"
    };

    /// <summary>
    /// Gets the scope type symbol for a service marked with [AutoRegisterIn].
    /// </summary>
    public static INamedTypeSymbol? GetScopeSymbol(ITypeSymbol typeSymbol) {
        var attr = typeSymbol.GetAttributes().FirstOrDefault(ad => 
            ad.AttributeClass?.ToDisplayString() == AutoRegisterInAttributeName ||
            (ad.AttributeClass?.IsGenericType == true && ad.AttributeClass.OriginalDefinition.ToDisplayString() == AutoRegisterInGenericAttributeName));

        if (attr == null) return null;

        // Generic version: [AutoRegisterIn<GameScope>]
        if (attr.AttributeClass?.IsGenericType == true && attr.AttributeClass.TypeArguments.Length > 0) {
            return attr.AttributeClass.TypeArguments[0] as INamedTypeSymbol;
        }

        // Positional version: [AutoRegisterIn(typeof(GameScope))]
        if (attr.ConstructorArguments.Length > 0) {
            var arg = attr.ConstructorArguments[0];
            if (arg.Kind == TypedConstantKind.Type && arg.Value is INamedTypeSymbol scopeType) {
                return scopeType;
            }
        }

        return null;
    }

    public static ImmutableArray<INamedTypeSymbol> GetMessageBrokerScopes(ITypeSymbol typeSymbol) {
        var builder = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

        foreach (var attr in typeSymbol.GetAttributes()) {
            if (attr.AttributeClass?.ToDisplayString() == AutoRegisterMessageBrokerInAttributeName) {
                if (TryGetScopeFromAttribute(attr, out var positionalScope)) {
                    builder.Add(positionalScope);
                }
                continue;
            }

            if (attr.AttributeClass?.IsGenericType == true &&
                attr.AttributeClass.OriginalDefinition.ToDisplayString() == AutoRegisterMessageBrokerInGenericAttributeName &&
                attr.AttributeClass.TypeArguments.FirstOrDefault() is INamedTypeSymbol genericScope) {
                builder.Add(genericScope);
            }
        }

        return builder.ToImmutable();
    }

    public static bool TryGetMessagePipeDependency(
        ITypeSymbol typeSymbol,
        out INamedTypeSymbol? messageType,
        out string role) {
        role = string.Empty;
        messageType = null;

        if (typeSymbol is not INamedTypeSymbol namedType ||
            !namedType.IsGenericType ||
            namedType.TypeArguments.Length != 1) {
            return false;
        }

        var fullName = namedType.OriginalDefinition.ToDisplayString();
        if (fullName is "MessagePipe.IPublisher<T>" or "MessagePipe.ISubscriber<T>") {
            messageType = namedType.TypeArguments[0] as INamedTypeSymbol;
            role = namedType.Name == "IPublisher" ? "Publisher" : "Subscriber";
            return messageType != null;
        }

        return false;
    }

    public static bool TryGetLoggerDependency(
        ITypeSymbol typeSymbol,
        out INamedTypeSymbol? categoryType) {
        categoryType = null;

        if (typeSymbol is not INamedTypeSymbol namedType ||
            !namedType.IsGenericType ||
            namedType.TypeArguments.Length != 1) {
            return false;
        }

        if (namedType.OriginalDefinition.ToDisplayString() != LoggerGenericTypeName) {
            return false;
        }

        categoryType = namedType.TypeArguments[0] as INamedTypeSymbol;
        return categoryType != null;
    }

    public static bool IsLoggerFactoryType(ITypeSymbol typeSymbol) {
        return typeSymbol.ToDisplayString() == LoggerFactoryTypeName ||
               typeSymbol.AllInterfaces.Any(i => i.ToDisplayString() == LoggerFactoryTypeName);
    }

    public static bool IsLoggerAdapterType(ITypeSymbol typeSymbol) {
        if (typeSymbol is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.ToDisplayString() == LoggerGenericTypeName) {
            return true;
        }

        return typeSymbol.AllInterfaces.Any(i =>
            i is INamedTypeSymbol namedInterface &&
            namedInterface.OriginalDefinition.ToDisplayString() == LoggerGenericTypeName);
    }

    public static bool IsRootScopeName(string scopeName) {
        return RootScopeNames.Contains(scopeName);
    }

    /// <summary>
    /// Checks if a target scope is reachable from the current scope.
    /// In VContainer, a scope can resolve dependencies from itself or its parents.
    /// </summary>
    public static bool IsScopeReachable(INamedTypeSymbol currentScope, INamedTypeSymbol targetScope) {
        if (SymbolEqualityComparer.Default.Equals(currentScope, targetScope)) return true;

        // Global/Project scope is always reachable
        if (RootScopeNames.Contains(targetScope.Name)) return true;

        // Traverse up the scope hierarchy if possible 
        // (This requires a mapping of which scope is a parent of which, 
        // which we can infer from the scene hierarchy or explicit config).
        return false; 
    }

    private static bool TryGetScopeFromAttribute(AttributeData attr, out INamedTypeSymbol scopeType) {
        scopeType = null!;

        if (attr.ConstructorArguments.Length == 0) {
            return false;
        }

        var arg = attr.ConstructorArguments[0];
        if (arg.Kind == TypedConstantKind.Type && arg.Value is INamedTypeSymbol namedType) {
            scopeType = namedType;
            return true;
        }

        return false;
    }
}
