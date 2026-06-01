# Acceptance Criteria: Registration Exposure API Cleanup 7.2.1

## Overview

This document defines the acceptance criteria for the registration exposure API cleanup in version 7.2.1.

## Product Principle
- Services declare intent
- Composition owns registration
- Diagnostics protect architecture

## Functional Requirements

### FR-7.2.1-1: Explicit [As] / [AsSelf] are Canonical
- [ ] Explicit `[As(typeof(TContract))]` attributes are documented as canonical
- [ ] Explicit `[AsSelf]` attribute is documented as canonical
- [ ] README.md canonical usage examples use explicit attributes
- [ ] AutoRegisterInAttribute XML docs emphasize canonical usage

### FR-7.2.1-2: AutoRegisterIn Declares Only Scope and Lifetime in Docs
- [ ] AutoRegisterInAttribute XML docs state it should only declare scope and lifetime
- [ ] README.md documents the rule: AutoRegisterIn decides where and how long
- [ ] README.md documents the rule: As / AsSelf decide as what
- [ ] README.md documents the rule: EntryPoint / RegisterComponentInHierarchy decide registration kind

### FR-7.2.1-3: Legacy Flags Remain Supported for Compatibility
- [ ] AutoRegisterIn.AsImplementedInterfaces flag still functions
- [ ] AutoRegisterIn.AsSelf flag still functions
- [ ] Legacy flag-only behavior produces identical output to 7.2.0
- [ ] No breaking changes to existing code

### FR-7.2.1-4: Explicit Attributes Win Over Legacy Flags
- [ ] If type has explicit `[As]` or `[AsSelf]`, generator uses only those
- [ ] If explicit exposure attributes exist, legacy flags do not create duplicate generated exposure
- [ ] NormalizeExplicitContractBehavior implements this logic
- [ ] GetSmartSuffix implements this logic

### FR-7.2.1-5: NHEM_DI_060 Warning for Mixed Style
- [ ] Diagnostic ID NHEM_DI_060 added to DiagnosticIds.cs
- [ ] Diagnostic descriptor added to DiagnosticCatalog.cs
- [ ] Diagnostic severity is Warning
- [ ] Diagnostic message: "Mixed registration exposure style. Prefer explicit [As] / [AsSelf]; AutoRegisterIn should only declare scope and lifetime."
- [ ] Diagnostic warns when type mixes explicit exposure attributes with legacy flags
- [ ] Diagnostic does not trigger for pure explicit style
- [ ] Diagnostic does not trigger for pure legacy style
- [ ] Diagnostic does not trigger when AutoRegisterIn has only scope and lifetime

### FR-7.2.1-6: Prevent Duplicate Generated .As<T>()
- [ ] Explicit `[As]` only generates one `.As<TContract>()`
- [ ] Explicit `[As]` plus legacy `AsImplementedInterfaces=true` does not duplicate `.As<TContract>()`
- [ ] Explicit `[AsSelf]` plus legacy `AsSelf=true` does not duplicate self registration
- [ ] Generator tests verify no duplicate registrations

### FR-7.2.1-7: Deterministic Contract Ordering
- [ ] Contract output is deterministic and sorted
- [ ] GetSmartSuffix sorts AsTypes alphabetically
- [ ] GetSmartEntryPointSuffix sorts AsTypes alphabetically
- [ ] Sorting uses StringComparer.Ordinal
- [ ] Generator tests verify deterministic ordering

### FR-7.2.1-8: Composition-Only Behavior Preserved
- [ ] Composition-only service assemblies still emit no VContainer code
- [ ] No reintroduction of VContainer dependency into service-only assemblies
- [ ] Generator tests verify composition-only behavior

### FR-7.2.1-9: No Resolve<T>() Generation
- [ ] Generator does not generate `Resolve<T>()` calls
- [ ] No changes to resolution behavior

### FR-7.2.1-10: No RegisterInstance Support
- [ ] No RegisterInstance generation added
- [ ] No changes to instance registration behavior

## Non-Functional Requirements

### NFR-7.2.1-1: Performance
- [ ] Generator execution time does not increase significantly
- [ ] Analyzer execution time does not increase significantly
- [ ] No O(n²) algorithms introduced
- [ ] Sorting overhead is minimal

### NFR-7.2.1-2: Backward Compatibility
- [ ] Legacy flag-only behavior unchanged
- [ ] Existing code continues to work exactly as before
- [ ] No breaking changes to generated output
- [ ] Zero breaking changes before 8.0.0

### NFR-7.2.1-3: Accuracy
- [ ] NHEM_DI_060 has zero false positives
- [ ] Generator has no regressions
- [ ] Analyzer has no regressions

## Documentation Requirements

