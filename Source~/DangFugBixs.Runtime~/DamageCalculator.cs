using NhemDangFugBixs.Attributes;

namespace NhemDangFugBixs.Runtime {
    [NhemDangFugBixs.Attributes.AutoRegisterIn(typeof(NhemDangFugBixs.Runtime.Testing.CrossLayerIdentity), Lifetime = NhemLifetime.Singleton)]
    public class DamageCalculator : IDamageCalculator {
        public float CalculateDamage(float baseDamage, float modifier) {
            return baseDamage * modifier;
        }
    }
}
