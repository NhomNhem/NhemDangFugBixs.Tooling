# Design: Composition-Only Generation Mode

## Context

`NhemDangFugBixs.Tooling` should let teams express DI intent in service assemblies without forcing those assemblies to become VContainer-aware.

The current per-compilation generation model breaks that promise because any compilation that contains `[AutoRegisterIn]` also receives generated VContainer registration code. In real Unity asmdef layouts, that causes low-level gameplay or application assemblies to fail unless they reference `VContainer`.

This design shifts the canonical generation boundary from "where the service type is declared" to "where composition is declared".

## Design Goals

- Keep service assemblies VContainer-agnostic.
- Keep composition assemblies responsible for VContainer wiring.
- Preserve local attribute diagnostics in service assemblies.
- Keep generated output stateless and readable.
- Minimize behavioral ambiguity by using explicit assembly references as the discovery boundary.
- Keep discovery and duplicate handling bounded and deterministic.
- Preserve incremental generator performance characteristics as the feature grows.

## Canonical Model

```text
Service assembly
  - declares [AutoRegisterIn], [As], [EntryPoint], [RegisterComponentInHierarchy]
  - no VContainer-generated output
  - no VContainer reference required

Composition assembly
  - declares [LifetimeScopeFor]
  - references VContainer
  - references the service assemblies it wants to compose
  - receives RegisterGeneratedFor<TScope>() and generated installers
```

## Architectural Rules

1. Assemblies with no `[LifetimeScopeFor]` are service-only compilations for generation purposes.
2. Service-only compilations never emit VContainer installer code.
3. Assemblies with one or more `[LifetimeScopeFor]` are composition targets.
4. Composition targets discover registration metadata from:
   - the current compilation
   - directly referenced assemblies only, for MVP
5. Discovery is opt-in through normal assembly references. If composition does not reference a service assembly, that assembly is intentionally invisible.
6. Generated installers remain stateless and must not call `Resolve<T>()`.
7. User-owned `LifetimeScope` classes remain the integration point. No generated `LifetimeScope` MonoBehaviours and no partial injection into user types.
8. Discovery must not scan all loaded Unity assemblies.
9. Generated output must be deterministic, sorted, and free of hidden runtime reflection behavior.

## Performance Requirements

### Runtime output

- Generated installer code must remain reflection-free.
- Generated installer code must remain stateless.
- No generated registration path may call `Resolve<T>()`.

### Discovery bounds

- Composition-only discovery inspects only:
  - current compilation
  - directly referenced assemblies
- MVP explicitly excludes transitive and "all loaded Unity assemblies" discovery.

### Generator architecture

- Use incremental generator candidate filtering so non-candidate syntax nodes avoid semantic analysis.
- Prefer pre-grouped dictionaries/lookups over repeated full-list scans.
- Duplicate detection must be O(n) or O(n log n) through grouping/dictionaries, not O(n²) nested loops.
- Final emission order must be stable and sorted for deterministic output and testability.

### Performance verification

- Add smoke tests for 10, 100, and 500 service inputs.
- Measure generator completion success and deterministic output for those sizes.
- Release gate should report Unity sample compile duration when Unity validation runs.

## Generator Pipeline Changes

### Phase 1: Determine generation mode

For the current compilation:

- collect `LifetimeScopeFor` declarations
- if none exist:
  - do not emit `NhemGeneratedVContainerExtensions`
  - do not emit installers
  - still allow metadata/diagnostic analysis
- if one or more exist:
  - treat compilation as a composition target
  - continue discovery and emission

This phase should be driven from candidate syntax filtering so non-candidate nodes do not incur semantic analysis.

### Phase 2: Collect registration metadata

Collection is split into two buckets.

#### Local metadata

Read all supported attributes from the current compilation:

- `AutoRegisterIn`
- `As`
- `AsSelf`
- `EntryPoint`
- `RegisterComponentInHierarchy`

If the current compilation is a composition target, local services are eligible for generation.
If it is service-only, local services contribute diagnostics only.

#### Referenced metadata

For composition targets, inspect directly referenced assemblies and discover eligible registration metadata from Roslyn symbols/metadata.

MVP constraints:

- direct references only
- no recursive graph walk
- deterministic iteration order
- best-effort filtering to types annotated with supported attributes
- no scan across all currently loaded Unity editor assemblies

### Phase 3: Validate

Validation is divided by scope.

#### Local validation in every compilation

Still report:

- invalid `As(...)` contract
- invalid scope marker
- invalid `EntryPoint` usage
- invalid `RegisterComponentInHierarchy` usage

#### Composition-target validation

Only when `[LifetimeScopeFor]` exists locally:

- validate duplicate local composition targets for the same scope where detectable
- validate duplicate discovered registrations in the same target
- validate whether VContainer types are visible where generation requires them

Cross-project or cross-asmdef situations that Roslyn cannot prove reliably stay out of strict compile-time enforcement and are pushed toward `di-smoke`.

