# Skill — Release Manager

## Purpose

Use this skill when preparing a package release.

## Required reading

```txt
AGENTS.md
.codex/rules/08-release.md
.codex/rules/05-unity-package.md
```

## Release checklist

```txt
- package.json version updated.
- CHANGELOG.md updated.
- README.md updated.
- Documentation~ updated.
- Samples~ validated.
- Runtime asmdef compiles.
- Editor asmdef compiles.
- Generator tests pass.
- Analyzer tests pass.
- CLI tests pass.
- Unity package layout validates.
- deploy branch workflow is ready.
```

## Commands

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
dotnet test Source~/NhemDangFugBixs.Tooling.sln -c Release
```

## Versioning

Use SemVer:

```txt
MAJOR.MINOR.PATCH
```

Before 1.0.0, public API may evolve, but avoid unnecessary breaking changes.

After 1.0.0, breaking changes require:

```txt
- Major version bump.
- Migration guide.
- CHANGELOG entry.
- Analyzer/code fix if possible.
```
