# Release Checklist: Registration Exposure API Cleanup 7.2.1

## Overview

This checklist ensures all requirements are met before releasing version 7.2.1 of NhemDangFugBixs.Tooling.

## Pre-Release Checklist

### Code Implementation

- [ ] Phase 1: Analyzer diagnostic implementation
  - [ ] NHEM_DI_060 ID added to DiagnosticIds.cs
  - [ ] NHEM_DI_060 descriptor added to DiagnosticCatalog.cs
  - [ ] MixedExposureStyle added to SupportedDiagnostics
  - [ ] HasMixedExposureStyle() method implemented in AttributeContractAnalyzer.cs
  - [ ] Detection logic called in AnalyzeType()
  - [ ] Only warns when legacy flags are explicitly set (not default values)

- [ ] Phase 2: Generator behavior implementation
  - [ ] NormalizeExplicitContractBehavior() enhanced in ClassAnalyzer.cs
  - [ ] Explicit [AsSelf] check added independently
  - [ ] GetSmartSuffix() updated with deterministic sorting
  - [ ] GetSmartEntryPointSuffix() updated with deterministic sorting
  - [ ] Sorting uses StringComparer.Ordinal

- [ ] Phase 3: Documentation updates
  - [ ] AutoRegisterInAttribute.cs XML docs updated
  - [ ] README.md canonical usage updated
  - [ ] README.md migration guide section added
  - [ ] CHANGELOG.md 7.2.1 section added
  - [ ] Legacy flag examples marked as such
  - [ ] Mixed style examples removed

- [ ] Phase 4: Sample updates
  - [ ] New samples use canonical explicit style
  - [ ] Legacy samples marked as "legacy"
  - [ ] EntryPoint placement verified in Composition assemblies
  - [ ] Service-only asmdef samples verified for VContainer independence

### Testing

- [ ] Generator tests
  - [ ] Explicit [As] only generates one .As<TContract>()
  - [ ] Explicit [As] plus legacy AsImplementedInterfaces=true does not duplicate
  - [ ] Explicit [AsSelf] plus legacy AsSelf=true does not duplicate
  - [ ] Legacy flag-only behavior still works
  - [ ] Contract output is deterministic and sorted
  - [ ] Composition-only service assemblies still emit no VContainer code
  - [ ] All existing generator tests pass

- [ ] Analyzer tests
  - [ ] NHEM_DI_060 is emitted for mixed explicit + legacy exposure style
  - [ ] NHEM_DI_060 is not emitted for pure explicit style
  - [ ] NHEM_DI_060 is not emitted for pure legacy style
  - [ ] NHEM_DI_060 is not emitted when AutoRegisterIn has only scope and lifetime
  - [ ] All existing analyzer tests pass

### Versioning

- [ ] package.json version set to 7.2.1
- [ ] DangFugBixs.Generators.csproj AssemblyVersion set to 7.2.1
- [ ] DangFugBixs.Generators.csproj AssemblyFileVersion set to 7.2.1
- [ ] DangFangFugBixs.Analyzers.csproj AssemblyVersion set to 7.2.1
- [ ] DangFangFugBixs.Analyzers.csproj AssemblyFileVersion set to 7.2.1
- [ ] DangFugBixs.Attributes.csproj AssemblyVersion set to 7.2.1
- [ ] DangFugBixs.Attributes.csproj AssemblyFileVersion set to 7.2.1

### Build

- [ ] Build Runtime/NhemDangFugBixs.Attributes.dll
- [ ] Build Runtime/NhemDangFugBixs.Runtime.dll
- [ ] Build Analyzers/NhemDangFugBixs.Generators.dll
- [ ] Build Analyzers/NhemDangFugBixs.Analyzers.dll
- [ ] Copy DLLs to appropriate Unity package locations
- [ ] Verify DLL versions match 7.2.1

## Release Gate Checklist

### Automated Checks

- [ ] Generator tests pass
  - Command: Run all generator tests
  - Expected: All tests pass
  - Exit code: 0

- [ ] Analyzer tests pass
  - Command: Run all analyzer tests
  - Expected: All tests pass
  - Exit code: 0

- [ ] Version drift check passes
  - Command: Run version drift validation
  - Expected: No version mismatches
  - Exit code: 0

- [ ] Docs check passes
  - Command: Run documentation validation
  - Expected: All docs are valid
  - Exit code: 0

### Unity Integration Checks (Conditional)

- [ ] Unity sample dotnet build (if NHEM_UNITY_PROJECT_ROOT is set)
  - Command: dotnet build on Unity samples
  - Expected: Build succeeds
  - Exit code: 0

- [ ] Unity sample compile (if UNITY_EXE is set)
  - Command: Unity batchmode compile
  - Expected: Compile succeeds
  - Exit code: 0

