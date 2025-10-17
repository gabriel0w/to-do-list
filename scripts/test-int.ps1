$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$backendPath = Join-Path $root 'backend'
Push-Location $backendPath
try {
  dotnet test tests/Todo.IntegrationTests -v minimal
}
finally {
  Pop-Location
}

