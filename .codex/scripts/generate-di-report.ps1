$ErrorActionPreference = "Stop"

$project = $args[0]
$out = if ($args.Count -gt 1) { $args[1] } else { "docs/generated/di-map.md" }

if ([string]::IsNullOrWhiteSpace($project)) {
    Write-Error "Usage: .codex/scripts/generate-di-report.ps1 <path-to-csproj> [out-file]"
}

$outDir = Split-Path $out -Parent
if ($outDir -and !(Test-Path $outDir)) {
    New-Item -ItemType Directory -Force -Path $outDir | Out-Null
}

dotnet di-smoke report $project --format markdown --out $out

Write-Host "[Nhem] DI report generated: $out"
