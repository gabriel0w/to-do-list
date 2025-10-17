$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$frontendPath = Join-Path $root 'frontend'
Push-Location $frontendPath
try {
  if (-not (Test-Path 'node_modules')) { npm ci }
  npx playwright install
  npm run e2e
}
finally {
  Pop-Location
}

