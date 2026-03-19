# Capability: messagepipe-broker-guardrails

## Requirement

- **Goal**: Detect when generated services inject `IPublisher<T>` or `ISubscriber<T>` without a reachable broker registration.
- **Scope Resolution**:
  - Treat a broker in the same scope as valid.
  - Treat a broker in an allowed parent or root scope as valid.
  - Report a finding when no reachable broker exists for the message type.
- **Alignment**:
  - The analyzer and DI smoke validator must surface the same structural broker-missing issue.
  - Findings must identify the consumer type, message type, and effective scope.
- **Metadata**:
  - Generated registration metadata must preserve broker message types, consumer message types, and owning scope.

## Verification

- **Same Scope**:
  - Given a service injecting `IPublisher<PlayerJoined>` in `GameLifetimeScope`.
  - Given a MessagePipe broker for `PlayerJoined` in `GameLifetimeScope`.
  - Expect no diagnostic and no smoke-validation failure.
- **Parent or Root Scope**:
  - Given a service injecting `ISubscriber<PlayerJoined>` in a child scope.
  - Given a broker for `PlayerJoined` in a reachable root scope.
  - Expect no diagnostic and no smoke-validation failure.
- **Missing Broker**:
  - Given a service injecting `IPublisher<PlayerJoined>` or `ISubscriber<PlayerJoined>` with no reachable broker.
  - Expect a missing-broker analyzer finding and a smoke-validation failure.
