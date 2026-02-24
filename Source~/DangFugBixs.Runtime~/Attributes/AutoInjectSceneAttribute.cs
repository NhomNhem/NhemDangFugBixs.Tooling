using System;

namespace NhemDangFugBixs.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AutoInjectSceneAttribute : Attribute
    {
        public AutoInjectSceneAttribute()
        {
        }
    }
}
