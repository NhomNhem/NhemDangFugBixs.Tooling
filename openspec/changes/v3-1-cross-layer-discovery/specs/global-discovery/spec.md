## ADDED Requirements

### Requirement: Cross-Assembly Metadata Scan
The Source Generator SHALL scan all referenced assemblies for `[AutoRegisterIn(typeof(T))]` attributes when the current assembly contains a class marked with `[LifetimeScopeFor(typeof(T))]`.

#### Scenario: Scan Referenced Assemblies
- **WHEN** assembly `Main` references assembly `Core`
- **THEN** SG in `Main` detects registrations in `Core` and emits registration code in `Main`

### Requirement: Opt-In Discovery
The Source Generator SHALL automatically discover and process any assembly that has a reference to `NhemDangFugBixs.Runtime.dll`.

#### Scenario: Automatic Discovery
- **WHEN** a new assembly `Services.asmdef` is added with a dependency on the Runtime DLL
- **THEN** it is automatically scanned by the SG in the high-level assembly
