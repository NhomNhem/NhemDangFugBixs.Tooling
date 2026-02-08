using DangFugBixs.Generators.Models;
using DangFugBixs.Generators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DangFugBixs.Generators.Analyzers;

public class ClassAnalyzer {
    public static ServiceInfo? ExtractInfo(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        var classDecl = (ClassDeclarationSyntax)context.Node;

        var attr = classDecl.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(x => x.Name.ToString() == "AutoRegister");

        if (attr == null) return null;
        
        var ns = classDecl.GetNamespace();

        string lifetime = "Singleton";

        if (attr.ArgumentList != null && attr.ArgumentList.Arguments.Count > 0) {
            var arg = attr.ArgumentList.Arguments[0].ToString();
            if (arg.Contains("Transient")) lifetime = "Transient";
            if (arg.Contains("Scoped")) lifetime = "Scoped";
        }
        
        return new ServiceInfo(ns, classDecl.Identifier.Text, lifetime);
    }
}