# Proposal: Package Import Diagnostics (7.2.2)

## Context

Version 7.2.1 has been released and fixes registration exposure API cleanup. During real Glass Refrain integration, Unity/Rider loaded an old NhemDangFugBixs.Attributes assembly version 1.0.0 while the expected package version was 7.2.x. This caused new attributes like AsAttribute and EntryPointAttribute to be missing even though the package docs/version expected them.

## Problem

Unity's PackageCache can become stale, causing:
- Old assembly versions to be loaded despite package.json showing newer version
- Missing attributes/types that should exist in the current version
- Confusion when docs show features that don't appear in the IDE
- Difficult to diagnose without manual inspection of PackageCache and packages-lock.json

## Goal

Add lightweight Unity Editor diagnostics to detect:
- Stale package imports
- Stale PackageCache
- package.json/version mismatch
- Missing analyzer DLLs
- Loaded assembly version mismatch

## Feature

Add Unity Editor menu item:

```
Tools/Nhem/Tooling Diagnostics/Print Diagnostics
```

The menu command should print a clear report to Unity Console.

### Report Contents

- Package path
- package.json version
- Loaded NhemDangFugBixs.Attributes assembly version
- Loaded NhemDangFugBixs.Runtime assembly version
- Whether AsAttribute exists
- Whether EntryPointAttribute exists
- Whether RegisterComponentInHierarchyAttribute exists
- Analyzer DLL path status:
  - Analyzers/NhemDangFugBixs.Generators.dll
  - Analyzers/NhemDangFugBixs.Analyzers.dll
- VContainer dependency status from package.json if easy to read
- Final PASS/WARNING/FAIL summary

### Behavior

- Do not throw exceptions if package.json or assemblies are missing
- Print actionable warnings instead
- Keep this Editor-only
- Do not add runtime reflection scanning
- Do not scan all Unity assemblies repeatedly
- This menu is manual only, not automatic on every compile
- Keep implementation small and safe

## Implementation

- Add Editor script under Editor/
- Use UnityEditor.MenuItem
- Use typeof(AutoRegisterInAttribute).Assembly for Attributes assembly
- Use a known runtime type for Runtime assembly if available
- Use reflection by name only for optional attribute existence checks:
  - NhemDangFugBixs.Attributes.AsAttribute
  - NhemDangFugBixs.Attributes.EntryPointAttribute
  - NhemDangFugBixs.Attributes.RegisterComponentInHierarchyAttribute
- Read package.json from PackageInfo.FindForAssembly or fallback to Packages/com.nhemdangfugbixs.tooling/package.json
- Parse package.json minimally, no external JSON dependency required if the repo avoids it
- If UnityEditor.PackageManager.PackageInfo is available, use it

## Documentation

Add troubleshooting section:

```
"Unity loads old Attributes assembly / Cannot resolve As / EntryPoint"
```

Include fix steps:
1. Close Unity
2. Delete Library/PackageCache/com.nhemdangfugbixs.tooling*
3. Check Packages/packages-lock.json
4. Reopen Unity
5. Regenerate project files

## Tests

- Add editor-safe tests if existing test infrastructure supports it
- If not, add unit tests for pure helper methods:
  - version mismatch detection
  - missing attribute detection
  - report formatting

## Version

- Bump package.json to 7.2.2
- Bump DangFugBixs.Generators.csproj version to 7.2.2 if version drift policy requires it
- Update CHANGELOG.md
- Rebuild shipped DLL payloads if Editor assembly/runtime binaries are affected

## Release Gate

- Generator tests PASS
- Analyzer tests PASS
- Version drift PASS
- Docs check PASS
- Unity sample dotnet build PASS
- Unity sample compile PASS
- release-gate.ps1 PASS

## Non-goals

- Do not build a full DI graph viewer yet
- Do not implement di-smoke yet
- Do not add MessagePipe automation
- Do not add RegisterInstance generation
- Do not add Addressables/prefab/pooling support
