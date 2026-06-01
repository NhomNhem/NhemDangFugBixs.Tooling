## 1. Scope Marker Foundations

- [x] 1.1 Add runtime marker-oriented attributes and models for `AutoRegisterIn`, `LifetimeScopeFor`, scope aliases, and preset resolution.
- [x] 1.2 Extend generator discovery to resolve scope markers, aliases, and concrete `LifetimeScope` mappings across asmdef boundaries.
- [x] 1.3 Emit generic and non-generic generated installer entry points for mapped scope markers.
- [x] 1.4 Add tests covering valid mappings, missing mappings, duplicate mappings, and alias resolution.

## 2. Registration Surface

- [x] 2.1 Extend registration metadata to represent explicit contracts, `AsSelf`, implemented-interface binding, and component registration modes.
- [x] 2.2 Generate entry point registrations for lifecycle services and validate explicit entry point usage.
- [x] 2.3 Generate scene hierarchy and new GameObject component registrations from component-specific attributes.
- [x] 2.4 Add installer and build-callback execution ordering to generated scope registration output.
- [x] 2.5 Add generator tests for service, entry point, component, installer, and build callback emission.

## 3. Analyzer Guardrails

- [x] 3.1 Expand scope analysis diagnostics to report missing and duplicate marker mappings.
- [x] 3.2 Add injection-style diagnostics for MonoBehaviour constructor injection, public `[Inject]` fields, and invalid inject methods.
- [x] 3.3 Add lifetime and architecture diagnostics for singleton-to-scoped dependencies, runtime singletons, and `IObjectResolver` misuse.
- [x] 3.4 Add configurable reactive guardrails for public `Subject<T>` exposure and undisposed subject ownership.
- [x] 3.5 Add analyzer and code-fix tests for each new diagnostic family.

## 4. MessagePipe and Report Metadata

- [x] 4.1 Update MessagePipe discovery and emission to preserve broker registrations under marker-based scope remapping.
- [x] 4.2 Record publisher/subscriber consumer metadata and scope ownership in shared report models.
- [x] 4.3 Extend generated report output with scope markers, aliases, entry points, components, installers, MessagePipe events, and warnings.
- [x] 4.4 Add tests that verify report output stays in sync with generated registrations.

## 5. CLI Tooling

- [x] 5.1 Add `di-smoke preflight` support for validating scope mappings, duplicates, and lifetime issues from generated metadata.
- [x] 5.2 Add `di-smoke list`, `graph`, and `report` commands for scope browsing and export.
- [x] 5.3 Support Markdown, JSON, and Mermaid-oriented output formats where applicable.
- [x] 5.4 Add CLI tests for success, failure, and export scenarios.

## 6. Unity Editor Tooling

- [x] 6.1 Create the `Window/Nhem/DI Diagnostics` editor window shell and refresh workflow.
- [x] 6.2 Render scope mappings, services, entry points, MessagePipe events, and diagnostics from generated metadata.
- [x] 6.3 Add toolbar actions for preflight, report generation, config access, and generated file navigation.
- [x] 6.4 Add editor tests or smoke coverage for window initialization and data rendering paths.

## 7. Package Polish

- [x] 7.1 Update package docs and samples to show marker-based scope architecture and generated registration usage.
- [x] 7.2 Add or refresh samples for basic registration, scope markers, MessagePipe, and scene components.
- [x] 7.3 Update changelog and release notes for the new package direction.
- [x] 7.4 Run generator, analyzer, CLI, and Unity package validation before implementation signoff.
