# Tasks: Composition-Only Generation Mode

## 1. Generator mode split

- [x] 1.1 Detect whether the current compilation contains any `[LifetimeScopeFor]` declarations.
- [x] 1.2 Skip emission of `RegisterGeneratedFor<TScope>()` and VContainer installers when no local composition target exists.
- [x] 1.3 Preserve local attribute parsing and diagnostics even when emission is skipped.
- [x] 1.4 Add tests proving service-only compilations emit no VContainer code.

## 2. Registration metadata discovery

- [ ] 2.1 Split the internal model between discovered registration metadata and local emission targets.
- [x] 2.2 Collect eligible registrations from the current compilation when it is a composition target.
- [x] 2.3 Collect eligible registrations from directly referenced assemblies for MVP discovery.
- [x] 2.4 Ensure discovery never scans all loaded Unity assemblies.
- [x] 2.5 Filter discovered registrations by scope marker and registration kind.
- [x] 2.6 Deduplicate discovered registrations deterministically.
- [x] 2.7 Add tests proving referenced-service discovery works and unreferenced assemblies stay invisible.

## 3. Composition-only emission

- [x] 3.1 Emit `NhemGeneratedVContainerExtensions.RegisterGeneratedFor<TScope>()` only into composition target assemblies.
- [x] 3.2 Emit one stateless installer per mapped scope into the composition target assembly.
- [x] 3.3 Use fully qualified type names in generated registrations where ambiguity is possible.
- [x] 3.4 Ensure generated installers do not call `Resolve<T>()` or keep mutable static state.
- [x] 3.5 Ensure generated output order is deterministic and sorted.
- [x] 3.6 Add snapshot tests for composition-only generated output.

## 4. Analyzer updates

- [x] 4.1 Keep local correctness diagnostics in service-only assemblies:
  - invalid `As(...)` contract
  - invalid scope marker
  - invalid `EntryPoint` usage
  - invalid `RegisterComponentInHierarchy` usage
- [x] 4.2 Stop reporting missing `LifetimeScopeFor` in service-only assemblies as a local failure.
- [ ] 4.3 Add diagnostics for duplicate local composition targets for the same scope when detectable.
- [ ] 4.4 Add diagnostics for duplicate discovered registrations in one composition target.
- [ ] 4.5 Add diagnostics for composition targets that cannot see required VContainer symbols, if detectable.
- [ ] 4.6 Document which validations are deferred to `di-smoke` because Roslyn per-compilation analysis is insufficient.

## 5. Performance and scalability

- [ ] 5.1 Keep the generator on incremental generator patterns and avoid semantic analysis for non-candidate syntax nodes.
- [x] 5.2 Implement duplicate detection with dictionary or grouping-based logic rather than O(n²) scans.
- [x] 5.3 Add generator performance smoke tests for 10 services.
- [x] 5.4 Add generator performance smoke tests for 100 services.
- [x] 5.5 Add generator performance smoke tests for 500 services.
- [x] 5.6 Verify deterministic output across repeated runs of the same input.

## 6. di-smoke follow-up contract

- [ ] 6.1 Define the first `di-smoke` checks needed for composition-only generation.
- [ ] 6.2 Cover cross-asmdef duplicate composition targets as a deferred validation path.
- [ ] 6.3 Cover project-level discovery drift as a deferred validation path.

## 7. Unity sample and release gate

- [x] 7.1 Restore separate asmdefs in the Unity sample: `Shared`, `Gameplay` or `Locomotion`, and `Composition`.
- [x] 7.2 Ensure service asmdef references package attributes but not VContainer.
- [x] 7.3 Ensure composition asmdef references VContainer and referenced service asmdefs.
- [x] 7.4 Update the release gate so Unity compile validates the separated-asmdef sample when Unity is configured.
- [x] 7.5 Report Unity sample compile duration in the release gate summary when Unity compile runs.
- [x] 7.6 Keep generator tests, analyzer tests, docs build, and version drift checks in the release gate.

## 8. Docs and migration

- [x] 8.1 Update README to explain composition-only generation as the canonical model.
- [x] 8.2 Update docs examples to show service-only asmdefs without VContainer references.
- [ ] 8.3 Add migration guidance from current per-assembly generation to composition-only generation.
- [x] 8.4 Document MVP discovery limits: direct references only, no transitive scan yet.
- [x] 8.5 Document performance boundaries and determinism guarantees.

## 9. Acceptance verification

- [x] 9.1 Service-only assembly with `[AutoRegisterIn]` compiles without VContainer reference.
- [x] 9.2 Service-only assembly emits no VContainer generated code.
- [x] 9.3 Service-only assembly still reports invalid `As(...)` contract.
- [x] 9.4 Composition assembly with `[LifetimeScopeFor]` emits `RegisterGeneratedFor<TScope>()`.
- [x] 9.5 Composition assembly discovers service from referenced assembly.
- [x] 9.6 Composition assembly registers both local and referenced services.
- [x] 9.7 Composition assembly does not discover unreferenced service assemblies.
- [x] 9.8 Generated installers use valid VContainer API.
- [x] 9.9 No generated `Resolve<T>()` appears.
- [x] 9.10 Generated output is deterministic and sorted.
- [x] 9.11 Performance smoke tests pass for 10, 100, and 500 services.
- [ ] 9.12 Unity sample compile gate passes with separated asmdefs and reports compile duration.
