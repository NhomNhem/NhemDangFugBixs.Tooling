namespace NhemDangFugBixs.Common.Models;

internal readonly struct SceneInjectionInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

    public SceneInjectionInfo(string ns, string className) 
        => (Namespace, ClassName) = (ns, className);
}
