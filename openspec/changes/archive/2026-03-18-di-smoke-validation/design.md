## Context

The repository currently validates DI behavior through generator tests and analyzer tests, but those only cover partial source snapshots. The remaining risk is runtime drift: generated registrations can compile while the built output still fails to resolve in a real container or scene.

## Goals / Non-Goals

**Goals:**
- Add a fast validation command that runs after build and inspects generated DI output.
- Verify registration consistency, scope wiring, and duplicate or missing bindings before Unity Play mode.
- Keep the validator usable in CI and local development without launching Unity.

**Non-Goals:**
- Replace Unity play-mode tests.
- Validate every possible runtime behavior inside the validator.
- Rebuild the entire DI analysis pipeline from scratch.

## Decisions

- Use build output and reflection as the validation surface instead of Unity scene execution, because it is faster and CI-friendly.
- Reuse the existing registration graph emitted by the generator rather than creating a separate source of truth.
- Fail fast on structural DI errors such as missing broker registration, missing root logger infrastructure, or duplicate bindings.
- Expose the validator as a command so it can be run explicitly in local builds and CI.

## Risks / Trade-offs

- [Reflection-only validation] → Faster than Unity, but it cannot prove scene behavior; keep play-mode tests for final verification.
- [False positives from custom containers] → Mitigate by making the validator graph-aware and scoped to generated injectors only.
- [Tooling drift] → The validator must be regenerated or run from the same build output to avoid stale results.
- [Broader scope over time] → Keep this change focused on the smoke command; MessagePipe and ILogger guards can build on it later.

## Migration Plan

1. Add the validator command and wire it into the build/test flow.
2. Run it against the existing generator output and add baseline tests for success and failure cases.
3. If issues appear, refine the validator before expanding to additional guardrails.

## Open Questions

- Should the command validate only generated assemblies, or also scan user assemblies that consume them?
- Should the failure mode stop at the first DI error or report the full set of graph issues?
