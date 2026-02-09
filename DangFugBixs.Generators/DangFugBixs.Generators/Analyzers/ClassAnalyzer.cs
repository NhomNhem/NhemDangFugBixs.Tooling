using DangFugBixs.Common.Models;
using DangFugBixs.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DangFugBixs.Generators.Analyzers;

public class ClassAnalyzer {
    private const string ExpectedAttributeName = "NhemDangFugBixs.Attributes.AutoRegisterAttribute";
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
        
        return new ServiceInfo(ns, classDecl.Identifier.Text, lifetime, scope);
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