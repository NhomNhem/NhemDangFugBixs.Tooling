namespace DangFugBixs.Generators.Models;

public readonly struct ServiceInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string Lifetime { get; }
    public string ScopeName { get; }

    public ServiceInfo(string ns, string className, string lifetime, string scopeName) 
        => (Namespace, ClassName, Lifetime, ScopeName) = (ns, className, lifetime, scopeName);
}

