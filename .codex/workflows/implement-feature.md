# Codex Workflow — Implement Feature

Use this workflow for normal feature work.

## Steps

1. Read `AGENTS.md`.
2. Read relevant `.codex/rules/*.md` files. Start with `.codex/rules/default.md`, then any numbered rule files if they exist.
3. Identify affected area:
   - Runtime attributes
   - Generator
   - Analyzer
   - CLI
   - Editor
   - Docs
   - Samples
4. Write or update tests first when practical.
5. Implement the smallest stable API.
6. Keep generated code readable.
7. Update docs if public behavior changes.
8. Run:

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```

9. Summarize:
   - What changed
   - Tests run
   - Risks
   - Follow-up work

## Rules

- Do not hard-code one game's architecture.
- Do not make presets mandatory.
- Do not add runtime service locator patterns.
- Do not put Editor code in Runtime.