### Phase 4: Deduplicate and sort

Before emission:

- group registrations by scope marker
- within each scope, deduplicate by implementation identity and registration kind
- sort output deterministically by scope marker, then implementation full name, then registration kind

This phase should use dictionaries or grouped lookups rather than repeated full-list comparisons.

### Phase 5: Emit

For each scope marker mapped by local `LifetimeScopeFor` declarations:

- gather all discovered registrations for that scope
- consume the deduplicated, sorted registration list
- emit one installer per scope marker
- emit one `RegisterGeneratedFor<TScope>()` dispatcher into the composition assembly only

Generated code should prefer fully qualified names for service and contract types to avoid namespace ambiguity.

## Data Model Changes

Introduce an internal distinction between metadata source and emission target.

### RegistrationMetadata

Represents DI intent discovered from any assembly.

Fields:

- `ImplementationFullName`
- `ImplementationDisplayName`
- `ScopeMarkerFullName`
- `ScopeMarkerDisplayName`
- `Lifetime`
- `Contracts`
- `AsSelf`
- `RegistrationKind`
- `SourceAssemblyName`
- `IsFromCurrentCompilation`

### CompositionTargetModel

Represents a local compilation that owns installer emission.

Fields:

- `AssemblyName`
- `MappedScopes`
- `VisibleVContainer`
- `VisibleReferencedAssemblies`
- `DiscoveredRegistrations`

This split lets the system analyze broadly but emit narrowly.

## Legacy Behavior Strategy

Per-assembly generation is considered legacy or prototype behavior.

Preferred rollout:

- composition-only generation becomes canonical
- per-assembly emission is disabled by default
- a temporary compatibility option may exist only if migration cost proves too high

The change proposal intentionally does not commit to the compatibility switch shape yet. That should be decided during implementation based on migration pressure.

## Analyzer Strategy

### Keep in Roslyn

- invalid local attribute usage
- invalid local marker usage
- invalid local entry point/component usage
- duplicate local composition targets in one compilation, if detectable
- duplicate discovered registrations within one composition compilation, if detectable
- missing VContainer visibility in composition targets, if detectable

### Move toward di-smoke

- duplicate composition targets across separate Unity asmdefs
- missing intended composition roots across a whole Unity project
- drift between composition targets and referenced service assemblies beyond one compilation graph

This avoids over-promising what per-compilation Roslyn analysis can prove.

## Unity Sample and Release Gate

The sample must return to a realistic separated-asmdef layout:

- `Shared`
- `Gameplay` or `Locomotion`
- `Composition`

Validation target:

- gameplay/service asmdef has package attributes but no VContainer reference
- composition asmdef references VContainer and service asmdef
- Unity compile passes
- generated code appears only in composition assembly
- release gate reports Unity compile duration when Unity execution is enabled

`release-gate.ps1` should keep Unity optional, but when Unity is configured it must validate this separated boundary.

## Migration Guide

### From current single-assembly or per-assembly generation

1. Keep existing service attributes.
2. Remove unnecessary `VContainer` references from service-only asmdefs.
3. Ensure composition asmdef references every service asmdef it wants to compose.
4. Add or keep `[LifetimeScopeFor]` in composition.
5. Call `builder.RegisterGeneratedFor<TScope>()` in user-owned `LifetimeScope`.
6. Re-run compile and release gate.

### Expected behavioral changes

- service-only assemblies stop receiving VContainer `.g.cs`
- composition assemblies become the only source of generated installers
- unreferenced service assemblies stop being auto-discovered by accident

## Risks

- Metadata discovery from referenced assemblies may expose symbol edge cases across Unity asmdef compilation boundaries.
- Users with broad prototype assemblies may not notice a change, while highly separated projects will notice it immediately.
- Compatibility mode, if retained, increases maintenance cost and testing surface.
- Duplicate registration deduplication rules must be deterministic and clearly documented.
- Performance regressions may appear if referenced-assembly scanning loses candidate filtering discipline.

## Open Questions

1. Should compatibility mode exist at all, or should the package make a clean canonical break?
2. If compatibility mode exists, should it be package-level, asmdef-level, or analyzer-configurable?
3. Should referenced-assembly discovery use custom metadata markers in the future for faster scanning?
4. When transitive discovery is evaluated later, how will ambiguity and performance be controlled?

## Acceptance Design Checklist

- Service-only compilation emits no VContainer `.g.cs`.
- Composition compilation emits installers only for mapped scopes.
- Referenced-assembly discovery works for direct references.
- Duplicate discovered registrations are handled deterministically.
- No generated `Resolve<T>()` appears.
- Generator path remains incremental and bounded to candidate nodes and direct references.
- Performance smoke tests cover 10, 100, and 500 services.
- Unity sample proves separate asmdefs compile with service asmdefs not referencing VContainer and reports compile duration.
