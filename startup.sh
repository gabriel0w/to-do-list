#!/usr/bin/env bash
set -euo pipefail

NO_CACHE=""
PULL=""
POP_DB=false
while [[ $# -gt 0 ]]; do
  case "$1" in
    --no-cache) NO_CACHE="--no-cache" ; shift ;;
    --pull)     PULL="--pull"       ; shift ;;
    --populate-db) POP_DB=true        ; shift ;;
    *) echo "Unknown arg: $1" ; exit 2 ;;
  esac
done

export DOCKER_BUILDKIT=1

COMPOSE_FILE="$(cd "$(dirname "$0")" && pwd)/deploy/docker-compose.dev.yml"
if [[ ! -f "$COMPOSE_FILE" ]]; then
  echo "Compose file not found: $COMPOSE_FILE" >&2
  exit 1
fi

echo "[startup] Using compose file: $COMPOSE_FILE"
echo "[startup] Building images..."
docker compose -f "$COMPOSE_FILE" build $NO_CACHE $PULL

echo "[startup] Bringing stack up (db + api + web)..."
docker compose -f "$COMPOSE_FILE" up -d

echo "[startup] Done. URLs:"
echo "  API  : http://localhost:8080 (Swagger in /swagger)"
echo "  WEB  : http://localhost:8081"

if [[ "$POP_DB" == true ]]; then
  "$(cd "$(dirname "$0")" && pwd)/scripts/populate-db.sh" "http://localhost:8080"
fi
