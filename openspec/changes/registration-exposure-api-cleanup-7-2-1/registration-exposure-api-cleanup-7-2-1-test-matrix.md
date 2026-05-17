# Test Matrix: Registration Exposure API Cleanup 7.2.1

## Overview

This test matrix defines all test cases required to validate the registration exposure API cleanup changes in version 7.2.1.

## Test Categories

1. Generator Tests - Validate code generation behavior
2. Analyzer Tests - Validate diagnostic behavior
3. Integration Tests - Validate end-to-end scenarios
4. Regression Tests - Ensure existing functionality remains intact

## Generator Tests

### File: `Source~/DangFugBixs.Generators~/DangFugBixs.Tests/BindingGenerationTests.cs`

#### New Tests

| Test ID | Test Name | Input | Expected Output | Status |
|---------|-----------|-------|-----------------|--------|
| G-001 | ExplicitAs_GeneratesSingleAsContract | `[AutoRegisterIn<Scope>] [As<IService>]` | `.As<IService>()` only, no `.AsImplementedInterfaces()` | Pending |
| G-002 | ExplicitAs_WithLegacyAsImplementedInterfacesTrue_NoDuplicate | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)] [As<IService>]` | `.As<IService>()` only, no `.AsImplementedInterfaces()` | Pending |
| G-003 | ExplicitAsSelf_WithLegacyAsSelfTrue_NoDuplicate | `[AutoRegisterIn<Scope>(AsSelf = true)] [AsSelf]` | One `.AsSelf()` only, not two | Pending |
| G-004 | LegacyFlagOnly_UnchangedBehavior | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true, AsSelf = true)]` | `.AsImplementedInterfaces().AsSelf()` | Pending |
| G-005 | MultipleExplicitAs_SortedDeterministically | `[AutoRegisterIn<Scope>] [As<IZ>] [As<IA>]` | `.As<IA>().As<IZ>()` (sorted) | Pending |
| G-006 | CompositionOnlyServiceAssembly_NoVContainerCode | Service asmdef without VContainer ref + explicit attributes | No VContainer types in generated code | Pending |
| G-007 | ComponentInHierarchy_ExplicitAs_CorrectSuffix | `[RegisterComponentInHierarchy] [AutoRegisterIn<Scope>] [As<IView>]` | `RegisterComponentInHierarchy().As<IView>()` | Pending |
| G-008 | EntryPoint_ExplicitAs_CorrectSuffix | `[AutoRegisterIn<Scope>] [EntryPoint] [As<ILoop>]` | `RegisterEntryPoint().As<ILoop>()` | Pending |
| G-009 | ExplicitAs_WithAsSelf_BothGenerated | `[AutoRegisterIn<Scope>] [As<IService>] [AsSelf]` | `.As<IService>().AsSelf()` | Pending |
| G-010 | ExplicitAs_WithoutAsSelf_OnlyContract | `[AutoRegisterIn<Scope>] [As<IService>]` | `.As<IService>()` only | Pending |

#### Existing Tests to Verify

| Test ID | Test Name | Verification | Status |
|---------|-----------|--------------|--------|
| G-EX-001 | AutoRegisterIn_AsSelfFalse_DoesNotEmitAsSelfBinding | Still passes with new logic | Pending |
| G-EX-002 | AutoRegisterIn_WithExplicitAsAttribute_EmitsTypedAsWithoutImplicitSelf | Still passes with new logic | Pending |
| G-EX-003 | All existing binding generation tests | No regressions | Pending |

## Analyzer Tests

### File: `Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/OpenSpecAnalyzerMvpTests.cs`

#### New Tests

