using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ScopeMappingAnalyzer : DiagnosticAnalyzer {
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticCatalog.MissingScopeMapping,
        DiagnosticCatalog.MissingGeneratedCall,
        DiagnosticCatalog.WrongGeneratedCall,
        DiagnosticCatalog.DuplicateGeneratedInvocation);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(static startContext => {
            var autoServices = new List<(INamedTypeSymbol Type, ITypeSymbol? Marker)>();
            var scopeMappings = new List<(INamedTypeSymbol Type, ITypeSymbol Marker)>();
            var gate = new object();

            startContext.RegisterSymbolAction(symbolContext => {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                if (type.TypeKind != TypeKind.Class) {
                    return;
                }

                var autoRegister = type.GetAttributes().FirstOrDefault(attr => IsAttribute(attr, "AutoRegisterInAttribute"));
                if (autoRegister != null) {
                    lock (gate) {
                        autoServices.Add((type, TryGetMarkerType(autoRegister)));
                    }
                }

                var scopeMapping = type.GetAttributes().FirstOrDefault(attr => IsAttribute(attr, "LifetimeScopeForAttribute"));
                var marker = TryGetMarkerType(scopeMapping);
                if (scopeMapping != null && marker != null) {
                    lock (gate) {
                        scopeMappings.Add((type, marker));
                    }
                }
            }, SymbolKind.NamedType);

            startContext.RegisterCompilationEndAction(endContext => {
                AnalyzeMappings(endContext, autoServices, scopeMappings);
                AnalyzeConfigureCalls(endContext, scopeMappings);
            });
        });
    }

    private static void AnalyzeMappings(
        CompilationAnalysisContext context,
        List<(INamedTypeSymbol Type, ITypeSymbol? Marker)> autoServices,
        List<(INamedTypeSymbol Type, ITypeSymbol Marker)> scopeMappings) {
        var markers = new HashSet<ITypeSymbol>(scopeMappings.Select(m => m.Marker), SymbolEqualityComparer.Default);

        foreach (var service in autoServices.Where(s => s.Marker != null)) {
            if (!markers.Contains(service.Marker!)) {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticCatalog.MissingScopeMapping,
                    service.Type.Locations.FirstOrDefault(),
                    service.Marker!.ToDisplayString()));
            }
        }
    }

    private static void AnalyzeConfigureCalls(
        CompilationAnalysisContext context,
        List<(INamedTypeSymbol Type, ITypeSymbol Marker)> scopeMappings) {
        foreach (var mapping in scopeMappings) {
            foreach (var syntaxRef in mapping.Type.DeclaringSyntaxReferences) {
                if (syntaxRef.GetSyntax() is not ClassDeclarationSyntax classDecl) {
                    continue;
                }

                var model = context.Compilation.GetSemanticModel(classDecl.SyntaxTree);
                var configureMethod = classDecl.Members
                    .OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault(method => method.Identifier.Text == "Configure");
                if (configureMethod == null) {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticCatalog.MissingGeneratedCall,
                        mapping.Type.Locations.FirstOrDefault(),
                        mapping.Type.Name,
                        mapping.Marker.ToDisplayString()));
                    continue;
                }

                var generatedInvocations = new List<ITypeSymbol>();
                int installerInvocationCount = 0;

                foreach (var invocation in configureMethod.DescendantNodes().OfType<InvocationExpressionSyntax>()) {
                    var symbol = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (symbol == null) {
                        continue;
                    }

                    if (symbol.Name == "RegisterGeneratedFor" && symbol.IsGenericMethod && symbol.TypeArguments.Length == 1) {
                        generatedInvocations.Add(symbol.TypeArguments[0]);
                    }

                    if (symbol.Name == "Register" &&
                        symbol.ContainingType?.Name.StartsWith("NhemGenerated", System.StringComparison.Ordinal) == true &&
                        symbol.ContainingType.Name.EndsWith("Installer", System.StringComparison.Ordinal)) {
                        installerInvocationCount++;
                    }
                }

                bool hasCorrectCall = generatedInvocations.Any(marker => SymbolEqualityComparer.Default.Equals(marker, mapping.Marker));
                var wrongCalls = generatedInvocations
                    .Where(marker => !SymbolEqualityComparer.Default.Equals(marker, mapping.Marker))
                    .ToList();
                int generatedCount = generatedInvocations.Count(marker => SymbolEqualityComparer.Default.Equals(marker, mapping.Marker));
                int totalGeneratedPaths = generatedCount + installerInvocationCount;

                if (!hasCorrectCall && wrongCalls.Count == 0 && installerInvocationCount == 0) {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticCatalog.MissingGeneratedCall,
                        configureMethod.Identifier.GetLocation(),
                        mapping.Type.Name,
                        mapping.Marker.ToDisplayString()));
                }

                foreach (var wrongCall in wrongCalls) {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticCatalog.WrongGeneratedCall,
                        configureMethod.Identifier.GetLocation(),
                        mapping.Type.Name,
                        mapping.Marker.ToDisplayString(),
                        wrongCall.ToDisplayString()));
                }

                if (totalGeneratedPaths > 1) {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticCatalog.DuplicateGeneratedInvocation,
                        configureMethod.Identifier.GetLocation(),
                        mapping.Type.Name,
                        mapping.Marker.ToDisplayString()));
                }
            }
        }
    }

    private static bool IsAttribute(AttributeData? attr, string name) {
        if (attr?.AttributeClass == null) {
            return false;
        }

        return attr.AttributeClass.Name == name ||
               (attr.AttributeClass.IsGenericType && attr.AttributeClass.OriginalDefinition.Name == name.Replace("Attribute", "Attribute`1"));
    }

    private static ITypeSymbol? TryGetMarkerType(AttributeData? attr) {
        if (attr?.AttributeClass?.IsGenericType == true && attr.AttributeClass.TypeArguments.Length > 0) {
            return attr.AttributeClass.TypeArguments[0];
        }

        if (attr != null &&
            attr.ConstructorArguments.Length > 0 &&
            attr.ConstructorArguments[0].Kind == TypedConstantKind.Type &&
            attr.ConstructorArguments[0].Value is ITypeSymbol typeSymbol) {
            return typeSymbol;
        }

        return null;
    }
}
