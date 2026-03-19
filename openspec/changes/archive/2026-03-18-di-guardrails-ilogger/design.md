## Context

The repository now has MessagePipe guardrails and a DI smoke validator that can fail on structural DI mistakes before Unity Play mode. Logging is the next predictable failure class: services may inject `ILogger<T>` while the container never registers `ILoggerFactory` or any way to resolve `ILogger<>`.

## Goals / Non-Goals

**Goals:**
- Detect missing root logging infrastructure for generated services that inject `ILogger<T>`.
- Reuse the same metadata and validation pattern already used by MessagePipe guardrails.
- Keep analyzer and smoke-validator output aligned for logger-root failures.

**Non-Goals:**
- Validate logging provider configuration details or provider quality.
- Model every custom logging framework integration in the first version.
- Replace runtime tests for logging behavior after container startup.

## Decisions

- Scope the first version to generated services that use constructor-injected `ILogger<T>`.
  Rationale: this keeps the rule aligned with the generator-owned DI graph and avoids broad false positives in unrelated code.
- Treat a logger dependency as valid only when root logging infrastructure is reachable.
  Rationale: `ILogger<T>` typically depends on `ILoggerFactory` plus an open-generic logger adapter or equivalent root registration.
- Reuse generator metadata to record logger consumers.
  Rationale: the generator already tracks services and scopes, so adding logger-consumer facts is cheaper and less error-prone than reconstructing them separately.
- Keep smoke validation and analyzer messages consistent.
  Rationale: CI and compile-time diagnostics should point to the same structural problem with the same wording.

## Risks / Trade-offs

- [Custom logger adapters] -> Some valid setups may not register `ILogger<>` in the obvious way. Mitigation: start with strict root requirements and add an escape hatch only if real usage demands it.
- [Manual registrations outside generator metadata] -> The first version may not see every manual root logging setup. Mitigation: keep the rule focused on generated services and document the limitation.
- [Overly broad root assumptions] -> If root scope naming varies, the rule could miss valid setups. Mitigation: reuse the same root-scope conventions already established in smoke validation.

## Migration Plan

1. Extend metadata with logger-consumer facts.
2. Implement analyzer checks for `ILogger<T>` constructor parameters.
3. Extend smoke validation to fail on missing root logging infrastructure.
4. Add passing and failing tests for valid root setup, missing `ILoggerFactory`, and missing logger adapter cases.

## Open Questions

- Should the first version treat a custom adapter type as valid if it resolves `ILogger<>` without explicitly registering `ILoggerFactory`?
- Do we want a dedicated diagnostic ID for logger-root failures, or should this remain a separate guardrail-specific diagnostic in the analyzer?
