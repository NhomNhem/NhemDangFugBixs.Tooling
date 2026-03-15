using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Registers a class with VContainer using string-based scope reference.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Deprecated:</b> In v3.0, use <see cref="AutoRegisterInAttribute{TScope}"/> for type-safe scope references.
    /// </para>
    /// <code>
    /// // Old (deprecated):
    /// [AutoRegister(Lifetime.Singleton, scope: "Gameplay")]
    /// 
    /// // New (recommended):
    /// [AutoRegisterIn&lt;GameplayLifetimeScope&gt;(Lifetime = Lifetime.Singleton)]
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Obsolete("Use AutoRegisterIn<TScope> for type-safe scope references. This attribute will be removed in v4.0.")]
    public sealed class AutoRegisterAttribute : Attribute {
        /// <summary>
        /// The lifetime for this registration.
        /// </summary>
        public Lifetime Lifetime { get; }

        /// <summary>
        /// The scope name for registration. This is deprecated in favor of type-safe scope references.
        /// </summary>
        /// <remarks>Deprecated: Use <see cref="AutoRegisterInAttribute{TScope}"/> instead.</remarks>
        [Obsolete("Use AutoRegisterIn<TScope> for type-safe scope references.")]
        public string Scope { get; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoRegisterAttribute"/> class.
        /// </summary>
        /// <param name="lifetime">The lifetime for this registration.</param>
        /// <param name="scope">The scope name (deprecated).</param>
        [Obsolete("Use AutoRegisterIn<TScope> for type-safe scope references.")]
        public AutoRegisterAttribute(NhemDangFugBixs.Attributes.Lifetime lifetime = NhemDangFugBixs.Attributes.Lifetime.Singleton, string scope = "Global") {
            this.Lifetime = lifetime;
            this.Scope = scope;
        }
    }
}