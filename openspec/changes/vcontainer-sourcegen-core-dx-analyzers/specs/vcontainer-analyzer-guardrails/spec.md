## ADDED Requirements

### Requirement: Enforce analyzer MVP rule set for contract and scope integrity
The analyzer package SHALL implement diagnostics `NHEM_DI_001`, `NHEM_DI_002`, `NHEM_DI_003`, `NHEM_DI_010`, `NHEM_DI_011`, and `NHEM_DI_012` with configured severities.

#### Scenario: Invalid contract mapping
- **WHEN** `As<TContract>` references a contract not implemented by the service type
- **THEN** analyzer SHALL emit `NHEM_DI_001` as Error

#### Scenario: Invalid scope marker type
- **WHEN** `AutoRegisterIn<TScope>` uses a type that is not a valid scope marker
- **THEN** analyzer SHALL emit `NHEM_DI_002` as Error

### Requirement: Detect duplicate registration and duplicate generated invocation hazards
The analyzer package SHALL implement diagnostics `NHEM_DI_020`, `NHEM_DI_021`, and `NHEM_DI_022`.

#### Scenario: Duplicate generated scope invocation
- **WHEN** both `RegisterGeneratedFor<TScope>()` and generated installer static call are invoked for the same scope
- **THEN** analyzer SHALL emit `NHEM_DI_022` as Error

### Requirement: Enforce entrypoint and resolver guardrails
The analyzer package SHALL implement diagnostics `NHEM_DI_040`, `NHEM_DI_041`, and `NHEM_DI_050`.

#### Scenario: EntryPoint without lifecycle interfaces
- **WHEN** a class is marked as EntryPoint but implements no supported lifecycle interfaces
- **THEN** analyzer SHALL emit `NHEM_DI_040` as Error

#### Scenario: Service locator injection pattern
- **WHEN** `IObjectResolver` is injected into a normal service class
- **THEN** analyzer SHALL emit `NHEM_DI_050` as Warning
