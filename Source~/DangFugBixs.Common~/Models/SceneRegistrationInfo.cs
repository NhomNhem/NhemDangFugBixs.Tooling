namespace NhemDangFugBixs.Common.Models;

/// <summary>
/// Information about a class marked with [AutoRegisterScene].
/// The generator will emit FindFirstObjectByType + RegisterComponent code for these.
/// </summary>
internal readonly struct SceneRegistrationInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

    public SceneRegistrationInfo(string ns, string className) 
        => (Namespace, ClassName) = (ns, className);
}
