# Capability: ilogger-root-guardrails

## Purpose
Detect logger infrastructure gaps for services injecting `ILogger<T>` and align analyzer and smoke-validation outcomes.

## Requirements
### Requirement: Require complete reachable logger root infrastructure
A logger dependency SHALL be considered valid only when reachable root setup includes both `ILoggerFactory` and a resolvable `ILogger<>` binding or adapter.

#### Scenario: Valid root logging setup
- **WHEN** a generated service injects `ILogger<T>`
- **AND** reachable root setup includes both `ILoggerFactory` and valid `ILogger<>` resolution
- **THEN** analyzer and smoke validation SHALL report no logger-root issue

#### Scenario: Missing logger factory
- **WHEN** a generated service injects `ILogger<T>`
- **AND** no reachable `ILoggerFactory` exists
- **THEN** analyzer and smoke validation SHALL report a logger-root issue

#### Scenario: Missing logger adapter or binding
- **WHEN** a generated service injects `ILogger<T>`
- **AND** `ILoggerFactory` exists but no reachable `ILogger<>` adapter or binding exists
- **THEN** analyzer and smoke validation SHALL report a logger-root issue

### Requirement: Preserve logger metadata for diagnostics
Generated metadata SHALL include logger consumer type, logger category type argument, and effective scope.

#### Scenario: Logger-root finding details
- **WHEN** a logger-root issue is reported
- **THEN** the finding SHALL identify consumer service and effective scope using generated metadata
