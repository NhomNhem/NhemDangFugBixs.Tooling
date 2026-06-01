# Release Checklist: Analyzer Maturity (7.3.0)

## Pre-Release

- [ ] OpenSpec proposal reviewed and approved
- [ ] OpenSpec design reviewed and approved
- [ ] All implementation tasks completed
- [ ] All analyzer tests passing
- [ ] All generator tests passing
- [ ] DiSmokeValidation tests passing
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Version bumped to 7.3.0

## Release Gate

- [ ] Generator tests PASS
- [ ] Analyzer tests PASS
- [ ] DiSmokeValidation tests PASS
- [ ] Version drift PASS
- [ ] Docs check PASS
- [ ] Unity sample dotnet build PASS
- [ ] Unity sample compile (attempted, may be SKIPPED/INCONCLUSIVE if Unity hangs)

## Post-Release

- [ ] Master pushed to origin
- [ ] Deploy branch updated
- [ ] Tag v7.3.0 created
- [ ] Tag v7.3.0 pushed
- [ ] Release notes published
- [ ] GitHub release created (if applicable)

## Validation

- [ ] NHEM_DI_061 emitted for duplicate [As] same contract
- [ ] NHEM_DI_061 not emitted for different contracts
- [ ] NHEM_DI_066 emitted for [RegisterComponentInHierarchy] on non-MonoBehaviour
- [ ] NHEM_DI_066 not emitted for MonoBehaviour subclass
- [ ] NHEM_DI_067 emitted for [EntryPoint] without lifecycle interface
- [ ] NHEM_DI_067 not emitted for IStartable
- [ ] NHEM_DI_067 not emitted for ITickable
- [ ] NHEM_DI_067 not emitted for IDisposable
- [ ] Existing analyzer tests still pass
- [ ] No breaking changes to 7.2.x compatibility
