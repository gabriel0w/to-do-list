$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..' 'frontend')
try {
  if (-not (Test-Path 'node_modules')) { npm ci }
  npx playwright install
  npm run e2e
} finally {
  Pop-Location
}

