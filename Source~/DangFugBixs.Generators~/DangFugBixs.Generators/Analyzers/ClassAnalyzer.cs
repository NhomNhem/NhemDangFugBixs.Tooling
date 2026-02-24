using NhemDangFugBixs.Common.Models;
using NhemDangFugBixs.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Generators.Analyzers;

internal class ClassAnalyzer {
    private const string ExpectedAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterAttribute";
    private const string SceneAttributeName = "NhemDangFugBixs.Attributes.AutoInjectSceneAttribute";
    private static readonly HashSet<string> ValidLifetimes = new() { "Singleton", "Transient", "Scoped" };

    public static ServiceInfo? ExtractInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        
        var classDecl = (ClassDeclarationSyntax)context.Node;

        var attr = classDecl.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(x => x.Name.ToString() == "AutoRegister");

        if (attr == null) return null;

        // Verify attribute type using semantic model
        var attrSymbol = context.SemanticModel.GetSymbolInfo(attr, cancellationToken).Symbol?.ContainingType;
        var fullTypeName = attrSymbol?.ToDisplayString();
        
        if (fullTypeName != ExpectedAttributeName) {
            // Not the attribute we're looking for, or couldn't resolve
            return null;
        }

        var ns = classDecl.GetNamespace();
        
        // Extract lifetime using semantic model (handles named arguments)
        string lifetime = ExtractLifetime(context, attr, cancellationToken);
        
        // Extract scope using semantic model (handles named arguments)
        string scope = ExtractScope(context, attr, cancellationToken);

        // Extract ALL interfaces and check for MonoBehavior/Component
        var interfaceNames = new List<string>();
        bool isComponent = false;

        if (classDecl.BaseList != null) {
            foreach (var baseType in classDecl.BaseList.Types) {
                var symbol = context.SemanticModel.GetTypeInfo(baseType.Type, cancellationToken).Type;
                if (symbol == null) continue;

                if (symbol.TypeKind == TypeKind.Interface) {
                    interfaceNames.Add(symbol.ToDisplayString());
                } else if (symbol.TypeKind == TypeKind.Class) {
                    // Check if it's a Component (directly or indirectly)
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
        
        return new ServiceInfo(ns, classDecl.Identifier.Text, lifetime, scope, interfaceNames.ToArray(), isComponent);
    }

    public static SceneInjectionInfo? ExtractSceneInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        
        var classDecl = (ClassDeclarationSyntax)context.Node;

        var attr = classDecl.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(x => x.Name.ToString() == "AutoInjectScene");

        if (attr == null) return null;

        var attrSymbol = context.SemanticModel.GetSymbolInfo(attr, cancellationToken).Symbol?.ContainingType;
        if (attrSymbol?.ToDisplayString() != SceneAttributeName) {
            return null;
        }

        var ns = classDecl.GetNamespace();
        return new SceneInjectionInfo(ns, classDecl.Identifier.Text);
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
}