## ADDED Requirements

### Requirement: Reject invalid MonoBehaviour injection patterns
The analyzer SHALL report diagnostics for MonoBehaviour injection styles that VContainer-oriented Unity projects must not use.

#### Scenario: MonoBehaviour constructor injection
- **WHEN** a `MonoBehaviour` declares a constructor with injected dependencies
- **THEN** the analyzer SHALL report an injection-style error

#### Scenario: Public injected field
- **WHEN** a type declares a public field marked with `[Inject]`
- **THEN** the analyzer SHALL report an injection-style error and MAY offer a code fix to convert it to `Construct(...)` injection

### Requirement: Report lifetime and resolver misuse
The analyzer SHALL report diagnostics for registrations that create unstable or architecture-breaking dependency chains.

#### Scenario: Singleton depends on scoped service
- **WHEN** a singleton registration depends on a scoped registration in the generated dependency graph
- **THEN** the analyzer SHALL report a lifetime diagnostic

#### Scenario: Resolver injected outside approved patterns
- **WHEN** `IObjectResolver` is injected into a type that is not marked as an approved factory, spawner, bootstrapper, or lifetime scope
- **THEN** the analyzer SHALL report a resolver-misuse diagnostic

### Requirement: Support configurable reactive guardrails
The analyzer SHALL support configurable conventions for reactive primitives without making them mandatory for every project.

#### Scenario: Public Subject exposure is forbidden
- **WHEN** reactive guardrails are enabled and a type exposes `Subject<T>` publicly
- **THEN** the analyzer SHALL report a reactive-convention diagnostic

#### Scenario: Subject owner does not dispose owned subjects
- **WHEN** reactive guardrails are enabled and a type owns `Subject<T>` instances without participating in disposal
- **THEN** the analyzer SHALL report a reactive-lifecycle warning
