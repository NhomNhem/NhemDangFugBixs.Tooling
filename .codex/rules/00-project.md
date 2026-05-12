# 00 — Project Rules

## Package identity

This repository is for a reusable Unity package:

```txt
NhemDangFugBixs.Tooling
```

Package goal:

```txt
A compile-time VContainer workflow toolkit for Unity.
```

It provides:

- Attribute-driven registration.
- Scope marker architecture.
- Source-generated installers.
- DI analyzers.
- CLI preflight.
- Unity diagnostics window.
- AI-friendly dependency reports.

## Main users

- Unity developers using VContainer.
- Indie teams with asmdef-separated architecture.
- Developers using AI coding agents.
- Projects that want compile-time DI validation.

## Non-goals

Do not turn this package into:

- A replacement for VContainer.
- A runtime service locator.
- A Solar Phobia-only helper.
- A package that forces fixed scope names.
- A package that auto-registers everything without explicit opt-in.

## Design principle

The package should not decide the user's architecture.
It should make the user's chosen architecture safer.

Use this wording when in doubt:

```txt
NhemDangFugBixs.Tooling does not force your architecture.
It lets your architecture become compile-time checked.
```

## Public API stability

Public attributes and diagnostics should be treated as API.
Before renaming or removing an attribute:

1. Add backward compatibility if possible.
2. Add an analyzer warning if migration is needed.
3. Document the migration path.
4. Update samples and docs.
