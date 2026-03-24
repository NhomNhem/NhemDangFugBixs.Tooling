using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Mark a class as a container for stats to enable source generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class StatContainerAttribute : Attribute { }

    /// <summary>
    /// Mark a field or property as a Stat to generate optimization logic.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class StatAttribute : Attribute {
        public string Name { get; }
        public StatAttribute(string name = "") {
            Name = name;
        }
    }
}
