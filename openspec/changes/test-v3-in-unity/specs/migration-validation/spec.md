## ADDED Requirements

### Requirement: Migration Path Validation
The migration guide from v2.x to v3.0 SHALL be accurate and actionable.

#### Scenario: Find-and-replace works
- **WHEN** user follows migration guide find-and-replace instructions
- **THEN** all `[AutoRegister(scope: "...")]` are converted to `[AutoRegisterIn(typeof(...))]`

#### Scenario: Migration produces working code
- **WHEN** user completes migration steps
- **THEN** Unity project compiles without errors related to AutoRegister

### Requirement: Deprecation Warnings
The `[Obsolete]` attribute on legacy `AutoRegisterAttribute` SHALL produce compiler warnings.

#### Scenario: Legacy usage produces warning
- **WHEN** user writes `[AutoRegister(scope: "Gameplay")]`
- **THEN** compiler produces CS0618 warning about using obsolete attribute

#### Scenario: Warning message is helpful
- **WHEN** user sees deprecation warning
- **THEN** message includes guidance to use `AutoRegisterIn(typeof(TScope))` instead

### Requirement: Diagnostic Feedback
Roslyn analyzer diagnostics SHALL provide actionable error messages.

#### Scenario: Invalid scope type produces error
- **WHEN** user writes `[AutoRegisterIn(typeof(string))]` (invalid type)
- **THEN** analyzer produces ND002 error with helpful message

#### Scenario: Missing scope type produces error
- **WHEN** user references non-existent scope type
- **THEN** analyzer produces ND001 error
