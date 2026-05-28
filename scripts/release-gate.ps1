[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
$generatorTests = Join-Path $repoRoot "Source~\DangFugBixs.Generators~\DangFugBixs.Tests\DangFugBixs.Tests.csproj"
$analyzerTests = Join-Path $repoRoot "Source~\DangFugBixs.Analyzers~\DangFugBixs.Analyzers.Tests\DangFugBixs.Analyzers.Tests.csproj"
$packageJsonPath = Join-Path $repoRoot "package.json"
$generatorProjectPath = Join-Path $repoRoot "Source~\DangFugBixs.Generators~\DangFugBixs.Generators\DangFugBixs.Generators.csproj"
$docsRoot = Join-Path $repoRoot "Source~\nhemdangfugbixs-tooling-docs"
$artifactsRoot = Join-Path $repoRoot "artifacts"
$unityLogPath = Join-Path $artifactsRoot "unity-sample-compile.log"
$unityStdOutPath = Join-Path $artifactsRoot "unity-sample-stdout.log"
$unityStdErrPath = Join-Path $artifactsRoot "unity-sample-stderr.log"
$unityProjectRoot = if ($env:NHEM_UNITY_PROJECT_ROOT) { $env:NHEM_UNITY_PROJECT_ROOT } else { $null }
$unitySampleSolutionPath = $null
$unityCompileDuration = $null
$unityExitCode = $null
$unityFailureReason = $null

$summary = [ordered]@{
    Restore = "PENDING"
    GeneratorTests = "PENDING"
    AnalyzerTests = "PENDING"
    VersionDrift = "PENDING"
    DocsCheck = "PENDING"
    UnitySampleDotnetBuild = "PENDING"
    UnitySampleCompile = "PENDING"
    CrossAsmdefValidation = "PENDING"
}

function Find-UnityFailureText {
    param([string]$Content)

    $patterns = @(
        "Aborting batchmode due to fatal error",
        "another Unity instance is running with this project open",
        "error CS\d{4}",
        "Compilation failed",
        "BuildFailedException",
        "Exiting without the bug reporter\. Application will terminate with return code 1"
    )

    foreach ($pattern in $patterns) {
        if ($Content -match $pattern) {
            return $matches[0]
        }
    }

    return $null
}

function Get-UnityLogSummary {
    param([string[]]$Paths)

    foreach ($path in $Paths) {
        if (-not [string]::IsNullOrWhiteSpace($path) -and (Test-Path -LiteralPath $path)) {
            $content = Get-Content -LiteralPath $path -Raw
            $match = Find-UnityFailureText -Content $content
            if ($match) {
                return "$match [$path]"
            }
        }
    }

    return $null
}

function Write-Section {
    param([string]$Message)
    Write-Host ""
    Write-Host "== $Message ==" -ForegroundColor Cyan
}

function Invoke-Step {
    param(
        [string]$Label,
        [scriptblock]$Action
    )

    Write-Section $Label
    & $Action
    Write-Host "${Label}: OK" -ForegroundColor Green
}

function Get-XmlProjectVersion {
    param([string]$Path)

    [xml]$projectXml = Get-Content -LiteralPath $Path
    $versionNode = $projectXml.Project.PropertyGroup.Version | Select-Object -First 1
    if (-not $versionNode) {
        throw "Missing <Version> in $Path"
    }

    return [string]$versionNode
}

function Write-Summary {
    Write-Host ""
    Write-Host "Release Gate Summary" -ForegroundColor Yellow
    Write-Host "--------------------"
    foreach ($entry in $summary.GetEnumerator()) {
        Write-Host ("{0,-20} {1}" -f $entry.Key, $entry.Value)
    }
}

function Test-TruthyEnvironmentValue {
    param([string]$Name)

    $value = [System.Environment]::GetEnvironmentVariable($Name)
    if ([string]::IsNullOrWhiteSpace($value)) {
        return $false
    }

    switch ($value.Trim().ToLowerInvariant()) {
        '1' { return $true }
        'true' { return $true }
        'yes' { return $true }
        'y' { return $true }
        default { return $false }
    }
}

function Resolve-UnitySampleSolutionPath {
    param([string]$ProjectRoot)

    if ([string]::IsNullOrWhiteSpace($ProjectRoot)) {
        return $null
    }

    $solution = Get-ChildItem -LiteralPath $ProjectRoot -Filter "*.sln" -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -eq $solution) {
        return $null
    }

    return $solution.FullName
}

