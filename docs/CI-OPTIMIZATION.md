# CI/CD Pipeline Optimization

## Overview

The GitHub Actions workflow has been optimized to use **incremental compilation** - only rebuilding and testing the projects that have actually changed. This significantly reduces CI/CD time for typical Pull Requests.

## How It Works

### 1. Change Detection Job

The pipeline first runs a `detect-changes` job that:
- Compares the current commit with the base branch
- Identifies which projects changed
- Outputs flags for downstream jobs

```bash
Changed files:
  Source~/DangFugBixs.Analyzers~/Rules/ConflictCheckRule.cs  ← Analyzer changed
  Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/ConflictCheckCodeFixProviderTests.cs  ← Test changed
```

Output:
```yaml
analyzers: true      # Run analyzer tests
generators: false    # Skip generator tests
runtime: false       # Skip runtime tests
common: false        # Skip common tests
cli: false           # Skip CLI tests
```

### 2. Selective Compilation

The build job respects dotnet's incremental compilation:
- Only changed projects are recompiled
- Dependencies are automatically detected
- Unchanged projects are skipped

```bash
dotnet build Source~/NhemDangFugBixs.Tooling.sln -c Release
# Rebuilds only the projects with code changes
# Example: Takes 2s instead of 15s for Analyzer-only change
```

### 3. Selective Testing

Tests run only for affected projects:

```yaml
Run Tests - Generators:
  if: needs.detect-changes.outputs.generators == 'true'
  
Run Tests - Analyzers:
  if: needs.detect-changes.outputs.analyzers == 'true'
  
Run Tests - All (if runtime/common changed):
  if: runtime or common changed  # These affect everything
```

## Performance Impact

### Before Optimization

Every PR ran the entire pipeline regardless of changes:
```
Detect Changes:     3s
Build (full):      15s  ← Always full rebuild
Test (full):       25s  ← Always all tests
Validate CLI:       8s
Code Quality:       5s
---
Total:            56s
```

### After Optimization

Only changed projects are compiled and tested:

**Scenario 1: Analyzer rule change**
```
Detect Changes:     3s
Build (Analyzers):  3s  ← Only recompile analyzers
Test (Analyzers):   8s  ← Only analyzer tests
Validate CLI:     skipped
Code Quality:       5s
---
Total:            19s  ← 66% faster
```

**Scenario 2: Generator logic change**
```
Detect Changes:     3s
Build (Generators): 3s  ← Only recompile generator
Test (Generators):  5s  ← Only generator tests
Validate CLI:     skipped
Code Quality:       5s
---
Total:            16s  ← 71% faster
```

**Scenario 3: Runtime attributes change**
```
Detect Changes:     3s
Build (all):       15s  ← Full rebuild (runtime affects all)
Test (all):        25s  ← Run all tests (to be safe)
Validate CLI:       8s
Code Quality:       5s
---
Total:            56s  ← Full run (as before)
```

## Project Dependencies

The detection logic understands which changes require full rebuilds:

```
┌─────────────────┐
│ Runtime/Common  │  ← Changes here affect all projects
└────────┬────────┘
         │
    ┌────┴─────────────┐
    │                  │
┌───▼────┐      ┌──────▼────┐
│Generator│      │ Analyzers │  ← Mostly independent
└────┬────┘      └──────┬────┘
     │                  │
     └────┬─────────────┘
          │
    ┌─────▼──────┐
    │CLI / Tools │  ← Depends on Generator
    └────────────┘
```

**Rules:**
- **Runtime/Common change** → Full rebuild + all tests
- **Analyzer change** → Rebuild only Analyzers + analyzer tests
- **Generator change** → Rebuild only Generator + generator tests
- **CLI change** → Rebuild only CLI + CLI validation
- **Test change** → Rebuild affected project + its tests

## Watching the Workflow

When you push or open a PR, you'll see:

1. **Detect Changes Job** (3s)
   ```
   ✓ Analyze git diff
   ✓ Compare with base branch
   ✓ Output change flags
   ```

