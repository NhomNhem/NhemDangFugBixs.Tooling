# Capability: smart-lifecycle-filtering

## MODIFIED Requirements

### Requirement: Preserve local correctness diagnostics in service-only assemblies

The analyzer SHALL continue to validate local registration-shape correctness even when generation is deferred to a composition assembly.

#### Scenario: Service-only assembly reports invalid As contract
- **WHEN** a service-only compilation annotates a class with `[As(typeof(IMissingContract))]` that it does not implement
- **THEN** the analyzer reports the invalid contract diagnostic locally
- **AND** the offending registration is not required to generate local VContainer code for the diagnostic to be useful

#### Scenario: Service-only assembly reports invalid entry point or component usage
- **WHEN** a service-only compilation applies `[EntryPoint]` or `[RegisterComponentInHierarchy]` incorrectly
- **THEN** the analyzer reports the corresponding local correctness diagnostic
- **AND** composition-only generation does not suppress those local diagnostics
