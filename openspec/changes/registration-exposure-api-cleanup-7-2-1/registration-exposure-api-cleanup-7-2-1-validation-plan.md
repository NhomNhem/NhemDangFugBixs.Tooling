# Validation Plan: Registration Exposure API Cleanup 7.2.1

## Overview

This validation plan defines the validation strategy for version 7.2.1 to ensure the registration exposure API cleanup is implemented correctly.

## Validation Phases

### Phase 1: Code Validation
### Phase 2: Test Validation
### Phase 3: Documentation Validation
### Phase 4: Integration Validation
### Phase 5: Release Gate Validation

## Phase 1: Code Validation

### CV-1.1: Analyzer Implementation
**Check**: NHEM_DI_060 diagnostic implemented correctly

**Validation Steps**:
1. Review DiagnosticIds.cs for NHEM_DI_060 constant
2. Review DiagnosticCatalog.cs for NHEM_DI_060 descriptor
3. Verify severity is Warning
4. Verify message matches specification
5. Review AttributeContractAnalyzer.cs for detection logic
6. Verify HasMixedExposureStyle() implementation
7. Verify diagnostic is added to SupportedDiagnostics

**Acceptance Criteria**:
- [ ] NHEM_DI_060 constant exists
- [ ] NHEM_DI_060 descriptor has correct properties
- [ ] Detection logic correctly identifies mixed style
- [ ] Detection logic does not trigger for pure explicit style
- [ ] Detection logic does not trigger for pure legacy style

---

### CV-1.2: Generator Implementation
**Check**: Generator behavior updated correctly

**Validation Steps**:
1. Review ClassAnalyzer.cs NormalizeExplicitContractBehavior() enhancement
2. Verify explicit [AsSelf] check added independently
3. Review RegistrationEmitter.cs GetSmartSuffix() sorting
4. Review RegistrationEmitter.cs GetSmartEntryPointSuffix() sorting
5. Verify sorting uses StringComparer.Ordinal
6. Verify no duplicate .As<T>() calls generated

**Acceptance Criteria**:
- [ ] NormalizeExplicitContractBehavior checks for [AsSelf] independently
- [ ] GetSmartSuffix sorts AsTypes alphabetically
- [ ] GetSmartEntryPointSuffix sorts AsTypes alphabetically
- [ ] Explicit attributes take precedence over legacy flags
- [ ] No duplicate registrations generated

---

### CV-1.3: Version Numbers
**Check**: All version numbers updated to 7.2.1

**Validation Steps**:
1. Review package.json version
2. Review DangFugBixs.Generators.csproj version
3. Review DangFugBixs.Analyzers.csproj version
4. Review DangFugBixs.Attributes.csproj version

**Acceptance Criteria**:
- [ ] package.json version is 7.2.1
- [ ] All project file versions are 7.2.1
- [ ] AssemblyVersion is 7.2.1
- [ ] AssemblyFileVersion is 7.2.1

---

## Phase 2: Test Validation

### TV-2.1: Generator Tests
**Check**: New generator tests pass

**Validation Steps**:
1. Run generator tests for explicit [As] only
2. Run generator tests for explicit [As] with legacy flags
3. Run generator tests for explicit [AsSelf] with legacy flags
4. Run generator tests for legacy flag-only behavior
5. Run generator tests for deterministic sorting
6. Run generator tests for composition-only behavior
7. Run all existing generator tests

**Acceptance Criteria**:
- [ ] Explicit [As] only generates one .As<TContract>()
- [ ] Explicit [As] plus legacy flags does not duplicate
- [ ] Explicit [AsSelf] plus legacy flags does not duplicate
- [ ] Legacy flag-only behavior still works
- [ ] Contract output is deterministic and sorted
- [ ] Composition-only service assemblies emit no VContainer code
- [ ] All existing generator tests pass

---

### TV-2.2: Analyzer Tests
**Check**: New analyzer tests pass

**Validation Steps**:
1. Run analyzer test for NHEM_DI_060 emission on mixed style
2. Run analyzer test for no emission on pure explicit style
3. Run analyzer test for no emission on pure legacy style
4. Run analyzer test for no emission on scope-only style
5. Run all existing analyzer tests

