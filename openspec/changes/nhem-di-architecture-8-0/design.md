## Context

NhemDangFugBixs.Tooling has grown through generator, analyzer, smoke validation, and reporting work. Each layer currently extracts or infers DI information in its own way: the generator has `ServiceInfo` and `ScopeMappingInfo`, analyzers have rule-local symbol scans, and smoke/report tools reconstruct registrations again. That makes real Unity asmdef projects fragile when marker interfaces live in shared assemblies, services live in feature assemblies, and composition roots live in bootstrap assemblies.

The current `afterimage` integration exposed the failure mode: `[LifetimeScopeFor<TScope>]` can make analyzers require `builder.RegisterGeneratedFor<TScope>()`, while generated code may not provide a usable entry point for the composition assembly. Version 8.0 will replace this scattered interpretation with a shared DI contract graph.

The package must remain reusable outside a single game project. It must not force a project architecture; it must make the user's chosen architecture observable and checkable.

## Goals / Non-Goals

**Goals:**
- Define one shared DI contract graph for scopes, services, composition roots, manual registrations, generated installers, assembly provenance, and diagnostic evidence.
- Use the graph as the source of truth for generator emission, analyzer diagnostics, smoke validation, and reports.
- Make cross-assembly Unity asmdef composition deterministic.
- Preserve source compatibility for public attributes and generated registration entry points unless a task explicitly documents a migration.
- Move project-wide checks into smoke validation when Roslyn per-compilation analysis cannot prove them safely.

**Non-Goals:**
- Do not require a specific layer naming scheme such as Project/Gameplay/MainMenu.
- Do not force all services into the composition assembly.
- Do not replace VContainer or change its registration semantics.
- Do not remove existing generated API names in the first 8.0 implementation pass.

## Decisions

### Decision: Introduce a shared DI contract graph

Create a common model under `Source~/DangFugBixs.Common~` that represents:
- `DiScopeIdentity`: marker type, alias, composition root type, source assembly.
- `DiServiceRegistration`: implementation, contracts, lifetime, component mode, entry point/factory/message-pipe flags, source assembly.
- `DiCompositionRoot`: `LifetimeScopeFor` target, configure calls, generated call evidence.
- `DiManualRegistration`: manual VContainer call target and location.
- `DiDiagnosticEvidence`: facts required to explain analyzer and smoke diagnostics.

Alternative considered: patch `ServiceInfo` and `ScopeMappingInfo` in place. That is faster, but it keeps generator-only assumptions in the center of the design and leaves analyzers/reporting to drift again.

### Decision: Keep Roslyn extraction thin and deterministic

Generator and analyzer symbol visitors should extract raw facts into the contract graph. Rule logic should consume graph queries such as "services for scope", "composition root for marker", and "manual registrations for implementation".

Alternative considered: keep each analyzer rule independent. That keeps files smaller short-term, but it repeats attribute parsing and keeps false-positive behavior inconsistent.

### Decision: Emit registration entry points for every composition root

Every `[LifetimeScopeFor<TScope>]` creates a generated `RegisterGeneratedFor<TScope>()` route. If the graph has no services for the scope, the generated installer is a no-op. If services are discovered from referenced assemblies, the installer registers them in the composition assembly output.

Alternative considered: only emit when services are present. That caused the current mismatch between analyzer guidance and generated API availability.

### Decision: Split per-compilation diagnostics from project-wide validation

Analyzers should report only what the current compilation can prove. Smoke validation should own checks that require a Unity project graph, asmdef references, or multiple assemblies not visible in one compilation.

Alternative considered: make analyzers aggressively report missing mappings. That catches more errors early but creates false positives in marker-based architectures where the service assembly cannot see the composition root.

### Decision: Reports come from the same graph as generation

Markdown, CSV, JSON, and AI-friendly report output should use the graph after the same filtering and grouping used for generation. The report must not be a best-effort reconstruction of emitted code.

Alternative considered: keep report generation separate. That preserves the current implementation shape but makes report drift inevitable.

## Risks / Trade-offs

- [Risk] This is a broad internal refactor. -> Mitigation: land behind regression tests first, then migrate generator, analyzers, smoke validation, and reports in separate tasks.
- [Risk] Existing diagnostics may change timing or severity. -> Mitigation: document diagnostic moves and add tests showing why per-compilation checks moved to smoke validation.
- [Risk] Generated code snapshots may churn. -> Mitigation: preserve public generated method names and update snapshots only after behavior tests pass.
- [Risk] Unity asmdef behavior can differ from Roslyn test references. -> Mitigation: include a fixture matching the marker/shared, service-feature, bootstrap-composition layout and validate against the `afterimage` pattern before release.

## Migration Plan

1. Add contract graph models and tests without changing emitted behavior.
2. Feed generator output from the graph and preserve current public generated APIs.
3. Migrate analyzer rules to graph-backed evidence while keeping diagnostic IDs stable where possible.
4. Migrate smoke validation and reports to graph-backed project analysis.
5. Validate against existing generator/analyzer tests and the `afterimage` integration.
6. Update package metadata and release notes for `8.0.0` only after validation is green.

Rollback strategy: keep the old extraction path until generator parity tests pass. If analyzer migration regresses too much, ship the graph-backed generator first and keep analyzer migration behind follow-up tasks.

## Open Questions

- Should project-wide smoke validation become the required CI gate for `NDFG014`-class missing mapping checks?
- Should the graph expose a public JSON schema in 8.0, or keep it internal until 8.1 after field feedback?
- Should no-op installers be reported as informational rows in generated reports, or omitted unless verbose output is enabled?
