using System;

namespace NhemDangFugBixs.Attributes
{
    /// <summary>
    /// Auto-registers a scene MonoBehaviour as a VContainer component.
    /// The generator emits: builder.RegisterComponent(Object.FindFirstObjectByType&lt;T&gt;());
    /// Use this for scene managers/services that other components depend on.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterSceneAttribute : Attribute
    {
        public AutoRegisterSceneAttribute()
        {
        }
    }
}
