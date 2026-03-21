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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ScopeReachabilityCodeFixProvider)), Shared]
public sealed class ScopeReachabilityCodeFixProvider : CodeFixProvider {
    private const string Title = "Move service to reachable scope";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ScopeReachabilityRule.ND006Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        var dependencyScope = diagnostic.Properties.GetValueOrDefault("DependencyScope");
        if (string.IsNullOrWhiteSpace(dependencyScope)) return;

        var serviceClass = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (serviceClass is null) return;

        var autoRegisterAttr = serviceClass.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(a => a.Name.ToString().Contains("AutoRegisterIn"));
        if (autoRegisterAttr is null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => UpdateScopeAsync(context.Document, autoRegisterAttr, dependencyScope, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> UpdateScopeAsync(
        Document document,
        AttributeSyntax autoRegisterAttr,
        string dependencyScope,
        CancellationToken cancellationToken) {

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var args = autoRegisterAttr.ArgumentList?.Arguments;
        if (!args.HasValue || args.Value.Count == 0) return document;

        var firstArg = args.Value[0];
        var updatedFirstArg = firstArg.WithExpression(
            SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(dependencyScope)));

        var newArgs = args.Value.Replace(firstArg, updatedFirstArg);
        var newAttribute = autoRegisterAttr.WithArgumentList(SyntaxFactory.AttributeArgumentList(newArgs));
        var newRoot = root.ReplaceNode(autoRegisterAttr, newAttribute);
        return document.WithSyntaxRoot(newRoot);
    }
}

