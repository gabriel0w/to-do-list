$ErrorActionPreference = 'Stop'
$compose = Join-Path $PSScriptRoot '..' 'deploy' 'docker-compose.dev.yml'
Write-Host "[compose-down] Using: $compose"
docker compose -f $compose down -v

