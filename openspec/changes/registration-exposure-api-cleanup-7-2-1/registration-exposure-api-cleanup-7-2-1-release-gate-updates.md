# Release Gate Updates: Registration Exposure API Cleanup 7.2.1

## Overview

This document describes the updates to the release gate (release-gate.ps1) for version 7.2.1.

## Current Release Gate

### Existing Checks
- Generator tests must pass
- Analyzer tests must pass
- Version drift check must pass
- Docs check must pass
- Unity sample dotnet build (if NHEM_UNITY_PROJECT_ROOT is set)
- Unity sample compile (if UNITY_EXE is set)
- release-gate.ps1 must fail if Unity returns non-zero

## New Checks for 7.2.1

### RG-7.2.1-1: New Generator Tests
- **Purpose**: Verify new generator behavior for explicit attributes
- **Implementation**: Run new generator tests from test matrix
- **Execution**: ~10 seconds
- **Condition**: Always runs
- **Failure Impact**: Block release

**Tests**:
- Explicit [As] only generates one .As<TContract>()
- Explicit [As] plus legacy AsImplementedInterfaces=true does not duplicate .As<TContract>()
- Explicit [AsSelf] plus legacy AsSelf=true does not duplicate self registration
- Legacy flag-only behavior still works
- Contract output is deterministic and sorted
- Composition-only service assemblies still emit no VContainer code

### RG-7.2.1-2: New Analyzer Tests
- **Purpose**: Verify new NHEM_DI_060 diagnostic behavior
- **Implementation**: Run new analyzer tests from test matrix
- **Execution**: ~10 seconds
- **Condition**: Always runs
- **Failure Impact**: Block release

**Tests**:
- NHEM_DI_060 is emitted for mixed explicit + legacy exposure style
- NHEM_DI_060 is not emitted for pure explicit style
- NHEM_DI_060 is not emitted for pure legacy style
- NHEM_DI_060 is not emitted when AutoRegisterIn has only scope and lifetime

### RG-7.2.1-3: Documentation Validation
- **Purpose**: Verify all documentation updates are complete
- **Implementation**: Check for required documentation changes
- **Execution**: ~5 seconds
- **Condition**: Always runs
- **Failure Impact**: Block release

**Checks**:
- README.md canonical usage updated
- README.md migration guide section added
- AutoRegisterInAttribute XML docs updated
- CHANGELOG.md 7.2.1 section added

### RG-7.2.1-4: Version Validation
- **Purpose**: Verify all version numbers are consistent
- **Implementation**: Compare package.json, project files, and assembly versions
- **Execution**: ~5 seconds
- **Condition**: Always runs
- **Failure Impact**: Block release

**Checks**:
- package.json version is 7.2.1
- DangFugBixs.Generators.csproj version is 7.2.1
- DangFugBixs.Analyzers.csproj version is 7.2.1
- DangFugBixs.Attributes.csproj version is 7.2.1

### RG-7.2.1-5: DLL Payload Validation
- **Purpose**: Verify DLL payloads are rebuilt with correct version
- **Implementation**: Check DLL file versions
- **Execution**: ~5 seconds
- **Condition**: Always runs
- **Failure Impact**: Block release

**Checks**:
- Runtime/NhemDangFugBixs.Attributes.dll version is 7.2.1
- Runtime/NhemDangFugBixs.Runtime.dll version is 7.2.1
- Analyzers/NhemDangFugBixs.Generators.dll version is 7.2.1
- Analyzers/NhemDangFugBixs.Analyzers.dll version is 7.2.1

## Modified Checks

### Generator Tests
- **Change**: Now includes 6 new tests for explicit attribute behavior
- **Execution Time**: Increased by ~10 seconds
- **Impact**: Minimal

### Analyzer Tests
- **Change**: Now includes 4 new tests for NHEM_DI_060 diagnostic
- **Execution Time**: Increased by ~10 seconds
- **Impact**: Minimal

### Version Drift Check
- **Change**: Now validates 4 project files instead of 2
- **Execution Time**: No significant change
- **Impact**: Minimal

### Docs Check
- **Change**: Now validates README.md, AutoRegisterInAttribute.cs, and CHANGELOG.md
- **Execution Time**: Increased by ~5 seconds
- **Impact**: Minimal

## Performance Impact

### Additional Time
- New generator tests: ~10 seconds
- New analyzer tests: ~10 seconds
- Documentation validation: ~5 seconds
- Version validation: ~5 seconds
- DLL payload validation: ~5 seconds
- **Total Additional Time**: ~35 seconds

### Total Release Gate Time
- **Initial Time**: ~2-3 minutes
- **Final Time**: ~2.5-3.5 minutes
- **Increase**: ~30-35 seconds
- **Impact**: Minimal

## Release Gate Script Changes

### Add New Test Sections

