## 1. Metadata Foundation

- [x] 1.1 Extend generated metadata to record `ILogger<T>` consumer services, target logger type arguments, and effective scopes.
- [x] 1.2 Define how root logging infrastructure is represented in generated metadata or validation inputs.

## 2. Analyzer Guardrail

- [x] 2.1 Add an analyzer rule that resolves constructor-injected `ILogger<T>` on generated services.
- [x] 2.2 Report a logger-root diagnostic when `ILoggerFactory` or a valid `ILogger<>` binding or adapter is not reachable from the effective root scope.

## 3. Smoke Validation

- [x] 3.1 Extend the DI smoke validator to parse logger-consumer metadata and validate root logging infrastructure.
- [x] 3.2 Keep analyzer and smoke-validator messages aligned for the same logger-root failure condition.

## 4. Verification

- [x] 4.1 Add passing tests for a generated service with a valid root logger setup.
- [x] 4.2 Add failing tests for missing `ILoggerFactory` and missing `ILogger<>` adapter or binding.
