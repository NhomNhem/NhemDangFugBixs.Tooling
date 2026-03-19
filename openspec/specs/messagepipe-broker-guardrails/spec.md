# Capability: messagepipe-broker-guardrails

## Purpose
Detect missing reachable MessagePipe brokers for `IPublisher<T>` and `ISubscriber<T>` dependencies.

## Requirements
### Requirement: Enforce reachable broker registration for message dependencies
The system SHALL report a broker-missing issue when no reachable broker exists for the injected message type.

#### Scenario: Broker exists in same scope
- **WHEN** a service injects `IPublisher<T>` or `ISubscriber<T>`
- **AND** broker for `T` is registered in the same scope
- **THEN** analyzer and smoke validation SHALL report no broker-missing issue

#### Scenario: Broker exists in reachable parent or root scope
- **WHEN** a service injects `IPublisher<T>` or `ISubscriber<T>` in a child scope
- **AND** broker for `T` is registered in a reachable parent or root scope
- **THEN** analyzer and smoke validation SHALL report no broker-missing issue

#### Scenario: No reachable broker exists
- **WHEN** a service injects `IPublisher<T>` or `ISubscriber<T>`
- **AND** no reachable broker registration exists for `T`
- **THEN** analyzer and smoke validation SHALL report a broker-missing issue

### Requirement: Preserve broker metadata for finding details
Generated registration metadata SHALL include broker message type, consumer message type, and owning scope.

#### Scenario: Broker-missing finding details
- **WHEN** a broker-missing issue is reported
- **THEN** finding output SHALL identify consumer type, message type, and effective scope
