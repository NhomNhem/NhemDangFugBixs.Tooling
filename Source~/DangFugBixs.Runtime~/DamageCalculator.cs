using NhemDangFugBixs.Attributes;

namespace NhemDangFugBixs.Runtime {
    [AutoRegister(Lifetime.Singleton, "Gameplay")]
    public class DamageCalculator : IDamageCalculator {
        public float CalculateDamage(float baseDamage, float modifier) {
            return baseDamage * modifier;
        }
    }
}
