# Skill — Test Writer

## Purpose

Use this skill when adding or updating tests.

## Required reading

```txt
AGENTS.md
.codex/rules/06-testing.md
```

## Test categories

```txt
Generator tests
Analyzer tests
CLI tests
Unity package tests
Editor tests
```

## Generator test checklist

Test:

- Attribute discovery.
- Scope marker mapping.
- Cross-assembly discovery.
- Generated registration output.
- Deduplication.
- Optional dependency absence.

## Analyzer test checklist

For every diagnostic:

```txt
- One invalid source that triggers diagnostic.
- One valid source that does not trigger diagnostic.
- Code fix test if applicable.
```

## CLI test checklist

Test:

```txt
- preflight output
- graph output
- report output
- json stability
- markdown stability
```

## Test style

Prefer small source snippets.
Keep expected output readable.
Avoid broad integration tests when a focused unit test is enough.

## Validation commands

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```
