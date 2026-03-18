using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DuplicateContractRegistrationRule : DiagnosticAnalyzer {
    public const string ND112Id = "ND112";

    public static readonly DiagnosticDescriptor ND112 = new(
        ND112Id,
        "Duplicate contract registration",
        "Interface '{0}' is registered by multiple types in scope '{1}': {2}. This may cause ambiguous resolution.",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Detects when the same interface is registered by multiple types in the same scope, which may lead to ambiguous resolution.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ND112);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private static void AnalyzeCompilation(CompilationAnalysisContext context) {
        var contractRegistrations = new Dictionary<string, List<(string Type, string Scope, Location Location)>>();

        // Scan all types with [AutoRegisterIn]
        foreach (var syntaxTree in context.Compilation.SyntaxTrees) {
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            foreach (var classDecl in root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()) {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
                if (classSymbol == null) continue;

                var attr = classSymbol.GetAttributes()
                    .FirstOrDefault(ad => 
                        ad.AttributeClass?.ToDisplayString() == "NhemDangFugBixs.Attributes.AutoRegisterInAttribute");

                if (attr == null) continue;

                // Get scope name
                var scopeName = GetScopeName(attr);
                if (string.IsNullOrEmpty(scopeName)) continue;

                // Get contracts this type will register
                var contracts = GetRegisteredContracts(classSymbol, attr);

                foreach (var contract in contracts) {
                    var key = $"{scopeName}|{contract}";
                    if (!contractRegistrations.ContainsKey(key)) {
                        contractRegistrations[key] = new List<(string, string, Location)>();
                    }
                    contractRegistrations[key].Add((classSymbol.Name, scopeName ?? "Unknown", classSymbol.Locations[0]));
                }
            }
        }

        // Report duplicates
        foreach (var kvp in contractRegistrations.Where(x => x.Value.Count > 1)) {
            var parts = kvp.Key.Split('|');
            var contract = parts[1];
            var scope = parts[0];
            var types = string.Join(", ", kvp.Value.Select(x => x.Type));

            var location = kvp.Value[0].Location;
            context.ReportDiagnostic(Diagnostic.Create(
                ND112,
                location,
                contract,
                scope,
                types));
        }
    }

    private static string? GetScopeName(AttributeData attr) {
        if (attr.ConstructorArguments.Length > 0) {
            var arg = attr.ConstructorArguments[0];
            if (arg.Kind == TypedConstantKind.Type && arg.Value is INamedTypeSymbol scopeType) {
                return scopeType.Name;
            }
        }
        return null;
    }

    private static IEnumerable<string> GetRegisteredContracts(INamedTypeSymbol typeSymbol, AttributeData attr) {
        // Check AsTypes
        var asTypesArg = attr.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == "AsTypes")
            .Value;

        if (asTypesArg.Values != null && asTypesArg.Values.Length > 0) {
            // Explicit contracts specified
            foreach (var typedConstant in asTypesArg.Values) {
                if (typedConstant.Kind == TypedConstantKind.Type && typedConstant.Value is ITypeSymbol type) {
                    yield return type.Name;
                }
            }
            yield break;
        }

        // Check AsImplementedInterfaces
        var asImplementedInterfaces = true;
        foreach (var namedArg in attr.NamedArguments) {
            if (namedArg.Key == "AsImplementedInterfaces" && namedArg.Value.Value is bool value) {
                asImplementedInterfaces = value;
                break;
            }
        }

        if (asImplementedInterfaces) {
            // Return all non-lifecycle interfaces
            var lifecycleInterfaces = new[] {
                "VContainer.Unity.ITickable",
                "VContainer.Unity.IInitializable",
                "VContainer.Unity.IStartable",
                "VContainer.Unity.IPostFixedUpdate",
                "VContainer.Unity.IPostLateUpdate",
                "VContainer.Unity.IPostStart",
                "VContainer.Unity.IFixedTickable",
                "VContainer.Unity.ILateTickable",
                "VContainer.Unity.IEntryPointExceptionHandler",
                "VContainer.Unity.IBuildCallback"
            };

            foreach (var iface in typeSymbol.AllInterfaces.Where(i => !lifecycleInterfaces.Contains(i.ToDisplayString()))) {
                yield return iface.Name;
            }
        }
    }
}
