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

namespace NhemDangFugBixs.Analyzers.Rules;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ViewComponentCodeFixProvider))]
[Shared]
public class ViewComponentCodeFixProvider : CodeFixProvider {
    private const string TitleAddAttribute = "Add [AutoRegisterIn] to View class";
    private const string TitleRegisterInInstaller = "Register Component in Installer";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ViewComponentRegistrationRule.ND110Id);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null) return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var parameter = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ParameterSyntax>().FirstOrDefault();
        if (parameter == null) return;

        // Get diagnostic arguments
        var typeName = diagnostic.Properties.GetValueOrDefault("TypeName");
        var viewInterface = diagnostic.Properties.GetValueOrDefault("ViewInterface");
        var viewImplementation = diagnostic.Properties.GetValueOrDefault("ViewImplementation");

        if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(viewInterface) || string.IsNullOrEmpty(viewImplementation)) return;

        // Register code fix: Add [AutoRegisterIn] attribute
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleAddAttribute,
                createChangedDocument: c => AddAttributeCommentAsync(context.Document, parameter, viewImplementation, c),
                equivalenceKey: TitleAddAttribute),
            diagnostic);

        // Register code fix: Register in Installer
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleRegisterInInstaller,
                createChangedDocument: c => AddInstallerRegistrationCommentAsync(context.Document, parameter, viewInterface, c),
                equivalenceKey: TitleRegisterInInstaller),
            diagnostic);
    }

    private async Task<Document> AddAttributeCommentAsync(
        Document document,
        ParameterSyntax parameter,
        string viewImplementation,
        CancellationToken cancellationToken) {
        
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var snippet = $@"
// 💡 Fix: Add [AutoRegisterIn] to {viewImplementation}:
// [AutoRegisterIn(typeof(GameScope))]
// public class {viewImplementation} : MonoBehaviour, I{viewImplementation.Replace("View", "")} {{ }}
";

        var trivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, snippet);
        var newParameter = parameter.WithLeadingTrivia(trivia);

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> AddInstallerRegistrationCommentAsync(
        Document document,
        ParameterSyntax parameter,
        string viewInterface,
        CancellationToken cancellationToken) {
        
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var snippet = $@"
// 💡 Fix: Add to your Installer Configure() method:
// builder.RegisterComponentInHierarchy<{viewInterface}>();
";

        var trivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, snippet);
        var newParameter = parameter.WithLeadingTrivia(trivia);

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }
}
