using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NhemDangFugBixs.Analyzers.Rules;

namespace NhemDangFugBixs.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingContractRegistrationCodeFixProvider)), Shared]
public sealed class MissingContractRegistrationCodeFixProvider : CodeFixProvider {
    private const string Title = "Enable AsImplementedInterfaces";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MissingContractRegistrationRule.ND111Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        var classNode = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classNode is null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => EnableAsImplementedInterfacesAsync(context.Document, classNode, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> EnableAsImplementedInterfacesAsync(
        Document document,
        ClassDeclarationSyntax classNode,
        CancellationToken cancellationToken) {

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var attr = classNode.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(a => a.Name.ToString().Contains("AutoRegisterIn"));

        if (attr is null) return document;

        var argumentList = attr.ArgumentList ?? SyntaxFactory.AttributeArgumentList();
        var args = argumentList.Arguments;

        var existingNamed = args.FirstOrDefault(a =>
            a.NameEquals?.Name.Identifier.ValueText == "AsImplementedInterfaces");

        var trueArgument = SyntaxFactory.AttributeArgument(
                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
            .WithNameEquals(SyntaxFactory.NameEquals("AsImplementedInterfaces"));

        SeparatedSyntaxList<AttributeArgumentSyntax> newArgs;
        if (existingNamed != default) {
            var updated = existingNamed.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            newArgs = args.Replace(existingNamed, updated);
        } else {
            newArgs = args.Count == 0 ? args.Add(trueArgument) : args.Add(trueArgument);
        }

        var newAttr = attr.WithArgumentList(SyntaxFactory.AttributeArgumentList(newArgs));
        var newRoot = root.ReplaceNode(attr, newAttr);
        return document.WithSyntaxRoot(newRoot);
    }
}

