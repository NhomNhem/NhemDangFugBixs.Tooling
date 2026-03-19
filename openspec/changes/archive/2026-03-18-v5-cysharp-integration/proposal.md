## Why

The current generator/analyzer stack is already close to VContainer, but it still treats DI mostly as registration code generation. The next step is to make it aware of scope boundaries, Cysharp integration points, and dependency graph validation so the tooling can prevent runtime DI mistakes before Unity play mode.

## What Changes

- Add MessagePipe support so the generator can emit broker and handler wiring that follows VContainer scope boundaries.
- Add a DI report/visualizer output in `.md` or `.csv` form to make generated registrations and scope mappings inspectable.
- Add a semantic scope analyzer for cross-scope dependency validation, including a new ND006 diagnostic.
- Tighten symbol-based analysis so generator and analyzer behavior stays aligned with real APIs rather than method-name matching.

## Capabilities

### New Capabilities
- `messagepipe-support`: Attribute and emit logic for MessagePipe registration and scope-aware broker wiring.
- `di-visualizer-report`: Generated registration summary output in Markdown or CSV.
- `semantic-scope-analyzer`: Cross-scope dependency analysis and ND006 diagnostics for invalid resolution paths.

## Impact

Affected areas include `Source~/DangFugBixs.Generators~/`, `Source~/DangFugBixs.Analyzers~/`, runtime attributes in `Source~/DangFugBixs.Runtime~/`, and the analyzer test projects. The change also introduces new dependencies on MessagePipe-aware wiring and reporting output generation.
