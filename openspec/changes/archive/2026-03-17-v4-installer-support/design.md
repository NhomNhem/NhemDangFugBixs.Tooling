## Context

Currently, the `NhemDangFugBixs.Tooling` Source Generator is excellent at automating standard `builder.Register<T>` calls. However, in large Unity projects, users still need to write significant manual registration code inside their `LifetimeScope.Configure` methods to handle third-party libraries (like ZLogger, MessagePipe), complex factory configurations, and ScriptableObject instances. This leads to bloated "God Classes." To solve this, we are introducing a Module/Installer pattern (inspired by Zenject's `IInstaller`), allowing users to encapsulate configuration logic into separate classes that the generator can automatically execute.

## Goals / Non-Goals

**Goals:**
- Provide a clean, interface-based approach (`IVContainerInstaller`) for organizing custom VContainer registration logic.
- Automatically detect and execute these installers during the generated registration phase.
- Ensure installers are executed *before* auto-registered services to establish foundational dependencies.
- Remove the deprecated string-based `[AutoRegister]` attribute.

**Non-Goals:**
- Handling complex dependency graphs between multiple installers (users should rely on VContainer's native resolution for that).
- Automatically injecting dependencies into the Installer instances themselves (they will be instantiated via a simple `new()`).

## Decisions

### 1. The `IVContainerInstaller` Interface
We will introduce `IVContainerInstaller` in the `NhemDangFugBixs.Runtime` assembly. It will have a single method: `void Install(global::VContainer.IContainerBuilder builder)`. This simple design mirrors industry standards and is easy to implement.

### 2. Detection via `ClassAnalyzer`
The generator will check if a class implements `IVContainerInstaller` and is decorated with `[AutoRegisterIn]`. If both are true, it sets a new flag `IsInstaller = true` in the `ServiceInfo` model.

### 3. Execution Order in `RegistrationEmitter`
Within the generated `Register[Scope]` method, we will group the registrations:
1.  **Call Installers**: `new global::MyInstaller().Install(builder);`
2.  **Call Special Callbacks**: Exception Handlers and Build Callbacks.
3.  **Call Standard Registrations**: `builder.Register<T>...`

Executing installers first allows them to configure base systems (like generic loggers or event buses) that standard services might depend on during their own initialization or resolution.

### 4. Removal of Legacy `[AutoRegister]`
Version 4.0 is a major bump, making it the perfect time to clean up technical debt. The `[AutoRegister(string)]` attribute, which was marked obsolete in v3.0, will be completely removed from `NhemDangFugBixs.Runtime`.

## Risks / Trade-offs

- **[Trade-off]** Installers are instantiated via `new()` rather than resolved from the container.
  - **[Decision]** This is intentional. Installers exist to *configure* the container; they shouldn't depend on the container being fully built. If they need data, they should access it via static contexts or be manually registered in the root scope.
- **[Risk]** Lack of strict execution order among multiple installers in the same scope.
  - **[Mitigation]** For v4.0, installers will be executed in the order they are discovered (which is deterministic but not explicitly controllable). If users need strict ordering, they should combine logic into a single installer. We may explore `[Order]` attributes in v4.1 if demanded.