### DR-7.2.1-1: README.md Updates
- [ ] README.md canonical usage updated to show explicit attributes first
- [ ] README.md migration guide section added for 7.2.1
- [ ] README.md documents the rule: AutoRegisterIn decides where and how long, As / AsSelf decide as what
- [ ] README.md removes examples that use AsImplementedInterfaces = false and AsSelf = false together with [As] or [AsSelf]

### DR-7.2.1-2: AutoRegisterInAttribute XML Docs
- [ ] AutoRegisterInAttribute.cs XML docs updated to emphasize canonical usage
- [ ] XML docs add migration note about preferring explicit attributes
- [ ] XML docs remove mixed style examples
- [ ] XML docs update property documentation with backward compatibility notes

### DR-7.2.1-3: CHANGELOG.md
- [ ] CHANGELOG.md 7.2.1 section added
- [ ] CHANGELOG.md documents new diagnostic NHEM_DI_060
- [ ] CHANGELOG.md documents generator behavior changes
- [ ] CHANGELOG.md documents backward compatibility guarantees

## Sample Requirements

### SR-7.2.1-1: Sample Updates
- [ ] Update samples to use canonical explicit exposure style
- [ ] EntryPoint examples that implement VContainer.Unity interfaces live in Composition assemblies
- [ ] Service-only asmdef samples do not reference VContainer or VContainer.Unity

## Versioning Requirements

### VR-7.2.1-1: Package Version
- [ ] package.json bumped to 7.2.1

### VR-7.2.1-2: Project Versions
- [ ] DangFugBixs.Generators.csproj version bumped to 7.2.1
- [ ] DangFugBixs.Analyzers.csproj version bumped to 7.2.1
- [ ] DangFugBixs.Attributes.csproj version bumped to 7.2.1

### VR-7.2.1-3: DLL Payloads
- [ ] Runtime/NhemDangFugBixs.Attributes.dll rebuilt
- [ ] Runtime/NhemDangFugBixs.Runtime.dll rebuilt
- [ ] Analyzers/NhemDangFugBixs.Generators.dll rebuilt
- [ ] Analyzers/NhemDangFugBixs.Analyzers.dll rebuilt

## Test Requirements

### TR-7.2.1-1: Generator Tests
- [ ] Explicit [As] only generates one .As<TContract>()
- [ ] Explicit [As] plus legacy AsImplementedInterfaces=true does not duplicate .As<TContract>()
- [ ] Explicit [AsSelf] plus legacy AsSelf=true does not duplicate self registration
- [ ] Legacy flag-only behavior still works
- [ ] Contract output is deterministic and sorted
- [ ] Composition-only service assemblies still emit no VContainer code
- [ ] All existing generator tests pass

### TR-7.2.1-2: Analyzer Tests
- [ ] NHEM_DI_060 is emitted for mixed explicit + legacy exposure style
- [ ] NHEM_DI_060 is not emitted for pure explicit style
- [ ] NHEM_DI_060 is not emitted for pure legacy style
- [ ] NHEM_DI_060 is not emitted when AutoRegisterIn has only scope and lifetime
- [ ] All existing analyzer tests pass

## Release Gate Requirements

### RG-7.2.1-1: Generator Tests
- [ ] Generator tests must pass
- [ ] New generator tests pass
- [ ] Existing generator tests pass

### RG-7.2.1-2: Analyzer Tests
- [ ] Analyzer tests must pass
- [ ] New analyzer tests pass
- [ ] Existing analyzer tests pass

### RG-7.2.1-3: Version Drift Check
- [ ] Version drift check must pass
- [ ] All version numbers consistent

### RG-7.2.1-4: Docs Check
- [ ] Docs check must pass
- [ ] All documentation updated

### RG-7.2.1-5: Unity Sample Build
- [ ] Unity sample dotnet build must pass when NHEM_UNITY_PROJECT_ROOT is set
- [ ] Unity sample compile must pass when UNITY_EXE is set
- [ ] release-gate.ps1 must fail if Unity returns non-zero

## Success Criteria

- [ ] Explicit [As] / [AsSelf] are documented as canonical
- [ ] AutoRegisterIn only declares scope + lifetime in docs and new samples
- [ ] Legacy AutoRegisterIn.AsImplementedInterfaces / AsSelf remains supported only for compatibility
- [ ] Explicit [As] / [AsSelf] wins over legacy flags when mixed
- [ ] NHEM_DI_060 warning added for mixed style
- [ ] Duplicate generated .As<T>() prevented
- [ ] Docs updated
- [ ] Samples updated
- [ ] Tests pass
- [ ] Version bumped
- [ ] CHANGELOG updated
- [ ] DLL payloads rebuilt
- [ ] Release gate passes

## Exit Criteria

Release 7.2.1 is considered complete when:
- All acceptance criteria are met
- All tests pass
- All documentation is updated
- Release gate passes
- Package is ready for distribution
