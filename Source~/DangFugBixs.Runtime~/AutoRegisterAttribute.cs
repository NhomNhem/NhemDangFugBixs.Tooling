using System;

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterAttribute : Attribute {
        public Lifetime Lifetime { get; }
        public string Scope { get; }
        
        public bool AsImplementedInterfaces { get; set; } = true;
        public bool AsSelf { get; set; } = true;

        public AutoRegisterAttribute(Lifetime lifetime = Lifetime.Singleton, string scope = "Global") {
            this.Lifetime = lifetime;
            this.Scope = scope;
        }
    }
}