| Test ID | Test Name | Input | Expected Diagnostic | Status |
|---------|-----------|-------|---------------------|--------|
| A-001 | MixedStyle_ExplicitAsWithAsImplementedInterfacesTrue_EmitsNhemDi060 | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)] [As<IService>]` | NHEM_DI_060 emitted | Pending |
| A-002 | MixedStyle_ExplicitAsSelfWithAsSelfTrue_EmitsNhemDi060 | `[AutoRegisterIn<Scope>(AsSelf = true)] [AsSelf]` | NHEM_DI_060 emitted | Pending |
| A-003 | MixedStyle_ExplicitAsWithAsSelfTrue_EmitsNhemDi060 | `[AutoRegisterIn<Scope>(AsSelf = true)] [As<IService>]` | NHEM_DI_060 emitted | Pending |
| A-004 | PureExplicitStyle_NoNhemDi060 | `[AutoRegisterIn<Scope>] [As<IService>]` | No NHEM_DI_060 | Pending |
| A-005 | PureLegacyStyle_NoNhemDi060 | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)]` | No NHEM_DI_060 | Pending |
| A-006 | ScopeOnlyStyle_NoNhemDi060 | `[AutoRegisterIn<Scope>]` | No NHEM_DI_060 (but NHEM_DI_003) | Pending |
| A-007 | ExplicitAsWithAsImplementedInterfacesFalse_NoNhemDi060 | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = false)] [As<IService>]` | No NHEM_DI_060 (intentional) | Pending |
| A-008 | ExplicitAsSelfWithAsSelfFalse_NoNhemDi060 | `[AutoRegisterIn<Scope>(AsSelf = false)] [AsSelf]` | No NHEM_DI_060 (intentional) | Pending |
| A-009 | NoAutoRegisterIn_NoNhemDi060 | `[As<IService>]` (no AutoRegisterIn) | No NHEM_DI_060 | Pending |
| A-010 | GenericScopeMixedStyle_EmitsNhemDi060 | `[AutoRegisterIn<IScope>(AsImplementedInterfaces = true)] [As<IService>]` | NHEM_DI_060 emitted | Pending |
| A-011 | TypeOfScopeMixedStyle_EmitsNhemDi060 | `[AutoRegisterIn(typeof(Scope), AsImplementedInterfaces = true)] [As<IService>]` | NHEM_DI_060 emitted | Pending |

#### Existing Tests to Verify

| Test ID | Test Name | Verification | Status |
|---------|-----------|--------------|--------|
| A-EX-001 | All existing analyzer tests | No regressions | Pending |
| A-EX-002 | NHEM_DI_001 InvalidContract | Still works correctly | Pending |
| A-EX-003 | NHEM_DI_002 InvalidScopeMarker | Still works correctly | Pending |
| A-EX-004 | NHEM_DI_003 MissingExposureIntent | Still works correctly | Pending |
| A-EX-005 | NHEM_DI_040 InvalidEntryPoint | Still works correctly | Pending |

## Integration Tests

### File: `Source~/DangFugBixs.Generators~/DangFugBixs.Tests/IntegrationTests.cs` (if exists)

| Test ID | Test Name | Scenario | Expected Outcome | Status |
|---------|-----------|----------|------------------|--------|
| I-001 | FullRegistrationFlow_ExplicitAs | Complete registration with explicit attributes | Correct generated code | Pending |
| I-002 | CrossAssemblyDiscovery_ExplicitAs | Explicit attributes across asmdef boundaries | Correct scope mapping | Pending |
| I-003 | TypeSafeScope_ExplicitAs | Type-safe generic scope with explicit attributes | Correct registration | Pending |
| I-004 | MessagePipeIntegration_ExplicitAs | MessagePipe with explicit attributes | No interference | Pending |
| I-005 | MultipleScopes_ExplicitAs | Same service in multiple scopes with explicit attributes | All registrations correct | Pending |

## Regression Tests

### Generator Regression

| Test ID | Test Name | Scenario | Expected Outcome | Status |
|---------|-----------|----------|------------------|--------|
| R-G-001 | LegacyFlagOnly_SingleInterface | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)]` | Same output as before | Pending |
| R-G-002 | LegacyFlagOnly_MultipleInterfaces | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)]` with multiple interfaces | Same output as before | Pending |
| R-G-003 | LegacyFlagOnly_SelfOnly | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = false, AsSelf = true)]` | Same output as before | Pending |
| R-G-004 | LegacyFlagOnly_NoExposure | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = false, AsSelf = false)]` | Same output as before | Pending |
| R-G-005 | EntryPoint_LegacyStyle | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)] [EntryPoint]` | Same output as before | Pending |
| R-G-006 | Component_LegacyStyle | `[RegisterComponentInHierarchy] [AutoRegisterIn<Scope>(AsImplementedInterfaces = true)]` | Same output as before | Pending |

### Analyzer Regression