**Acceptance Criteria**:
- [ ] NHEM_DI_060 emitted for mixed explicit + legacy exposure style
- [ ] NHEM_DI_060 not emitted for pure explicit style
- [ ] NHEM_DI_060 not emitted for pure legacy style
- [ ] NHEM_DI_060 not emitted when AutoRegisterIn has only scope and lifetime
- [ ] All existing analyzer tests pass

---

## Phase 3: Documentation Validation

### DV-3.1: README.md Updates
**Check**: README.md updated correctly

**Validation Steps**:
1. Review README.md canonical usage section
2. Verify canonical usage shows explicit attributes first
3. Verify migration guide section added
4. Verify rule documentation added
5. Verify mixed style examples removed

**Acceptance Criteria**:
- [ ] Canonical usage shows explicit attributes first
- [ ] Migration guide section exists
- [ ] Rule documentation: AutoRegisterIn decides where and how long
- [ ] Rule documentation: As / AsSelf decide as what
- [ ] Rule documentation: EntryPoint / RegisterComponentInHierarchy decide registration kind
- [ ] Mixed style examples removed

---

### DV-3.2: AutoRegisterInAttribute XML Docs
**Check**: AutoRegisterInAttribute.cs XML docs updated

**Validation Steps**:
1. Review AutoRegisterInAttribute.cs summary comment
2. Verify canonical usage emphasized
3. Review XML examples
4. Verify canonical-first examples
5. Verify mixed style examples removed
6. Review property documentation
7. Verify backward compatibility notes added

**Acceptance Criteria**:
- [ ] Summary comment mentions canonical usage
- [ ] XML examples show canonical-first
- [ ] Mixed style examples removed
- [ ] Property docs include backward compatibility notes
- [ ] AsImplementedInterfaces docs mention canonical preference
- [ ] AsSelf docs mention canonical preference

---

### DV-3.3: CHANGELOG.md
**Check**: CHANGELOG.md updated

**Validation Steps**:
1. Review CHANGELOG.md for 7.2.1 section
2. Verify new diagnostic NHEM_DI_060 documented
3. Verify generator behavior changes documented
4. Verify backward compatibility guarantees documented

**Acceptance Criteria**:
- [ ] 7.2.1 section exists
- [ ] NHEM_DI_060 documented
- [ ] Generator behavior changes documented
- [ ] Backward compatibility guarantees documented

---

## Phase 4: Integration Validation

### IV-4.1: DLL Payloads
**Check**: DLL payloads rebuilt with correct version

**Validation Steps**:
1. Verify Runtime/NhemDangFugBixs.Attributes.dll exists
2. Verify Runtime/NhemDangFugBixs.Runtime.dll exists
3. Verify Analyzers/NhemDangFugBixs.Generators.dll exists
4. Verify Analyzers/NhemDangFugBixs.Analyzers.dll exists
5. Check file versions are 7.2.1

**Acceptance Criteria**:
- [ ] All DLLs exist
- [ ] All DLLs have version 7.2.1
- [ ] DLLs are rebuilt (not stale)

---

### IV-4.2: Unity Sample (Optional)
**Check**: Unity sample compiles correctly

**Validation Steps**:
1. If NHEM_UNITY_PROJECT_ROOT is set, run dotnet build
2. If UNITY_EXE is set, run Unity compile
3. Verify sample uses canonical style
4. Verify sample compiles without errors

**Acceptance Criteria**:
- [ ] Unity sample dotnet build passes (if available)
- [ ] Unity sample compile passes (if available)
- [ ] Sample uses canonical explicit style
- [ ] No Unity errors

---

## Phase 5: Release Gate Validation

### RV-5.1: Release Gate Execution
**Check**: Release gate passes

**Validation Steps**:
1. Run release-gate.ps1
2. Verify generator tests pass
3. Verify analyzer tests pass
4. Verify version drift check passes
5. Verify docs check passes
6. Verify new 7.2.1 checks pass
7. Verify Unity sample build passes (if available)
8. Verify Unity sample compile passes (if available)

