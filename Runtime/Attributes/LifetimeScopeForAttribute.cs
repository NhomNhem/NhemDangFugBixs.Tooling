using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Maps a LifetimeScope to a specific Identity Type.
    /// This identity is used by services in other assemblies to register themselves
    /// without needing a direct reference to the LifetimeScope type.
    /// </summary>
    /// <example>
    /// <code>
    /// [LifetimeScopeFor(typeof(GameScope))]
    /// public class GameLifetimeScope : LifetimeScope {
    ///     protected override void Configure(IContainerBuilder builder) {
    ///         VContainerRegistration.RegisterAll(builder);
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class LifetimeScopeForAttribute : Attribute {
        /// <summary>
        /// The Identity Type (marker class or interface) used for discovery.
        /// </summary>
        public Type IdentityType { get; }

        public LifetimeScopeForAttribute(Type identityType) {
            IdentityType = identityType;
        }
    }

    /// <summary>
    /// Generic version for C# 11.0+.
    /// [LifetimeScopeFor<GameScope>]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class LifetimeScopeForAttribute<TIdentity> : Attribute { }
}
