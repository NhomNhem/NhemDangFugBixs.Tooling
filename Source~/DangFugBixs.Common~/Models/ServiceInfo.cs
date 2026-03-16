namespace NhemDangFugBixs.Common.Models;

internal readonly struct ServiceInfo {
    public string Namespace { get; }
    public string ClassName { get; }
    public string Lifetime { get; }
    public string ScopeName { get; }
    public string[] InterfaceNames { get; }
    public bool IsComponent { get; }
    public bool AsImplementedInterfaces { get; }
    public bool AsSelf { get; }
    public bool RegisterInHierarchy { get; }
    public bool IsEntryPoint { get; }
    public bool IsFactory { get; }
    public string[] AsTypes { get; }
    public string? ScopeTypeName { get; }
    public bool UsesTypeSafeScope { get; }
    public bool IsExceptionHandler { get; }
    public bool IsBuildCallback { get; }

    public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

    public ServiceInfo(
        string ns,
        string className,
        string lifetime,
        string scopeName,
        string[] interfaceNames,
        bool isComponent,
        bool asImplementedInterfaces,
        bool asSelf,
        bool registerInHierarchy,
        string[] asTypes,
        bool isEntryPoint,
        bool isFactory,
        string? scopeTypeName = null,
        bool usesTypeSafeScope = false,
        bool isExceptionHandler = false,
        bool isBuildCallback = false)
        => (Namespace, ClassName, Lifetime, ScopeName, InterfaceNames, IsComponent, AsImplementedInterfaces, AsSelf, RegisterInHierarchy, AsTypes, IsEntryPoint, IsFactory, ScopeTypeName, UsesTypeSafeScope, IsExceptionHandler, IsBuildCallback) =
           (ns, className, lifetime, scopeName, interfaceNames, isComponent, asImplementedInterfaces, asSelf, registerInHierarchy, asTypes, isEntryPoint, isFactory, scopeTypeName, usesTypeSafeScope, isExceptionHandler, isBuildCallback);
}
