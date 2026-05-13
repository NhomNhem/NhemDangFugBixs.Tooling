# Getting Started with NhemDangFugBixs.Tooling

This guide will walk you through installing and using `NhemDangFugBixs.Tooling` in your Unity project.

## Installation

### Via Unity Package Manager (Recommended)

1. Open Unity and navigate to **Window > Package Manager**
2. Click the **+** button in the top-left corner
3. Select **Add package from git URL...**
4. Enter the repository URL: `https://github.com/yourusername/NhemDangFugBixs.Tooling.git`
5. Click **Add** and wait for Unity to install the package

### Manual Installation

1. Download the latest release from the GitHub repository
2. Extract the contents to your Unity project's `Packages` folder
3. Rename the extracted folder to `com.nhemdangfugbixs.tooling`
4. Unity will automatically detect and import the package

## Prerequisites

Before using `NhemDangFugBixs.Tooling`, ensure you have:

- Unity 2021.3 LTS or later
- [VContainer](https://github.com/hadashiA/VContainer) installed in your project
- .NET 4.x or .NET Standard 2.0 compatibility level in Player Settings

## Basic Usage

### Step 1: Define Scope Markers

Create marker interfaces in a shared assembly that represents your architectural layers:

```csharp
// SolarPhobia.Shared.Composition.cs
namespace SolarPhobia.Shared.Composition;

public interface IScopeMarker { }

public interface IProjectScope : IScopeMarker { }
public interface IGameplayScope : IScopeMarker { }
public interface IMainMenuScope : IScopeMarker { }
```

### Step 2: Mark Your Services

Add attributes to your service classes to declare their registration intent:

```csharp
// SolarPhobia.Application.Services.DayPhaseMechanicsService.cs
using NhemDangFugBixs.Tooling.Attributes;

namespace SolarPhobia.Application.Services;

[AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
[As<IDayPhaseMechanicsService>]
public sealed class DayPhaseMechanicsService : IDayPhaseMechanicsService
{
    private readonly INhemLogger _logger;
    private readonly ISoulRepository _soulRepository;
    private readonly IAnimationService _animationService;

    public DayPhaseMechanicsService(
        INhemLogger logger,
        ISoulRepository soulRepository,
        IAnimationService animationService)
    {
        _logger = logger;
        _soulRepository = soulRepository;
        _animationService = animationService;
    }

    // Service implementation...
}
```

### Step 3: Map Markers to LifetimeScopes

In your composition assemblies, map the scope markers to actual LifetimeScope implementations:

```csharp
// SolarPhobia.Composition.Scopes.GameplayLifetimeScope.cs
using NhemDangFugBixs.Tooling.Attributes;
using VContainer;

namespace SolarPhobia.Composition.Scopes;

[LifetimeScopeFor<IGameplayScope>]
public sealed class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // This will call the generated registration code
        builder.RegisterGeneratedFor<IGameplayScope>();
    }
}
```

### Step 4: Use the Generated Installer

The source generator will create extension methods that you can call from your LifetimeScopes:

```csharp
// The generator creates this method automatically:
// public static void RegisterGeneratedFor<TScopeMarker>(this IContainerBuilder builder)

// In your LifetimeScope.Configure() method:
protected override void Configure(IContainerBuilder builder)
{
    builder.RegisterGeneratedFor<IGameplayScope>();
}
```

## Verifying Your Setup

After setting up your services and scopes, you should see generated files in your project:

- `NhemVContainerGeneratedInstaller.g.cs` - Contains the registration extension methods
- `NhemDiReport.g.cs` - Contains diagnostic information (if enabled)

You can also use the CLI or Editor window to validate your setup:

```bash
# Using the CLI
di-smoke preflight MyGame.csproj

# Or open the Editor window: Window > Nhem/DI Diagnostics
```

## Next Steps

- Read the [Scope Marker Pattern](scope-marker-pattern.md) guide for a deeper understanding
- Explore the [Attributes Reference](attributes.md) for all available options
- Learn about [Entry Points](entry-points.md) for lifecycle-managed services
- Check out the [Samples~](Samples~) folder in the package for working examples

## Troubleshooting

If you encounter issues:

1. Ensure VContainer is properly installed in your project
2. Check that your assembly definitions are set up correctly
3. Verify that the source generator is running (look for generated .g.cs files)
4. Consult the [Troubleshooting](troubleshooting.md) guide for common problems