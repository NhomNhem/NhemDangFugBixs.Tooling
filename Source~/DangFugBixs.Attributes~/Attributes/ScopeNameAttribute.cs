using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Overrides the convention-based registration method name for a LifetimeScope.
    /// By default, the generator strips "LifetimeScope" suffix from the type name.
    /// Use this attribute to specify a custom name.
    /// </summary>
    /// <example>
    /// <code>
    /// [ScopeName("UI")]
    /// public class UserInterfaceLifetimeScope : LifetimeScope { }
    /// // Generates: RegisterUI() instead of RegisterUserInterface()
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class)]
#if NDF_INTERNAL_ATTRIBUTES
    internal
#else
    public
#endif
    sealed class ScopeNameAttribute : Attribute {
        /// <summary>
        /// The custom name to use for the registration method.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The custom registration method name (without "Register" prefix).</param>
        public ScopeNameAttribute(string name) {
            Name = name;
        }
    }
}
