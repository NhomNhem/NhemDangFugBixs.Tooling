using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NhemDangFugBixs.Analyzers.Rules;

namespace NhemDangFugBixs.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DuplicateContractRegistrationCodeFixProvider)), Shared]
public sealed class DuplicateContractRegistrationCodeFixProvider : CodeFixProvider {
    private const string Title = "Remove duplicate registration type";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DuplicateContractRegistrationRule.ND112Id);

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
                createChangedDocument: c => RemoveClassAsync(context.Document, classNode, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> RemoveClassAsync(
        Document document,
        ClassDeclarationSyntax classNode,
        CancellationToken cancellationToken) {

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var newRoot = root.RemoveNode(classNode, SyntaxRemoveOptions.KeepNoTrivia);
        return newRoot is null ? document : document.WithSyntaxRoot(newRoot);
    }
}
