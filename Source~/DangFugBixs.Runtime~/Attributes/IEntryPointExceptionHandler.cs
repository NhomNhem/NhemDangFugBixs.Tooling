using System;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Identifies a class as a global exception handler for VContainer entry points.
    /// The generator will automatically register this using builder.RegisterEntryPointExceptionHandler.
    /// Note: VContainer only supports one exception handler per LifetimeScope.
    /// </summary>
    public interface IEntryPointExceptionHandler {
        void OnException(Exception ex);
    }
}
