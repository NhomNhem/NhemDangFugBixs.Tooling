## Context

`NhemDangFugBixs.Tooling` already ships useful VContainer-focused generation and analyzer features, but the current capability set is fragmented across package code and archived changes. The new package direction needs a cohesive architecture that supports Unity asmdef layering, keeps application code independent from composition assemblies, and produces outputs that humans, CI, and AI coding agents can all trust.

The main constraint is that the package must strengthen VContainer workflows without forcing one project architecture. It also has to stay compatible with Unity package conventions, keep editor code isolated from runtime assemblies, and treat optional integrations such as MessagePipe and reactive guardrails as capability layers rather than hard requirements.

## Goals / Non-Goals

**Goals:**
- Establish scope markers as the canonical abstraction between lower-layer services and concrete VContainer `LifetimeScope` types.
- Define a unified registration model for normal services, entry points, components, installers, and build callbacks.
- Expand analyzer coverage so scope mapping, injection style, lifetime, and resolver mistakes fail earlier and more consistently.
- Make generated reports and CLI/editor tooling reflect the same semantic graph used for registration emission.
- Preserve existing MessagePipe support while adapting it to marker-based registration and richer diagnostics.

**Non-Goals:**
- Replacing VContainer or hiding its concepts behind a custom runtime container.
- Implementing every advanced registration feature from the long-term roadmap in the first implementation pass.
- Forcing fixed scope names such as `Project` or `Gameplay` as hard-coded concepts.
- Requiring editor assemblies from game runtime code or introducing runtime reflection scanning as the primary registration mechanism.

## Decisions

### 1. Make scope markers the package foundation

Use `[LifetimeScopeFor]` mappings from marker types to concrete `LifetimeScope` classes, and allow registration attributes to target markers, aliases, or generic marker syntax. This keeps application and infrastructure asmdefs free from composition references while still letting the generator emit explicit `builder.Register...` calls for real scopes.

Alternative considered: targeting concrete `LifetimeScope` types directly everywhere. Rejected because it recreates the current layering problem and makes cross-asmdef usage brittle.

### 2. Keep generated registration explicit and VContainer-shaped

Generated methods should look like ordinary VContainer code, with both `RegisterGeneratedFor<TScopeMarker>()` and non-generic helper entry points. Entry points, components, installers, and build callbacks should all flow through the same semantic model so reports and diagnostics match emitted behavior.

Alternative considered: a reflection-heavy runtime installer abstraction. Rejected because it weakens compile-time validation and makes generated behavior harder to inspect.

### 3. Split responsibilities across focused capabilities

Treat marker registration, component/entry-point generation, architecture guardrails, CLI validation, and editor diagnostics as separate capabilities connected by shared metadata. This keeps the proposal implementable in phases while still producing one coherent package direction.

Alternative considered: bundling everything under one monolithic `vcontainer-registration` capability. Rejected because it would blur the contract between generation, validation, and tooling.

### 4. Use one semantic graph for emission, reports, CLI, and editor tooling

The generator should produce metadata that captures services, contracts, scopes, entry points, component modes, installers, MessagePipe brokers, and warnings. The CLI and editor window should consume that shared metadata rather than reconstructing registrations from scratch.

Alternative considered: having CLI/editor tooling rescan assemblies independently. Rejected because it increases drift risk and duplicates analyzer logic.

### 5. Treat optional integrations as availability-aware extensions

MessagePipe behavior should remain opt-in and guarded by symbol availability checks. Reactive guardrails such as public `Subject<T>` rules should be configurable so the core package stays reusable across teams with different stacks.

Alternative considered: hard-wiring MessagePipe and reactive conventions into the mandatory core. Rejected because it would over-constrain the package and complicate adoption.

## Risks / Trade-offs

- [Broad MVP surface] → Prioritize the scope-marker architecture, core registration attributes, key analyzers, report output, and basic CLI/editor flows first; defer advanced keyed/prefab/open-generic work.
- [Generator/model growth] → Centralize registration metadata in shared models and keep optional fields nullable so advanced features do not overcomplicate the common path.
- [Spec overlap between analyzers and generation] → Use clear capability boundaries: generation specs describe emitted behavior, analyzer specs describe validation behavior, and reports/tooling specs consume shared metadata.
- [Migration ambiguity for existing users] → Preserve backward-compatible attribute paths where practical, emit explicit mapping diagnostics, and document alias-based presets as optional sugar rather than the foundation.
- [Unity/package maintenance cost] → Mirror Unity package conventions (`Runtime`, `Editor`, `Samples~`, `Documentation~`, `Source~`) so long-term maintenance stays familiar for contributors.

## Migration Plan

1. Add marker-based APIs and new generation paths without removing current registration primitives.
2. Extend analyzer coverage with new diagnostics for mapping and injection mistakes while preserving existing diagnostic IDs where behavior already exists.
3. Update report metadata consumers so CLI and editor tooling can render richer scope-aware outputs.
4. Add samples and docs that show marker-based registration as the preferred path for new projects.
5. In a later release, deprecate purely architecture-coupled patterns only after marker-based alternatives and migration docs are stable.

Rollback strategy: each implementation area should be isolated enough that generator/report/tooling additions can be disabled or reverted without breaking the existing runtime attribute model.

## Open Questions

- Should omitted lifetimes remain legal with warnings, or should explicit lifetime become a requirement for public release defaults?
- Should entry-point lifecycle interface auto-detection stay enabled by default, or should explicit `[EntryPoint]` be required for generation?
- How much of the installer/build-callback surface should be included in the initial implementation versus documented as post-MVP?
- Should reactive guardrails ship in the core analyzer assembly but default to disabled, or move into a separate optional analyzer package later?
