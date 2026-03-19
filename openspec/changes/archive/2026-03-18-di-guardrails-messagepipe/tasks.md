## 1. Metadata Foundation

- [x] 1.1 Extend generated registration metadata to record MessagePipe broker message types per scope.
- [x] 1.2 Record generated consumer dependencies for `IPublisher<T>` and `ISubscriber<T>` in the report shape used by smoke validation.

## 2. Analyzer Guardrail

- [x] 2.1 Add a MessagePipe analyzer rule that resolves publisher/subscriber constructor parameters and maps them to the effective generated scope.
- [x] 2.2 Implement reachability checks that accept same-scope and allowed parent-scope broker registrations, then emit a diagnostic for missing brokers.

## 3. Smoke Validation

- [x] 3.1 Extend the DI smoke validator to parse broker-consumer metadata and fail on missing reachable brokers.
- [x] 3.2 Keep analyzer and smoke-validator finding messages aligned for the same missing-broker condition.

## 4. Verification

- [x] 4.1 Add generator or analyzer tests covering same-scope and parent-scope valid broker resolution.
- [x] 4.2 Add failing tests for missing broker registrations and for metadata drift between emitted brokers and consumers.
