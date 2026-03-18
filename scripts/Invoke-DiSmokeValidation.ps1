param(
    [Parameter(Mandatory = $true)]
    [string]$AssemblyPath,

    [string]$Configuration = "Debug",

    [switch]$Json
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solutionPath = Join-Path $repoRoot "Source~/NhemDangFugBixs.Tooling.sln"
$validatorProject = Join-Path $repoRoot "Source~/DangFugBixs.Tools~/DangFugBixs.DiSmokeValidation/DangFugBixs.DiSmokeValidation.csproj"
$dotnetHome = Join-Path $repoRoot ".dotnet"

$env:DOTNET_CLI_HOME = $dotnetHome
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"

$format = if ($Json.IsPresent) { "json" } else { "text" }

dotnet build $solutionPath --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

dotnet run --project $validatorProject --configuration $Configuration -- --format $format $AssemblyPath
exit $LASTEXITCODE
