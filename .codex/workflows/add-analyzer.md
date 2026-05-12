# Codex Workflow — Add Analyzer

Use this when adding a Roslyn diagnostic or code fix.

## Required reading

- `AGENTS.md`
- `.codex/rules/default.md`
- `.codex/rules/04-analyzers.md` if present
- `.codex/rules/06-testing.md` if present

## Steps

1. Define diagnostic ID and severity.
2. Add descriptor.
3. Add analyzer implementation.
4. Add tests for positive and negative cases.
5. Add code fix if the fix is mechanical.
6. Update `Documentation~/diagnostics.md` when that doc exists.
7. Update README if the diagnostic is important.

## Severity guide

```txt
Error   — invalid or dangerous
Warning — likely mistake
Info    — convention/style
```

## Avoid

- Noisy warnings on unrelated code.
- Analyzing all code when only attributed/DI code matters.
- Diagnostics that cannot be understood from the message.
