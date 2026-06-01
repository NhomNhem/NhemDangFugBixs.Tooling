namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Optional marker interface for Identity Types.
    /// Using this is not required, but it provides a clear intention that a type 
    /// is being used as a VContainer Scope Identity.
    /// </summary>
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    interface ILifetimeScopeIdentity {
    }
}