function Get-UnityProjectVersion {
    param([string]$ProjectRoot)

    if ([string]::IsNullOrWhiteSpace($ProjectRoot)) {
        return $null
    }

    $projectVersionPath = Join-Path $ProjectRoot "ProjectSettings\ProjectVersion.txt"
    if (-not (Test-Path -LiteralPath $projectVersionPath)) {
        return $null
    }

    foreach ($line in Get-Content -LiteralPath $projectVersionPath) {
        if ($line -match '^m_EditorVersion:\s*(.+)$') {
            return [pscustomobject]@{
                Version = $matches[1].Trim()
                Source = "ProjectVersion.txt"
                Path = $projectVersionPath
            }
        }
    }

    throw "Unable to parse m_EditorVersion from $projectVersionPath"
}

function Get-RequestedUnityVersion {
    param([string]$ProjectRoot)

    $explicitVersion = [System.Environment]::GetEnvironmentVariable("NHEM_UNITY_VERSION")
    if (-not [string]::IsNullOrWhiteSpace($explicitVersion)) {
        return [pscustomobject]@{
            Version = $explicitVersion.Trim()
            Source = "NHEM_UNITY_VERSION"
            Path = $null
        }
    }

    $projectVersion = Get-UnityProjectVersion -ProjectRoot $ProjectRoot
    if ($null -ne $projectVersion) {
        return $projectVersion
    }

    return [pscustomobject]@{
        Version = $null
        Source = $null
        Path = $null
    }
}

function Get-UnitySearchRoots {
    return @(
        "C:\Program Files\Unity\Hub\Editor",
        "D:\Program Files\Unity\Hub\Editor",
        "E:\Program Files\Unity\Hub\Editor",
        "C:\Unity\Hub\Editor",
        "D:\Unity\Hub\Editor",
        "E:\Unity\Hub\Editor",
        "I:\",
        "J:\"
    )
}

function Get-UnityExeCandidates {
    param(
        [string]$Version,
        [switch]$IncludeAllVersions
    )

    $searchRoots = Get-UnitySearchRoots
    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $results = [System.Collections.Generic.List[string]]::new()

    foreach ($root in $searchRoots) {
        if (-not (Test-Path -LiteralPath $root)) {
            continue
        }

        foreach ($match in Get-ChildItem -LiteralPath $root -Filter "Unity.exe" -Recurse -File -ErrorAction SilentlyContinue) {
            if (-not $IncludeAllVersions) {
                if ([string]::IsNullOrWhiteSpace($Version)) {
                    continue
                }

                if ($match.FullName -notlike "*$Version*") {
                    continue
                }
            }

            if ($seen.Add($match.FullName)) {
                $results.Add($match.FullName)
            }
        }
    }

    return $results
}

