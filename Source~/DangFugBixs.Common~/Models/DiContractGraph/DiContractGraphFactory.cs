using System;
using System.Collections.Generic;
using System.Linq;
using NhemDangFugBixs.Common.Models;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal static class DiContractGraphFactory {
    public static DiContractGraph FromLegacy(
        string observedAssembly,
        IEnumerable<ServiceInfo> services,
        IEnumerable<ScopeMappingInfo> scopeMappings) {
        var serviceList = (services ?? Array.Empty<ServiceInfo>()).ToList();
        var mappingList = (scopeMappings ?? Array.Empty<ScopeMappingInfo>()).ToList();
        var scopeByAlias = mappingList
            .Where(static mapping => !string.IsNullOrWhiteSpace(mapping.AliasName))
            .GroupBy(static mapping => mapping.AliasName!, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        var scopes = mappingList.Select(mapping => {
            var marker = DiTypeIdentity.FromFullName(mapping.IdentityTypeName);
            var root = DiTypeIdentity.FromFullName(mapping.OriginalFullName, observedAssembly);
            return new DiScopeIdentity(
                marker,
                new DiAssemblyProvenance(observedAssembly, observedAssembly, DiEvidenceSource.CurrentCompilation, new[] { observedAssembly }),
                mapping.AliasName,
                root);
        });

        var compositionRoots = mappingList.Select(mapping =>
            new DiCompositionRoot(
                DiTypeIdentity.FromFullName(mapping.OriginalFullName, observedAssembly),
                DiTypeIdentity.FromFullName(mapping.IdentityTypeName),
                new DiAssemblyProvenance(observedAssembly, observedAssembly, DiEvidenceSource.CurrentCompilation, new[] { observedAssembly })));

        var registrations = serviceList.Select(service => ToRegistration(service, observedAssembly, scopeByAlias));
        var registrationList = registrations.ToList();
        var installers = mappingList.Select(mapping => {
            var marker = DiTypeIdentity.FromFullName(mapping.IdentityTypeName);
            var hasServices = registrationList.Any(registration =>
                registration.ScopeMarkerType.HasValue &&
                registration.ScopeMarkerType.Value.Equals(marker));
            return new DiGeneratedInstaller(
                marker,
                $"NhemGenerated{GetInstallerStem(mapping.IdentityTypeName)}Installer",
                !hasServices,
                new DiAssemblyProvenance(observedAssembly, observedAssembly, DiEvidenceSource.CurrentCompilation, new[] { observedAssembly }));
        });

        return new DiContractGraph(
            scopes,
            registrationList,
            compositionRoots,
            generatedInstallers: installers);
    }

    private static DiServiceRegistration ToRegistration(
        ServiceInfo service,
        string observedAssembly,
        IReadOnlyDictionary<string, ScopeMappingInfo> scopeByAlias) {
        var declaringAssembly = service.Metadata.TryGetValue("DeclaringAssembly", out var assemblyName) && !string.IsNullOrWhiteSpace(assemblyName)
            ? assemblyName
            : observedAssembly;
        var evidence = service.IsFromCurrentCompilation || string.Equals(declaringAssembly, observedAssembly, StringComparison.Ordinal)
            ? DiEvidenceSource.CurrentCompilation
            : DiEvidenceSource.ReferencedAssembly;
        var scopeMarker = ResolveScopeMarker(service, scopeByAlias);
        var contracts = service.AsTypes.Length > 0
            ? service.AsTypes.Select(type => DiTypeIdentity.FromFullName(type))
            : service.InterfaceNames.Select(type => DiTypeIdentity.FromFullName(type));

        return new DiServiceRegistration(
            DiTypeIdentity.FromFullName(service.FullName, declaringAssembly),
            contracts,
            service.Lifetime,
            new DiAssemblyProvenance(declaringAssembly, observedAssembly, evidence, BuildReferencePath(observedAssembly, declaringAssembly)),
            scopeMarker,
            service.IsComponent,
            service.IsEntryPoint,
            service.IsFactory,
            service.IsMessagePipeBroker,
            service.Metadata);
    }

    private static DiTypeIdentity? ResolveScopeMarker(ServiceInfo service, IReadOnlyDictionary<string, ScopeMappingInfo> scopeByAlias) {
        if (service.UsesTypeSafeScope && !string.IsNullOrWhiteSpace(service.ScopeTypeName)) {
            return DiTypeIdentity.FromFullName(service.ScopeTypeName!);
        }

        if (!string.IsNullOrWhiteSpace(service.ScopeName) &&
            scopeByAlias.TryGetValue(service.ScopeName, out var mapping)) {
            return DiTypeIdentity.FromFullName(mapping.IdentityTypeName);
        }

        return null;
    }

    private static IEnumerable<string> BuildReferencePath(string observedAssembly, string declaringAssembly) {
        yield return observedAssembly;
        if (!string.Equals(observedAssembly, declaringAssembly, StringComparison.Ordinal)) {
            yield return declaringAssembly;
        }
    }

    private static string GetInstallerStem(string scopeKey) {
        var simpleName = scopeKey.Contains(".") ? scopeKey.Split('.').Last() : scopeKey;
        if (simpleName.StartsWith("I", StringComparison.Ordinal) &&
            simpleName.Length > 1 &&
            char.IsUpper(simpleName[1])) {
            simpleName = simpleName.Substring(1);
        }

        if (!simpleName.EndsWith("Scope", StringComparison.Ordinal)) {
            simpleName += "Scope";
        }

        return SanitizeIdentifier(simpleName);
    }

    private static string SanitizeIdentifier(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return "Scope";
        }

        var chars = value.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_').ToArray();
        if (char.IsDigit(chars[0])) {
            return "_" + new string(chars);
        }

        return new string(chars);
    }
}
