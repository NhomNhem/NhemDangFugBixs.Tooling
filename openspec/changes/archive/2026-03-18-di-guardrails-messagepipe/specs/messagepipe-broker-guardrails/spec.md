## ADDED Requirements

### Requirement: Broker dependencies must resolve within the effective scope
The system MUST detect when a generated service depends on `IPublisher<T>` or `ISubscriber<T>` and no broker registration for `T` exists in the same scope or an allowed parent scope.

#### Scenario: Same-scope broker satisfies dependency
- **WHEN** a generated service injects `IPublisher<PlayerJoined>` and the same scope registers a MessagePipe broker for `PlayerJoined`
- **THEN** the system SHALL treat the dependency as valid and produce no broker-missing finding

#### Scenario: Parent-scope broker satisfies dependency
- **WHEN** a generated service injects `ISubscriber<PlayerJoined>` and a parent scope reachable through generated scope metadata registers the broker for `PlayerJoined`
- **THEN** the system SHALL treat the dependency as valid and produce no broker-missing finding

#### Scenario: Missing broker produces a finding
- **WHEN** a generated service injects `IPublisher<PlayerJoined>` or `ISubscriber<PlayerJoined>` and no reachable scope registers a broker for `PlayerJoined`
- **THEN** the system SHALL report a broker-missing finding that identifies the consumer service, message type, and scope

### Requirement: Broker guardrails must align across analyzer and smoke validation
The system MUST surface the same structural MessagePipe broker issue in both compile-time analysis and build-time smoke validation.

#### Scenario: Analyzer reports missing broker
- **WHEN** source analysis finds a generated service with a broker dependency that has no reachable registration
- **THEN** the analyzer SHALL emit a diagnostic for the missing broker condition

#### Scenario: Smoke validator reports missing broker
- **WHEN** build output metadata contains a generated service with a broker dependency that has no reachable registration
- **THEN** the DI smoke-validation command SHALL fail and include the missing broker condition in its summary

### Requirement: Generated metadata must preserve broker and consumer facts
The system MUST emit registration metadata that includes broker message types, consumer message types, and scope identity needed for MessagePipe guardrails.

#### Scenario: Report includes broker registrations
- **WHEN** the generator emits a MessagePipe broker registration
- **THEN** the generated report SHALL include the broker message type and owning scope

#### Scenario: Report includes broker consumers
- **WHEN** the generator emits or analyzes a generated service that injects `IPublisher<T>` or `ISubscriber<T>`
- **THEN** the generated report SHALL include the consumer service, message type, and effective scope
