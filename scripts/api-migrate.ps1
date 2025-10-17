$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..' 'backend')
try {
  dotnet tool restore | Out-Null
  dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api
} finally {
  Pop-Location
}

