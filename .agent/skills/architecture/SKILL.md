---
name: Project Architecture
description: Managing the unique UPM + Hidden Source structure.
---

# Project Architecture Skill

## Repository Management
- **The Core Law**: The `NhemDangFugBixs.Tooling` project is a UPM package designed for zero-configuration.
- Source code is isolated in `Source~`.
- External dependencies (DLLs) are deployed to `Runtime/` and `Analyzers/`.

## Shared Logic (Common Library)
- Always check `Source~/DangFugBixs.Common~` for shared definitions.
- When adding a new cross-cutting feature, identify if a "Common" model is required first.

## Validation
- After any build, verify that the `.meta` files in the package root match the verified templates.
- Ensure `isAutoReferenced` is `0` for all Analyzer/Generator DLLs to prevent runtime pollution.
