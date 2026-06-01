# Proposal: Recover Analyzer Source Layout

## Context

Version 7.3.0 analyzer maturity implementation is blocked because the current repository working tree contains only compiled analyzer DLL payloads under `Analyzers/`, but no analyzer source code or analyzer tests.

### Current observed tree

```
Analyzers/
├── NhemDangFugBixs.Analyzers.dll
├── NhemDangFugBixs.Analyzers.dll.meta
├── NhemDangFugBixs.Generators.dll
└── NhemDangFugBixs.Generators.dll.meta

Source~/
├── .idea/
├── DangFugBixs.Generators~/
│   └── DangFugBixs.Sandbox/
└── nhemdangfugbixs-tooling-docs/

Missing:
- Source~/DangFugBixs.Analyzers~/
- DiagnosticIds.cs
- DiagnosticCatalog.cs
- AttributeContractAnalyzer.cs
- OpenSpecAnalyzerMvpTests.cs
```

## Problem

The analyzer source code is not present in the current working tree, making it impossible to:
- Implement new diagnostics (NHEM_DI_061, NHEM_DI_066, NHEM_DI_067)
- Run analyzer tests
- Rebuild analyzer DLL payloads
- Maintain analyzer diagnostics

The compiled DLL files in `Analyzers/` are binary-only payloads with no source-of-truth.

## Goal

Recover or document the canonical analyzer source layout before 7.3.0 can proceed.

## Scope

### Tasks

1. **Search git history for analyzer source**
   - AttributeContractAnalyzer.cs
   - DiagnosticIds.cs
   - DiagnosticCatalog.cs
   - NHEM_DI_060
   - OpenSpecAnalyzerMvpTests.cs
   - DangFugBixs.Analyzers~

2. **Search all branches and tags for analyzer source**
   - Check all local branches
   - Check all remote branches
   - Check all tags (v7.0.0 through v7.2.2)

3. **Check for analyzer source in other locations**
   - Other local clones
   - Submodules
   - Private repositories
   - Archived branches

4. **If source is found**
   - Restore it under canonical Source~ layout
   - Restore analyzer test project
   - Document build command
   - Document how DLL payloads are rebuilt from source

5. **If source is not found**
   - Document that Analyzers/*.dll are binary-only payloads
   - Block analyzer maturity roadmap until source is restored
   - Add repository health warning

## Acceptance Criteria

- The repository has a documented source-of-truth for analyzer source.
- There is a repeatable command to build analyzer DLLs.
- There is a repeatable command to run analyzer tests.
- 7.3.0 analyzer maturity remains blocked until this is satisfied.

## Non-goals

- Do not implement new diagnostics yet.
- Do not edit Analyzers/*.dll directly.
- Do not bump package version.
- Do not tag a release.
