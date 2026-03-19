## ADDED Requirements

### Requirement: Global VContainer Usings
The system SHALL include `using VContainer;` and `using VContainer.Unity;` in the header of all generated registration files to ensure extension methods like `RegisterEntryPoint` are available.

#### Scenario: Verify Header Usings
- **WHEN** any registration file is generated
- **THEN** the header contains `using VContainer;` and `using VContainer.Unity;`.
