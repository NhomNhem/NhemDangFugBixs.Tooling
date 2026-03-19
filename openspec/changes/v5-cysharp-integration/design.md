## Context

The current tooling (v4.2.0) performs attribute-driven discovery for VContainer registrations, installer ordering, and analyzer checks (ND001-ND009, ND103-ND107). It excels at compile-time generation but lacks:
1. First-class MessagePipe broker wiring with scope awareness
2. Human-readable registration reports for inspection
3. Cross-scope dependency validation (ND006)

v4.2.0 already has MessagePipe guard (ND008) and ILogger guard (ND009) as analyzers, but the generator doesn't emit MessagePipe registration code automatically.

## Goals / Non-Goals

**Goals:**
- Extend the generator to emit MessagePipe broker registration code (`builder.RegisterMessageBroker<T>()`)
- Produce Markdown and CSV reports from the same semantic graph used for code generation
- Implement ND006 cross-scope dependency analyzer with symbol-based validation
- Share semantic helpers (`SemanticScopeUtils`) across generator and analyzers

**Non-Goals:**
- Replace VContainer or MessagePipe APIs
- Add runtime scene scanning beyond current `[AutoInjectScene]` support
- Support R3 or UniTask in this release (future v5.x)
- Breaking changes to existing v4.x `[AutoRegisterIn]` usage

## Decisions

1. **MessagePipe Registration**: Use `[AutoRegisterMessageBrokerIn(typeof(TScope))]` attribute for broker declaration. Generator emits `builder.RegisterMessageBroker<T>()` in the target scope's registration method.

2. **Report Generation**: Emit `RegistrationReport.g.cs` with const fields and string arrays. Separate CLI tool (`DangFugBixs.DiSmokeValidation`) reads metadata and outputs Markdown/CSV.

3. **ND006 Implementation**: Build scope graph from `[AutoRegisterIn]` and `[LifetimeScopeFor]` attributes. Traverse dependency edges and report ND006 when no valid scope bridge exists.

4. **Shared Semantics**: Move `SemanticScopeUtils` to `DangFugBixs.Common` for use by both generator and analyzers. Avoid circular dependencies.

5. **Backward Compatibility**: All v4.2.0 features remain unchanged. MessagePipe support is opt-in via new attributes.

## Risks / Trade-offs

- **[More metadata]** → `ServiceInfo` grows with `MessagePipeType`, `BrokerMessageType` fields. Mitigation: Use nullable fields, only populate when relevant.

- **[Report staleness]** → Reports generated from old binaries can mislead. Mitigation: Include build timestamp and hash in report header; smoke test validates freshness.

- **[Analyzer complexity]** → ND006 scope graph traversal is O(n²) in worst case. Mitigation: Cache scope graph per compilation, use incremental generators.

- **[MessagePipe versioning]** → Different projects may use different MessagePipe versions. Mitigation: Use string-based type matching for `IPublisher<T>` / `ISubscriber<T>` detection.
