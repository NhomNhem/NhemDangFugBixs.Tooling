# 05 — Unity Package Rules

## Package layout

Follow Unity package layout:

```txt
package.json
README.md
CHANGELOG.md
LICENSE.md
Third Party Notices.md
Runtime/
Editor/
Tests/
Samples~/
Documentation~/
Source~/
```

## Runtime folder

Runtime may contain:

- Attributes.
- Lightweight models.
- Runtime helpers.
- Runtime asmdef.

Runtime must not contain:

- UnityEditor code.
- Roslyn generator implementation.
- CLI implementation.
- Editor windows.

## Editor folder

Editor may contain:

- Diagnostics window.
- Menu items.
- UnityEditor integrations.
- Editor asmdef.

Editor must not be referenced by Runtime.

## Analyzers folder

The Unity package should include prebuilt analyzer/generator DLLs when needed.

Do not place full source generator project directly in Runtime.

## Source~ folder

`Source~` is for development source:

- Generator projects.
- Analyzer projects.
- CLI projects.
- Tests.
- Benchmarks.

Unity should ignore this folder during package import.

## Samples~ folder

Samples should be importable through Unity Package Manager.

Required samples:

```txt
BasicAutoRegister
ScopeMarkerArchitecture
MessagePipeIntegration
SceneComponents
SolarPhobiaStyleArchitecture
```

## Documentation~ folder

Documentation should include:

```txt
index.md
getting-started.md
scope-marker-pattern.md
attributes.md
diagnostics.md
cli.md
editor-window.md
troubleshooting.md
migration-guide.md
```

## package.json

Ensure `package.json` includes:

- name
- version
- displayName
- description
- unity
- author
- documentationUrl if available
- changelogUrl if available
- licensesUrl if available
- dependencies where appropriate

VContainer should usually be treated as a peer dependency documented in README unless the package intentionally depends on it.
