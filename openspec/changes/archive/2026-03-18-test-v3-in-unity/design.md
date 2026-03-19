## Context

**Current State:**
- v3.0 type-safe scopes implementation is complete in NhemDangFugBixs.Tooling
- DLLs built and deployed to UPM folders (Runtime/, Analyzers/)
- No real-world testing yet - only sandbox tests
- GameFeelUnity is the target Unity project for integration testing

**Target Project:**
- Path: `i:\unityVers\GameFeelUnity`
- Should have VContainer already installed
- Will be the testbed for v3.0 before public release

**Constraints:**
- Testing should not break existing GameFeelUnity functionality
- Must be reversible if critical issues found
- Test results need to be documented for release decision

## Goals / Non-Goals

**Goals:**
- Deploy v3.0 DLLs to GameFeelUnity project
- Create test LifetimeScopes and services
- Validate type-safe scope registration works in Unity
- Test parent-child scope injection pattern
- Document any bugs or issues found
- Validate migration guide accuracy

**Non-Goals:**
- Modifying GameFeelUnity production code permanently
- Performance testing or benchmarks
- Testing with multiple Unity projects (one is sufficient for now)
- Creating comprehensive test suite in GameFeelUnity

## Decisions

### Decision 1: Deployment Method

**Choice:** Manual DLL copy to Unity project's Assets/Plugins

**Alternatives Considered:**
- UPM package reference via Git URL - more complex, harder to rollback
- Local UPM package - requires package.json setup in Unity project

**Rationale:**
- Simple and fast for testing
- Easy to rollback (just delete folders)
- Matches the documented installation process

### Decision 2: Test Scope Structure

**Choice:** Create minimal test scopes mirroring the sandbox examples

```
GameLifetimeScope (Parent)
├── GameService (Singleton, ITickable)
└── AudioService (Singleton)

GameplayLifetimeScope (Child)
├── EnemySpawner (Scoped, IInitializable)
└── PlayerController (Scoped, ITickable)
```

**Rationale:**
- Covers all major v3.0 features
- Simple enough to debug if issues arise
- Matches documentation examples for validation

### Decision 3: Rollback Strategy

**Choice:** Keep v2.x DLLs as backup

**Plan:**
1. Before deploying v3.0, backup current DLLs
2. If critical issues found, restore v2.x DLLs
3. Document issues and fix in tooling project

## Risks / Trade-offs

| Risk | Impact | Mitigation |
|------|--------|------------|
| **Generator doesn't run in Unity** | High | Check Unity console for errors, verify analyzer settings |
| **DLL version mismatch** | Medium | Ensure all DLLs built from same commit |
| **VContainer API differences** | Medium | Test with same VContainer version used in sandbox |
| **Unity compilation errors** | Medium | Check .meta files, assembly definition references |
| **Parent-child injection fails** | High | Verify LifetimeScope hierarchy setup |
| **Testing blocks other work** | Low | Use separate test scene, not main development scene |

## Migration Plan

### Step 1: Backup (5 min)
```bash
# In GameFeelUnity project
# Backup current DLLs if they exist
```

### Step 2: Deploy v3.0 DLLs (10 min)
```bash
# Copy from tooling to Unity project
copy Runtime\NhemDangFugBixs.Runtime.dll → GameFeelUnity\Assets\Plugins\
copy Analyzers\NhemDangFugBixs.Generators.dll → GameFeelUnity\Assets\Plugins\Analyzers\
copy Analyzers\NhemDangFugBixs.Analyzers.dll → GameFeelUnity\Assets\Plugins\Analyzers\
```

### Step 3: Create Test Scopes (15 min)
- Create GameLifetimeScope
- Create GameplayLifetimeScope
- Setup hierarchy

### Step 4: Create Test Services (20 min)
- Create services with `[AutoRegisterIn(typeof(TScope))]`
- Test parent-child injection
- Test convention-based naming

### Step 5: Validate (15 min)
- Check generated code
- Run Unity scene
- Verify DI works

### Step 6: Document Results (10 min)
- Note any errors or issues
- Capture screenshots if needed
- Update CHANGELOG if fixes needed

## Open Questions

1. **What VContainer version is GameFeelUnity using?**
   - Need to ensure compatibility with generator expectations

2. **Does GameFeelUnity have existing AutoRegister usages?**
   - If yes, need to migrate or test side-by-side

3. **Unity version?**
   - Should be 2021.3+ for Roslyn Analyzer support
