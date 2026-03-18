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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MessagePipeBrokerCodeFixProvider))]
[Shared]
public class MessagePipeBrokerCodeFixProvider : CodeFixProvider {
    private const string TitleRegisterBroker = "Register MessagePipe broker in scope";
    private const string TitleManualRegistration = "Add manual broker registration";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MessagePipeBrokerGuardRule.ND008Id);

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
        var messageType = diagnostic.Properties.GetValueOrDefault("MessageType");

        if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(scopeName) || string.IsNullOrEmpty(messageType)) return;

        // Register code fix: Auto-register broker
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleRegisterBroker,
                createChangedDocument: c => AddBrokerAttributeAsync(context.Document, parameter, scopeName, messageType, c),
                equivalenceKey: TitleRegisterBroker),
            diagnostic);

        // Register code fix: Manual registration comment
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleManualRegistration,
                createChangedDocument: c => AddManualRegistrationCommentAsync(context.Document, parameter, scopeName, messageType, c),
                equivalenceKey: TitleManualRegistration),
            diagnostic);
    }

    private async Task<Document> AddBrokerAttributeAsync(
        Document document,
        ParameterSyntax parameter,
        string scopeName,
        string messageType,
        CancellationToken cancellationToken) {
        
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        // Find the class containing this parameter
        var classDecl = parameter.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDecl == null) return document;

        // Create the broker class
        var brokerClassName = $"{messageType}Broker";
        
        // Generate snippet as comment (user will create the actual class)
        var snippet = $@"
// 💡 Fix: Create a broker class for {messageType}
// [AutoRegisterMessageBrokerIn(typeof({scopeName}))]
// public class {brokerClassName} : IPublisher<{messageType}>, ISubscriber<{messageType}> {{ }}
";

        var trivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, snippet);
        var newParameter = parameter.WithLeadingTrivia(trivia);

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> AddManualRegistrationCommentAsync(
        Document document,
        ParameterSyntax parameter,
        string scopeName,
        string messageType,
        CancellationToken cancellationToken) {
        
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var snippet = $@"
// 💡 Fix: Add to your {scopeName} Configure() method:
// builder.RegisterMessageBroker<{messageType}>(Lifetime.Singleton);
";

        var trivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, snippet);
        var newParameter = parameter.WithLeadingTrivia(trivia);

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return document.WithSyntaxRoot(newRoot);
    }
}
