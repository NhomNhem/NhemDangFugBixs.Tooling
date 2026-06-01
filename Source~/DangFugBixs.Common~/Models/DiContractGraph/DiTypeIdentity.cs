using System;

namespace NhemDangFugBixs.Common.Models.DiContractGraph;

internal readonly struct DiTypeIdentity : IEquatable<DiTypeIdentity>, IComparable<DiTypeIdentity> {
    public string Namespace { get; }
    public string Name { get; }
    public string AssemblyName { get; }
    public string MetadataName { get; }

    public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";

    public DiTypeIdentity(string ns, string name, string assemblyName = "", string? metadataName = null) {
        Namespace = ns ?? string.Empty;
        Name = name ?? string.Empty;
        AssemblyName = assemblyName ?? string.Empty;
        MetadataName = metadataName ?? FullName;
    }

    public static DiTypeIdentity FromFullName(string fullName, string assemblyName = "") {
        var value = fullName ?? string.Empty;
        var index = value.LastIndexOf('.');
        return index < 0
            ? new DiTypeIdentity(string.Empty, value, assemblyName)
            : new DiTypeIdentity(value.Substring(0, index), value.Substring(index + 1), assemblyName);
    }

    public int CompareTo(DiTypeIdentity other) {
        var assembly = string.Compare(AssemblyName, other.AssemblyName, StringComparison.Ordinal);
        if (assembly != 0) {
            return assembly;
        }

        return string.Compare(MetadataName, other.MetadataName, StringComparison.Ordinal);
    }

    public bool Equals(DiTypeIdentity other)
        => string.Equals(AssemblyName, other.AssemblyName, StringComparison.Ordinal)
           && string.Equals(MetadataName, other.MetadataName, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is DiTypeIdentity other && Equals(other);

    public override int GetHashCode() {
        unchecked {
            return ((AssemblyName != null ? StringComparer.Ordinal.GetHashCode(AssemblyName) : 0) * 397)
                   ^ (MetadataName != null ? StringComparer.Ordinal.GetHashCode(MetadataName) : 0);
        }
    }

    public override string ToString()
        => string.IsNullOrEmpty(AssemblyName) ? MetadataName : $"{MetadataName}, {AssemblyName}";
}
