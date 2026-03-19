# Capability: semantic-scope-analyzer

## Requirement

- **Goal**: Detect invalid dependencies that cross scope boundaries without an allowed bridge or identity mapping.
- **Diagnostics**:
  - Report `ND006` when a service depends on another service outside its reachable scope.
  - Suppress `ND006` when a declared identity scope or mapping makes the dependency valid.
- **Analysis Quality**:
  - Resolve symbols for services, scopes, and registration APIs before evaluating scope validity.
  - Do not rely on name-only matches for scope or DI API detection.

## Verification

- **Invalid Cross-Scope Dependency**:
  - Given a service that depends on a service registered in an unreachable scope.
  - Expect `ND006`.
- **Valid Identity Mapping**:
  - Given a dependency bridged through a declared identity scope or mapping.
  - Expect no `ND006`.
- **Name Collision**:
  - Given a method or type with a matching name but unsupported symbol identity.
  - Expect it to be ignored by the analyzer.
