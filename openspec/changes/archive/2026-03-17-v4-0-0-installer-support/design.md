## Context

Currently, the `NhemDangFugBixs.Tooling` Source Generator is excellent at automating standard `builder.Register<T>` calls. However, in large Unity projects, users still need to write significant manual registration code inside their `LifetimeScope.Configure` methods to handle third-party libraries (like ZLogger, MessagePipe), complex factory configurations, and ScriptableObject instances. This leads to bloated "God Classes." To solve this, we are introducing a Module/Installer pattern (inspired by Zenject's `IInstaller`), allowing users to encapsulate configuration logic into separate classes that the generator can automatically execute.

## Goals / Non-Goals

**Goals:**
- Provide a clean, interface-based approach (`IVContainerInstaller`) for organizing custom VContainer registration logic.
- Automatically detect and execute these installers during the generated registration phase.
- Support explicit execution ordering via `[InstallerOrder]` attribute to resolve dependency issues between installers.
- Ensure installers are executed *before* special callbacks and standard registrations to establish foundational dependencies.
- Remove the deprecated string-based `[AutoRegister]` and `[AutoRegisterFactory]` attributes.

**Non-Goals:**
- Automatically injecting dependencies into the Installer instances themselves (they will be instantiated via a parameterless constructor `new()`).
- Supporting complex dependency graphs between multiple installers; users should rely on the explicit `[InstallerOrder]` for linear control.

## Decisions

### 1. The `IVContainerInstaller` Interface
We will introduce `IVContainerInstaller` in the `NhemDangFugBixs.Runtime` assembly. It will have a single method: `void Install(global::VContainer.IContainerBuilder builder)`. This simple design mirrors industry standards and is easy to implement.

### 2. Detection via `ClassAnalyzer`
The generator will check if a class implements `IVContainerInstaller`. If true, it sets a new flag `IsInstaller = true` in the `ServiceInfo` model. It will also look for the `[InstallerOrder(int)]` attribute to capture the requested priority (defaulting to 0).

### 3. Execution Order in `RegistrationEmitter`
Within the generated `Register[Scope]` method, we will structure the code as follows:
1.  **Instantiate and Call Installers**: Sort all detected installers for the scope by `InstallerOrder` (ascending). Emit `new global::FullName().Install(builder);` for each.
2.  **Call Standard Registrations**: `builder.Register<T>...` (Standard services).
3.  **Call Special Callbacks**: Exception Handlers and Build Callbacks (Run LAST to ensure all services from installers and standard registrations are available).

Executing installers first allows them to configure base systems (like generic loggers or event buses) that standard services might depend on during their own initialization or resolution.

### 4. Unbreakable Safety Measures (Analyzers)
To prevent generated code from failing to compile, we will implement strict Roslyn Analyzers:
- **ND105 (Constructor)**: Error if an `IVContainerInstaller` lacks a public parameterless constructor.
- **ND106 (Accessibility)**: Error if an `IVContainerInstaller` is `internal` or `private` (preventing `CS0122`).
- **ND107 (Type Constraint)**: Error if an `IVContainerInstaller` inherits from `UnityEngine.Component` or `MonoBehaviour` (as they cannot be instantiated via `new()`).

### 5. Unified Diagnostic System
We will migrate all legacy `NHMxxx` codes to the `NDxxx` format to ensure a consistent and professional diagnostic experience for the user.

### 6. Removal of Legacy Attributes
Version 4.0 is a major bump, making it the perfect time to clean up technical debt. The `[AutoRegister(string)]` and `[AutoRegisterFactory]` attributes, which were marked obsolete in v3.0, will be completely removed from `NhemDangFugBixs.Runtime`.
