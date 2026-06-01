## ADDED Requirements

### Requirement: Validate project-wide DI graph gaps
The smoke validator SHALL validate DI graph conditions that require project-wide assembly and asmdef evidence.

#### Scenario: Composition assembly does not reference service assembly
- **WHEN** a service is registered for a scope but no composition root can discover the service assembly through project references
- **THEN** smoke validation SHALL report a project-wide composition gap with service, scope, and assembly details

#### Scenario: Per-compilation analyzer cannot prove missing mapping
- **WHEN** a service assembly lacks visibility into the composition root
- **THEN** smoke validation SHALL own the missing mapping diagnostic instead of requiring a per-compilation analyzer error

### Requirement: Produce actionable smoke validation evidence
Smoke validation diagnostics SHALL include the scope marker, affected registrations, composition root candidates, and assembly reference path when available.

#### Scenario: Missing generated call evidence
- **WHEN** a composition root maps a scope but generated registration is not called
- **THEN** smoke validation SHALL identify the lifetime scope type, expected marker, and missing call

### Requirement: Support CI gating for release validation
The smoke validator SHALL support a deterministic command suitable for CI gating of 8.0 release candidates.

#### Scenario: Release validation succeeds
- **WHEN** a package integration project has no graph gaps, duplicate registrations, or missing generated calls
- **THEN** the smoke validator SHALL exit successfully with a machine-readable report
