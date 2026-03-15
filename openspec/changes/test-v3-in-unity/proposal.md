## Why

The v3.0 type-safe scopes implementation is complete in the tooling project, but it needs real-world validation in an actual Unity project (GameFeelUnity) before release. Testing in a production-like environment will uncover integration issues, validate the migration path, and ensure the generator works correctly with Unity's build pipeline.

## What Changes

- **Deploy v3.0 DLLs to GameFeelUnity**: Copy Runtime, Generators, and Analyzers DLLs to the Unity project
- **Setup test LifetimeScopes**: Create GameLifetimeScope and GameplayLifetimeScope in the Unity project
- **Migrate existing services**: Convert existing `[AutoRegister]` usages to `[AutoRegisterIn<TScope>]`
- **Validate generated code**: Verify convention-based naming and scope grouping works correctly
- **Test parent-child injection**: Ensure child scopes can inject parent services
- **Document issues**: Capture any bugs, errors, or UX problems encountered

## Capabilities

### New Capabilities

- `unity-deployment`: Process for deploying tooling DLLs to a Unity project for testing
- `integration-testing`: Testing v3.0 features in a real Unity project environment
- `migration-validation`: Validating the v2.x → v3.0 migration path works as documented

### Modified Capabilities

- None (this is a testing/validation change, not a tooling modification)

## Impact

- **GameFeelUnity Project**: Will have v3.0 tooling installed for testing
- **v3.0 Release**: Test results will inform final release decisions
- **Documentation**: Real-world migration experience may update migration guide
- **Bug Discovery**: Any issues found will be fixed before v3.0 public release
