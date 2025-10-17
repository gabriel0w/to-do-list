$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$backendPath = Join-Path $root 'backend'
Push-Location $backendPath
try {
  dotnet test tests/Todo.UnitTests -v minimal
}
finally {
  Pop-Location
}

