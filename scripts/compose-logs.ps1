$ErrorActionPreference = 'Stop'
$compose = Join-Path $PSScriptRoot '..' 'deploy' 'docker-compose.dev.yml'
Write-Host "[compose-logs] Using: $compose"
param(
  [string]$Service = ""
)
if ($Service) {
  docker compose -f $compose logs -f $Service
} else {
  docker compose -f $compose logs -f
}

