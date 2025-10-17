$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$backendPath = Join-Path $root 'backend'
Push-Location $backendPath
try {
  dotnet tool restore | Out-Null
  dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api
}
finally {
  Pop-Location
}

