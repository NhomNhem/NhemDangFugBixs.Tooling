## ADDED Requirements

### Requirement: Block release when mandatory verification gates fail
Release workflow SHALL fail if any required gate is unmet.

#### Scenario: Test or compile gate fails
- **WHEN** dotnet tests, generator snapshots, analyzer tests, Unity sample compile, or README compile checks fail
- **THEN** release SHALL be blocked

### Requirement: Enforce metadata and version consistency gates
Release workflow SHALL validate package metadata integrity and version consistency across package, generator banner, and docs.

#### Scenario: Version drift detected
- **WHEN** version strings differ between package metadata and generated/docs banners
- **THEN** release SHALL be blocked with actionable output

### Requirement: Enforce generated code safety gates
Release workflow SHALL verify generated code has no mutable static runtime state and no references to nonexistent documented APIs.

#### Scenario: Unsafe generated static state detected
- **WHEN** generated code introduces mutable static runtime state
- **THEN** release SHALL be blocked
