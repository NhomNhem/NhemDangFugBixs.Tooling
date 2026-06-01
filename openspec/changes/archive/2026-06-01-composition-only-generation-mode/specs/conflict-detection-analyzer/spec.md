# Capability: conflict-detection-analyzer

## MODIFIED Requirements

### Requirement: Detect duplicate composition targets within one compilation when possible

The analyzer SHALL report duplicate local composition targets for the same scope marker when they are detectable within the same compilation.

#### Scenario: Two local LifetimeScopeFor declarations target the same scope
- **WHEN** a compilation declares two `[LifetimeScopeFor(typeof(IGameplayScope))]` mappings
- **THEN** the analyzer reports a duplicate composition target diagnostic
- **AND** the diagnostic identifies the conflicting scope marker

### Requirement: Detect duplicate discovered registration in one composition target

The generator or analyzer SHALL handle duplicate discovered registrations deterministically for a single composition target.

#### Scenario: Same implementation is discovered twice for one scope
- **WHEN** duplicate registration metadata resolves to the same implementation and scope in one composition compilation
- **THEN** the toolchain reports or suppresses duplicates deterministically
- **AND** generated installer output does not emit duplicate registrations nondeterministically
