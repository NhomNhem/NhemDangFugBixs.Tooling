using System;

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class EntryPointAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class AsyncEntryPointAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class SceneComponentAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class SceneComponentAttribute<TScope> : Attribute {
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Scoped;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class NewGameObjectComponentAttribute : Attribute {
        public Type ScopeType { get; }
        public string Name { get; }
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Scoped;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();

        public NewGameObjectComponentAttribute(Type scopeType, string name = "") {
            ScopeType = scopeType;
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class NewGameObjectComponentAttribute<TScope> : Attribute {
        public string Name { get; }
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Scoped;
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();

        public NewGameObjectComponentAttribute(string name = "") {
            Name = name;
        }
    }
}
