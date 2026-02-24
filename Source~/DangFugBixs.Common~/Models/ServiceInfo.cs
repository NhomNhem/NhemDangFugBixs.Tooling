namespace NhemDangFugBixs.Common.Models;

internal readonly struct ServiceInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string Lifetime { get; }
    public string ScopeName { get; }
    public string[] InterfaceNames { get; }
    public bool IsComponent { get; }

    public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

    public ServiceInfo(string ns, string className, string lifetime, string scopeName, string[] interfaceNames, bool isComponent) 
        => (Namespace, ClassName, Lifetime, ScopeName, InterfaceNames, IsComponent) = (ns, className, lifetime, scopeName, interfaceNames, isComponent);
}
