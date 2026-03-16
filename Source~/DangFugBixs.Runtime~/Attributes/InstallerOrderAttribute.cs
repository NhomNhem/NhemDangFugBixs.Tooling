using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Explicitly defines the execution priority of an IVContainerInstaller.
    /// Lower numbers are executed first (ascending order).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InstallerOrderAttribute : Attribute {
        public int Order { get; }

        public InstallerOrderAttribute(int order = 0) {
            Order = order;
        }
    }
}
