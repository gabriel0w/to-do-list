param(
  [switch]$NoCache,
  [switch]$Pull,
  [switch]$PopulateDb
)

$ErrorActionPreference = 'Stop'

Write-Host "[startup] Enabling Docker BuildKit for this session..."
$env:DOCKER_BUILDKIT = '1'

$compose = Join-Path -Path $PSScriptRoot -ChildPath 'deploy/docker-compose.dev.yml'
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

if ($PopulateDb) {
  Write-Host "[startup] Populating database via API..."
  $populate = Join-Path -Path $PSScriptRoot -ChildPath 'scripts/populate-db.ps1'
  if (-not (Test-Path $populate)) { throw "Populate script not found: $populate" }
  & $populate -ApiBase 'http://localhost:8080'
}
