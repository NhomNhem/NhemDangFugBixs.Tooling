## Why

The package currently has contract drift between documentation, generator output, runtime DX, and package metadata, causing import failures, unresolved API usage (`RegisterGeneratedFor<TScope>()`), and inconsistent registration behavior across Unity projects. We need a production-ready, marker-first VContainer extension baseline that senior teams can trust in CI and at release time.

## What Changes

- Stabilize and formalize attribute-driven registration primitives for services, entry points, keyed registrations, and component-in-hierarchy registration.
- Standardize source generator output into explicit, stateless static installers plus a single extension-dispatch DX layer (`builder.RegisterGeneratedFor<TScope>()`) without duplicate invocation paths.
- Add analyzer guardrails for contract validity, scope mapping integrity, entry point correctness, duplicate invocation/registration, and service-locator misuse.
- Add strict release gates for package metadata integrity, docs/API sync, version drift detection, and generated-code safety.
- Define migration path for existing manual VContainer projects with gradual analyzer enforcement.
- **BREAKING**: Tighten analyzer diagnostics and package validation rules that may fail previously tolerated invalid patterns.

## Capabilities

### New Capabilities
- `vcontainer-sourcegen-core-dx`: Marker-first source-generated installer pipeline and extension dispatch (`RegisterGeneratedFor<TScope>()`) with user-owned LifetimeScope policy.
- `vcontainer-registration-primitives`: Public attribute surface for `AutoRegisterIn`, `As`, `AsSelf`, `LifetimeScopeFor`, `EntryPoint`, `Keyed`, and `RegisterComponentInHierarchy` behavior.
- `vcontainer-analyzer-guardrails`: Analyzer MVP ruleset (`NHEM_DI_001`..`NHEM_DI_050`) for invalid contracts, missing mappings/calls, duplicate registrations, and misuse warnings.
- `vcontainer-release-gates`: CI/release validation capability for package metadata, version consistency, generated-code statelessness, and sample/docs compile checks.
- `vcontainer-migration-strategy`: Progressive adoption contract for manual-to-generated registration transitions.

### Modified Capabilities
- `semantic-scope-analyzer`: Extend scope-marker diagnostics to include missing `LifetimeScopeFor<TScope>`, missing/incorrect `RegisterGeneratedFor<TScope>()` calls, and root-scope misuse warnings.
- `smart-lifecycle-filtering`: Align entry point generation/validation with explicit `EntryPoint` diagnostics and no unintended service exposure.
- `conflict-detection-analyzer`: Expand duplicate detection to include duplicate generated installer invocation and duplicate scoped implementation/contract registrations.

## Impact

- Affected code: `Source~/DangFugBixs.Attributes~`, `Source~/DangFugBixs.Generators~`, `Source~/DangFugBixs.Analyzers~`, runtime bridge namespace, package metadata, docs, and tests.
- Affected APIs: registration attributes, generated installer class names, generated extension dispatch method, analyzer diagnostic set.
- Affected dependencies: VContainer package version baseline and optional tooling/test dependencies.
- Affected systems: CI/release pipeline, Unity sample validation, docs sample compilation, and downstream package consumers upgrading from manual registration patterns.
