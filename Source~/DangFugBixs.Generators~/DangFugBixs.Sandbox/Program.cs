using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program {
    static void Main() {
        string code = "[AutoRegister(Lifetime.Singleton, AsSelf = false)] class Test {}";
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();
        var attr = root.DescendantNodes().OfType<AttributeSyntax>().First();
        
        foreach (var arg in attr.ArgumentList.Arguments) {
            Console.WriteLine($"Arg: {arg}");
            if (arg.NameEquals != null) {
                Console.WriteLine($"  NameEquals: {arg.NameEquals.Name.Identifier.Text}");
            } else {
                Console.WriteLine($"  No NameEquals");
            }
            if (arg.NameColon != null) {
                Console.WriteLine($"  NameColon: {arg.NameColon.Name.Identifier.Text}");
            }
            
            Console.WriteLine($"  Expression: {arg.Expression.GetType().Name} -> {arg.Expression}");
        }
    }
}