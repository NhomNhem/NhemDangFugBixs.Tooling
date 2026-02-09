using System;

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterAttribute : Attribute {
        public Lifetime Lifetime { get; }
        public string Scope { get; }

        public AutoRegisterAttribute(Lifetime lifetime = Lifetime.Singleton, string scope = "Global") {
            this.Lifetime = lifetime;
            this.Scope = scope;
        }
    }
}