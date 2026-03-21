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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SceneViewBindingMismatchCodeFixProvider)), Shared]
public sealed class SceneViewBindingMismatchCodeFixProvider : CodeFixProvider {
    private const string TitleAddAttribute = "Add [AutoRegisterIn] to View class";
    private const string TitleAddManualRegistration = "Add manual view registration hint";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SceneViewBindingMismatchRule.ND113Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        var scopeName = diagnostic.Properties.GetValueOrDefault("ScopeName");
        var viewInterface = diagnostic.Properties.GetValueOrDefault("ViewInterface");
        var viewImplementation = diagnostic.Properties.GetValueOrDefault("ViewImplementation");

        if (string.IsNullOrWhiteSpace(scopeName) || string.IsNullOrWhiteSpace(viewInterface)) return;

        if (!string.IsNullOrWhiteSpace(viewImplementation)) {
            var viewClass = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.ValueText == viewImplementation);

            if (viewClass is not null) {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: TitleAddAttribute,
                        createChangedDocument: c => AddAttributeAsync(context.Document, viewClass, scopeName!, c),
                        equivalenceKey: TitleAddAttribute),
                    diagnostic);
            }
        }

        var parameter = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ParameterSyntax>();
        if (parameter is null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleAddManualRegistration,
                createChangedDocument: c => AddManualHintAsync(context.Document, parameter, viewInterface!, c),
                equivalenceKey: TitleAddManualRegistration),
            diagnostic);
    }

    private static async Task<Document> AddAttributeAsync(
        Document document,
        ClassDeclarationSyntax viewClass,
        string scopeName,
        CancellationToken cancellationToken) {

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var autoRegisterAttr = viewClass.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(a => a.Name.ToString().Contains("AutoRegisterIn"));

        var hierarchyArgument = SyntaxFactory.AttributeArgument(
                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
            .WithNameEquals(SyntaxFactory.NameEquals("RegisterInHierarchy"));

        AttributeSyntax replacement;
        if (autoRegisterAttr is not null) {
            var args = autoRegisterAttr.ArgumentList?.Arguments ?? default;
            if (args.Count == 0) return document;

            var firstArg = args[0];
            var updatedFirstArg = firstArg.WithExpression(
                SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(scopeName)));

            var updatedArgs = args.Replace(firstArg, updatedFirstArg);
            var existingHierarchy = updatedArgs.FirstOrDefault(a => a.NameEquals?.Name.Identifier.ValueText == "RegisterInHierarchy");
            if (existingHierarchy == default) {
                updatedArgs = updatedArgs.Add(hierarchyArgument);
            } else {
                updatedArgs = updatedArgs.Replace(existingHierarchy, hierarchyArgument);
            }

            replacement = autoRegisterAttr.WithArgumentList(SyntaxFactory.AttributeArgumentList(updatedArgs));
            var updatedRoot = root.ReplaceNode(autoRegisterAttr, replacement);
            return document.WithSyntaxRoot(updatedRoot);
        }

        var newAttribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName("NhemDangFugBixs.Attributes.AutoRegisterIn"),
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SeparatedList(new[] {
                    SyntaxFactory.AttributeArgument(
                        SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(scopeName))),
                    hierarchyArgument
                })));

        var newAttributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(newAttribute))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        var newViewClass = viewClass.AddAttributeLists(newAttributeList);
        var newRoot = root.ReplaceNode(viewClass, newViewClass);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> AddManualHintAsync(
        Document document,
        ParameterSyntax parameter,
        string viewInterface,
        CancellationToken cancellationToken) {

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var trivia = SyntaxFactory.Comment($"// Fix: builder.RegisterComponentInHierarchy<{viewInterface}>();");
        var newParameter = parameter.WithLeadingTrivia(parameter.GetLeadingTrivia().Add(trivia).Add(SyntaxFactory.CarriageReturnLineFeed));
        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }
}
