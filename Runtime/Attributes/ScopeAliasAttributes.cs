using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Declares a human-friendly alias for a marker-mapped LifetimeScope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class RegisterScopeAliasAttribute : Attribute {
        public string Alias { get; }

        public RegisterScopeAliasAttribute(string alias) {
            Alias = alias;
        }
    }

    /// <summary>
    /// Registers a service into a scope by alias instead of marker type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class AutoRegisterInScopeAttribute : Attribute {
        public string ScopeAlias { get; }
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Singleton;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public bool RegisterInHierarchy { get; set; }
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();

        public AutoRegisterInScopeAttribute(string scopeAlias) {
            ScopeAlias = scopeAlias;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class ProjectServiceAttribute : Attribute {
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Singleton;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public bool RegisterInHierarchy { get; set; }
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class GameplayServiceAttribute : Attribute {
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Scoped;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public bool RegisterInHierarchy { get; set; }
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class MainMenuServiceAttribute : Attribute {
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Scoped;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public bool RegisterInHierarchy { get; set; }
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();
    }
}
