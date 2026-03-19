## Context

The repository already has MessagePipe registration emission and a DI smoke validator that can compare generated injector methods against `RegistrationReport` metadata. The remaining gap is semantic validation: services can inject `IPublisher<T>` or `ISubscriber<T>` and still compile even when no broker is registered for `T` in the effective scope.

## Goals / Non-Goals

**Goals:**
- Detect missing MessagePipe brokers before Unity Play mode.
- Reuse generated registration metadata and scope information instead of creating a second DI graph model.
- Keep analyzer and smoke-validator results aligned for the same broker issue.

**Non-Goals:**
- Validate every MessagePipe API surface such as request handlers or global provider setup.
- Prove runtime subscription disposal semantics.
- Replace Unity runtime tests for scene-specific behavior.

## Decisions

- Record broker registrations and consumer dependencies in generated metadata.
  Rationale: the generator already knows scope, service type, and broker message type. Extending that metadata is lower risk than reverse-engineering DI state purely from reflection.
- Add a dedicated analyzer rule for MessagePipe broker reachability.
  Rationale: constructor injection issues should surface during compilation, not only in the smoke tool. The analyzer will resolve constructor parameter symbols and compare them against scoped broker metadata.
- Keep parent-scope reachability explicit.
  Rationale: broker registrations in the same scope are always valid. Parent-scope access is only valid when the scope model already allows inheritance through the generated registration graph.
- Emit the same finding shape in smoke validation.
  Rationale: CI and local command-line validation should fail on the same structural broker issue as the analyzer.

## Risks / Trade-offs

- [Incomplete scope metadata] -> The guardrail could miss valid parent-scope registrations. Mitigation: use only generated scope mappings and fail conservatively when scope lineage is unknown.
- [False positives for manual registrations] -> Manual broker setup may not appear in generator metadata. Mitigation: scope the rule to generated injectors first and allow future suppression/escape hatches.
- [Analyzer/runtime drift] -> The analyzer and smoke validator may disagree if they parse metadata differently. Mitigation: keep a shared report shape and the same scope resolution rules.

## Migration Plan

1. Extend report metadata with broker and consumer entries.
2. Implement analyzer reachability checks for `IPublisher<T>` and `ISubscriber<T>`.
3. Add smoke-validator checks for the same missing-broker condition.
4. Add tests for same-scope success, parent-scope success, and missing-broker failures.

## Open Questions

- Should the first version include `IBufferedPublisher<T>` and `IBufferedSubscriber<T>` or only the plain interfaces?
- Do we need an explicit suppression mechanism for manual broker registration outside generator metadata?
