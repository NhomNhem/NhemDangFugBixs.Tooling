namespace NhemDangFugBixs.Common.Models;

/// <summary>
/// Information about a LifetimeScope mapped to an Identity Type.
/// </summary>
internal readonly struct ScopeMappingInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string IdentityTypeName { get; }
    public string OriginalClassName { get; }
    public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";
    public string OriginalFullName => string.IsNullOrEmpty(Namespace) ? OriginalClassName : $"{Namespace}.{OriginalClassName}";

    public ScopeMappingInfo(string ns, string className, string identityTypeName, string originalClassName)
        => (Namespace, ClassName, IdentityTypeName, OriginalClassName) = (ns, className, identityTypeName, originalClassName);
}
