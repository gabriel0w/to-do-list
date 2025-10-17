$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$backendPath = Join-Path $root 'backend'
Push-Location $backendPath
try {
  dotnet run --project src/Todo.Api
}
finally {
  Pop-Location
}

