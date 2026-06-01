using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Marks a service as intentionally excluded from auto-generated registration.
    /// Use this when a service requires manual registration via factory lambda,
    /// WithParameter, or RegisterInstance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class ManualFactoryAttribute : Attribute {
        /// <summary>
        /// Optional explanation for why this service requires manual registration.
        /// </summary>
        public string? Reason { get; init; }

        public ManualFactoryAttribute() { }
    }
}
