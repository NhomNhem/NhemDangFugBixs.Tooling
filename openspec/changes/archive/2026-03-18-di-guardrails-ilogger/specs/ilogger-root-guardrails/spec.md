## ADDED Requirements

### Requirement: ILogger dependencies must have reachable root logging infrastructure
The system MUST detect when a generated service injects `ILogger<T>` and no reachable root logging infrastructure exists.

#### Scenario: Root logger setup satisfies dependency
- **WHEN** a generated service injects `ILogger<PlayerService>` and the root scope registers `ILoggerFactory` plus a valid `ILogger<>` binding or adapter
- **THEN** the system SHALL treat the logger dependency as valid and produce no logger-root finding

#### Scenario: Missing ILoggerFactory produces a finding
- **WHEN** a generated service injects `ILogger<PlayerService>` and the DI graph has no reachable `ILoggerFactory`
- **THEN** the system SHALL report a logger-root finding that identifies the consumer service and scope

#### Scenario: Missing ILogger adapter produces a finding
- **WHEN** a generated service injects `ILogger<PlayerService>` and the DI graph has `ILoggerFactory` but no valid `ILogger<>` binding or adapter
- **THEN** the system SHALL report a logger-root finding that identifies the consumer service and scope

### Requirement: Logger guardrails must align across analyzer and smoke validation
The system MUST surface the same structural logger-root issue in both compile-time analysis and build-time smoke validation.

#### Scenario: Analyzer reports missing logger root
- **WHEN** source analysis finds a generated service with `ILogger<T>` and no valid root logging setup
- **THEN** the analyzer SHALL emit a logger-root diagnostic

#### Scenario: Smoke validator reports missing logger root
- **WHEN** build output metadata contains a generated service with `ILogger<T>` and no valid root logging setup
- **THEN** the DI smoke-validation command SHALL fail and include the logger-root issue in its summary

### Requirement: Generated metadata must preserve logger consumer facts
The system MUST emit metadata that includes logger consumer services, target logger type arguments, and effective scope.

#### Scenario: Report includes logger consumers
- **WHEN** the generator analyzes a generated service that injects `ILogger<T>`
- **THEN** the generated report SHALL include the consumer service, requested logger type, and effective scope
