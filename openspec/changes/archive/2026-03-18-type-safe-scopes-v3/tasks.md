## 1. Runtime Attributes

- [x] 1.1 Create `AutoRegisterInAttribute.cs` with generic type parameter `TScope`
- [x] 1.2 Create `ScopeNameAttribute.cs` for custom scope name override
- [x] 1.3 Update `AutoRegisterAttribute.cs` to mark string `scope` parameter as deprecated
- [x] 1.4 Add XML documentation comments to all new attributes
- [x] 1.5 Build `DangFugBixs.Runtime` and verify DLL output

## 2. Generator Model Updates

- [x] 2.1 Add `ScopeType` property to `ServiceInfo.cs` in `DangFugBixs.Common`
- [x] 2.2 Add `UsesTypeSafeScope` boolean property to `ServiceInfo.cs`
- [x] 2.3 Rebuild `DangFugBixs.Common` to update shared models

## 3. Generator Analyzer Updates

- [x] 3.1 Update `ClassAnalyzer.ExtractInfo()` to detect `[AutoRegisterIn(typeof(TScope))]` generic attribute
- [x] 3.2 Implement scope type extraction from generic argument
- [x] 3.3 Implement scope type extraction from `typeof()` expression
- [x] 3.4 Add semantic validation: verify type inherits `LifetimeScope`
- [x] 3.5 Handle fallback to string-based `scope` parameter for backward compat check

## 4. Generator Emitter Updates

- [x] 4.1 Update `RegistrationEmitter.GenerateSource()` to use convention-based naming
- [x] 4.2 Implement "LifetimeScope" suffix stripping logic
- [x] 4.3 Add `[ScopeName("Custom")]` override support in emitter
- [x] 4.4 Handle namespace-safe method naming for duplicate class names
- [x] 4.5 Update generated code header to indicate v3.0

## 5. Roslyn Analyzer Diagnostics

- [x] 5.1 Implement ND001: "Scope type not found" diagnostic
- [x] 5.2 Implement ND002: "Invalid scope type (not LifetimeScope)" diagnostic
- [x] 5.3 Implement ND003: "Circular scope dependency" diagnostic
- [x] 5.4 Implement ND004: "Parent→Child dependency violation" diagnostic
- [x] 5.5 Implement ND103: "Unused scope registration" warning
- [x] 5.6 Add diagnostic descriptors with help link URLs
- [x] 5.7 Build `DangFugBixs.Analyzers` and verify DLL output

## 6. Unit Tests

- [x] 6.1 Create generator tests for `[AutoRegisterIn(typeof(TScope))]` syntax
- [x] 6.2 Create generator tests for convention-based naming
- [ ] 6.3 Create analyzer tests for ND001, ND002, ND003, ND004
- [ ] 6.4 Create analyzer tests for ND103 warning
- [x] 6.5 Create integration tests with mock `LifetimeScope` types
- [ ] 6.6 Run all tests and verify 100% pass rate

## 7. Sandbox & Examples

- [x] 7.1 Update `DangFugBixs.Sandbox` with v3.0 syntax examples
- [x] 7.2 Create `GameLifetimeScope` and `GameplayLifetimeScope` mock examples
- [x] 7.3 Demonstrate parent→child injection pattern
- [x] 7.4 Demonstrate `[ScopeName("Custom")]` override
- [x] 7.5 Build sandbox and verify generated output

## 8. Documentation

- [x] 8.1 Update `README.md` with v3.0 usage examples
- [x] 8.2 Add migration guide (v2.x → v3.0) to README
- [x] 8.3 Update `CHANGELOG.md` with v3.0.0 release notes
- [x] 8.4 Document all diagnostic codes (ND001-ND103) in README
- [x] 8.5 Add cross-scope dependency best practices guide

## 9. Release Preparation

- [x] 9.1 Update `package.json` version to 3.0.0
- [ ] 9.2 Run full build pipeline (`dotnet build`)
- [x] 9.3 Copy DLLs to `Runtime/` and `Analyzers/` folders
- [x] 9.4 Verify `.meta` files are correctly configured
- [ ] 9.5 Tag release: `git tag v3.0.0`
- [ ] 9.6 Push to remote: `git push origin main --tags`
