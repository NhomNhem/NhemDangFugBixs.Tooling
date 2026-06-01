using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NhemDangFugBixs.Common.Models.DiContractGraph;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SemanticScopeDependencyAnalyzer : DiagnosticAnalyzer {
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DiagnosticCatalog.CrossScopeDependency);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(static startContext => {
            var services = new List<ServiceFact>();
            var gate = new object();

            startContext.RegisterSymbolAction(symbolContext => {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                if (type.TypeKind != TypeKind.Class) {
                    return;
                }

                var scope = TryGetAutoRegisterScope(type);
                if (scope == null) {
                    return;
                }

                lock (gate) {
                    services.Add(new ServiceFact(type, scope));
                }
            }, SymbolKind.NamedType);

            startContext.RegisterCompilationEndAction(endContext => Analyze(endContext, services));
        });
    }

    private static void Analyze(CompilationAnalysisContext context, IReadOnlyList<ServiceFact> services) {
        if (services.Count < 2) {
            return;
        }

        var registrationByImplementation = new Dictionary<string, GraphServiceFact>();
        var registrationByContract = new Dictionary<string, List<GraphServiceFact>>();
        var graphServices = new List<DiServiceRegistration>();

        foreach (var service in services) {
            var implementation = ToIdentity(service.Type);
            var scope = ToIdentity(service.Scope);
            var registration = new DiServiceRegistration(
                implementation,
                service.Type.AllInterfaces.Select(ToIdentity),
                GetLifetime(service.Type),
                DiAssemblyProvenance.CurrentCompilation(service.Type.ContainingAssembly?.Name ?? string.Empty),
                scope);
            var fact = new GraphServiceFact(service, registration);
            graphServices.Add(registration);
            registrationByImplementation[implementation.MetadataName] = fact;

            foreach (var contract in registration.ContractTypes) {
                if (!registrationByContract.TryGetValue(contract.MetadataName, out var list)) {
                    list = new List<GraphServiceFact>();
                    registrationByContract[contract.MetadataName] = list;
                }

                list.Add(fact);
            }
        }

        var graph = new DiContractGraph(services: graphServices);
        foreach (var service in services) {
            var sourceScope = ToIdentity(service.Scope);
            var sourceRegistration = graph.ServicesForScope(sourceScope)
                .FirstOrDefault(registration => registration.ImplementationType.MetadataName == ToIdentity(service.Type).MetadataName);
            if (sourceRegistration == null) {
                continue;
            }

            foreach (var dependency in ConstructorDependencyTypes(service.Type)) {
                foreach (var target in ResolveDependencyTargets(dependency, registrationByImplementation, registrationByContract)) {
                    var targetScope = target.Registration.ScopeMarkerType;
                    if (!targetScope.HasValue || targetScope.Value.Equals(sourceScope)) {
                        continue;
                    }

                    if (!graph.ServicesForScope(targetScope.Value).Any(registration => registration.ImplementationType.Equals(target.Registration.ImplementationType))) {
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticCatalog.CrossScopeDependency,
                        dependency.Locations.FirstOrDefault() ?? service.Type.Locations.FirstOrDefault(),
                        service.Type.Name,
                        sourceScope.FullName,
                        target.Service.Type.Name,
                        targetScope.Value.FullName));
                }
            }
        }
    }

    private static IEnumerable<ITypeSymbol> ConstructorDependencyTypes(INamedTypeSymbol service) {
        var constructor = service.InstanceConstructors
            .Where(ctor => ctor.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal)
            .OrderByDescending(ctor => ctor.Parameters.Length)
            .FirstOrDefault();

        return constructor?.Parameters.Select(parameter => parameter.Type) ?? Enumerable.Empty<ITypeSymbol>();
    }

    private static IEnumerable<GraphServiceFact> ResolveDependencyTargets(
        ITypeSymbol dependency,
        IReadOnlyDictionary<string, GraphServiceFact> byImplementation,
        IReadOnlyDictionary<string, List<GraphServiceFact>> byContract) {
        var id = ToIdentity(dependency);
        if (byImplementation.TryGetValue(id.MetadataName, out var implementationTarget)) {
            yield return implementationTarget;
        }

        if (byContract.TryGetValue(id.MetadataName, out var contractTargets)) {
            foreach (var target in contractTargets) {
                yield return target;
            }
        }
    }

    private static INamedTypeSymbol? TryGetAutoRegisterScope(INamedTypeSymbol type) {
        foreach (var attr in type.GetAttributes()) {
            if (attr.AttributeClass?.Name != "AutoRegisterInAttribute") {
                continue;
            }

            if (attr.AttributeClass.IsGenericType && attr.AttributeClass.TypeArguments.Length > 0 &&
                attr.AttributeClass.TypeArguments[0] is INamedTypeSymbol genericScope) {
                return genericScope;
            }

            if (attr.ConstructorArguments.Length > 0 &&
                attr.ConstructorArguments[0].Kind == TypedConstantKind.Type &&
                attr.ConstructorArguments[0].Value is INamedTypeSymbol ctorScope) {
                return ctorScope;
            }
        }

        return null;
    }

    private static string GetLifetime(INamedTypeSymbol type) {
        var attr = type.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name == "AutoRegisterInAttribute");
        if (attr == null) {
            return "Singleton";
        }

        foreach (var arg in attr.NamedArguments) {
            if (arg.Key != "Lifetime") {
                continue;
            }

            var value = arg.Value.Value?.ToString() ?? "Singleton";
            return value switch {
                "0" => "Singleton",
                "1" => "Transient",
                "2" => "Scoped",
                _ => value
            };
        }

        return "Singleton";
    }

    private static DiTypeIdentity ToIdentity(ITypeSymbol type)
        => DiTypeIdentity.FromFullName(type.ToDisplayString(), type.ContainingAssembly?.Name ?? string.Empty);

    private sealed class ServiceFact {
        public ServiceFact(INamedTypeSymbol type, INamedTypeSymbol scope) {
            Type = type;
            Scope = scope;
        }

        public INamedTypeSymbol Type { get; }
        public INamedTypeSymbol Scope { get; }
    }

    private sealed class GraphServiceFact {
        public GraphServiceFact(ServiceFact service, DiServiceRegistration registration) {
            Service = service;
            Registration = registration;
        }

        public ServiceFact Service { get; }
        public DiServiceRegistration Registration { get; }
    }
}
