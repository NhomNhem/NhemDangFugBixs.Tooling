## ADDED Requirements

### Requirement: Single-command validation orchestration
The system SHALL provide a CLI command that orchestrates regenerate, build, smoke validation, and report generation.

#### Scenario: Successful preflight validation
- **WHEN** user runs `dotnet di-smoke preflight --project MyGame.csproj`
- **THEN** the tool SHALL execute in sequence:
  1. Clean generated files (optional, `--clean` flag)
  2. `dotnet build` (triggers source generator)
  3. Run DI smoke validation on generated assembly
  4. Output validation report (text or markdown)
  5. Return exit code 0 if all checks pass, 1 if any fail

#### Scenario: Build failure handling
- **WHEN** `dotnet build` fails (compilation errors)
- **THEN** the tool SHALL:
  1. Display build errors
  2. Skip smoke validation
  3. Return exit code 1

#### Scenario: Smoke validation failure
- **WHEN** DI smoke validation finds errors (missing registrations, scope violations)
- **THEN** the tool SHALL:
  1. Display detailed error messages with code snippets
  2. Return exit code 1

#### Scenario: Markdown report output
- **WHEN** user specifies `--format markdown` or `--output report.md`
- **THEN** the tool SHALL generate a Markdown report file with:
  1. Summary statistics (services, scopes, errors)
  2. List of all registrations by scope
  3. List of all errors with fix suggestions

### Requirement: CLI integration with dotnet
The preflight command SHALL be invokable as a .NET tool.

#### Scenario: Global tool installation
- **WHEN** user runs `dotnet tool install -g DangFugBixs.Cli`
- **THEN** the command `dotnet di-smoke preflight` SHALL be available globally

#### Scenario: Local manifest installation
- **WHEN** user adds tool to `.config/dotnet-tools.json`
- **THEN** the command SHALL be available via `dotnet tool run di-smoke`
