## ADDED Requirements

### Requirement: Provide an editor window for DI diagnostics
The system SHALL provide a Unity Editor window for browsing generated DI data and analyzer feedback.

#### Scenario: Window shows scope mappings and services
- **WHEN** the user opens `Window/Nhem/DI Diagnostics`
- **THEN** the window SHALL display known scope mappings and the registrations grouped under each scope

#### Scenario: Window shows diagnostics for selected scope or service
- **WHEN** the user selects a scope or service in the diagnostics window
- **THEN** the window SHALL show related diagnostics, warnings, and registration details for that selection

### Requirement: Provide editor actions for validation and generated outputs
The editor window SHALL expose actions that trigger common validation and inspection workflows.

#### Scenario: Run preflight from editor
- **WHEN** the user activates the preflight action in the editor window
- **THEN** the tooling SHALL run the same validation workflow used by the CLI and surface the results in the window

#### Scenario: Open generated outputs
- **WHEN** the user activates actions to open generated registrations or reports
- **THEN** the tooling SHALL navigate to the generated file or output location associated with the selected item
