## 1. Runtime Updates

- [x] 1.1 Create `IVContainerInstaller.cs` interface in `NhemDangFugBixs.Runtime/Attributes`.
- [x] 1.2 Create `InstallerOrderAttribute.cs` in `NhemDangFugBixs.Runtime/Attributes` to support explicit execution ordering.
- [x] 1.3 Remove the obsolete `AutoRegisterAttribute.cs` (string-based scope registration).
- [x] 1.4 Remove the obsolete `AutoRegisterFactoryAttribute.cs`.
- [x] 1.5 Clean up any remaining references to the obsolete attributes in `NhemDangFugBixs.Runtime` (e.g., `DamageCalculator.cs`).

## 2. Generator Core Updates

- [ ] 2.1 Update `ServiceInfo.cs` in `NhemDangFugBixs.Common` to include `IsInstaller` (bool) and `InstallerOrder` (int) properties.
- [ ] 2.2 Update `ClassAnalyzer.cs` to check if a class implements `IVContainerInstaller` and extract its `[InstallerOrder]` if present.
- [ ] 2.3 Add a validation check in `ClassAnalyzer.cs` (or via diagnostics) to ensure `IVContainerInstaller` implementations have a parameterless constructor (emit `ND105` if they don't).
- [ ] 2.4 Remove logic from `ClassAnalyzer.cs` that parses the legacy `AutoRegister` attributes.
- [ ] 2.5 Update `ReferencedAssemblyScanner.cs` to detect installers and their order in referenced assemblies.

## 3. Emission Updates

- [ ] 3.1 Update `RegistrationEmitter.cs` to separate installers from the standard service list during the grouping phase.
- [ ] 3.2 Update `RegistrationEmitter.cs` to sort the detected installers by their `InstallerOrder` (ascending).
- [ ] 3.3 Update `RegistrationEmitter.cs` to emit `new global::FullName().Install(builder);` for each installer, placing this block BEFORE exception handlers and standard registrations.

## 4. Documentation & Release

- [ ] 4.1 Update `README.md` to document the new Module/Installer pattern, the `[InstallerOrder]` attribute, and announce the removal of legacy attributes.
- [ ] 4.2 Update `CHANGELOG.md` for major version `4.0.0`.
- [ ] 4.3 Increment package version to `4.0.0` in `package.json`.
