# Skill — Architecture Review

## Purpose

Use this skill before merging changes that affect architecture, public API, generator behavior, or package layout.

## Required reading

```txt
AGENTS.md
.codex/rules/00-project.md
.codex/rules/01-architecture.md
.codex/rules/03-scope-marker-pattern.md
.codex/rules/09-do-not.md
```

## Review checklist

Ask:

```txt
1. Does this force one project architecture?
2. Does this keep lower layers independent from Composition?
3. Does this make generated code readable?
4. Does this introduce service locator behavior?
5. Does this keep Runtime free from Editor/generator code?
6. Does this work across asmdef boundaries?
7. Does this have diagnostics for misuse?
8. Does this have tests and docs?
```

## Red flags

```txt
- Hard-coded GameplayLifetimeScope.
- Application referencing Composition.
- Runtime referencing UnityEditor.
- IObjectResolver injected into normal services.
- Attribute without tests.
- Analyzer without docs.
- Generated code using broad reflection.
- Silent generator failure.
```

## Output

An architecture review should produce:

```txt
- Approved / Needs changes.
- Main risks.
- Required follow-ups.
- Suggested simplifications.
```
