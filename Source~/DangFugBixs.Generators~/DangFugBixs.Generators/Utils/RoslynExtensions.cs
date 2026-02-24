using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NhemDangFugBixs.Generators.Utils;

public static class RoslynExtensions {
    public static string GetNamespace(this BaseTypeDeclarationSyntax syntax) {
        string nameSpace = string.Empty;

        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax &&
               potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax) {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent) {
            nameSpace = namespaceParent.Name.ToString();

            while (true) {
                if (namespaceParent.Parent is not BaseNamespaceDeclarationSyntax parent) {
                    break;
                }

                nameSpace = $"{parent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }
        
        return nameSpace;
    }

}
