$ErrorActionPreference = 'Stop'
$compose = Join-Path $PSScriptRoot '..' 'deploy' 'docker-compose.dev.yml'
Write-Host "[compose-up] Using: $compose"
docker compose -f $compose up -d

