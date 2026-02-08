namespace DangFugBixs.Generators.Models;

public readonly struct ServiceInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string Lifetime { get; }

    public ServiceInfo(string ns, string className, string lifetime) =>
        (Namespace, ClassName, Lifetime) = (ns, className, lifetime);
}

