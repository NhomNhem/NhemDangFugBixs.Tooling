#!/usr/bin/env bash
set -euo pipefail

PROJECT="${1:-}"
OUT="${2:-docs/generated/di-map.md}"

if [ -z "$PROJECT" ]; then
  echo "Usage: .codex/scripts/generate-di-report.sh <path-to-csproj> [out-file]"
  exit 1
fi

mkdir -p "$(dirname "$OUT")"

dotnet di-smoke report "$PROJECT" --format markdown --out "$OUT"

echo "[Nhem] DI report generated: $OUT"
