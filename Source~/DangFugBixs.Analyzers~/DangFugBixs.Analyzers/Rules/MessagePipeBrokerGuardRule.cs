using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NhemDangFugBixs.Common.Utils;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MessagePipeBrokerGuardRule : DiagnosticAnalyzer {
    public const string ND008Id = "ND008";

    public static readonly DiagnosticDescriptor ND008 = new(
        ND008Id,
        "Missing MessagePipe broker registration",
        "Type '{0}' in scope '{1}' depends on MessagePipe {2}<{3}>, but no reachable broker registration exists",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND008);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilationStart);
    }

    private static void RegisterCompilationStart(CompilationStartAnalysisContext context) {
        var brokers = CollectMessagePipeBrokers(context.Compilation.Assembly.GlobalNamespace);
        context.RegisterSymbolAction(symbolContext => AnalyzeNamedType(symbolContext, brokers), SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context, IReadOnlyList<BrokerRegistration> brokers) {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;
        var serviceScope = SemanticScopeUtils.GetScopeSymbol(typeSymbol);
        if (serviceScope == null) {
            return;
        }

        foreach (var ctor in typeSymbol.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)) {
            foreach (var parameter in ctor.Parameters) {
                if (!SemanticScopeUtils.TryGetMessagePipeDependency(parameter.Type, out var messageType, out var role) ||
                    messageType == null) {
                    continue;
                }

                var isSatisfied = brokers.Any(broker =>
                    SymbolEqualityComparer.Default.Equals(broker.MessageType, messageType) &&
                    SemanticScopeUtils.IsScopeReachable(serviceScope, broker.Scope));

                if (!isSatisfied && parameter.Locations.Length > 0) {
                    context.ReportDiagnostic(Diagnostic.Create(
                        ND008,
                        parameter.Locations[0],
                        typeSymbol.Name,
                        serviceScope.Name,
                        role,
                        messageType.ToDisplayString()));
                }
            }
        }
    }

    private static List<BrokerRegistration> CollectMessagePipeBrokers(INamespaceSymbol namespaceSymbol) {
        var brokers = new List<BrokerRegistration>();

        foreach (var member in namespaceSymbol.GetMembers()) {
            if (member is INamespaceSymbol childNamespace) {
                brokers.AddRange(CollectMessagePipeBrokers(childNamespace));
                continue;
            }

            if (member is not INamedTypeSymbol typeSymbol) {
                continue;
            }

            foreach (var scope in SemanticScopeUtils.GetMessageBrokerScopes(typeSymbol)) {
                brokers.Add(new BrokerRegistration(typeSymbol, scope));
            }
        }

        return brokers;
    }

    private readonly struct BrokerRegistration {
        public INamedTypeSymbol MessageType { get; }
        public INamedTypeSymbol Scope { get; }

        public BrokerRegistration(INamedTypeSymbol messageType, INamedTypeSymbol scope) {
            MessageType = messageType;
            Scope = scope;
        }
    }
}
