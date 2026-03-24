using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Lifetime enum for AutoRegister attribute.
    /// NOTE: These values map directly to VContainer.Lifetime enum values.
    /// Renamed to NhemLifetime to avoid naming conflicts with VContainer.Lifetime.
    /// </summary>
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    enum NhemLifetime { Singleton, Transient, Scoped }
}