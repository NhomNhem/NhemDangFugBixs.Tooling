## Why

While the generator has made significant progress, it still lacks full parity with VContainer's native registration capabilities. Recent bugs have highlighted issues with Entry Point detection for MonoBehaviours and missing namespace imports in generated code. To become a truly professional tool, it must support the full breadth of VContainer's features seamlessly and reliably.

## What Changes

- **Automatic Namespace Resolution**: Ensure `VContainer` and `VContainer.Unity` are always imported in generated files.
- **Robust Component Lifecycle Handling**: Fix the logic where MonoBehaviours implementing `IInitializable` etc. are incorrectly registered or lose their interface bindings.
- **Enhanced Registration API**: Ensure all VContainer registration methods (Register, RegisterComponent, RegisterEntryPoint, RegisterFactory) are used correctly based on the service type.
- **Code Generation Quality**: Eliminate CS1503 and CS1061 errors by ensuring correct method overloads are called with fully qualified types.

## Capabilities

### New Capabilities
- `automatic-vcontainer-imports`: Reliable inclusion of necessary VContainer namespaces.
- `polymorphic-component-registration`: Correct registration of MonoBehaviours that implement life-cycle interfaces.

### Modified Capabilities
- `vcontainer-registration`: Update to handle all registration scenarios (C# class, MonoBehaviour, Factory) with correct life-cycle interface binding.
- `robust-entry-point-detection`: Extend to ensure it differentiates between standard classes and Components for registration purposes.

## Impact

- **NhemDangFugBixs.Generators**: Updates to `RegistrationEmitter` and `ClassAnalyzer`.
- **Unity Projects**: Total removal of manual registration requirements for all VContainer-supported patterns.
