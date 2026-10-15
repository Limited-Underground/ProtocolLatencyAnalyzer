[CmdletBinding()]
param(
    [ValidatePattern('^[a-zA-Z0-9][a-zA-Z0-9._-]{0,63}$')][string]$OutputName = 'win-x64',
    [string]$ExecutionDirectory
)
$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'Package verification requires PowerShell 7 or later for exact process arguments.' }
$projectRoot = Split-Path -Parent $PSScriptRoot
$validationRoot = Join-Path $projectRoot ".artifacts/validation/$OutputName"
$archivePath = Join-Path $projectRoot ".artifacts/ProtocolLatencyAnalyzer-0.1.0-$OutputName.zip"
if (Test-Path -LiteralPath $validationRoot) { throw 'Validation output exists. Preserve it or use a fresh output name.' }
$executionLocation = 'Project artifacts'
$extractionRoot = Join-Path $validationRoot 'extracted'
if ($PSBoundParameters.ContainsKey('ExecutionDirectory')) {
    if ([string]::IsNullOrWhiteSpace($ExecutionDirectory) -or -not [IO.Path]::IsPathFullyQualified($ExecutionDirectory)) { throw 'ExecutionDirectory must be an explicitly chosen absolute filesystem path.' }
    $extractionRoot = [IO.Path]::GetFullPath($ExecutionDirectory)
    if (Test-Path -LiteralPath $extractionRoot) { throw 'ExecutionDirectory already exists. Choose a new directory; existing content will not be overwritten.' }
    $executionLocation = 'Explicit directory'
}
$expectedHash = (Get-Content -LiteralPath "$archivePath.sha256" -Raw).Split(' ')[0]
if ((Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash -ne $expectedHash) { throw 'Package checksum does not match.' }
Expand-Archive -LiteralPath $archivePath -DestinationPath $extractionRoot
if (-not (Test-Path -LiteralPath $validationRoot)) { $null = New-Item -ItemType Directory -Path $validationRoot }
$applicationPath = Join-Path $extractionRoot 'ProtocolLatencyAnalyzer.exe'
$previewPath = Join-Path $validationRoot 'dashboard-preview.png'
$checks = [Collections.Generic.List[object]]::new()
$runtime = Get-Content -LiteralPath (Join-Path $extractionRoot 'ProtocolLatencyAnalyzer.runtimeconfig.json') -Raw | ConvertFrom-Json
function Write-ValidationResults {
    [PSCustomObject]@{
        TimestampUtc=[DateTimeOffset]::UtcNow.ToString('O')
        Scope='Synthetic model/layout/storage/render checks and a real-window startup/dispatcher check on the current host. Startup initiates read-only local interface inventory; no probes, traffic acquisition, capture or user-data writes. No interactive workflow or other-OS acceptance.'
        ArchiveSha256=$expectedHash
        ExecutionLocation=$executionLocation
        IncludedFrameworks=$runtime.runtimeOptions.includedFrameworks
        Checks=$checks.ToArray()
        Preview=$previewPath
    } | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $validationRoot 'results.json')
}
function Invoke-OfflineCheck([string]$Name, [string[]]$Arguments) {
    $clock = [Diagnostics.Stopwatch]::StartNew()
    $validationProcess = [Diagnostics.Process]::new()
    $startInfo = [Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $applicationPath
    $startInfo.WorkingDirectory = Split-Path -Parent $applicationPath
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $validationProcess.StartInfo = $startInfo
    $stdoutTask = $null
    $stderrTask = $null
    $exitCode = $null
    $failure = $null
    $timedOut = $false
    $launched = $false
    try {
        if ($null -eq $startInfo.ArgumentList) { throw 'Package verification requires PowerShell 7 or later for exact process arguments.' }
        foreach ($argument in $Arguments) { $startInfo.ArgumentList.Add($argument) }
        if (-not $validationProcess.Start()) { throw 'Process did not start.' }
        $launched = $true
        $stdoutTask = $validationProcess.StandardOutput.ReadToEndAsync()
        $stderrTask = $validationProcess.StandardError.ReadToEndAsync()
        $remaining = [Math]::Max(0, 60000 - [int]$clock.ElapsedMilliseconds)
        if (-not $validationProcess.WaitForExit($remaining)) {
            $timedOut = $true
            $failure = 'ProcessTimeout'
        } else {
            $exitCode = $validationProcess.ExitCode
            # Include asynchronous stream completion in the same deadline.
            $remaining = [Math]::Max(0, 60000 - [int]$clock.ElapsedMilliseconds)
            if (-not [Threading.Tasks.Task]::WaitAll([Threading.Tasks.Task[]]@($stdoutTask, $stderrTask), $remaining)) {
                $timedOut = $true
                $failure = 'OutputDrainTimeout'
            } elseif ($exitCode -ne 0) {
                $failure = 'NonzeroExitCode'
            }
        }
    } catch {
        $failure = 'ProcessExecutionFailure'
    } finally {
        try {
            if ($launched -and -not $validationProcess.HasExited) {
                $validationProcess.Kill($true)
                if (-not $validationProcess.WaitForExit(5000)) { $failure = 'ProcessTerminationFailure' }
            }
        } catch { $failure = 'ProcessTerminationFailure' }
        # A killed process closes the redirected streams; keep this cleanup bounded.
        foreach ($drainTask in @($stdoutTask, $stderrTask)) {
            try {
                if ($null -ne $drainTask -and -not $drainTask.IsCompleted) { $null = $drainTask.Wait(5000) }
            } catch { if ($null -eq $failure) { $failure = 'OutputDrainFailure' } }
        }
        $stdout = if ($null -ne $stdoutTask -and $stdoutTask.IsCompletedSuccessfully) { $stdoutTask.Result } else { '' }
        $stderr = if ($null -ne $stderrTask -and $stderrTask.IsCompletedSuccessfully) { $stderrTask.Result } else { '' }
        $artifactName = 'check-{0:D2}' -f ($checks.Count + 1)
        # Raw output stays in ignored validation artifacts and is never added to public results.
        $stdout | Set-Content -LiteralPath (Join-Path $validationRoot "$artifactName.stdout.log")
        $stderr | Set-Content -LiteralPath (Join-Path $validationRoot "$artifactName.stderr.log")
        $allowedStages = @('app-constructor', 'entry', 'storage-checks-begin', 'storage-checks-end', 'model-checks-begin', 'model-checks-end', 'model-demo-on-begin', 'model-demo-on-end', 'model-demo-off-begin', 'model-demo-off-end', 'window-constructor-begin', 'window-constructor-end', 'window-demo-off-begin', 'window-demo-off-end', 'window-demo-on-begin', 'window-demo-on-end', 'measure-begin', 'measure-end', 'arrange-begin', 'arrange-end', 'update-layout-begin', 'update-layout-end', 'render-begin', 'render-end', 'normal-window-constructor-begin', 'normal-window-constructor-end', 'normal-window-show-begin', 'normal-window-show-end', 'normal-window-content-rendered', 'normal-window-application-idle', 'normal-window-close-begin', 'normal-window-close-end', 'validation-failed', 'shutdown-requested')
        $stages = @($stderr -split '\r?\n' | ForEach-Object {
            if ($_ -cmatch '^validation: ([a-z-]+)$' -and $Matches[1] -cin $allowedStages) { $Matches[1] }
        })
        $clock.Stop()
        $checks.Add([PSCustomObject]@{Name=$Name;ExitCode=$exitCode;DurationSeconds=$clock.Elapsed.TotalSeconds;TimedOut=$timedOut;Failure=$failure;Stages=$stages})
        $validationProcess.Dispose()
        Write-ValidationResults
    }
    if ($timedOut) { throw "$Name exceeded the 60-second process deadline. Partial results and private logs were preserved." }
    if ($null -ne $failure) { throw "$Name failed ($failure). Partial results and private logs were preserved." }
}
Invoke-OfflineCheck -Name 'View model and WPF layout' -Arguments @('--smoke-check')
Invoke-OfflineCheck -Name 'Packaged SQLite and configuration' -Arguments @('--verify-storage', (Join-Path $validationRoot 'synthetic-storage'))
Invoke-OfflineCheck -Name 'Synthetic dashboard render' -Arguments @('--render-preview', $previewPath)
Invoke-OfflineCheck -Name 'Real window startup and dispatcher responsiveness' -Arguments @('--verify-startup')
Get-Content -LiteralPath (Join-Path $validationRoot 'results.json')
