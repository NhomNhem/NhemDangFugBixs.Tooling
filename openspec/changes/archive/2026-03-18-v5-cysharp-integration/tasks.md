## 1. Core Graph Foundation

- [x] 1.1 Extend the discovery model to carry MessagePipe, scope, and reporting metadata through the existing `ServiceInfo` flow.
- [x] 1.2 Add shared semantic helpers for resolving scope ownership, registration kind, and dependency edges from Roslyn symbols.
- [x] 1.3 Update analyzer tests to cover symbol-based matching for new integration points.
- [x] 1.4 Move analyzer-used semantic helpers into a shared location so `DangFugBixs.Analyzers` does not reference generator-only assemblies.

## 2. MessagePipe Support

- [x] 2.1 Add runtime attributes or metadata needed to declare MessagePipe-aware services and handlers.
- [x] 2.2 Extend generator emitters to produce scope-aware MessagePipe registration code.
- [x] 2.3 Add tests for MessagePipe wiring and a false-positive case for unrelated APIs with similar names.
- [x] 2.4 Preserve MessagePipe broker metadata when remapping services into type-safe scopes.
- [x] 2.5 Support multiple MessagePipe target attributes on the same type instead of stopping at the first match.
- [x] 2.6 Align attribute targets with discovery support, or extend discovery to scan structs if struct targets remain allowed.

## 3. DI Visualizer / Report

- [x] 3.1 Define the report schema for registrations, scopes, lifetimes, installers, and special callbacks.
- [x] 3.2 Emit a Markdown report from the same semantic graph used for code generation.
- [x] 3.3 Add optional CSV export support and verify it matches the Markdown data.

## 4. Scope Awareness Analyzer (ND006)


- [x] 4.1 Implement scope graph analysis that detects invalid cross-scope dependencies.
- [x] 4.2 Report ND006 for unresolved or out-of-scope dependencies while honoring valid identity mappings.
- [x] 4.3 Add analyzer tests for valid bridge cases, invalid dependencies, and duplicate registration conflicts.
