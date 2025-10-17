param(
  [string]$ApiBase = 'http://localhost:8080'
)

$ErrorActionPreference = 'Stop'

function Wait-ApiReady {
  param([string]$Url, [int]$TimeoutSec = 60)

  $end = (Get-Date).AddSeconds($TimeoutSec)
  while ((Get-Date) -lt $end) {
    try {
      # Compatível com PS 5.1 (sem TimeoutSec) e PS 7+ (com TimeoutSec)
      $iw = @{
        Uri = "$Url/health"
        UseBasicParsing = $true
      }
      if ( (Get-Command Invoke-WebRequest).Parameters.ContainsKey('TimeoutSec') ) {
        $iw['TimeoutSec'] = 5
      }

      $r = Invoke-WebRequest @iw
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
