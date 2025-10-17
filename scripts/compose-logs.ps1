$ErrorActionPreference = 'Stop'
param(
  [string]$Service = ""
)
$root = Split-Path -Parent $PSScriptRoot
$compose = Join-Path (Join-Path $root 'deploy') 'docker-compose.dev.yml'
Write-Host "[compose-logs] Using: $compose"
if ($Service) {
  docker compose -f $compose logs -f $Service
} else {
  docker compose -f $compose logs -f
}

