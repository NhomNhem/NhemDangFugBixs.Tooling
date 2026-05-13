# NhemDangFugBixs.Tooling Documentation

Welcome to the official documentation for `NhemDangFugBixs.Tooling` - a compile-time VContainer workflow toolkit for Unity projects.

This documentation will help you understand how to use the package to improve your dependency injection workflow with VContainer, making registration safer, faster, and more architecture-aware.

## What is NhemDangFugBixs.Tooling?

`NhemDangFugBixs.Tooling` is a Unity package that improves the developer workflow around VContainer by providing:

- Source-generated VContainer registrations
- Compile-time analyzers for DI architecture mistakes
- Scope marker mapping across asmdef/layer boundaries
- Attribute-driven registration for services, entry points, MonoBehaviours, installers, MessagePipe brokers
- CLI and Editor diagnostics for dependency graphs, scope maps, duplicate registrations, and architecture reports
- Documentation and reports that help both humans and AI coding agents understand the project architecture

## Documentation Overview

| Guide | Description |
|-------|-------------|
| [Getting Started](getting-started.md) | Installation, setup, and basic usage |
| [Scope Marker Pattern](scope-marker-pattern.md) | Understanding the core architectural pattern |
| [Attributes Reference](attributes.md) | Complete reference for all registration attributes |
| [Binding Features](binding.md) | Advanced binding capabilities (keyed, collection, open generics) |
| [Entry Points](entry-points.md) | Lifecycle management for startable services |
| [Scene Components](scene-components.md) | Integrating MonoBehaviour objects with VContainer |
| [MessagePipe Integration](messagepipe.md) | Working with MessagePipe event brokers |
| [Diagnostics & Analyzers](diagnostics.md) | Understanding compiler warnings and errors |
| [CLI Reference](cli.md) | Command-line tool for validation and reporting |
| [Editor Window](editor-window.md) | Unity Editor diagnostics window guide |
| [Architecture Guide](architecture.md) | Design principles and best practices |
| [Migration Guide](migration-guide.md) | Upgrading between versions |
| [Troubleshooting](troubleshooting.md) | Common issues and solutions |

## Quick Start

1. Install the package via Unity Package Manager
2. Add attributes to your service classes:
   ```csharp
   [AutoRegisterIn<IGameplayScope>(Lifetime = NhemLifetime.Scoped)]
   [As<IPlayerService>]
   public sealed class PlayerService : IPlayerService { }
   ```
3. Add scope mappings in your composition assemblies:
   ```csharp
   [LifetimeScopeFor<IGameplayScope>]
   public sealed class GameplayLifetimeScope : LifetimeScope { }
   ```
4. Call the generated installer from your composition scope:
   ```csharp
   builder.RegisterGeneratedFor<IGameplayScope>();
   ```

## Prerequisites

- Unity 2021.3 or later
- VContainer installed in your project
- C# 9.0 or later (for source generator features)

## Support

If you encounter issues or have questions, please:
- Check the [Troubleshooting](troubleshooting.md) guide
- Look through existing [GitHub Issues](https://github.com/yourusername/NhemDangFugBixs.Tooling/issues)
- Submit a new issue if your problem isn't already reported

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.