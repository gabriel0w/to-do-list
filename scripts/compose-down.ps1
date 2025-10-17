$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$compose = Join-Path (Join-Path $root 'deploy') 'docker-compose.dev.yml'
Write-Host "[compose-down] Using: $compose"
docker compose -f $compose down -v