| Test ID | Test Name | Scenario | Expected Outcome | Status |
|---------|-----------|----------|------------------|--------|
| R-A-001 | LegacyFlagOnly_NoNewDiagnostic | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = true)]` | No NHEM_DI_060 | Pending |
| R-A-002 | NoAutoRegisterIn_NoNewDiagnostic | Type without AutoRegisterIn | No NHEM_DI_060 | Pending |
| R-A-003 | InvalidContract_StillReports | `[AutoRegisterIn<Scope>] [As<INotImplemented>]` | NHEM_DI_001 still emitted | Pending |
| R-A-004 | MissingExposure_StillReports | `[AutoRegisterIn<Scope>(AsImplementedInterfaces = false, AsSelf = false)]` | NHEM_DI_003 still emitted | Pending |

## Edge Case Tests

| Test ID | Test Name | Scenario | Expected Outcome | Status |
|---------|-----------|----------|------------------|--------|
| E-001 | EmptyAsTypesArray | `[AutoRegisterIn<Scope>(AsTypes = [])]` | Handles gracefully | Pending |
| E-002 | DuplicateContractTypes | `[AutoRegisterIn<Scope>] [As<IService>] [As<IService>]` | Deduplicated in output | Pending |
| E-003 | CircularReferences | Types with circular dependencies | No change to existing handling | Pending |
| E-004 | GenericContracts | `[AutoRegisterIn<Scope>] [As<IService<T>>]` | Correct full type name | Pending |
| E-005 | NestedGenerics | `[AutoRegisterIn<Scope>] [As<IDictionary<string, IService>>]` | Correct full type name | Pending |
| E-006 | MultipleAutoRegisterIn | Two `[AutoRegisterIn]` on same type | Both analyzed | Pending |
| E-007 | InheritedAttributes | Base has AutoRegisterIn, derived has As | No NHEM_DI_060 (per-type) | Pending |
| E-008 | ExplicitAsWithLifetime | `[AutoRegisterIn<Scope>(Lifetime = NhemLifetime.Scoped)] [As<IService>]` | Lifetime preserved | Pending |

## Performance Tests

| Test ID | Test Name | Scenario | Expected Outcome | Status |
|---------|-----------|----------|------------------|--------|
| P-001 | LargeProject_ExplicitAs | 1000 services with explicit attributes | No significant slowdown | Pending |
| P-002 | LargeProject_LegacyStyle | 1000 services with legacy flags | No significant slowdown | Pending |
| P-003 | AnalyzerPerformance_MixedStyle | 1000 types with mixed style | No significant slowdown | Pending |

## Test Execution Order

### Phase 1: Unit Tests (Fast)
1. Generator Tests (G-001 to G-010)
2. Analyzer Tests (A-001 to A-011)
3. Verify Existing Tests (G-EX-001 to G-EX-003, A-EX-001 to A-EX-005)

### Phase 2: Regression Tests (Medium)
1. Generator Regression (R-G-001 to R-G-006)
2. Analyzer Regression (R-A-001 to R-A-004)

### Phase 3: Edge Cases (Medium)
1. Edge Case Tests (E-001 to E-008)

### Phase 4: Integration Tests (Slow)
1. Integration Tests (I-001 to I-005)

### Phase 5: Performance Tests (Slow)
1. Performance Tests (P-001 to P-003)

## Test Success Criteria

### Generator Tests
- All new generator tests pass
- All existing generator tests pass without modification
- Generated output matches expected output exactly
- No duplicate `.As<T>()` calls in any scenario
- Contract ordering is deterministic and sorted

### Analyzer Tests
- All new analyzer tests pass
- All existing analyzer tests pass without modification
- NHEM_DI_060 is emitted only for mixed exposure style
- NHEM_DI_060 is not emitted for canonical or legacy styles
- Other diagnostics are not affected

### Integration Tests
- All integration tests pass
- Cross-assembly discovery works correctly
- Type-safe scope mapping works correctly
- MessagePipe integration is not affected

### Regression Tests
- All legacy flag-only scenarios produce identical output
- No changes to generated output for existing code
- No new diagnostics for existing code patterns
- Backward compatibility is maintained

### Performance Tests
- No significant performance degradation
- Analyzer overhead remains minimal
- Generator compilation time remains acceptable

## Test Data Requirements

### Test Fixtures

Create test fixtures for:
- Scope markers (IScope, IGameplayScope, etc.)
- Service interfaces (IService, ICombatService, etc.)
- Concrete services implementing various patterns
- Entry points with VContainer lifecycle interfaces
- Unity components with MonoBehaviour

### Mock Assemblies

Create mock asmdef configurations for:
- Service-only assemblies (no VContainer reference)
- Composition assemblies (with VContainer reference)
- Shared assemblies (scope markers only)

## Test Automation

### CI/CD Integration

Add to CI pipeline:
1. Run all generator tests on every commit
2. Run all analyzer tests on every commit
3. Run integration tests on every PR
4. Run performance tests on release branch

### Test Reporting

Generate test reports showing:
- Pass/fail status for each test
- Execution time for each test
- Comparison with baseline (for performance tests)
- Coverage metrics

## Test Maintenance

### Test Updates for Future Versions

When adding new features:
1. Add new tests to appropriate category
2. Update existing tests if behavior changes
3. Document any test deprecations
4. Update this test matrix

### Test Review Schedule

Review test matrix:
- Before each release
- After major feature additions
- When adding new diagnostics
- When changing generator behavior
