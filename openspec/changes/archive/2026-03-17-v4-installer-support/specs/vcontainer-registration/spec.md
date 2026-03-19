## MODIFIED Requirements

### Requirement: VContainer registration method generation
The generator SHALL generate registration methods for each scope, including calls to instantiate and invoke detected Installer classes. The generator MUST execute installers before standard service registrations and MUST sort the installers based on their defined execution order.

**Previous Behavior:**
- Generated methods only included `builder.Register...` calls for services, components, and entry points. No module/installer concept existed.

**New Behavior:**
- For any detected Installer class within a scope, the generator emits `new global::InstallerClass().Install(builder);`.
- The list of installers MUST be sorted by their execution order (ascending: lower numbers run first) before being emitted.
- All installer invocations MUST occur before exception handlers, build callbacks, and standard service registrations.

#### Scenario: Ordered Installer Invocation
- **WHEN** a scope has `InstallerA` (Order = 10), `InstallerB` (Order = -5), and a standard service
- **THEN** the generated method first calls `InstallerB.Install()`, then `InstallerA.Install()`, and finally registers the standard service.
