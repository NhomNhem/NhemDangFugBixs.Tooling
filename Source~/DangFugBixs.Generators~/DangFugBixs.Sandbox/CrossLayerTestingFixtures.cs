using NhemDangFugBixs.Attributes;

namespace DangFugBixs.Sandbox.Testing;

/// <summary>
/// Test-only scope marker used by sandbox coverage.
/// This must not live in the shipped runtime package surface.
/// </summary>
public interface CrossLayerIdentity { }

/// <summary>
/// Test-only service used to validate cross-assembly-style scope mapping in sandbox coverage.
/// </summary>
[AutoRegisterIn(typeof(CrossLayerIdentity))]
public sealed class CrossLayerService
{
    public void DoSomething() => System.Console.WriteLine("CrossLayerService is working!");
}