- [ ] release-gate.ps1 Unity check
  - Command: ./scripts/release-gate.ps1
  - Expected: Fails if Unity returns non-zero
  - Exit code: Matches Unity exit code

## Release Verification

### Manual Verification

- [ ] Review generated code for explicit [As] attributes
  - Verify: Only explicit contracts are registered
  - Verify: No duplicate .As<T>() calls
  - Verify: Contracts are sorted alphabetically

- [ ] Review generated code for legacy flag-only code
  - Verify: Output is identical to 7.2.0
  - Verify: No changes to behavior
  - Verify: Backward compatibility maintained

- [ ] Review analyzer diagnostics
  - Verify: NHEM_DI_060 warns for mixed style
  - Verify: NHEM_DI_060 does not warn for canonical style
  - Verify: NHEM_DI_060 does not warn for legacy style
  - Verify: Other diagnostics are not affected

- [ ] Review documentation
  - Verify: README.md examples compile
  - Verify: AutoRegisterInAttribute.cs examples compile
  - Verify: Migration guide is clear
  - Verify: No broken links

- [ ] Review samples
  - Verify: New samples use canonical style
  - Verify: Legacy samples are marked
  - Verify: Samples compile in Unity
  - Verify: No VContainer references in service-only assemblies

### Edge Case Testing

- [ ] Test with empty AsTypes array
- [ ] Test with duplicate contract types
- [ ] Test with generic contracts
- [ ] Test with nested generics
- [ ] Test with multiple AutoRegisterIn attributes
- [ ] Test with inherited attributes

## Release Notes Preparation

### CHANGELOG.md

- [ ] 7.2.1 section added with date
- [ ] "Added" section lists new features
- [ ] "Changed" section lists behavioral changes
- [ ] "Deprecated" section notes legacy flags (not deprecated, but canonical recommended)
- [ ] "Compatibility" section emphasizes backward compatibility

### Release Announcement

- [ ] Draft release notes
- [ ] Highlight new diagnostic NHEM_DI_060
- [ ] Emphasize canonical explicit style recommendation
- [ ] Reassure backward compatibility
- [ ] Point to migration guide
- [ ] Include examples of before/after

## Post-Release Checklist

### Monitoring

- [ ] Monitor for user feedback on NHEM_DI_060
- [ ] Monitor for questions about migration
- [ ] Monitor for any unexpected behavior changes
- [ ] Track adoption of canonical explicit style

### Documentation Updates (if needed)

- [ ] Update FAQ based on common questions
- [ ] Add additional migration scenarios if needed
- [ ] Clarify any confusing points

### Future Planning

- [ ] Consider deprecation timeline for legacy flags
- [ ] Consider code fix provider for NHEM_DI_060
- [ ] Plan for next version based on feedback

## Release Sign-Off

### Approval

- [ ] All checklist items completed
- [ ] All tests passing
- [ ] All documentation updated
- [ ] All version numbers correct
- [ ] Release notes prepared
- [ ] Approval obtained from maintainers

### Release Execution

- [ ] Tag release as v7.2.1
- [ ] Push to package repository
- [ ] Update package manager (if applicable)
- [ ] Publish release notes
- [ ] Announce to users

## Rollback Plan

If critical issues are discovered after release:

- [ ] Revert to 7.2.0 package
- [ ] Document issues in issue tracker
- [ ] Fix issues in 7.2.2
- [ ] Re-run full test suite
- [ ] Re-release as 7.2.2

## Release Criteria

The release is considered ready when:

1. **All automated checks pass**: Generator tests, analyzer tests, version drift check, docs check
2. **All manual verifications pass**: Code review, documentation review, sample review
3. **All edge cases tested**: No unexpected behavior in edge case scenarios
4. **Backward compatibility verified**: Legacy flag-only code produces identical output
5. **Documentation complete**: All docs updated, examples verified, migration guide ready
6. **Release notes prepared**: CHANGELOG updated, announcement drafted
7. **Approval obtained**: Maintainers have reviewed and approved

## Blocking Issues

Release is blocked if any of the following occur:

- Generator tests fail
- Analyzer tests fail
- Version drift detected
- Documentation errors found
- Backward compatibility broken
- Critical bugs discovered
- Maintainer approval not obtained

## Non-Blocking Issues

The following do not block release but should be tracked:

- Minor documentation typos
- Sample code style inconsistencies
- Performance regressions within acceptable threshold
- Non-critical edge case behaviors

## Release Timeline

- **Implementation complete**: [Date]
- **Testing complete**: [Date]
- **Documentation complete**: [Date]
- **Release gate passed**: [Date]
- **Release date**: [Date]

## Contact Information

For questions about this release:
- Maintainer: [Name]
- Issue tracker: [URL]
- Documentation: [URL]
