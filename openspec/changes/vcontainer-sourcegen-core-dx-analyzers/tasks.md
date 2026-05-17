## 1. Package Baseline and Public API Surface

- [x] 1.1 Normalize `package.json` metadata, dependency baseline (VContainer), and version source of truth policy.
- [x] 1.2 Finalize public attributes: `AutoRegisterIn<TScope>`, `AutoRegisterIn(Type)`, `As<TContract>`, `AsSelf`, `LifetimeScopeFor<TScope>`, `EntryPoint`, `Keyed`, `RegisterComponentInHierarchy` intent.
- [x] 1.3 Document and enforce user-owned `LifetimeScope` policy (no generated scope MonoBehaviours by default).

## 2. Generator Pipeline and Intermediate Model

- [x] 2.1 Implement/clean collect phase for all supported attributes and registration kinds.
- [x] 2.2 Implement normalized `RegistrationModel` pipeline (implementation type, marker, lifetime, contracts, AsSelf, kind, key).
- [x] 2.3 Implement validate phase that emits diagnostics and blocks broken code emission.
- [x] 2.4 Emit deterministic generated outputs: scope installers, extension dispatch, optional report metadata.
- [x] 2.5 Ensure extension dispatch is DX sugar only and never duplicates registration logic.
- [x] 2.6 Enforce no mutable static runtime state and no service locator behavior in generated code.

## 3. Generated Code Contracts and Naming

- [x] 3.1 Emit `NhemGeneratedVContainerExtensions.RegisterGeneratedFor<TScope>(this IContainerBuilder builder)` with null-check + missing-mapping exception.
- [x] 3.2 Emit per-scope installer partial classes (`NhemGenerated{ScopeName}ScopeInstaller` or chosen final naming policy) with explicit `RegisterServices`, `RegisterEntryPoints`, `RegisterComponents` flows.
- [x] 3.3 Validate keyed emission (`.Keyed(...)`) and component hierarchy emission (`RegisterComponentInHierarchy<T>()`) against target VContainer API.
- [x] 3.4 Guarantee no duplicate `RegisterGeneratedFor<TScope>()` signatures and no orphan wrapper methods.

## 4. Analyzer MVP Guardrails (`NHEM_DI_*`)

- [x] 4.1 Implement `NHEM_DI_001`, `NHEM_DI_002`, `NHEM_DI_003` (contracts + marker validity + missing exposure intent).
- [x] 4.2 Implement `NHEM_DI_010`, `NHEM_DI_011`, `NHEM_DI_012` (mapping and configure-call integrity).
- [ ] 4.3 Implement `NHEM_DI_020`, `NHEM_DI_021`, `NHEM_DI_022` (duplicate registration and duplicate generated invocation).
- [ ] 4.4 Implement `NHEM_DI_030` (root-scope mutable gameplay state warning).
- [ ] 4.5 Implement `NHEM_DI_040`, `NHEM_DI_041` (entrypoint misuse and dual exposure warning).
- [x] 4.6 Implement `NHEM_DI_050` (IObjectResolver service-locator warning).

## 5. Test Matrix Implementation

- [x] 5.1 Add generator tests for marker forms (generic + non-generic), invalid marker, missing mapping, and mapping-call mismatch.
- [x] 5.2 Add contract binding tests (`As<T>`, `AsSelf`, multi-contract, invalid contract, missing exposure intent).
- [ ] 5.3 Add lifetime tests (`Singleton`, `Scoped`, `Transient`, root-scope misuse warning).
- [ ] 5.4 Add entrypoint tests for supported lifecycle interfaces and invalid entrypoint class.
- [x] 5.5 Add component tests for valid MonoBehaviour hierarchy registration and invalid non-component usage.
- [x] 5.6 Add keyed tests (enum/string/int keys, duplicate key behavior, detectable mismatch behavior).
- [x] 5.7 Add duplicate-path tests (same implementation twice, generated installer invoked twice, manual + generated conflict).
- [x] 5.8 Add compile/snapshot tests for generated extension + installers and no missing-API references.

## 6. Unity Package Integrity and Release Gates

- [x] 6.1 Add package validation checks: metadata completeness, asmdef reference validity, dependency correctness.
- [x] 6.2 Add version drift checks across `package.json`, generated banners, and docs.
- [x] 6.3 Add README/docs compile smoke checks for all published API examples.
- [ ] 6.4 Add Unity sample compile validation in CI.
- [ ] 6.5 Add release-gate script that blocks publish unless all required checks pass.

## 7. Migration, Docs, and Adoption Safety

- [ ] 7.1 Write migration guide: manual lifetime scopes -> marker introduction -> low-risk annotations -> generated call -> strict analyzers.
- [x] 7.2 Document explicit non-goals (no default generated `LifetimeScope` MonoBehaviours, no forced architecture, no runtime resolver caches).
- [ ] 7.3 Document manual installer escape-hatch policy and future explicit hook approach (`ManualInstallerFor<TScope>` if introduced later).

## 8. Acceptance Criteria and Final Signoff

- [ ] 8.1 All spec requirements in this change have passing automated tests mapped to scenarios.
- [ ] 8.2 `dotnet test`, generator snapshots, analyzer suites, and Unity sample compile all pass.
- [x] 8.3 README/API docs match shipped APIs and compile in smoke checks.
- [ ] 8.4 Release gate checklist passes with zero blocking violations.

## 9. Test Plan (Execution Order)

- [x] 9.1 Run fast generator/analyzer unit tests locally for red-green feedback.
- [x] 9.2 Run integration tests for generated output compilation and duplicate-detection paths.
- [x] 9.3 Run package metadata/version sync validator.
- [ ] 9.4 Run Unity sample compile and docs sample compile checks.
- [ ] 9.5 Run full CI parity suite before release candidate tag.

## 10. Migration Notes (Operational Rollout)

- [ ] 10.1 Roll out analyzers in warning mode first for existing projects.
- [ ] 10.2 Fix surfaced diagnostics and switch to strict profile in staged milestones.
- [ ] 10.3 Keep manual installers for exceptions during migration; avoid mixed duplicate invocation patterns.
- [ ] 10.4 Record known suppressions/allow-list decisions with owner and expiry date.
