# Capability: di-visualizer-report

## Requirement

- **Goal**: Generate a registration report derived from the same semantic graph used for code generation.
- **Outputs**:
  - Produce a Markdown report describing discovered registrations, scopes, lifetimes, installers, and special callbacks.
  - Optionally produce CSV output with the same underlying data.
- **Consistency**:
  - The report must match the generated registration graph exactly.
  - Scope grouping, lifetimes, installer ordering, and special registration flags must not drift from emitted code.

## Verification

- **Markdown Report**:
  - Given a successful generator run.
  - Expect a Markdown registration report to be produced.
- **CSV Export**:
  - Given structured export is enabled.
  - Expect CSV output with the same rows and scope grouping as the Markdown report.
- **Graph Match**:
  - Given services grouped into scopes during emission.
  - Expect the report to reflect the same scope graph and registration properties.
