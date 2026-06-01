## Why

The current generator/analyzer stack (v4.2.0) provides DI guardrails (ND008, ND009) and smoke testing, but still lacks first-class Cysharp ecosystem integration and visual diagnostics. v5.0 completes the vision by adding MessagePipe broker wiring, cross-scope dependency validation (ND006), and a DI visualizer report for inspectable generated registrations.

## What Changes

- **MessagePipe Support**: Generator emits scope-aware broker registration code for `IPublisher<T>` and `ISubscriber<T>` dependencies with proper VContainer scope mapping.
- **DI Visualizer Report**: Generated Markdown/CSV report showing all registrations, scopes, lifetimes, and dependencies for inspection before Unity Play mode.
- **Cross-Scope Analyzer (ND006)**: Semantic analyzer detecting invalid cross-scope dependencies that violate scope boundaries without approved bridges.
- **Symbol-Based Analysis**: Tighten all generator and analyzer logic to use Roslyn symbols exclusively, eliminating name-only matching.

## Capabilities

### New Capabilities
- `messagepipe-support`: Generator support for MessagePipe broker/handler registration with scope-aware wiring and `[AutoRegisterMessageBrokerIn]` attribute.
- `di-visualizer-report`: Generated registration summary in Markdown and CSV format for inspection and debugging.
- `semantic-scope-analyzer`: Cross-scope dependency validation with ND006 diagnostic for invalid scope resolution paths.

### Modified Capabilities
- None (all requirements are additive; no breaking changes to existing v4.x behavior)

## Impact

**Affected Areas:**
- `Source~/DangFugBixs.Generators~/`: Extended emitter for MessagePipe wiring and report generation
- `Source~/DangFugBixs.Analyzers~/`: New ND006 analyzer and symbol-based validation
- `Source~/DangFugBixs.Runtime~/`: MessagePipe attributes and report metadata models
- `Source~/DangFugBixs.Common~/`: Shared semantic helpers for scope analysis
- Test projects: MessagePipe wiring tests, ND006 analyzer tests, report validation tests

**Dependencies:**
- MessagePipe (for broker registration support)
- No breaking changes to existing VContainer or analyzer dependencies

**Migration:**
- v4.x → v5.0 is backward compatible
- Existing `[AutoRegisterIn]` usage unchanged
- MessagePipe support is opt-in via new attributes
