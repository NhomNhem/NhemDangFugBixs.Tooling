namespace NhemDangFugBixs.Common.Models;

/// <summary>
/// Information about a LifetimeScope mapped to an Identity Type.
/// </summary>
internal readonly struct ScopeMappingInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string IdentityTypeName { get; }
    public string OriginalClassName { get; }
    public string? AliasName { get; }
    public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";
    public string OriginalFullName => string.IsNullOrEmpty(Namespace) ? OriginalClassName : $"{Namespace}.{OriginalClassName}";

    public ScopeMappingInfo(string ns, string className, string identityTypeName, string originalClassName, string? aliasName = null)
        => (Namespace, ClassName, IdentityTypeName, OriginalClassName, AliasName) = (ns, className, identityTypeName, originalClassName, aliasName);
}
