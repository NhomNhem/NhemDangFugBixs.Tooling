# Tasks: Registration Exposure API Cleanup 7.2.1

## Phase 1: Analyzer Diagnostic Implementation

### Task 1.1: Add Diagnostic ID
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/DiagnosticIds.cs`
- Add `public const string MixedExposureStyle = "NHEM_DI_060";`
- Verify ID follows existing pattern

### Task 1.2: Add Diagnostic Descriptor
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/DiagnosticCatalog.cs`
- Add `MixedExposureStyle` descriptor
- Set severity to Warning
- Set category to Usage
- Verify message matches specification

### Task 1.3: Update SupportedDiagnostics
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add `DiagnosticCatalog.MixedExposureStyle` to `SupportedDiagnostics` array

### Task 1.4: Implement Detection Logic
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/Rules/AttributeContractAnalyzer.cs`
- Add `HasMixedExposureStyle()` method
- Implement logic to detect mixed explicit + legacy exposure style
- Call detection in `AnalyzeType()` method
- Ensure only warns when legacy flags are explicitly set (not default values)

### Task 1.5: Write Analyzer Tests
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/OpenSpecAnalyzerMvpTests.cs`
- Add test for NHEM_DI_060 emission on mixed style
- Add test for no emission on pure explicit style
- Add test for no emission on pure legacy style
- Add test for no emission when AutoRegisterIn has only scope and lifetime
- Verify existing analyzer tests still pass

## Phase 2: Generator Behavior Implementation

### Task 2.1: Enhance NormalizeExplicitContractBehavior
**File**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Analyzers/ClassAnalyzer.cs`
- Update `NormalizeExplicitContractBehavior()` to check for explicit `[AsSelf]` attribute independently
- Ensure explicit `[AsSelf]` takes precedence over legacy `AsSelf` flag
- Ensure explicit `[As]` contracts take precedence over legacy `AsImplementedInterfaces` flag

### Task 2.2: Add Deterministic Contract Sorting
**File**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/Emitters/RegistrationEmitter.cs`
- Update `GetSmartSuffix()` to sort `AsTypes` alphabetically
- Update `GetSmartEntryPointSuffix()` to sort `AsTypes` alphabetically
- Verify sorting uses `StringComparer.Ordinal` for consistency

### Task 2.3: Write Generator Tests
**File**: `Source~/DangFugBixs.Generators~/DangFugBixs.Tests/BindingGenerationTests.cs`
- Add test for explicit `[As]` generating only one `.As<TContract>()`
- Add test for explicit `[As]` plus legacy `AsImplementedInterfaces=true` not duplicating
- Add test for explicit `[AsSelf]` plus legacy `AsSelf=true` not duplicating
- Add test for legacy flag-only behavior still working
- Add test for deterministic contract sorting
- Add test for composition-only service assemblies emitting no VContainer code
- Verify existing generator tests still pass

## Phase 3: Documentation Updates

### Task 3.1: Update AutoRegisterInAttribute XML Docs
**File**: `Source~/DangFugBixs.Attributes~/Attributes/AutoRegisterInAttribute.cs`
- Update XML documentation to emphasize canonical explicit usage
- Add migration note about preferring explicit attributes
- Keep legacy flag examples for backward compatibility reference
- Remove examples showing mixed usage with `AsImplementedInterfaces = false` and `AsSelf = false` together with `[As]`

### Task 3.2: Update README.md
**File**: `README.md`
- Update canonical usage section to show explicit attributes first
- Add migration guide section for 7.2.1
- Document the rule: AutoRegisterIn decides where and how long, As/AsSelf decide as what
- Update Phase 2 examples to use canonical explicit style

### Task 3.3: Update CHANGELOG.md
**File**: `CHANGELOG.md`
- Add 7.2.1 section
- Document new diagnostic NHEM_DI_060
- Document generator behavior changes
- Document backward compatibility guarantees

## Phase 4: Sample Updates

