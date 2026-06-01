using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Identifies a type to be registered as a MessagePipe broker in a specific VContainer scope.
    /// The generator will emit builder.RegisterMessageBroker<T>().
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    class AutoRegisterMessageBrokerInAttribute : Attribute {
        public Type ScopeType { get; }
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Singleton;

        public AutoRegisterMessageBrokerInAttribute(Type scopeType) {
            ScopeType = scopeType;
        }
    }

    /// <summary>
    /// Generic version for C# 11.0+.
    /// [AutoRegisterMessageBrokerIn<GameScope>]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    class AutoRegisterMessageBrokerInAttribute<TScope> : Attribute {
        public NhemLifetime Lifetime { get; set; } = NhemLifetime.Singleton;
    }
}
