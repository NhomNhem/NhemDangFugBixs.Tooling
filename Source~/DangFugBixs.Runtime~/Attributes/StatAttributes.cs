using System;

namespace NhemDangFugBixs.Runtime.Attributes {
    /// <summary>
    /// Mark a class as a container for stats to enable source generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class StatContainerAttribute : Attribute { }

    /// <summary>
    /// Mark a field or property as a Stat to generate optimization logic.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public sealed class StatAttribute : Attribute {
        public string Name { get; }
        public StatAttribute(string name = "") {
            Name = name;
        }
    }
}