**Acceptance Criteria**:
- [ ] Generator tests pass
- [ ] Analyzer tests pass
- [ ] Version drift check passes
- [ ] Docs check passes
- [ ] New generator tests pass
- [ ] New analyzer tests pass
- [ ] Documentation validation passes
- [ ] Version validation passes
- [ ] DLL payload validation passes
- [ ] Unity sample build passes (if available)
- [ ] Unity sample compile passes (if available)

---

## Validation Metrics

### Code Coverage
- Analyzer implementation: 100%
- Generator implementation: 100%
- New tests: 100%
- Existing tests: 100%

### Test Pass Rate
- Generator tests: 100%
- Analyzer tests: 100%
- Integration tests: 100%

### Documentation Completeness
- README.md: 100%
- AutoRegisterInAttribute.cs: 100%
- CHANGELOG.md: 100%

### Version Consistency
- package.json: 7.2.1
- All project files: 7.2.1
- All DLLs: 7.2.1

## Validation Tools

### Automated Validation
- Release gate script (release-gate.ps1)
- Unit tests (dotnet test)
- Version check script

### Manual Validation
- Code review
- Documentation review
- Sample review

### Validation Commands
```powershell
# Run release gate
.\scripts\release-gate.ps1

# Run generator tests
dotnet test Source~/DangFugBixs.Generators~/DangFugBixs.Tests/DangFugBixs.Tests.csproj

# Run analyzer tests
dotnet test Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/DangFugBixs.Analyzers.Tests.csproj

# Check versions
Get-Content package.json | Select-String "version"
```

## Validation Schedule

### Pre-Release Validation
- 1 week before release: Complete code validation
- 1 week before release: Complete test validation
- 3 days before release: Complete documentation validation
- 2 days before release: Complete integration validation
- 1 day before release: Complete release gate validation

### Validation Owners
- Code validation: Technical Lead
- Test validation: Quality Lead
- Documentation validation: Documentation Lead
- Integration validation: Technical Lead
- Release gate validation: Release Manager

## Validation Reporting

### Validation Report Template
```markdown
# Validation Report for 7.2.1

## Phase 1: Code Validation
- CV-1.1: PASS/FAIL
- CV-1.2: PASS/FAIL
- CV-1.3: PASS/FAIL

## Phase 2: Test Validation
- TV-2.1: PASS/FAIL
- TV-2.2: PASS/FAIL

## Phase 3: Documentation Validation
- DV-3.1: PASS/FAIL
- DV-3.2: PASS/FAIL
- DV-3.3: PASS/FAIL

## Phase 4: Integration Validation
- IV-4.1: PASS/FAIL
- IV-4.2: PASS/FAIL

## Phase 5: Release Gate Validation
- RV-5.1: PASS/FAIL

## Overall Status: PASS/FAIL

## Issues
- List any issues found

## Remediation
- List any remediation steps
```

## Success Criteria

Validation is considered successful when:
- All validation phases pass
- All acceptance criteria met
- Release gate passes
- No blocking issues identified
- All documentation complete
- All tests pass
- Version numbers consistent
- DLL payloads rebuilt

## Failure Handling

### If Validation Fails
1. Identify failing phase
2. Identify specific failure
3. Determine root cause
4. Implement fix
5. Re-run validation
6. Document failure and fix

### Escalation
- If validation fails and cannot be fixed quickly: Escalate to Technical Lead
- If release gate fails: Block release until fixed
- If critical issue found: Consider delaying release

## Validation Artifacts

### Artifacts to Retain
- Validation report
- Test results
- Release gate output
- Version check output
- DLL version check output

### Artifact Location
- Artifacts/7.2.1/validation-report.md
- Artifacts/7.2.1/test-results.xml
- Artifacts/7.2.1/release-gate-output.txt

## Conclusion

This validation plan ensures comprehensive validation of the registration exposure API cleanup in version 7.2.1. By following this plan, we can ensure the implementation is correct, tests pass, documentation is complete, and the release is ready for distribution.
