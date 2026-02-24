using NhemDangFugBixs.Runtime.Attributes;

namespace NhemDangFugBixs.Sandbox {
    [StatContainer]
    public partial class HeroStats {
        [Stat] public float Attack = 10;
        [Stat] public float Defense = 5;
        [Stat] public float MoveSpeed = 300;
    }
}
