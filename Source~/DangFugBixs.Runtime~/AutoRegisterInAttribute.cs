using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Registers a class with VContainer using type-safe scope reference.
    /// Supports multi-contract registration with explicit control over interface bindings.
    /// 
    /// The scope type should inherit from VContainer.Unity.LifetimeScope, OR it can be an
    /// Identity Type mapped via [LifetimeScopeFor] for cross-assembly discovery.
    ///
    /// Note: Uses NhemDangFugBixs.Attributes.NhemLifetime which has the same values as VContainer.Lifetime.
    /// In your Unity project files, use the fully qualified name or an alias to avoid ambiguity.
    /// </summary>
    /// <example>
    /// <code>
    /// // Basic Usage - Auto-detect all interfaces (default)
    /// [AutoRegisterIn(typeof(GameScope))]
    /// public class GameService : IGameService, ITickable { }
    /// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().AsImplementedInterfaces().AsSelf();
    ///
    /// // Explicit Contracts - Specify exactly which interfaces to register
    /// [AutoRegisterIn(typeof(GameScope), AsTypes = [typeof(IGameService)])]
    /// public class GameService : IGameService, ITickable { }
    /// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().As&lt;IGameService&gt;();
    ///
    /// // Disable Auto-Detection - Only register as self
    /// [AutoRegisterIn(typeof(GameScope), AsImplementedInterfaces = false)]
    /// public class GameService : IGameService, ITickable { }
    /// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().AsSelf();
    ///
    /// // Multiple Explicit Contracts
    /// [AutoRegisterIn(typeof(GameScope), AsTypes = [typeof(IGameService), typeof(ITickable)])]
    /// public class GameService : IGameService, ITickable { }
    /// // Generates: builder.RegisterEntryPoint&lt;GameService&gt;().As&lt;IGameService&gt;().As&lt;ITickable&gt;();
    ///
    /// // Identity Type (Cross-Layer / Decoupled)
    /// [AutoRegisterIn(typeof(GameScope))]  // GameScope is empty type in shared assembly
    /// public class GameService : IGameService { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterInAttribute : Attribute {
        /// <summary>
        /// The LifetimeScope type (or Identity Type) that will register this service.
        /// </summary>
        public Type ScopeType { get; }

        /// <summary>
        /// The lifetime for this registration.
        /// Values match VContainer.Lifetime (Singleton, Transient, Scoped).
        /// </summary>
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Singleton;

        /// <summary>
        /// Whether to bind to all implemented interfaces.
        /// Default: true (auto-detect all interfaces except VContainer lifecycle interfaces).
        /// Set to false to disable auto-detection and use explicit AsTypes only.
        /// </summary>
        public bool AsImplementedInterfaces { get; set; } = true;

        /// <summary>
        /// Whether to bind to self (the concrete type).
        /// Default: true. Set to false if you only want interface registrations.
        /// </summary>
        public bool AsSelf { get; set; } = true;

        /// <summary>
        /// Whether to find existing instance in hierarchy (for MonoBehaviour).
        /// Uses RegisterComponentInHierarchy when true.
        /// </summary>
        public bool RegisterInHierarchy { get; set; } = false;

        /// <summary>
        /// Explicit interface types to bind to.
        /// When specified, this OVERRIDES AsImplementedInterfaces (only these contracts will be registered).
        /// Use this for precise control over which interfaces are exposed.
        /// 
        /// Example: AsTypes = [typeof(IService1), typeof(IService2)]
        /// </summary>
        public Type[] AsTypes { get; set; } = Array.Empty<Type>();

        public AutoRegisterInAttribute(Type scopeType) {
            ScopeType = scopeType;
        }
    }
}
