## 1. Runtime Updates

- [x] 1.1 Add `LifetimeScopeForAttribute<T>` to `NhemDangFugBixs.Runtime`.
- [x] 1.2 Update `AutoRegisterInAttribute<T>` documentation to mention Identity Type usage.
- [x] 1.3 Add any necessary marker interfaces or base types for Identity support.

## 2. Generator Logic - Analysis Phase

- [x] 2.1 Update `ClassAnalyzer.cs` to detect `[LifetimeScopeFor(typeof(T))]` attribute.
- [x] 2.2 Implement `ReferencedAssemblyScanner` to find `[AutoRegisterIn(typeof(T))]` in metadata.
- [x] 2.3 Create mapping logic between discovered Identity Types and mapped `LifetimeScope` types.
- [x] 2.4 Add validation to prevent multiple `[LifetimeScopeFor(typeof(T))]` for the same Identity.

## 3. Generator Logic - Emission Phase

- [x] 3.1 Update `RegistrationEmitter.cs` to generate `VContainerRegistration.Master.g.cs`.
- [x] 3.2 Implement `RegisterAll(IContainerBuilder builder)` unified entry point.
- [x] 3.3 Ensure grouped registration methods (e.g., `RegisterForGame`) are generated correctly using Metadata info.
- [x] 3.4 Handle namespace resolution correctly for types in external assemblies.

## 4. Tests & Verification

- [x] 4.1 Create a test assembly representing a low-level layer with Identity references.
- [x] 4.2 Create a test assembly representing a high-level layer with `LifetimeScopeFor`.
- [x] 4.3 Verify that the generated code in the high-level layer correctly registers services from the low-level layer.
- [x] 4.4 Verify backward compatibility with v3.0 local registrations.

## 5. Documentation & Samples

- [x] 5.1 Update `README.md` with instructions for cross-layer setups.
- [x] 5.2 Add a new sample in `GameFeelUnity-Test-Files` showcasing the 3-layer bridge pattern.
