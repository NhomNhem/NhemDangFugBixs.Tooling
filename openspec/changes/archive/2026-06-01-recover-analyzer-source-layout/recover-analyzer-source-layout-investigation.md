# Investigation: Analyzer Source Recovery

## Search Strategy

### 1. Git History Search

Search for analyzer source files in git history:

```bash
git log --all --full-history --diff-filter=D -- "Source~/DangFugBixs.Analyzers~"
git log --all --full-history --diff-filter=D -- "*AttributeContractAnalyzer.cs"
git log --all --full-history --diff-filter=D -- "*DiagnosticIds.cs"
git log --all --full-history --diff-filter=D -- "*DiagnosticCatalog.cs"
git log --all --full-history --diff-filter=D -- "*OpenSpecAnalyzerMvpTests.cs"
git log --all --full-history -- "*NHEM_DI_060*"
```

### 2. Branch and Tag Search

Check all branches and tags:

```bash
git branch -a | grep -i analyzer
git tag | sort -V
git log --all --oneline | grep -i "analyzer\|diagnostic"
```

### 3. Remote Search

Check remote repository for analyzer source:

```bash
git ls-remote origin | grep analyzer
git ls-remote origin | grep diagnostic
```

### 4. Submodule Check

Check for submodules:

```bash
git config --file .gitmodules --name-only --get-regexp path
cat .gitmodules
```

### 5. File Existence Check

Verify current state:

```bash
ls -la Analyzers/
ls -la Source~/
find . -name "*Analyzer*.cs" -o -name "*Diagnostic*.cs"
```

## Expected Outcomes

### Scenario A: Source Found in History

If analyzer source is found in git history:
- Restore from specific commit
- Document commit hash
- Document recovery command
- Verify test suite is present
- Document build process

### Scenario B: Source Found in Branch

If analyzer source is found in a branch:
- Document branch name
- Document merge/rebase strategy
- Verify test suite is present
- Document build process

### Scenario C: Source Found in Other Location

If analyzer source is in another clone or repository:
- Document location
- Document synchronization process
- Document build process
- Consider adding as submodule

### Scenario D: Source Not Found

If analyzer source cannot be found:
- Document that Analyzers/*.dll are binary-only
- Block 7.3.0 implementation
- Add repository health warning
- Recommend source reconstruction or external source location

## Investigation Results

### Files Found

- [x] AttributeContractAnalyzer.cs
- [x] DiagnosticIds.cs
- [x] DiagnosticCatalog.cs
- [x] OpenSpecAnalyzerMvpTests.cs
- [x] DangFugBixs.Analyzers.csproj
- [x] DangFugBixs.Analyzers.Tests.csproj

### Location

All analyzer source files were found in tag `v7.1.0` (commit `9a82a26d`).

### Recovery Command

```bash
git checkout v7.1.0 -- "Source~/DangFugBixs.Analyzers~"
```

### Build Command

```bash
dotnet build "Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/DangFugBixs.Analyzers.csproj" -c Release
```

### Test Command

```bash
dotnet test "Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/DangFugBixs.Analyzers.Tests.csproj" -c Release
```

### Recovery Steps

1. Identified analyzer source in git history at tag v7.1.0
2. Restored files using `git checkout v7.1.0 -- "Source~/DangFugBixs.Analyzers~"`
3. Verified all expected files are present
4. Implemented new diagnostics for 7.3.0:
   - NHEM_DI_061: DuplicateContractExposure
   - NHEM_DI_066: RegisterComponentInHierarchyOnNonMonoBehaviour
   - NHEM_DI_067: EntryPointWithoutLifecycleInterface

## Recommendations

1. The analyzer source should be kept in the working tree going forward
2. Consider adding a CI check to ensure analyzer source is present
3. Document the recovery process in CONTRIBUTING.md
4. Proceed with 7.3.0 implementation now that source is restored
