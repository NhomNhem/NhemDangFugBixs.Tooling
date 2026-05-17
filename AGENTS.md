# AGENTS.md — NhemDangFugBixs.Tooling

## Purpose

This repository builds `NhemDangFugBixs.Tooling`, a reusable Unity package that improves VContainer workflows through:

- Source-generated VContainer registrations.
- Roslyn analyzers and code fixes.
- Scope marker mapping across Unity asmdef/layer boundaries.
- CLI and Unity Editor diagnostics.
- AI-friendly DI reports and dependency graphs.

The package must remain reusable outside a single game project.

---

## Core Principle

Do not force one project architecture.

This package should provide generic primitives that users can map to their own architecture.

Use this positioning when making design decisions:

```txt
NhemDangFugBixs.Tooling does not force your architecture.
It lets your architecture become compile-time checked.