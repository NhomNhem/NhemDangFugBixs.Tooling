using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Maps a LifetimeScope to a specific Identity Type.
    /// This identity is used by services in other assemblies to register themselves
    /// without needing a direct reference to the LifetimeScope type.
    /// </summary>
    /// <typeparam name="TIdentity">The Identity Type (marker class or interface) used for discovery.</typeparam>
    /// <example>
    /// <code>
    /// [LifetimeScopeFor&lt;GameScope&gt;]
    /// public class GameLifetimeScope : LifetimeScope {
    ///     protected override void Configure(IContainerBuilder builder) {
    ///         VContainerRegistration.RegisterAll(builder);
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class LifetimeScopeForAttribute<TIdentity> : Attribute {
    }
}
