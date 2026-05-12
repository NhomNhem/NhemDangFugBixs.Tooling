# 08 — Release Rules

## Versioning

Use SemVer:

```txt
MAJOR.MINOR.PATCH
```

Recommended stages:

```txt
0.1.0 — experimental core generator
0.2.0 — analyzer rules
0.3.0 — CLI report
0.4.0 — Unity Editor diagnostics
1.0.0 — stable public API
```

## Release checklist

Before release:

1. Update `package.json` version.
2. Update `CHANGELOG.md`.
3. Update documentation.
4. Run build.
5. Run tests.
6. Validate Unity package import.
7. Validate samples.
8. Build deploy branch contents.
9. Tag release as `vX.Y.Z`.
10. Publish GitHub release.
11. Optional: publish/check OpenUPM readiness.

## Deploy branch

The `deploy` branch should be the minimal Unity Package Manager import surface.

It should contain:

```txt
package.json
README.md
CHANGELOG.md
LICENSE.md
Third Party Notices.md
Runtime/
Editor/
Analyzers/
Tests/
Samples~/
Documentation~/
```

It should not contain unnecessary development-only files unless intentionally included.

## Breaking changes

Breaking changes require:

- CHANGELOG entry.
- Migration guide.
- Analyzer/code fix if possible.
- Major version bump after 1.0.0.