```powershell
# Add after existing generator tests
Write-Host "Running new generator tests for 7.2.1..."
Test-GeneratorExplicitAttributes
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

# Add after existing analyzer tests
Write-Host "Running new analyzer tests for 7.2.1..."
Test-AnalyzerNHEMDI060
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

# Add after existing docs check
Write-Host "Validating documentation updates..."
Test-DocumentationUpdates
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

# Add after existing version drift check
Write-Host "Validating version numbers..."
Test-VersionNumbers
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

# Add after existing checks
Write-Host "Validating DLL payload versions..."
Test-DLLPayloads
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
```

## Test Implementation Details

### Test-GeneratorExplicitAttributes
```powershell
function Test-GeneratorExplicitAttributes {
    dotnet test Source~/DangFugBixs.Generators~/DangFugBixs.Tests/DangFugBixs.Tests.csproj `
        --filter "FullyQualifiedName~Explicit" `
        --logger "console;verbosity=detailed"
    return $LASTEXITCODE
}
```

### Test-AnalyzerNHEMDI060
```powershell
function Test-AnalyzerNHEMDI060 {
    dotnet test Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers.Tests/DangFugBixs.Analyzers.Tests.csproj `
        --filter "FullyQualifiedName~NHEMDI060" `
        --logger "console;verbosity=detailed"
    return $LASTEXITCODE
}
```

### Test-DocumentationUpdates
```powershell
function Test-DocumentationUpdates {
    # Check README.md for migration guide section
    if (-not (Select-String -Path "README.md" -Pattern "Migration Guide for 7.2.1")) {
        Write-Error "README.md missing migration guide section"
        return 1
    }
    
    # Check AutoRegisterInAttribute.cs for canonical usage
    if (-not (Select-String -Path "Source~/DangFugBixs.Attributes~/Attributes/AutoRegisterInAttribute.cs" -Pattern "Canonical usage")) {
        Write-Error "AutoRegisterInAttribute.cs missing canonical usage documentation"
        return 1
    }
    
    # Check CHANGELOG.md for 7.2.1 section
    if (-not (Select-String -Path "CHANGELOG.md" -Pattern "## 7.2.1")) {
        Write-Error "CHANGELOG.md missing 7.2.1 section"
        return 1
    }
    
    return 0
}
```

### Test-VersionNumbers
```powershell
function Test-VersionNumbers {
    $packageJson = Get-Content "package.json" | ConvertFrom-Json
    if ($packageJson.version -ne "7.2.1") {
        Write-Error "package.json version is not 7.2.1"
        return 1
    }
    
    # Check project files
    $projectFiles = @(
        "Source~/DangFugBixs.Generators~/DangFugBixs.Generators/DangFugBixs.Generators.csproj",
        "Source~/DangFugBixs.Analyzers~/DangFugBixs.Analyzers/DangFugBixs.Analyzers.csproj",
        "Source~/DangFugBixs.Attributes~/DangFugBixs.Attributes/DangFugBixs.Attributes.csproj"
    )
    
    foreach ($projectFile in $projectFiles) {
        if (-not (Select-String -Path $projectFile -Pattern "7.2.1")) {
            Write-Error "$projectFile version is not 7.2.1"
            return 1
        }
    }
    
    return 0
}
```

### Test-DLLPayloads
```powershell
function Test-DLLPayloads {
    $dllFiles = @(
        "Runtime/NhemDangFugBixs.Attributes.dll",
        "Runtime/NhemDangFugBixs.Runtime.dll",
        "Analyzers/NhemDangFugBixs.Generators.dll",
        "Analyzers/NhemDangFugBixs.Analyzers.dll"
    )
    
    foreach ($dllFile in $dllFiles) {
        if (-not (Test-Path $dllFile)) {
            Write-Error "$dllFile not found"
            return 1
        }
        
        $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dllFile)
        if ($versionInfo.FileVersion -ne "7.2.1.0") {
            Write-Error "$dllFile version is not 7.2.1.0"
            return 1
        }
    }
    
    return 0
}
```

## Rollback Plan

If release gate fails:

1. **Identify Failure**
   - Check which check failed
   - Review error message
   - Determine root cause

2. **Fix Issue**
   - Fix code or documentation
   - Rebuild DLLs if needed
   - Update version numbers if needed

3. **Re-run Release Gate**
   - Run release-gate.ps1 again
   - Verify all checks pass
   - Proceed with release

## Validation

### Manual Validation Steps

Before running release gate:
1. Verify all acceptance criteria are met
2. Verify all tests pass locally
3. Verify documentation is complete
4. Verify version numbers are correct
5. Verify DLLs are rebuilt

### Automated Validation Steps

Release gate automatically:
1. Runs all generator tests
2. Runs all analyzer tests
3. Validates documentation updates
4. Validates version numbers
5. Validates DLL payloads
6. Runs Unity sample build (if available)
7. Runs Unity sample compile (if available)

## Success Criteria

Release gate passes when:
- All generator tests pass
- All analyzer tests pass
- Version drift check passes
- Documentation validation passes
- Version validation passes
- DLL payload validation passes
- Unity sample build passes (if available)
- Unity sample compile passes (if available)
