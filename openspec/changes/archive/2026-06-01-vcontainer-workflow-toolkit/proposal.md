## Why

The package already has strong VContainer generation and diagnostics primitives, but it still feels like a project-specific toolset rather than a reusable Unity package with a clear architectural contract. We need a proposal that turns the current feature set into a public-facing VContainer workflow toolkit centered on scope markers, safer registration patterns, and AI-friendly diagnostics before more ad hoc additions make the surface area harder to evolve.

## What Changes

- Introduce marker-based scope registration so lower-layer asmdefs can declare VContainer intent without referencing concrete `LifetimeScope` types.
- Add a first-class registration model for services, entry points, scene components, new GameObject components, installers, and build callbacks.
- Expand analyzer guardrails to cover scope mapping errors, injection style issues, lifetime misuse, resolver misuse, and configurable reactive conventions.
- Extend DI reports and MessagePipe support so generated outputs reflect scope markers, consumer metadata, and architecture warnings.
- Add a `di-smoke` workflow for preflight validation, scope listings, graph generation, and report export.
- Add a Unity Editor diagnostics window for browsing scope mappings, registrations, diagnostics, and generated outputs.

## Capabilities

### New Capabilities
- `scope-marker-registration`: Marker-based scope mapping, alias-based registration, and generated VContainer installer entry points.
- `entry-point-component-registration`: Generated registration support for entry points, scene components, hierarchy components, new GameObject components, installers, and build callbacks.
- `architecture-guardrails`: Analyzer and code-fix rules for injection style, lifetime mistakes, resolver misuse, and configurable reactive conventions.
- `di-smoke-validation`: CLI preflight, graph, list, and report commands for generated DI metadata.
- `unity-di-diagnostics`: Editor tooling for inspecting scopes, services, diagnostics, and generated reports.

### Modified Capabilities
- `semantic-scope-analyzer`: Expand scope validation to use marker-to-scope mappings and emit missing or duplicate mapping diagnostics.
- `di-visualizer-report`: Expand report output to include scope markers, aliases, entry points, components, MessagePipe metadata, and architecture warnings.
- `messagepipe-support`: Preserve MessagePipe broker behavior under marker-based scope remapping and expose publisher/subscriber metadata in reports and tooling.

## Impact

- Affected code spans `Runtime`, `Editor`, `Analyzers`, and the `Source~` generator, analyzer, CLI, and test projects.
- Public runtime API grows with new attributes and lightweight models, while VContainer remains a peer dependency rather than being hidden.
- Analyzer coverage and generated metadata become part of the package contract, so samples, docs, and tests must expand alongside implementation.
- This change establishes the package’s MVP architecture for a public release and will shape later advanced features such as keyed, collection, and prefab-oriented registrations.
