## Why

In complex Unity projects, `LifetimeScope` files (like `GameLifetimeScope`) often become "God Classes" containing hundreds of lines of registration code, third-party library configurations (e.g., ZLogger, MessagePipe), and ScriptableObject bindings. While the Source Generator automates `builder.Register<T>`, it does not provide a structural way to organize and modularize custom builder configurations. Introducing a Module/Installer pattern (similar to Zenject) will allow users to split their DI configuration into clean, maintainable, and decoupled pieces that the generator can automatically assemble. Furthermore, ensuring these installers execute in a predictable order is crucial for dependent configurations.

## What Changes

- ****BREAKING** Version Bump**: Bumping the major version to v4.0.0 as this introduces a significant new architectural pattern for users and removes legacy APIs.
- **Installer Interface**: Add a new `IVContainerInstaller` interface to the `NhemDangFugBixs.Runtime` assembly.
- **Installer Ordering**: Add an `[InstallerOrder(int)]` attribute to allow users to explicitly define the execution priority of their installers.
- **Automatic Installer Registration**: Update the Source Generator to detect classes implementing `IVContainerInstaller`, sort them by their assigned order, and automatically invoke their `Install(IContainerBuilder)` method within the generated scope registration.
- **Deprecation Cleanup**: Officially remove the legacy string-based `[AutoRegister]` attribute that was marked obsolete in v3.0.

## Capabilities

### New Capabilities
- `installer-support`: Automatic detection, instantiation, and execution of classes implementing `IVContainerInstaller`.
- `installer-ordering`: Control the execution order of installers using the new `[InstallerOrder]` attribute.

### Modified Capabilities
- `vcontainer-registration`: Update the emission logic to instantiate installers, sort them by priority, and invoke their `Install` methods before standard service registrations.

## Impact

- **NhemDangFugBixs.Runtime**: Addition of `IVContainerInstaller` interface and `[InstallerOrder]` attribute. Removal of legacy `[AutoRegister]` attribute.
- **NhemDangFugBixs.Generators**: Updates to `ClassAnalyzer` to detect the new interface and attribute, and `RegistrationEmitter` to support ordered installer instantiation and invocation.
- **Unity Projects**: Users can drastically simplify their `Configure` methods by creating decoupled Installer classes. Legacy `[AutoRegister("Scope")]` usages will break and must be migrated to `[AutoRegisterIn]`.
