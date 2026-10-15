[CmdletBinding()]
param([ValidatePattern('^[a-zA-Z0-9][a-zA-Z0-9._-]{0,63}$')][string]$OutputName = 'win-x64')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$validationRoot = Join-Path $projectRoot ".artifacts/validation/$OutputName"
$archivePath = Join-Path $projectRoot ".artifacts/ProtocolLatencyAnalyzer-0.1.0-$OutputName.zip"
if (Test-Path -LiteralPath $validationRoot) { throw 'Validation output exists. Preserve it or use a fresh output name.' }
$expectedHash = (Get-Content -LiteralPath "$archivePath.sha256" -Raw).Split(' ')[0]
if ((Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash -ne $expectedHash) { throw 'Package checksum does not match.' }
Expand-Archive -LiteralPath $archivePath -DestinationPath (Join-Path $validationRoot 'extracted')
$applicationPath = Join-Path $validationRoot 'extracted/ProtocolLatencyAnalyzer.exe'
$previewPath = Join-Path $validationRoot 'dashboard-preview.png'
$checks = [Collections.Generic.List[object]]::new()
function Invoke-OfflineCheck([string]$Name, [string[]]$Arguments) {
    $started = [DateTimeOffset]::UtcNow
    $validationProcess = Start-Process -FilePath $applicationPath -ArgumentList $Arguments -WorkingDirectory (Split-Path -Parent $applicationPath) -WindowStyle Hidden -PassThru
    if (-not $validationProcess.WaitForExit(60000)) {
        $validationProcess.Kill()
        throw "$Name exceeded the 60-second process timeout."
    }
    if ($validationProcess.ExitCode -ne 0) { throw "$Name failed with exit code $($validationProcess.ExitCode)." }
    $checks.Add([PSCustomObject]@{Name=$Name;ExitCode=$validationProcess.ExitCode;DurationSeconds=([DateTimeOffset]::UtcNow-$started).TotalSeconds})
}
Invoke-OfflineCheck -Name 'View model and WPF layout' -Arguments @('--smoke-check')
Invoke-OfflineCheck -Name 'Packaged SQLite and configuration' -Arguments @('--verify-storage', ('"' + (Join-Path $validationRoot 'synthetic-storage') + '"'))
Invoke-OfflineCheck -Name 'Synthetic dashboard render' -Arguments @('--render-preview', ('"' + $previewPath + '"'))
$runtime = Get-Content -LiteralPath (Join-Path $validationRoot 'extracted/ProtocolLatencyAnalyzer.runtimeconfig.json') -Raw | ConvertFrom-Json
[PSCustomObject]@{
    TimestampUtc=[DateTimeOffset]::UtcNow.ToString('O')
    Scope='Offline synthetic checks on current host only; no interface inventory, traffic or capture. No interactive workflow or other-OS acceptance.'
    ArchiveSha256=$expectedHash
    IncludedFrameworks=$runtime.runtimeOptions.includedFrameworks
    Checks=$checks
    Preview=$previewPath
} | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $validationRoot 'results.json')
Get-Content -LiteralPath (Join-Path $validationRoot 'results.json')
