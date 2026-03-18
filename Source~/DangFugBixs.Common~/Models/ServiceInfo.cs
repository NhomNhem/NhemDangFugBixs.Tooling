using System.Collections.Generic;
using NhemDangFugBixs.Attributes;

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
    public bool IsInstaller { get; }
    public int InstallerOrder { get; }

    public bool IsMessagePipeBroker { get; }
    public string? MessageType { get; }
    public MessagePipeType MessagePipeKind { get; }
    public Dictionary<string, string> Metadata { get; }

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
        bool isBuildCallback = false,
        bool isInstaller = false,
        int installerOrder = 0,
        bool isMessagePipeBroker = false,
        string? messageType = null,
        MessagePipeType messagePipeKind = MessagePipeType.Publisher,
        Dictionary<string, string>? metadata = null)
        => (Namespace, ClassName, Lifetime, ScopeName, InterfaceNames, IsComponent, AsImplementedInterfaces, AsSelf, RegisterInHierarchy, AsTypes, IsEntryPoint, IsFactory, ScopeTypeName, UsesTypeSafeScope, IsExceptionHandler, IsBuildCallback, IsInstaller, InstallerOrder, IsMessagePipeBroker, MessageType, MessagePipeKind, Metadata) =
           (ns, className, lifetime, scopeName, interfaceNames, isComponent, asImplementedInterfaces, asSelf, registerInHierarchy, asTypes, isEntryPoint, isFactory, scopeTypeName, usesTypeSafeScope, isExceptionHandler, isBuildCallback, isInstaller, installerOrder, isMessagePipeBroker, messageType, messagePipeKind, metadata ?? new Dictionary<string, string>());
}
