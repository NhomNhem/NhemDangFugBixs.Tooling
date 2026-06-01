using System;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal sealed class DiContractGraph {
    private static readonly IComparer<DiServiceRegistration> ServiceComparer = Comparer<DiServiceRegistration>.Create(CompareServices);
    private static readonly IComparer<DiCompositionRoot> CompositionRootComparer = Comparer<DiCompositionRoot>.Create(CompareCompositionRoots);
    private static readonly IComparer<DiManualRegistration> ManualRegistrationComparer = Comparer<DiManualRegistration>.Create(CompareManualRegistrations);
    private static readonly IComparer<DiGeneratedInstaller> GeneratedInstallerComparer = Comparer<DiGeneratedInstaller>.Create(CompareGeneratedInstallers);

    public IReadOnlyList<DiScopeIdentity> Scopes { get; }
    public IReadOnlyList<DiServiceRegistration> Services { get; }
    public IReadOnlyList<DiCompositionRoot> CompositionRoots { get; }
    public IReadOnlyList<DiManualRegistration> ManualRegistrations { get; }
    public IReadOnlyList<DiGeneratedInstaller> GeneratedInstallers { get; }
    public IReadOnlyList<DiDiagnosticEvidence> DiagnosticEvidence { get; }

    public DiContractGraph(
        IEnumerable<DiScopeIdentity>? scopes = null,
        IEnumerable<DiServiceRegistration>? services = null,
        IEnumerable<DiCompositionRoot>? compositionRoots = null,
        IEnumerable<DiManualRegistration>? manualRegistrations = null,
        IEnumerable<DiGeneratedInstaller>? generatedInstallers = null,
        IEnumerable<DiDiagnosticEvidence>? diagnosticEvidence = null) {
        Scopes = (scopes ?? Array.Empty<DiScopeIdentity>())
            .OrderBy(static scope => scope.MarkerType)
            .ThenBy(static scope => scope.CompositionRootType ?? default)
            .ToArray();
        Services = (services ?? Array.Empty<DiServiceRegistration>())
            .OrderBy(static service => service, ServiceComparer)
            .ToArray();
        CompositionRoots = (compositionRoots ?? Array.Empty<DiCompositionRoot>())
            .OrderBy(static root => root, CompositionRootComparer)
            .ToArray();
        ManualRegistrations = (manualRegistrations ?? Array.Empty<DiManualRegistration>())
            .OrderBy(static registration => registration, ManualRegistrationComparer)
            .ToArray();
        GeneratedInstallers = (generatedInstallers ?? Array.Empty<DiGeneratedInstaller>())
            .OrderBy(static installer => installer, GeneratedInstallerComparer)
            .ToArray();
        DiagnosticEvidence = (diagnosticEvidence ?? Array.Empty<DiDiagnosticEvidence>())
            .OrderBy(static evidence => evidence.Id, StringComparer.Ordinal)
            .ThenBy(static evidence => evidence.Kind, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyList<DiServiceRegistration> ServicesForScope(DiTypeIdentity scopeMarkerType)
        => Services
            .Where(service => service.ScopeMarkerType.HasValue && service.ScopeMarkerType.Value.Equals(scopeMarkerType))
            .GroupBy(static service => service.ImplementationType)
            .Select(static group => group.OrderByDescending(service => EvidenceRank(service.Provenance.EvidenceSource)).ThenBy(static service => service, ServiceComparer).First())
            .OrderBy(static service => service, ServiceComparer)
            .ToArray();

    public DiCompositionRoot? CompositionRootForMarker(DiTypeIdentity scopeMarkerType)
        => CompositionRoots.FirstOrDefault(root => root.ScopeMarkerType.Equals(scopeMarkerType));

    public IReadOnlyList<DiManualRegistration> ManualRegistrationsForImplementation(DiTypeIdentity implementationType)
        => ManualRegistrations
            .Where(registration => registration.ImplementationType.Equals(implementationType))
            .OrderBy(static registration => registration, ManualRegistrationComparer)
            .ToArray();

    public IReadOnlyList<DiGeneratedInstaller> GeneratedInstallersForScope(DiTypeIdentity scopeMarkerType)
        => GeneratedInstallers
            .Where(installer => installer.ScopeMarkerType.Equals(scopeMarkerType))
            .OrderBy(static installer => installer, GeneratedInstallerComparer)
            .ToArray();

    private static int CompareServices(DiServiceRegistration? left, DiServiceRegistration? right) {
        if (ReferenceEquals(left, right)) {
            return 0;
        }

        if (left is null) {
            return -1;
        }

        if (right is null) {
            return 1;
        }

        var scope = CompareNullableTypes(left.ScopeMarkerType, right.ScopeMarkerType);
        if (scope != 0) {
            return scope;
        }

        var implementation = left.ImplementationType.CompareTo(right.ImplementationType);
        if (implementation != 0) {
            return implementation;
        }

        return string.Compare(left.Provenance.DeclaringAssembly, right.Provenance.DeclaringAssembly, StringComparison.Ordinal);
    }

    private static int CompareCompositionRoots(DiCompositionRoot? left, DiCompositionRoot? right) {
        if (ReferenceEquals(left, right)) {
            return 0;
        }

        if (left is null) {
            return -1;
        }

        if (right is null) {
            return 1;
        }

        var scope = left.ScopeMarkerType.CompareTo(right.ScopeMarkerType);
        if (scope != 0) {
            return scope;
        }

        return left.LifetimeScopeType.CompareTo(right.LifetimeScopeType);
    }

    private static int CompareManualRegistrations(DiManualRegistration? left, DiManualRegistration? right) {
        if (ReferenceEquals(left, right)) {
            return 0;
        }

        if (left is null) {
            return -1;
        }

        if (right is null) {
            return 1;
        }

        var implementation = left.ImplementationType.CompareTo(right.ImplementationType);
        if (implementation != 0) {
            return implementation;
        }

        var location = string.Compare(left.Location, right.Location, StringComparison.Ordinal);
        if (location != 0) {
            return location;
        }

        return string.Compare(left.ApiName, right.ApiName, StringComparison.Ordinal);
    }

    private static int CompareGeneratedInstallers(DiGeneratedInstaller? left, DiGeneratedInstaller? right) {
        if (ReferenceEquals(left, right)) {
            return 0;
        }

        if (left is null) {
            return -1;
        }

        if (right is null) {
            return 1;
        }

        var scope = left.ScopeMarkerType.CompareTo(right.ScopeMarkerType);
        if (scope != 0) {
            return scope;
        }

        return string.Compare(left.InstallerTypeName, right.InstallerTypeName, StringComparison.Ordinal);
    }

    private static int CompareNullableTypes(DiTypeIdentity? left, DiTypeIdentity? right) {
        if (left.HasValue && right.HasValue) {
            return left.Value.CompareTo(right.Value);
        }

        if (left.HasValue) {
            return 1;
        }

        return right.HasValue ? -1 : 0;
    }

    private static int EvidenceRank(DiEvidenceSource source) {
        switch (source) {
            case DiEvidenceSource.CurrentCompilation:
                return 3;
            case DiEvidenceSource.ReferencedAssembly:
                return 2;
            case DiEvidenceSource.ProjectWideSmokeValidation:
                return 1;
            default:
                return 0;
        }
    }
}
