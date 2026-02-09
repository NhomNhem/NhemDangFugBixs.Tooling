using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Lifetime enum for AutoRegister attribute.
    /// NOTE: These values map directly to VContainer.Lifetime enum values.
    /// The generator will emit code using VContainer.Lifetime in the generated registration methods.
    /// </summary>
    public enum Lifetime { Singleton, Transient, Scoped }
}