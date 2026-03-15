namespace NhemDangFugBixs.Runtime.Testing {
    /// <summary>
    /// A test identity for cross-layer bridge verification.
    /// </summary>
    public class CrossLayerIdentity {}

    /// <summary>
    /// A test service defined in a separate assembly (Runtime.dll)
    /// that should be discovered by the generator in Sandbox.
    /// </summary>
    [NhemDangFugBixs.Attributes.AutoRegisterIn(typeof(CrossLayerIdentity))]
    public class CrossLayerService {
        public void DoSomething() => System.Console.WriteLine("CrossLayerService is working!");
    }
}
