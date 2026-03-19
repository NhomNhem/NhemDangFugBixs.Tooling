## Why

The DI graph can currently compile while services inject `ILogger<T>` without any root logging infrastructure registered. That failure is predictable and structural, so it should be caught before Unity Play mode in the same way as the existing MessagePipe broker guardrails.

## What Changes

- Add an `ILogger<T>` guardrail that checks generated services for constructor-injected logger dependencies.
- Require a valid root logging setup, including `ILoggerFactory` and a resolvable `ILogger<>` binding or equivalent adapter, before treating logger injection as valid.
- Surface the same missing-logger-root issue in analyzer diagnostics and DI smoke validation.
- Extend generated metadata with the minimum logger consumer facts needed for build-time validation.

## Capabilities

### New Capabilities
- `ilogger-root-guardrails`: Detect missing root logging infrastructure for generated services that inject `ILogger<T>`.

### Modified Capabilities

## Impact

Affected areas include generator metadata emission, analyzer diagnostics for generated services, the DI smoke-validation command, and test coverage around logging-aware dependency graphs.
