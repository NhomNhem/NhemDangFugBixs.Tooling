## 1. Runtime Updates

- [x] 1.1 Create `IVContainerInstaller.cs` interface in `NhemDangFugBixs.Runtime/Attributes`.
- [x] 1.2 Create `InstallerOrderAttribute.cs` in `NhemDangFugBixs.Runtime/Attributes` to support explicit execution ordering.
- [x] 1.3 Remove the obsolete `AutoRegisterAttribute.cs` (string-based scope registration).
- [x] 1.4 Remove the obsolete `AutoRegisterFactoryAttribute.cs`.
- [x] 1.5 Clean up any remaining references to the obsolete attributes in `NhemDangFugBixs.Runtime` (e.g., `DamageCalculator.cs`).

## 2. Generator & Analyzer Core Updates

- [x] 2.1 Update `ServiceInfo.cs` in `NhemDangFugBixs.Common` to include `IsInstaller` (bool) and `InstallerOrder` (int) properties.
- [x] 2.2 Update `ClassAnalyzer.cs` to detect `IVContainerInstaller` and `[InstallerOrder]`.
- [x] 2.3 Implement/Update `AutoRegisterRules.cs` (Analyzer) to include:
    - `ND105`: Error if Installer lacks public parameterless constructor.
    - `ND106`: Error if Installer is not `public`.
    - `ND107`: Error if Installer inherits from `Component/MonoBehaviour`.
- [x] 2.4 **Diagnostic Migration**: Rename all `NHMxxx` diagnostics to `NDxxx` in `AutoRegisterRules.cs` and related files.
- [x] 2.5 Update `ReferencedAssemblyScanner.cs` to correctly extract `IsInstaller` and `InstallerOrder` from referenced DLLs.

## 3. Emission Updates

- [x] 3.1 Update `RegistrationEmitter.cs` to separate installers from the standard service list during the grouping phase.
- [x] 3.2 Update `RegistrationEmitter.cs` to sort the detected installers by their `InstallerOrder` (ascending).
- [x] 3.3 Update `RegistrationEmitter.cs` to emit `new global::FullName().Install(builder);` for each installer.
- [x] 3.4 **Re-order Emission**: Ensure order is: `Installers` -> `Standard Services` -> `Special Callbacks`.

## 4. Documentation & Release

- [x] 4.1 Update `README.md` to document the new Module/Installer pattern and the updated Diagnostic codes (`ND001-ND107`).
- [x] 4.2 Update `CHANGELOG.md` for major version `4.0.0`.
- [x] 4.3 Increment package version to `4.0.0` in `package.json`.
