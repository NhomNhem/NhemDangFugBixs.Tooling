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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConflictCheckCodeFixProvider)), Shared]
public sealed class ConflictCheckCodeFixProvider : CodeFixProvider {
    private const string Title = "Remove manual registration";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConflictCheckRule.ND005Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var invocation = node.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        var statement = invocation?.FirstAncestorOrSelf<StatementSyntax>();
        if (statement is null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => RemoveStatementAsync(context.Document, statement, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> RemoveStatementAsync(Document document, StatementSyntax statement, CancellationToken cancellationToken) {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var newRoot = root.RemoveNode(statement, SyntaxRemoveOptions.KeepNoTrivia);
        return newRoot is null ? document : document.WithSyntaxRoot(newRoot);
    }
}

