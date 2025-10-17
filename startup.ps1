$ErrorActionPreference = 'Stop'

param(
  [switch]$NoCache,
  [switch]$Pull
)

Write-Host "[startup] Enabling Docker BuildKit for this session..."
$env:DOCKER_BUILDKIT = '1'

$compose = Join-Path $PSScriptRoot 'deploy' 'docker-compose.dev.yml'
if (-not (Test-Path $compose)) {
  throw "Compose file not found: $compose"
}

Write-Host "[startup] Using compose file: $compose"

$buildArgs = @('compose','-f', $compose, 'build')
if ($NoCache) { $buildArgs += '--no-cache' }
if ($Pull)    { $buildArgs += '--pull' }

Write-Host "[startup] docker $($buildArgs -join ' ')"
docker @buildArgs

Write-Host "[startup] Bringing stack up (db + api + web) ..."
docker compose -f $compose up -d

Write-Host "[startup] Done. URLs:" -ForegroundColor Green
Write-Host "  API  : http://localhost:8080 (Swagger em /swagger)"
Write-Host "  WEB  : http://localhost:8081"

