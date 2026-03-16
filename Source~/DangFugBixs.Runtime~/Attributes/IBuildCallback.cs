using VContainer;

namespace NhemDangFugBixs.Attributes {
    /// <summary>
    /// Identifies a class that should be executed as a callback immediately after the container is built.
    /// The generator will automatically register this using builder.RegisterBuildCallback.
    /// The class itself will also be registered as a Singleton service so it can be resolved.
    /// </summary>
    public interface IBuildCallback {
        void OnBuild(IObjectResolver container);
    }
}
