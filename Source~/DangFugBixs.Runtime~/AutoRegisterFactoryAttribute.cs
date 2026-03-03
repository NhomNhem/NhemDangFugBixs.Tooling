using System;

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterFactoryAttribute : Attribute {
        public Lifetime Lifetime { get; }
        public string Scope { get; }

        public AutoRegisterFactoryAttribute(Lifetime lifetime = Lifetime.Singleton, string scope = "Global") {
            this.Lifetime = lifetime;
            this.Scope = scope;
        }
    }
}
