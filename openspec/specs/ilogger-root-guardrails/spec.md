# Capability: ilogger-root-guardrails

## Requirement

- **Goal**: Detect when generated services inject `ILogger<T>` without reachable root logging infrastructure.
- **Root Requirements**:
  - Treat a logger dependency as valid only when reachable root setup includes `ILoggerFactory`.
  - Treat a logger dependency as valid only when reachable root setup also includes a resolvable `ILogger<>` binding or adapter.
  - Report a finding when either part of the root logging setup is missing.
- **Alignment**:
  - The analyzer and DI smoke validator must surface the same structural logger-root issue.
  - Findings must identify the consumer service and effective scope.
- **Metadata**:
  - Generated metadata must preserve logger consumer services, target logger type arguments, and effective scope.

## Verification

- **Valid Root Setup**:
  - Given a generated service injecting `ILogger<PlayerService>`.
  - Given reachable root setup containing both `ILoggerFactory` and a valid `ILogger<>` adapter or binding.
  - Expect no analyzer diagnostic and no smoke-validation failure.
- **Missing Factory**:
  - Given a generated service injecting `ILogger<PlayerService>`.
  - Given no reachable `ILoggerFactory`.
  - Expect a logger-root finding.
- **Missing Adapter**:
  - Given a generated service injecting `ILogger<PlayerService>`.
  - Given `ILoggerFactory` exists but no valid `ILogger<>` adapter or binding exists.
  - Expect a logger-root finding.
