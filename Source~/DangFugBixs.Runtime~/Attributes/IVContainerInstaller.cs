using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Identifies a class as an installer module for VContainer.
    /// Classes implementing this interface and decorated with [AutoRegisterIn] will be automatically
    /// instantiated and executed by the generator.
    /// </summary>
    public interface IVContainerInstaller {
        /// <summary>
        /// Installs dependencies into the container builder.
        /// The builder argument is passed as object to avoid a direct dependency on VContainer in the Runtime assembly,
        /// but it will be cast to IContainerBuilder in the generated code.
        /// </summary>
        void Install(object builder);
    }
}
