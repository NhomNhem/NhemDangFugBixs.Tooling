## Context

The current tooling already performs attribute-driven discovery for VContainer registrations, installer ordering, and some analyzer checks. It is strong at compile-time generation but still lacks first-class support for Cysharp packages and a unified semantic view of DI scope boundaries.

## Goals / Non-Goals

**Goals:**
- Extend the generator/analyzer pipeline so it understands MessagePipe registration patterns.
- Produce a human-readable registration report for generated scopes and services.
- Add semantic scope analysis that can detect invalid cross-scope dependencies before runtime.

**Non-Goals:**
- Replace VContainer or MessagePipe APIs.
- Add runtime scene scanning beyond what is needed for validation and reporting.
- Solve all async/reactive lifecycle patterns in one release.

## Decisions

- Keep VContainer as the primary DI model and treat Cysharp support as extensions on top of the existing attribute graph.
- Build a shared semantic model from the existing `ServiceInfo`/`ScopeMappingInfo` data instead of introducing a second discovery pipeline.
- Emit reports from the same graph used for registration generation so the report always matches generated code.
- Implement ND006 as a symbol-based analyzer over scope relationships, not as a regex or syntax-only check.

## Risks / Trade-offs

- [More metadata] → The generator and runtime attributes will grow, but the graph stays explicit and testable.
- [Broader surface area] → MessagePipe, R3, and UniTask may expose mismatched lifetimes; diagnostics and integration tests must cover this.
- [Report staleness] → A report generated from old binaries can mislead users; the build must regenerate it alongside code output.
- [Analyzer complexity] → Semantic scope checks are more expensive than syntax-only checks, so caching and targeted symbol resolution are important.
