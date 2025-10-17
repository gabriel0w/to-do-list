$ErrorActionPreference = 'Stop'
param(
  [string]$ApiBase = 'http://localhost:8080'
)

function Wait-ApiReady {
  param([string]$Url, [int]$TimeoutSec = 60)
  $end = (Get-Date).AddSeconds($TimeoutSec)
  while ((Get-Date) -lt $end) {
    try {
      $r = Invoke-WebRequest -UseBasicParsing -Uri "$Url/health" -TimeoutSec 5
      if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 500) { return }
    } catch {}
    Start-Sleep -Seconds 2
  }
  Write-Warning "API health not ready within $TimeoutSec seconds"
}

Write-Host "[populate-db] Waiting API at $ApiBase ..."
Wait-ApiReady -Url $ApiBase

$titles = @(
  'Buy groceries','Read a book','Write code','Workout','Water plants',
  'Plan the week','Review PRs','Fix bugs','Learn Angular','Study .NET'
)

foreach ($t in $titles) {
  try {
    $body = @{ title = $t } | ConvertTo-Json -Depth 2
    Invoke-RestMethod -Method Post -ContentType 'application/json' -Uri "$ApiBase/api/tasks" -Body $body | Out-Null
    Write-Host "[populate-db] Created: $t"
  } catch {
    Write-Warning "[populate-db] Failed to create '$t': $_"
  }
}

Write-Host "[populate-db] Done."

