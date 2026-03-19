## Why

The current generator can emit MessagePipe registrations, but it still allows projects to compile while `IPublisher<T>` or `ISubscriber<T>` dependencies are missing a matching broker in the resolved scope. A dedicated guardrail is needed now so MessagePipe integration fails fast before Unity Play mode and stays aligned with the new DI smoke validator.

## What Changes

- Add a MessagePipe guardrail that inspects constructor-injected `IPublisher<T>` and `ISubscriber<T>` dependencies against generated registration metadata.
- Report missing broker registrations when a service depends on a message type that is not registered in the same scope or an allowed parent scope.
- Surface the guardrail through analyzer diagnostics and smoke-validation output so local builds and CI catch the same issue.
- Extend generated registration metadata to carry the minimum broker information needed for scope-aware validation.

## Capabilities

### New Capabilities
- `messagepipe-broker-guardrails`: Detect missing or inconsistent MessagePipe broker registrations for generated DI graphs.

### Modified Capabilities

## Impact

Affected areas include generator metadata emission, Roslyn analyzer rules for DI graph validation, the DI smoke-validation command, and tests covering MessagePipe broker scenarios across scopes.
