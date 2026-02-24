using NUnit.Framework;
using NhemDangFugBixs.Sandbox;
using NhemDangFugBixs.Runtime.Models;

namespace NhemDangFugBixs.Tests {
    public class StatSystemTests {
        [Test]
        public void TestStatCalculation() {
            var stats = new HeroStats {
                Attack = 100 // Base value
            };

            // Initial value
            Assert.That(stats.AttackValue, Is.EqualTo(100f));

            // Add Flat
            stats.AddAttackModifier(new StatModifier(20, ModifierType.Flat));
            Assert.That(stats.AttackValue, Is.EqualTo(120f));

            // Add PercentAdd (20% + 10% = 30%)
            stats.AddAttackModifier(new StatModifier(0.2f, ModifierType.PercentAdd));
            stats.AddAttackModifier(new StatModifier(0.1f, ModifierType.PercentAdd));
            // (100 + 20) * (1 + 0.3) = 120 * 1.3 = 156
            Assert.That(stats.AttackValue, Is.EqualTo(156f));

            // Add PercentMult (x2)
            stats.AddAttackModifier(new StatModifier(2.0f, ModifierType.PercentMult));
            // 156 * 2 = 312
            Assert.That(stats.AttackValue, Is.EqualTo(312f));
        }

        [Test]
        public void TestDirtyFlagCaching() {
            var stats = new HeroStats { Attack = 100 };
            
            // First calculation
            float v1 = stats.AttackValue;
            
            // Value shouldn't change if no modifiers added
            Assert.That(stats.AttackValue, Is.EqualTo(v1));
            
            // Add and remove should dirty it
            var mod = new StatModifier(50, ModifierType.Flat);
            stats.AddAttackModifier(mod);
            Assert.That(stats.AttackValue, Is.EqualTo(150f));
            
            stats.RemoveAttackModifier(mod);
            Assert.That(stats.AttackValue, Is.EqualTo(100f));
        }
    }
}
