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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ILoggerRootCodeFixProvider))]
[Shared]
public class ILoggerRootCodeFixProvider : CodeFixProvider {
    private const string TitleAddLoggerFactory = "Add LoggerFactory registration to scope";
    private const string TitleNavigateToRoot = "Navigate to root scope";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ILoggerRootGuardRule.ND009Id);

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
        var scopeName = diagnostic.Properties.GetValueOrDefault("ScopeName");
        var categoryType = diagnostic.Properties.GetValueOrDefault("CategoryType");

        if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(scopeName) || string.IsNullOrEmpty(categoryType)) return;

        // Register code fix: Add LoggerFactory registration
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleAddLoggerFactory,
                createChangedDocument: c => AddLoggerFactoryCommentAsync(context.Document, parameter, scopeName, categoryType, c),
                equivalenceKey: TitleAddLoggerFactory),
            diagnostic);

        // Register code fix: Show navigation hint
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleNavigateToRoot,
                createChangedDocument: c => AddNavigationCommentAsync(context.Document, parameter, scopeName, c),
                equivalenceKey: TitleNavigateToRoot),
            diagnostic);
    }

    private async Task<Document> AddLoggerFactoryCommentAsync(
        Document document,
        ParameterSyntax parameter,
        string scopeName,
        string categoryType,
        CancellationToken cancellationToken) {
        
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var snippet = $@"
// 💡 Fix: Add to your {scopeName} Configure() method:
// builder.Register<LoggerFactory>(Lifetime.Singleton).As<ILoggerFactory>();
// builder.Register<LoggerAdapter<{categoryType}>>(Lifetime.Singleton);
";

        var trivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, snippet);
        var newParameter = parameter.WithLeadingTrivia(trivia);

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> AddNavigationCommentAsync(
        Document document,
        ParameterSyntax parameter,
        string scopeName,
        CancellationToken cancellationToken) {
        
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var rootScopeName = GetRootScopeName(scopeName);
        var snippet = $@"
// 💡 Fix: Register logging infrastructure in {rootScopeName}:
// 1. Open {rootScopeName}.cs
// 2. In Configure() method, add:
//    builder.Register<LoggerFactory>(Lifetime.Singleton).As<ILoggerFactory>();
//    builder.Register<LoggerAdapter<T>>(Lifetime.Singleton);
";

        var trivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, snippet);
        var newParameter = parameter.WithLeadingTrivia(trivia);

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }

    private static string GetRootScopeName(string scopeName) {
        // Common root scope naming patterns
        var index = scopeName.IndexOf("LifetimeScope", System.StringComparison.Ordinal);
        if (index >= 0) {
            var prefix = scopeName.Substring(0, index);
            return prefix + "LifetimeScope";
        }
        return "ProjectLifetimeScope"; // Default fallback
    }
}
