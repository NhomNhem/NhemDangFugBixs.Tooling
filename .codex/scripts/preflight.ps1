$ErrorActionPreference = "Stop"

$solution = "Source~/NhemDangFugBixs.Tooling.sln"

if (!(Test-Path $solution)) {
    Write-Error "[Nhem] ERROR: solution not found: $solution"
}

Write-Host "[Nhem] Restore"
dotnet restore $solution

Write-Host "[Nhem] Build"
dotnet build $solution -c Release --no-restore

Write-Host "[Nhem] Test"
dotnet test $solution -c Release --no-build

Write-Host "[Nhem] Preflight complete"
