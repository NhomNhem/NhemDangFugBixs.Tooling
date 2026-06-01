using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Adds a VContainer keyed registration to generated output.
    /// Supported key expressions include string, numeric literals, enum members, and typeof(T).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class KeyedAttribute : Attribute {
        public object? Key { get; }

        public KeyedAttribute(string key) {
            Key = key;
        }

        public KeyedAttribute(int key) {
            Key = key;
        }

        public KeyedAttribute(long key) {
            Key = key;
        }
    }
}
