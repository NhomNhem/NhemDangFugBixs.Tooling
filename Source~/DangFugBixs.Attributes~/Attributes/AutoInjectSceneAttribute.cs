using System;

namespace NhemDangFugBixs.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class AutoInjectSceneAttribute : Attribute
    {
        public AutoInjectSceneAttribute()
        {
        }
    }
}
