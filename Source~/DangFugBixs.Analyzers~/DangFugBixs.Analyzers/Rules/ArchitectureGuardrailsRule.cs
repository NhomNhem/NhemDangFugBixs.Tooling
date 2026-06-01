using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NhemDangFugBixs.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ArchitectureGuardrailsRule : DiagnosticAnalyzer {
    public static readonly DiagnosticDescriptor NDFG010 = new(
        "NDFG010", "Missing Scope Mapping",
        "Type '{0}' uses scope marker '{1}' but no [LifetimeScopeFor] mapping exists.",
        "Architecture", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDFG011 = new(
        "NDFG011", "Duplicate Scope Mapping",
        "Scope marker '{0}' is mapped by multiple LifetimeScope types.",
        "Architecture", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDFG014 = new(
        "NDFG014", "Orphan service — no composition target",
        "{0} is registered for scope {1} but no [LifetimeScopeFor<{1}>] was found. This service will never be registered at runtime.",
        "Architecture", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF020 = new(
        "NDF020", "MonoBehaviour constructor injection",
        "MonoBehaviour '{0}' should not use constructor injection.",
        "Injection", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF021 = new(
        "NDF021", "Public [Inject] field",
        "Field '{0}' uses [Inject] and should not be public.",
        "Injection", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF022 = new(
        "NDF022", "Private [Inject] method",
        "Private [Inject] method '{0}' is not compatible with VContainer Source Generator. Use public.",
        "Injection", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF023 = new(
        "NDF023", "Async [Inject] method",
        "Method '{0}' uses [Inject] and should not be async.",
        "Injection", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF024 = new(
        "NDF024", "Non-standard construct method name",
        "Method '{0}' uses [Inject]. Consider using 'Construct'.",
        "Injection", DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NDF025 = new(
        "NDF025", "Primitive constructor parameter",
        "{0} has primitive constructor parameter '{1} {2}' that cannot be resolved from the container. Add [ManualFactory] or use WithParameter.",
        "Registration", DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NDF026 = new(
        "NDF026", "ScriptableObject constructor parameter",
        "{0} constructor receives ScriptableObject-derived parameter '{1} {2}'. Ensure it is registered with RegisterInstance before calling RegisterGeneratedFor.",
        "Registration", DiagnosticSeverity.Info, true);

    public static readonly DiagnosticDescriptor NDF030 = new(
        "NDF030", "Singleton depends on Scoped",
        "Singleton service '{0}' depends on scoped service '{1}'.",
        "Lifetime", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF031 = new(
        "NDF031", "Runtime namespace singleton",
        "Service '{0}' appears runtime-scoped by namespace but is registered as Singleton.",
        "Lifetime", DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NDF032 = new(
        "NDF032", "Disposable transient service",
        "Transient service '{0}' implements IDisposable.",
        "Lifetime", DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NDF033 = new(
        "NDF033", "Gameplay service missing explicit lifetime",
        "Service '{0}' in gameplay scope should set Lifetime explicitly.",
        "Lifetime", DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NDF052 = new(
        "NDF052", "IObjectResolver usage outside factory/spawner/bootstrapper",
        "Type '{0}' injects IObjectResolver outside allowed patterns.",
        "Architecture", DiagnosticSeverity.Warning, true);

    public static readonly DiagnosticDescriptor NDF070 = new(
        "NDF070", "Public Subject<T>",
        "Subject field '{0}' should not be public. Expose observable/read-only surface instead.",
        "R3", DiagnosticSeverity.Error, true);

    public static readonly DiagnosticDescriptor NDF071 = new(
        "NDF071", "Subject owner should implement IDisposable",
        "Type '{0}' owns Subject<T> but does not implement IDisposable.",
        "R3", DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        NDFG011, NDF020, NDF021, NDF022, NDF023, NDF024, NDF025, NDF026, NDF030, NDF031, NDF032, NDF033, NDF052, NDF070, NDF071);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(static startContext => {
            var autoRegistered = new ConcurrentBag<(INamedTypeSymbol Type, string ScopeFQN)>();
            var gate = new object();

            startContext.RegisterSymbolAction(symbolContext => {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                if (type.TypeKind != TypeKind.Class) return;

                var attrs = type.GetAttributes();
                var hasAutoRegister = false;
                string? scopeFQN = null;
                foreach (var attr in attrs) {
                    var attrName = attr.AttributeClass?.Name;
                    if (attrName == "AutoRegisterInAttribute") {
                        hasAutoRegister = true;
                        scopeFQN = TryGetScopeName(attr);
                        lock (gate) {
                            autoRegistered.Add((type, scopeFQN ?? ""));
                        }
                    }
                }

                if (hasAutoRegister && !HasManualFactory(type)) {
                    var ctor = type.InstanceConstructors
                        .Where(c => c.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal)
                        .OrderByDescending(c => c.Parameters.Length)
                        .FirstOrDefault();
                    if (ctor != null) {
                        foreach (var p in ctor.Parameters) {
                            if (IsPrimitiveType(p.Type)) {
                                symbolContext.ReportDiagnostic(Diagnostic.Create(NDF025, type.Locations.FirstOrDefault(), type.Name, p.Type.Name, p.Name));
                            }
                            if (IsScriptableObject(p.Type)) {
                                symbolContext.ReportDiagnostic(Diagnostic.Create(NDF026, p.Locations.FirstOrDefault(), type.Name, p.Type.Name, p.Name));
                            }
                        }
                    }
                }

                AnalyzeInjectionStyle(symbolContext, type);
                AnalyzeR3(symbolContext, type);
            }, SymbolKind.NamedType);

            startContext.RegisterCompilationEndAction(endContext => {
                if (autoRegistered.IsEmpty) return;
                var scopeMappings = CollectScopeMappings(endContext.Compilation);
                AnalyzeScopeMapping(endContext, autoRegistered, scopeMappings);
                AnalyzeLifetimeArchitecture(endContext, autoRegistered);
            });
        });
    }

    private static Dictionary<string, List<INamedTypeSymbol>> CollectScopeMappings(Compilation compilation) {
        var result = new Dictionary<string, List<INamedTypeSymbol>>();
        foreach (var type in GetAllNamedTypes(compilation.GlobalNamespace)) {
            if (type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Interface) continue;
            foreach (var attr in type.GetAttributes()) {
                if (attr.AttributeClass?.Name == "LifetimeScopeForAttribute") {
                    var id = TryGetIdentityName(attr);
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    if (!result.TryGetValue(id!, out var list)) {
                        list = new List<INamedTypeSymbol>();
                        result[id!] = list;
                    }
                    list.Add(type);
                }
            }
        }
        return result;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllNamedTypes(INamespaceSymbol namespaceSymbol) {
        foreach (var type in namespaceSymbol.GetTypeMembers()) {
            yield return type;
            foreach (var nested in GetNestedTypes(type)) {
                yield return nested;
            }
        }
        foreach (var nestedNs in namespaceSymbol.GetNamespaceMembers()) {
            foreach (var type in GetAllNamedTypes(nestedNs)) {
                yield return type;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type) {
        foreach (var nested in type.GetTypeMembers()) {
            yield return nested;
            foreach (var deeper in GetNestedTypes(nested)) {
                yield return deeper;
            }
        }
    }

    private static void AnalyzeScopeMapping(CompilationAnalysisContext context, ConcurrentBag<(INamedTypeSymbol Type, string ScopeFQN)> services, Dictionary<string, List<INamedTypeSymbol>> mappings) {
        foreach (var pair in mappings.Where(p => p.Value.Count > 1)) {
            foreach (var type in pair.Value) {
                context.ReportDiagnostic(Diagnostic.Create(NDFG011, type.Locations.FirstOrDefault(), pair.Key));
            }
        }

        // NDFG014 removed from source analyzer - false positive in marker-based architecture
        // Service assembly cannot see Composition assembly's LifetimeScopeFor mapping
        // Validation moved to di-smoke cross-assembly validation
    }

    private static void AnalyzeLifetimeArchitecture(CompilationAnalysisContext context, ConcurrentBag<(INamedTypeSymbol Type, string ScopeFQN)> services) {
        var lifetimes = new Dictionary<INamedTypeSymbol, string>(SymbolEqualityComparer.Default);
        var explicitLifetime = new Dictionary<INamedTypeSymbol, bool>(SymbolEqualityComparer.Default);
        var scopes = new Dictionary<INamedTypeSymbol, string>(SymbolEqualityComparer.Default);

        foreach (var (svc, _) in services) {
            var attr = svc.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "AutoRegisterInAttribute");
            if (attr == null) continue;
            lifetimes[svc] = GetLifetime(attr, out var isExplicit);
            explicitLifetime[svc] = isExplicit;
            scopes[svc] = TryGetScopeName(attr) ?? "";
        }

        foreach (var (svc, _) in services) {
            if (!lifetimes.TryGetValue(svc, out var life)) continue;
            var ns = svc.ContainingNamespace.ToDisplayString();
            if (life == "Singleton" && IsRuntimeNamespace(ns)) {
                context.ReportDiagnostic(Diagnostic.Create(NDF031, svc.Locations.FirstOrDefault(), svc.Name));
            }
            if (life == "Transient" && ImplementsDisposable(svc)) {
                context.ReportDiagnostic(Diagnostic.Create(NDF032, svc.Locations.FirstOrDefault(), svc.Name));
            }
            if (scopes.TryGetValue(svc, out var scopeName) &&
                scopeName.Contains("Gameplay") &&
                explicitLifetime.TryGetValue(svc, out var hasExplicit) &&
                !hasExplicit) {
                context.ReportDiagnostic(Diagnostic.Create(NDF033, svc.Locations.FirstOrDefault(), svc.Name));
            }

            if (life == "Singleton") {
                var ctor = svc.InstanceConstructors.FirstOrDefault(c => c.DeclaredAccessibility == Accessibility.Public);
                if (ctor != null) {
                    foreach (var p in ctor.Parameters) {
                        if (p.Type is INamedTypeSymbol dep &&
                            lifetimes.TryGetValue(dep, out var depLifetime) &&
                            depLifetime == "Scoped") {
                            context.ReportDiagnostic(Diagnostic.Create(NDF030, p.Locations.FirstOrDefault(), svc.Name, dep.Name));
                        }
                        if (p.Type.Name == "IObjectResolver" &&
                            !HasAllowedResolverPattern(svc) &&
                            !InheritsLifetimeScope(svc)) {
                            context.ReportDiagnostic(Diagnostic.Create(NDF052, p.Locations.FirstOrDefault(), svc.Name));
                        }
                    }
                }
            }
        }
    }

    private static void AnalyzeInjectionStyle(SymbolAnalysisContext context, INamedTypeSymbol type) {
        var isMono = InheritsMonoBehaviour(type);
        if (isMono) {
            foreach (var ctor in type.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public && c.Parameters.Length > 0)) {
                context.ReportDiagnostic(Diagnostic.Create(NDF020, ctor.Locations.FirstOrDefault(), type.Name));
            }
        }

        foreach (var member in type.GetMembers()) {
            if (member is IFieldSymbol field && HasInject(field.GetAttributes()) && field.DeclaredAccessibility == Accessibility.Public) {
                context.ReportDiagnostic(Diagnostic.Create(NDF021, field.Locations.FirstOrDefault(), field.Name));
            }
            if (member is IMethodSymbol method && method.MethodKind == MethodKind.Ordinary && HasInject(method.GetAttributes())) {
                if (method.DeclaredAccessibility == Accessibility.Private) {
                    context.ReportDiagnostic(Diagnostic.Create(NDF022, method.Locations.FirstOrDefault(), method.Name));
                }
                if (method.IsAsync || method.ReturnType.Name == "Task" || method.ReturnType.Name == "ValueTask") {
                    context.ReportDiagnostic(Diagnostic.Create(NDF023, method.Locations.FirstOrDefault(), method.Name));
                }
                if (method.Name == "Constructs") {
                    context.ReportDiagnostic(Diagnostic.Create(NDF024, method.Locations.FirstOrDefault(), method.Name));
                }
            }
        }
    }

    private static void AnalyzeR3(SymbolAnalysisContext context, INamedTypeSymbol type) {
        bool hasSubject = false;
        foreach (var field in type.GetMembers().OfType<IFieldSymbol>()) {
            if (field.Type is INamedTypeSymbol named && named.Name == "Subject" && named.IsGenericType) {
                hasSubject = true;
                if (field.DeclaredAccessibility == Accessibility.Public) {
                    context.ReportDiagnostic(Diagnostic.Create(NDF070, field.Locations.FirstOrDefault(), field.Name));
                }
            }
        }
        if (hasSubject && !ImplementsDisposable(type)) {
            context.ReportDiagnostic(Diagnostic.Create(NDF071, type.Locations.FirstOrDefault(), type.Name));
        }
    }

    private static string? TryGetIdentityName(AttributeData attr) {
        if (attr.AttributeClass?.IsGenericType == true && attr.AttributeClass.TypeArguments.Length > 0) {
            return attr.AttributeClass.TypeArguments[0].ToDisplayString();
        }
        if (attr.ConstructorArguments.Length > 0 &&
            attr.ConstructorArguments[0].Kind == TypedConstantKind.Type &&
            attr.ConstructorArguments[0].Value is INamedTypeSymbol t) {
            return t.ToDisplayString();
        }
        return null;
    }

    private static string? TryGetScopeName(AttributeData attr) {
        if (attr.AttributeClass?.IsGenericType == true && attr.AttributeClass.TypeArguments.Length > 0) {
            return attr.AttributeClass.TypeArguments[0].ToDisplayString();
        }
        if (attr.ConstructorArguments.Length > 0 &&
            attr.ConstructorArguments[0].Kind == TypedConstantKind.Type &&
            attr.ConstructorArguments[0].Value is INamedTypeSymbol t) {
            return t.ToDisplayString();
        }
        return null;
    }

    private static string GetLifetime(AttributeData attr, out bool isExplicit) {
        isExplicit = false;
        foreach (var kv in attr.NamedArguments) {
            if (kv.Key == "Lifetime") {
                isExplicit = true;
                if (kv.Value.Value != null) {
                    var raw = kv.Value.Value.ToString() ?? "Singleton";
                    if (raw is "0" or "Singleton") return "Singleton";
                    if (raw is "1" or "Transient") return "Transient";
                    if (raw is "2" or "Scoped") return "Scoped";
                    return raw;
                }
            }
        }
        return "Singleton";
    }

    private static bool HasInject(ImmutableArray<AttributeData> attrs)
        => attrs.Any(a => a.AttributeClass?.Name == "InjectAttribute");

    private static bool InheritsMonoBehaviour(INamedTypeSymbol type) {
        var current = type;
        while (current != null) {
            var f = current.ToDisplayString();
            if (f == "UnityEngine.MonoBehaviour" || f == "UnityEngine.Component") return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool ImplementsDisposable(INamedTypeSymbol type)
        => type.AllInterfaces.Any(i => i.ToDisplayString() == "System.IDisposable");

    private static bool InheritsLifetimeScope(INamedTypeSymbol type) {
        var current = type;
        while (current != null) {
            if (current.Name == "LifetimeScope" || current.ToDisplayString() == "VContainer.Unity.LifetimeScope") return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool HasAllowedResolverPattern(INamedTypeSymbol type) {
        var n = type.Name;
        if (n.Contains("Factory") || n.Contains("Spawner") || n.Contains("Bootstrapper")) return true;
        return type.GetAttributes().Any(a => {
            var an = a.AttributeClass?.Name ?? "";
            return an == "FactoryAttribute" || an == "SpawnerAttribute" || an == "BootstrapperAttribute";
        });
    }

    private static bool HasManualFactory(INamedTypeSymbol type)
        => type.GetAttributes().Any(a => a.AttributeClass?.Name == "ManualFactoryAttribute");

    private static bool IsPrimitiveType(ITypeSymbol type) {
        return type.SpecialType is
            SpecialType.System_String or
            SpecialType.System_Int32 or
            SpecialType.System_Single or
            SpecialType.System_Double or
            SpecialType.System_Boolean or
            SpecialType.System_Int64;
    }

    private static bool IsScriptableObject(ITypeSymbol type) {
        var current = type;
        while (current != null) {
            if (current.Name == "ScriptableObject" || current.ToDisplayString() == "UnityEngine.ScriptableObject") return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool IsRuntimeNamespace(string ns) {
        string[] keys = [".Gameplay.", ".Phase.", ".Combat.", ".Player.", ".Resources.", ".Hazards."];
        return keys.Any(k => ns.Contains(k));
    }
}