### Task 4.1: Update Samples to Canonical Style
**Location**: Samples~/ directory
- Update all samples to use explicit `[As]` and `[AsSelf]` attributes
- Remove legacy flag usage from new examples
- Ensure EntryPoint examples implementing VContainer.Unity interfaces live in Composition assemblies
- Ensure service-only asmdef samples do not reference VContainer or VContainer.Unity

## Phase 5: Versioning and Build

### Task 5.1: Bump Package Version
**File**: `package.json`
- Update version from 7.2.1 to 7.2.1 (already set, verify)

### Task 5.2: Bump Generator Project Version
**File**: `Source~/DangFugBixs.Generators~/DangFugBixs.Generators/DangFugBixs.Generators.csproj`
- Update AssemblyVersion to 7.2.1
- Update AssemblyFileVersion to 7.2.1

### Task 5.3: Bump Analyzer Project Version
**File**: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/DangFugBixs.Analyzers.csproj`
- Update AssemblyVersion to 7.2.1
- Update AssemblyFileVersion to 7.2.1

### Task 5.4: Bump Attributes Project Version
**File**: `Source~/DangFugBixs.Attributes~/DangFugBixs.Attributes/DangFugBixs.Attributes.csproj`
- Update AssemblyVersion to 7.2.1
- Update AssemblyFileVersion to 7.2.1

### Task 5.5: Rebuild DLL Payloads
- Build Runtime/NhemDangFugBixs.Attributes.dll
- Build Runtime/NhemDangFugBixs.Runtime.dll
- Build Analyzers/NhemDangFugBixs.Generators.dll
- Build Analyzers/NhemDangFugBixs.Analyzers.dll
- Copy to appropriate Unity package locations

## Phase 6: Validation and Release

### Task 6.1: Run Generator Tests
- Execute all generator tests
- Verify new tests pass
- Verify existing tests pass
- No regressions

### Task 6.2: Run Analyzer Tests
- Execute all analyzer tests
- Verify new tests pass
- Verify existing tests pass
- No regressions

### Task 6.3: Version Drift Check
- Run version drift validation
- Ensure all project versions are consistent
- Ensure package.json version matches assembly versions

### Task 6.4: Docs Check
- Verify documentation is consistent
- Verify examples compile
- Verify migration guide is clear

### Task 6.5: Unity Sample Build (Optional)
- If NHEM_UNITY_PROJECT_ROOT is set, run dotnet build
- Verify samples compile
- Verify no VContainer references in service-only assemblies

### Task 6.6: Unity Sample Compile (Optional)
- If UNITY_EXE is set, run Unity batchmode compile
- Verify samples compile in Unity
- Verify no Unity errors

### Task 6.7: Run Release Gate
- Execute `.\scripts\release-gate.ps1`
- Verify all checks pass
- Verify Unity check fails appropriately if Unity returns non-zero

## Task Dependencies

- Phase 1 must complete before Phase 2 (analyzer tests validate behavior)
- Phase 2 must complete before Phase 3 (generator behavior must be stable before docs)
- Phase 3 must complete before Phase 4 (docs must reflect changes before samples)
- Phase 4 must complete before Phase 5 (samples must be updated before versioning)
- Phase 5 must complete before Phase 6 (versioning must be complete before release)

## Estimated Effort

- Phase 1: 2-3 hours
- Phase 2: 3-4 hours
- Phase 3: 1-2 hours
- Phase 4: 1-2 hours
- Phase 5: 1 hour
- Phase 6: 1-2 hours

**Total**: 9-14 hours

## Risk Mitigation

- **Backward compatibility**: Ensure all legacy flag usage continues to work exactly as before
- **Test coverage**: Comprehensive test coverage for all exposure style combinations
- **Documentation**: Clear migration guide to help users understand changes
- **Diagnostic severity**: Warning level ensures no build breakage
- **Incremental implementation**: Each phase can be validated independently
