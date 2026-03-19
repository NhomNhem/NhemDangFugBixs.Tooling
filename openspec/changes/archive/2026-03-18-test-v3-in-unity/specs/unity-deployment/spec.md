## ADDED Requirements

### Requirement: Unity Deployment Process
The system SHALL provide a clear process for deploying v3.0 DLLs to a Unity project for testing.

#### Scenario: Deploy DLLs to Unity project
- **WHEN** tester copies DLLs from tooling to Unity project
- **THEN** Runtime DLL goes to `Assets/Plugins/` and Analyzer/Generator DLLs go to `Assets/Plugins/Analyzers/`

#### Scenario: Verify DLL installation
- **WHEN** Unity recompiles after DLL copy
- **THEN** No compilation errors related to NhemDangFugBixs assemblies

### Requirement: Rollback Capability
The system SHALL support easy rollback to v2.x if critical issues are found.

#### Scenario: Backup before deployment
- **WHEN** deploying v3.0 to a project with existing installation
- **THEN** backup current DLLs before overwriting

#### Scenario: Restore previous version
- **WHEN** critical issue discovered during testing
- **THEN** restore backup DLLs and Unity project returns to working state
