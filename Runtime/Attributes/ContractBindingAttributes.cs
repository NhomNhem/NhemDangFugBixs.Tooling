using System;

namespace NhemDangFugBixs.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class AsAttribute : Attribute {
        public Type ContractType { get; }

        public AsAttribute(Type contractType) {
            ContractType = contractType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class AsAttribute<TContract> : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class AsSelfAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class RegisterComponentInHierarchyAttribute : Attribute { }
}
