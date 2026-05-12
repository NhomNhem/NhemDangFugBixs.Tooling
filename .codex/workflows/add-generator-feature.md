# Codex Workflow — Add Generator Feature

Use this when changing source generation.

## Required reading

- `AGENTS.md`
- `.codex/rules/default.md`
- `.codex/rules/02-vcontainer-generator.md` if present
- `.codex/rules/03-scope-marker-pattern.md` if present
- `.codex/rules/06-testing.md` if present

## Steps

1. Add generator test input snippet.
2. Add expected generated output or assertions.
3. Update extraction model if needed.
4. Update emitter.
5. Add diagnostics for invalid usage.
6. Update docs.
7. Run generator tests.
8. Run full solution tests.

## Must preserve

- Deterministic output.
- Readable generated code.
- Cross-asmdef safety.
- Backward compatibility for existing attributes when possible.

## Avoid

- Broad runtime reflection.
- Deduplication by class name only.
- Emitting services into wrong scope.
- Crashing when optional dependencies are absent.
