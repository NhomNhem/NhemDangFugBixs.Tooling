#!/usr/bin/env bash
set -euo pipefail

SOLUTION="Source~/NhemDangFugBixs.Tooling.sln"

if [ ! -f "$SOLUTION" ]; then
  echo "[Nhem] ERROR: solution not found: $SOLUTION"
  exit 1
fi

echo "[Nhem] Restore"
dotnet restore "$SOLUTION"

echo "[Nhem] Build"
dotnet build "$SOLUTION" -c Release --no-restore

echo "[Nhem] Test"
dotnet test "$SOLUTION" -c Release --no-build

echo "[Nhem] Preflight complete"