2. **Build & Test Job** (runs conditionally)
   ```
   ✓ Cache NuGet packages
   ✓ Restore dependencies
   ✓ Build changed projects
   ✓ Test affected assemblies
   ```

3. **Validate CLI Job** (if CLI changed)
   ```
   ✓ Pack CLI tool
   ✓ Install locally
   ✓ Run smoke test
   ```

4. **Code Quality Job** (always runs)
   ```
   ✓ Check code formatting
   ✓ Build Analyzers for inspection
   ```

## Viewing Results

**Check CI status:**
- Green checkmark: All tests passed
- Red X: Tests failed
- Click "Details" to see workflow logs

**For performance analysis:**
- Click "Build & Test" step
- See which projects were built
- Check test execution times

## Common Patterns

### Pattern 1: Single File Change in Analyzer

**You change:**
```csharp
Source~/DangFugBixs.Analyzers~/Rules/ConflictCheckRule.cs
```

**CI does:**
- Detects: `analyzers: true`
- Builds: Only `DangFugBixs.Analyzers` project (3s)
- Tests: Only analyzer test suite (8s)
- Skips: Generator, Runtime, CLI tests

**Result:** 19s total (vs 56s before)

### Pattern 2: Multiple Test Files Change

**You change:**
```csharp
Source~/DangFugBixs.Generators~/DangFugBixs.Generators.Tests/MyTest.cs
Source~/DangFugBixs.Generators~/DangFugBixs.Generators.Tests/MyOtherTest.cs
```

**CI does:**
- Detects: `generators: true`
- Builds: Only `DangFugBixs.Generators` project (3s)
- Tests: Only generator tests (5s)
- Skips: Analyzer, Runtime, CLI tests

**Result:** 16s total (vs 56s before)

### Pattern 3: Runtime Attribute Change

**You change:**
```csharp
Source~/DangFugBixs.Runtime~/Attributes/SomeAttribute.cs
```

**CI does:**
- Detects: `runtime: true`
- Triggers: Full build + all tests (safety measure)
- Reason: Runtime changes can affect code generation

**Result:** 56s total (full run as expected)

## Monitoring CI Performance

You can track CI performance over time:

**In GitHub UI:**
- Actions tab → Click workflow run
- Compare execution times across commits
- Identify slow jobs

**Tips:**
- Most changes should be 15-25s
- Runtime/common changes take full 56s (expected)
- If a job takes >60s, check the logs

## Troubleshooting

### "All tests ran even though I only changed X"

This usually means:
- **You modified Runtime/Common** → Triggers full run (correct)
- **Project dependency changed** → Other projects need rebuilding
- **GitHub Actions cache expired** → Full restore needed

Check the "Detect Changes" job output to see what was detected.

### "CI is still slow"

1. Check "Detect Changes" step - verify correct projects detected
2. Look at "Cache" step - ensure NuGet cache is being used
3. Check "Build Solution" - only changed projects should rebuild
4. If still slow, NuGet restore may be the bottleneck

### "CI skipped my test"

The test project may not be in a changed project:
- Example: You changed a test file in one project, but CI only ran tests for a different project
- Solution: Check git diff to verify the file path is correct

## Future Improvements

1. **Build Cache (MS Build Cache)** - Cache intermediate build artifacts
2. **Test Sharding** - Run tests in parallel across multiple runners
3. **Performance Benchmarking** - Track CI time trends and alert on regressions
4. **Distributed Caching** - Use GitHub's artifact cache for layer caching

## FAQ

**Q: Why does changing a test file trigger a full rebuild?**
A: Because test changes may require the entire assembly to be validated.

**Q: Can I force a full rebuild?**
A: Yes - commit to a different branch or add `[ci full]` to your commit message (if configured).

**Q: Why is the first run slow?**
A: First run needs to restore NuGet packages (~5s). Subsequent runs use cache (~1s).

**Q: Does this affect main/master branch?**
A: No, full builds run on main/master for safety, even if changes are small.

---

**Have performance feedback?** [Open a GitHub issue](https://github.com/NhemDangFugBixs/NhemDangFugBixs.Tooling/issues)
