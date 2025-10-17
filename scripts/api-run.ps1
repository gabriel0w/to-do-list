$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..' 'backend')
try {
  dotnet run --project src/Todo.Api
} finally {
  Pop-Location
}

