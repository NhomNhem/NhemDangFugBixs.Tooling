using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Registers a class with VContainer using type-safe scope reference.
    /// The scope type should inherit from VContainer.Unity.LifetimeScope.
    /// 
    /// Note: Uses NhemDangFugBixs.Attributes.Lifetime which has the same values as VContainer.Lifetime.
    /// In your Unity project files, use the fully qualified name or an alias to avoid ambiguity.
    /// </summary>
    /// <typeparam name="TScope">The LifetimeScope type that will register this service.</typeparam>
    /// <example>
    /// <code>
    /// // Option 1: Use alias at top of file
    /// using NLifetime = NhemDangFugBixs.Attributes.Lifetime;
    /// [AutoRegisterIn&lt;GameplayLifetimeScope&gt;(Lifetime = NLifetime.Singleton)]
    /// 
    /// // Option 2: Use fully qualified name
    /// [AutoRegisterIn&lt;GameplayLifetimeScope&gt;(Lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton)]
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterInAttribute<TScope> : Attribute {
        /// <summary>
        /// The lifetime for this registration.
        /// Values match VContainer.Lifetime (Singleton, Transient, Scoped).
        /// </summary>
        public Lifetime Lifetime { get; set; } = Lifetime.Singleton;

        /// <summary>
        /// Whether to bind to all implemented interfaces.
        /// </summary>
        public bool AsImplementedInterfaces { get; set; } = true;

        /// <summary>
        /// Whether to bind to self (the concrete type).
        /// </summary>
        public bool AsSelf { get; set; } = true;

        /// <summary>
        /// Whether to find existing instance in hierarchy (for MonoBehaviour).
        /// </summary>
        public bool RegisterInHierarchy { get; set; } = false;

        /// <summary>
        /// Explicit interface types to bind to (overrides AsImplementedInterfaces).
        /// </summary>
        public Type[] AsTypes { get; set; }
    }
}
