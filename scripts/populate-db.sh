#!/usr/bin/env bash
set -euo pipefail

API_BASE="${1:-http://localhost:8080}"

wait_api() {
  local url="$1"; local end=$((SECONDS+60))
  while (( SECONDS < end )); do
    if curl -sS "$url/health" >/dev/null; then return 0; fi
    sleep 2
  done
  echo "[populate-db] WARN: API not ready within timeout" >&2
}

echo "[populate-db] Waiting API at $API_BASE ..."
wait_api "$API_BASE"

titles=(
  "Buy groceries" "Read a book" "Write code" "Workout" "Water plants"
  "Plan the week" "Review PRs" "Fix bugs" "Learn Angular" ".NET Study"
)

for t in "${titles[@]}"; do
  curl -sS -X POST "$API_BASE/api/tasks" -H 'Content-Type: application/json' \
    -d "{\"title\":\"$t\"}" >/dev/null || echo "[populate-db] Failed: $t" >&2
  echo "[populate-db] Created: $t"
done

echo "[populate-db] Done."

