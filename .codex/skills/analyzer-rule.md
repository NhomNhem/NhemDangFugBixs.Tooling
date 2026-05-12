# Skill — Analyzer Rule

## Purpose

Use this skill when adding a Roslyn diagnostic or code fix.

Examples:

- Public `[Inject]` field analyzer.
- MonoBehaviour constructor injection analyzer.
- Missing scope mapping analyzer.
- Duplicate scope mapping analyzer.
- Singleton depends on Scoped analyzer.
- IObjectResolver misuse analyzer.
- Public R3 `Subject<T>` analyzer.

## Required reading

```txt
AGENTS.md
.codex/rules/04-analyzers.md
.codex/rules/06-testing.md
.codex/rules/07-documentation.md
```

## Inputs

The task should specify:

```txt
Diagnostic ID
Title
Severity
Invalid examples
Valid examples
Whether code fix is expected
```

## Output

A completed analyzer rule should include:

```txt
- DiagnosticDescriptor.
- Analyzer implementation.
- Positive tests.
- Negative tests.
- Code fix if mechanical.
- Documentation entry.
```

## Workflow

1. Define diagnostic descriptor.
2. Write invalid source example test.
3. Write valid source example test.
4. Implement analyzer.
5. Implement code fix if appropriate.
6. Update docs.
7. Run analyzer tests.

## Severity guide

```txt
Error   — invalid or dangerous.
Warning — likely architecture mistake.
Info    — style/convention guidance.
```

## Guardrails

Do not:

- Analyze unrelated code too broadly.
- Create noisy warnings.
- Require every class to use `[AutoRegisterIn]`.
- Use diagnostics without clear user action.

## Diagnostic message style

Use actionable messages:

Bad:

```txt
Invalid injection.
```

Good:

```txt
MonoBehaviour 'PlayerView' should not use constructor injection. Use [Inject] private void Construct(...) instead.
```
