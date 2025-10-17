$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..' 'backend')
try {
  dotnet test tests/Todo.IntegrationTests -v minimal
} finally {
  Pop-Location
}

