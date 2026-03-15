using System;

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterFactoryAttribute : Attribute {
        public NhemLifetime Lifetime { get; }
        public string Scope { get; }

        public AutoRegisterFactoryAttribute(NhemLifetime lifetime = NhemLifetime.Singleton, string scope = "Global") {
            this.Lifetime = lifetime;
            this.Scope = scope;
        }
    }
}