function Resolve-UnityExecutableSelection {
    param(
        [string]$UnityExe,
        [string]$RequestedVersion,
        [string]$RequestedVersionSource,
        [bool]$AllowFallback
    )

    if (-not [string]::IsNullOrWhiteSpace($UnityExe)) {
        if (Test-Path -LiteralPath $UnityExe) {
            return [pscustomobject]@{
                RequestedVersion = $RequestedVersion
                RequestedVersionSource = $RequestedVersionSource
                SelectedPath = (Resolve-Path -LiteralPath $UnityExe).Path
                SelectionSource = "UNITY_EXE"
            }
        }

        Write-Warning "UNITY_EXE is set but invalid: $UnityExe. Continuing with version-based selection."
    }

    if (-not [string]::IsNullOrWhiteSpace($RequestedVersion)) {
        $preferredPaths = @(
            "C:\Program Files\Unity\Hub\Editor\$RequestedVersion\Editor\Unity.exe",
            "D:\Program Files\Unity\Hub\Editor\$RequestedVersion\Editor\Unity.exe",
            "E:\Program Files\Unity\Hub\Editor\$RequestedVersion\Editor\Unity.exe",
            "C:\Unity\Hub\Editor\$RequestedVersion\Editor\Unity.exe",
            "D:\Unity\Hub\Editor\$RequestedVersion\Editor\Unity.exe",
            "E:\Unity\Hub\Editor\$RequestedVersion\Editor\Unity.exe",
            "I:\$RequestedVersion\Editor\Unity.exe",
            "J:\$RequestedVersion\Editor\Unity.exe"
        )

        foreach ($preferredPath in $preferredPaths) {
            if (Test-Path -LiteralPath $preferredPath) {
                return [pscustomobject]@{
                    RequestedVersion = $RequestedVersion
                    RequestedVersionSource = $RequestedVersionSource
                    SelectedPath = (Resolve-Path -LiteralPath $preferredPath).Path
                    SelectionSource = $RequestedVersionSource
                }
            }
        }

        $versionCandidates = Get-UnityExeCandidates -Version $RequestedVersion
        if ($versionCandidates.Count -gt 0) {
            $selectedVersionCandidate = $versionCandidates | Sort-Object | Select-Object -First 1
            return [pscustomobject]@{
                RequestedVersion = $RequestedVersion
                RequestedVersionSource = $RequestedVersionSource
                SelectedPath = $selectedVersionCandidate
                SelectionSource = $RequestedVersionSource
            }
        }
    }

    if ($AllowFallback) {
        $fallbackCandidates = Get-UnityExeCandidates -Version $null -IncludeAllVersions
        if ($fallbackCandidates.Count -gt 0) {
            $selectedFallback = $fallbackCandidates | Sort-Object | Select-Object -First 1
            return [pscustomobject]@{
                RequestedVersion = $RequestedVersion
                RequestedVersionSource = $RequestedVersionSource
                SelectedPath = $selectedFallback
                SelectionSource = "fallback"
            }
        }
    }

    return [pscustomobject]@{
        RequestedVersion = $RequestedVersion
        RequestedVersionSource = $RequestedVersionSource
        SelectedPath = $null
        SelectionSource = $null
    }
}

