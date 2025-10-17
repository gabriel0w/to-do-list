$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot '..' 'backend')
try {
  dotnet test tests/Todo.UnitTests -v minimal
} finally {
  Pop-Location
}

