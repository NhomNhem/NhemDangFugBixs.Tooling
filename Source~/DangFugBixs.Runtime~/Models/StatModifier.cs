namespace NhemDangFugBixs.Runtime.Models {
    public struct StatModifier {
        public float Value;
        public ModifierType Type;
        public object Source;

        public StatModifier(float value, ModifierType type, object source = null) {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}
