# Skill — Unity Package

## Purpose

Use this skill when changing Unity package structure, asmdefs, samples, Runtime/Editor folders, or UPM deployment.

## Required reading

```txt
AGENTS.md
.codex/rules/05-unity-package.md
.codex/rules/08-release.md
```

## Required package layout

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
Source~/
```

## Runtime rules

Runtime may contain:

```txt
Attributes
Marker types
Small runtime models
Runtime asmdef
```

Runtime must not contain:

```txt
UnityEditor
Generator implementation
Analyzer implementation
CLI code
Editor windows
```

## Editor rules

Editor may contain:

```txt
Diagnostics window
Menu items
Editor-only validation
Report generation UI
```

## Output

A Unity package change should include:

```txt
- Valid asmdef references.
- package.json still valid.
- No Editor reference in Runtime.
- Samples updated if needed.
- Documentation updated if needed.
```

## Validation

Run or reason through:

```txt
- Does package import through Unity Package Manager?
- Does Runtime asmdef compile without Editor?
- Does Editor asmdef compile only in Editor?
- Are Samples~ importable?
- Does deploy branch contain only package surface?
```
