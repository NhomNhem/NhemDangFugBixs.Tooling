# Tasks: Analyzer Maturity (7.3.0)

## Implementation Tasks

- [ ] Add NHEM_DI_061 diagnostic descriptor to DiagnosticDescriptors.cs
- [ ] Implement CheckDuplicateContractExposure logic in Diagnostics.cs
- [ ] Add NHEM_DI_066 diagnostic descriptor to DiagnosticDescriptors.cs
- [ ] Implement CheckRegisterComponentInHierarchyUsage logic in Diagnostics.cs
- [ ] Add NHEM_DI_067 diagnostic descriptor to DiagnosticDescriptors.cs
- [ ] Implement CheckEntryPointLifecycleInterface logic in Diagnostics.cs
- [ ] Integrate new diagnostics into existing diagnostic analysis flow
- [ ] Add analyzer test for NHEM_DI_061 duplicate [As] same contract
- [ ] Add analyzer test for NHEM_DI_061 not emitted for different contracts
- [ ] Add analyzer test for NHEM_DI_066 on non-MonoBehaviour
- [ ] Add analyzer test for NHEM_DI_066 not emitted for MonoBehaviour subclass
- [ ] Add analyzer test for NHEM_DI_067 without lifecycle interface
- [ ] Add analyzer test for NHEM_DI_067 not emitted for IStartable
- [ ] Add analyzer test for NHEM_DI_067 not emitted for ITickable
- [ ] Add analyzer test for NHEM_DI_067 not emitted for IDisposable
- [ ] Verify existing analyzer tests still pass
- [ ] Add diagnostics documentation for NHEM_DI_061, NHEM_DI_066, NHEM_DI_067
- [ ] Bump package.json to 7.3.0
- [ ] Bump DangFugBixs.Generators.csproj to 7.3.0
- [ ] Update CHANGELOG.md for 7.3.0
- [ ] Run release gate
- [ ] Deploy/tag v7.3.0 if gate passes

## Documentation Tasks

- [ ] Document NHEM_DI_061 with bad/good examples
- [ ] Document NHEM_DI_066 with bad/good examples
- [ ] Document NHEM_DI_067 with bad/good examples
- [ ] Add note about project-level validations deferred to 7.4.0 di-smoke
