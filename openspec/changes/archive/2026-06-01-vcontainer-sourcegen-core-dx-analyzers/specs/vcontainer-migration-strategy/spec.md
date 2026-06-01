## ADDED Requirements

### Requirement: Support incremental migration from manual to generated registration
The package documentation and analyzer policy SHALL support phased adoption for existing manual VContainer projects.

#### Scenario: Initial adoption
- **WHEN** a team first enables generated registration
- **THEN** migration guidance SHALL preserve manual `LifetimeScope` ownership and allow low-risk service annotation first

### Requirement: Allow staged analyzer strictness
The package SHALL support warning-first rollout before strict error enforcement in mature adoption phases.

#### Scenario: Team enables analyzer warnings
- **WHEN** analyzer rollout begins
- **THEN** diagnostics policy SHALL allow warning-level enforcement before escalation to strict mode

### Requirement: Preserve manual installer escape hatch
The migration strategy SHALL keep manual installers allowed for exceptional cases without generator guessing behavior.

#### Scenario: Special case manual registration
- **WHEN** a team has edge-case registrations unsuitable for generator output
- **THEN** migration guidance SHALL allow manual installer usage with explicit intent policy
