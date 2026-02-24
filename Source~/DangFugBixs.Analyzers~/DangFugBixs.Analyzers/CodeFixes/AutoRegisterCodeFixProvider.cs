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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AutoRegisterCodeFixProvider)), Shared]
public class AutoRegisterCodeFixProvider : CodeFixProvider {
    public override ImmutableArray<string> FixableDiagnosticIds 
        => ImmutableArray.Create(AutoRegisterRules.NHM001Id);

    public override FixAllProvider GetFixAllProvider() 
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()
            .OfType<ClassDeclarationSyntax>().FirstOrDefault();

        if (declaration == null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Fix class modifiers for [AutoRegister]",
                createChangedDocument: c => FixModifiersAsync(context.Document, declaration, c),
                equivalenceKey: nameof(AutoRegisterCodeFixProvider)),
            diagnostic);
    }

    private async Task<Document> FixModifiersAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken) {
        var newModifiers = classDeclaration.Modifiers
            .Where(m => !m.IsKind(SyntaxKind.StaticKeyword) && !m.IsKind(SyntaxKind.AbstractKeyword));

        var newClassDeclaration = classDeclaration.WithModifiers(SyntaxFactory.TokenList(newModifiers));
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

        return document.WithSyntaxRoot(newRoot);
    }
}
