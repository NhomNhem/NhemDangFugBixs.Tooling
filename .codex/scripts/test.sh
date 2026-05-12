#!/usr/bin/env bash
set -euo pipefail

SOLUTION="Source~/NhemDangFugBixs.Tooling.sln"
FILTER="${1:-}"

if [ -z "$FILTER" ]; then
  dotnet test "$SOLUTION" -c Release
else
  dotnet test "$SOLUTION" -c Release --filter "$FILTER"
fi