try {
    Invoke-Step "Restore" {
        dotnet restore $generatorTests
        dotnet restore $analyzerTests
    }
    $summary.Restore = "PASS"

    Invoke-Step "Generator tests" {
        dotnet test $generatorTests --no-restore
    }
    $summary.GeneratorTests = "PASS"

    Invoke-Step "Analyzer tests" {
        dotnet test $analyzerTests --no-restore
    }
    $summary.AnalyzerTests = "PASS"

    Invoke-Step "Version drift check" {
        $packageVersion = (Get-Content -LiteralPath $packageJsonPath -Raw | ConvertFrom-Json).version
        $generatorVersion = Get-XmlProjectVersion -Path $generatorProjectPath

        if ([string]::IsNullOrWhiteSpace($packageVersion)) {
            throw "package.json version is missing."
        }

        if ($packageVersion -ne $generatorVersion) {
            throw "Version drift detected. package.json=$packageVersion generator.csproj=$generatorVersion"
        }
    }
    $summary.VersionDrift = "PASS"

    if ((Test-Path -LiteralPath $docsRoot) -and (Get-Command pnpm -ErrorAction SilentlyContinue)) {
        Invoke-Step "Docs check" {
            pnpm --dir $docsRoot build
        }
        $summary.DocsCheck = "PASS"
    } else {
        $summary.DocsCheck = "SKIPPED"
        Write-Host ""
        Write-Host "== Docs check ==" -ForegroundColor Cyan
        Write-Host "Docs check: SKIPPED (pnpm not available or docs directory missing)" -ForegroundColor Yellow
    }

    $unitySampleSolutionPath = Resolve-UnitySampleSolutionPath -ProjectRoot $unityProjectRoot
    if (-not [string]::IsNullOrWhiteSpace($unityProjectRoot) -and -not [string]::IsNullOrWhiteSpace($unitySampleSolutionPath)) {
        Invoke-Step "Unity sample dotnet build" {
            dotnet build $unitySampleSolutionPath --nologo
        }
        $summary.UnitySampleDotnetBuild = "PASS"
    } elseif (-not [string]::IsNullOrWhiteSpace($unityProjectRoot)) {
        $summary.UnitySampleDotnetBuild = "SKIPPED"
        Write-Host ""
        Write-Host "== Unity sample dotnet build ==" -ForegroundColor Cyan
        Write-Host "Unity sample dotnet build: SKIPPED (no .sln found under NHEM_UNITY_PROJECT_ROOT)" -ForegroundColor Yellow
    } else {
        $summary.UnitySampleDotnetBuild = "SKIPPED"
        Write-Host ""
        Write-Host "== Unity sample dotnet build ==" -ForegroundColor Cyan
        Write-Host "Unity sample dotnet build: SKIPPED (NHEM_UNITY_PROJECT_ROOT not set)" -ForegroundColor Yellow
    }

    $requestedUnity = Get-RequestedUnityVersion -ProjectRoot $unityProjectRoot
    $unitySelection = Resolve-UnityExecutableSelection `
        -UnityExe $env:UNITY_EXE `
        -RequestedVersion $requestedUnity.Version `
        -RequestedVersionSource $requestedUnity.Source `
        -AllowFallback (Test-TruthyEnvironmentValue -Name "NHEM_ALLOW_UNITY_VERSION_FALLBACK")

    Write-Host ""
    Write-Host "== Unity selection ==" -ForegroundColor Cyan
    Write-Host ("Requested Unity version: {0}" -f $(if ($unitySelection.RequestedVersion) { $unitySelection.RequestedVersion } else { "<none>" }))
    Write-Host ("Requested Unity source: {0}" -f $(if ($unitySelection.RequestedVersionSource) { $unitySelection.RequestedVersionSource } else { "<none>" }))
    Write-Host ("Selected Unity executable: {0}" -f $(if ($unitySelection.SelectedPath) { $unitySelection.SelectedPath } else { "<none>" }))
    Write-Host ("Selection source: {0}" -f $(if ($unitySelection.SelectionSource) { $unitySelection.SelectionSource } else { "<none>" }))

    $diSmokeProject = Join-Path $repoRoot "Source~\DangFugBixs.Tools~\DangFugBixs.DiSmokeValidation\DangFugBixs.DiSmokeValidation.csproj"

    if (-not $unitySelection.SelectedPath) {
        $ciUnityRequired = Test-TruthyEnvironmentValue -Name "CI_UNITY_REQUIRED"
        if ($ciUnityRequired) {
            throw "No matching Unity.exe was found and CI_UNITY_REQUIRED=true."
        }

        Write-Host ""
        Write-Host "== Unity sample compile ==" -ForegroundColor Cyan
        Write-Host "Unity sample compile: SKIPPED (no matching Unity.exe found. Set CI_UNITY_REQUIRED=true to fail.)" -ForegroundColor Yellow
        $summary.UnitySampleCompile = "SKIPPED"
    } else {
        if (-not (Test-Path -LiteralPath $unityProjectRoot)) {
            $summary.UnitySampleCompile = "SKIPPED"
            Write-Host ""
            Write-Host "== Unity sample compile ==" -ForegroundColor Cyan
            Write-Host "Unity sample compile: SKIPPED (NHEM_UNITY_PROJECT_ROOT path does not exist)" -ForegroundColor Yellow
        } else {
            Invoke-Step "Unity sample compile" {
                New-Item -ItemType Directory -Force -Path $artifactsRoot | Out-Null
                if (Test-Path -LiteralPath $unityLogPath) {
                    Remove-Item -LiteralPath $unityLogPath -Force
                }
                if (Test-Path -LiteralPath $unityStdOutPath) {
                    Remove-Item -LiteralPath $unityStdOutPath -Force
                }
                if (Test-Path -LiteralPath $unityStdErrPath) {
                    Remove-Item -LiteralPath $unityStdErrPath -Force
                }

                $timer = [System.Diagnostics.Stopwatch]::StartNew()
                $process = Start-Process `
                    -FilePath $unitySelection.SelectedPath `
                    -ArgumentList @(
                        "-batchmode",
                        "-nographics",
                        "-accept-apiupdate",
                        "-quit",
                        "-projectPath", $unityProjectRoot,
                        "-logFile", $unityLogPath
                    ) `
                    -NoNewWindow `
                    -Wait `
                    -PassThru `
                    -RedirectStandardOutput $unityStdOutPath `
                    -RedirectStandardError $unityStdErrPath
                $timer.Stop()
                $script:unityCompileDuration = $timer.Elapsed
                $script:unityExitCode = $process.ExitCode

                Start-Sleep -Milliseconds 500
                $script:unityFailureReason = Get-UnityLogSummary -Paths @($unityStdOutPath, $unityStdErrPath, $unityLogPath)

                if ($script:unityExitCode -ne 0) {
                    throw "Unity sample compile failed with exit code $script:unityExitCode. Log: $unityLogPath"
                }

                if ($script:unityFailureReason) {
                    throw "Unity sample compile failed: $script:unityFailureReason. Log: $unityLogPath"
                }
            }
            if ($unityCompileDuration -ne $null) {
                $summary.UnitySampleCompile = ("PASS ({0:n1}s, exit={1}, log={2})" -f $unityCompileDuration.TotalSeconds, $unityExitCode, $unityLogPath)
            } else {
                $summary.UnitySampleCompile = ("PASS (exit={0}, log={1})" -f $unityExitCode, $unityLogPath)
            }
        }
    }

    # Cross-asmdef composition validation for composition-only generation (Tasks 6.1-6.3)
    if ((Test-Path -LiteralPath $diSmokeProject) -and (-not [string]::IsNullOrWhiteSpace($unityProjectRoot))) {
        Invoke-Step "Cross-asmdef validation (di-smoke)" {
            $asmPaths = Get-ChildItem -LiteralPath $unityProjectRoot -Filter "*.dll" -Recurse -File -ErrorAction SilentlyContinue |
                Where-Object { $_.FullName -like "*Library\ScriptAssemblies*" } |
                Select-Object -ExpandProperty FullName

            if ($asmPaths.Count -gt 0) {
                $diSmokeExe = Join-Path $repoRoot "Source~\DangFugBixs.Tools~\DangFugBixs.DiSmokeValidation\bin\Debug\net10.0\DangFugBixs.DiSmokeValidation.dll"
                if (-not (Test-Path -LiteralPath $diSmokeExe)) {
                    dotnet build $diSmokeProject --nologo | Out-Null
                }

                $argsList = @($diSmokeExe) + $asmPaths
                $result = & dotnet $argsList 2>&1
                Write-Host $result
            } else {
                Write-Host "No ScriptAssemblies found. Skipping cross-asmdef validation." -ForegroundColor Yellow
            }
        }
        $summary.CrossAsmdefValidation = "PASS"
    } else {
        $summary.CrossAsmdefValidation = "SKIPPED"
        Write-Host ""
        Write-Host "== Cross-asmdef validation (di-smoke) ==" -ForegroundColor Cyan
        Write-Host "Cross-asmdef validation: SKIPPED (di-smoke project or Unity project root not found)" -ForegroundColor Yellow
    }

    Write-Summary
    exit 0
}
catch {
    $failedMessage = $_.Exception.Message

    if ($summary.Restore -eq "PENDING") { $summary.Restore = "FAIL" }
    elseif ($summary.GeneratorTests -eq "PENDING") { $summary.GeneratorTests = "FAIL" }
    elseif ($summary.AnalyzerTests -eq "PENDING") { $summary.AnalyzerTests = "FAIL" }
    elseif ($summary.VersionDrift -eq "PENDING") { $summary.VersionDrift = "FAIL" }
    elseif ($summary.DocsCheck -eq "PENDING") { $summary.DocsCheck = "FAIL" }
    elseif ($summary.UnitySampleDotnetBuild -eq "PENDING") { $summary.UnitySampleDotnetBuild = "FAIL" }
    elseif ($summary.UnitySampleCompile -eq "PENDING") {
        if ($unityExitCode -ne $null) {
            $summary.UnitySampleCompile = ("FAIL (exit={0}, log={1})" -f $unityExitCode, $unityLogPath)
        } elseif (Test-Path -LiteralPath $unityLogPath) {
            $summary.UnitySampleCompile = ("FAIL (log={0})" -f $unityLogPath)
        } else {
            $summary.UnitySampleCompile = "FAIL"
        }
    }
    elseif ($summary.CrossAsmdefValidation -eq "PENDING") { $summary.CrossAsmdefValidation = "FAIL" }

    if ($summary.UnitySampleCompile -eq "FAIL" -and (Test-Path -LiteralPath $unityLogPath)) {
        Write-Host ""
        Write-Host "Unity compile log tail ($unityLogPath)" -ForegroundColor Yellow
        Get-Content -LiteralPath $unityLogPath -Tail 80
    }

    Write-Host ""
    Write-Host "Release gate failed: $failedMessage" -ForegroundColor Red
    Write-Summary
    exit 1
}
