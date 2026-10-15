[CmdletBinding()]
param([ValidatePattern('^[a-zA-Z0-9][a-zA-Z0-9._-]{0,63}$')][string]$OutputName = 'win-x64')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$env:DOTNET_CLI_HOME = Join-Path $projectRoot '.artifacts/dotnet-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'
$publishRoot = Join-Path $projectRoot ".artifacts/publish/$OutputName"
$archivePath = Join-Path $projectRoot ".artifacts/ProtocolLatencyAnalyzer-0.1.0-$OutputName.zip"
Push-Location -LiteralPath $projectRoot
try {
    if (Test-Path -LiteralPath $publishRoot) { throw 'Publish output exists. Preserve it or choose a fresh project-local output before publishing again.' }
    & dotnet publish src/ProtocolLatencyAnalyzer.Desktop -c Release -r win-x64 --self-contained true -p:RuntimeFrameworkVersion=10.0.13 -p:RestoreLockedMode=true -p:PublishSingleFile=false -p:PublishTrimmed=false -o $publishRoot
    if ($LASTEXITCODE -ne 0) { throw 'Publish failed.' }
    $licenseRoot = Join-Path $projectRoot 'docs/licenses'
    foreach ($runtimePack in @('microsoft.netcore.app.runtime.win-x64', 'microsoft.windowsdesktop.app.runtime.win-x64')) {
        $runtimeSource = Join-Path $projectRoot ".artifacts/packages/$runtimePack/10.0.13"
        $noticeFiles = @(Get-ChildItem -LiteralPath $runtimeSource -File | Where-Object Name -Match '^(LICENSE|THIRD-PARTY-NOTICES)')
        if ($noticeFiles.Count -eq 0) { throw "Missing notices for $runtimePack." }
        foreach ($noticeFile in $noticeFiles) {
            Copy-Item -LiteralPath $noticeFile.FullName -Destination (Join-Path $licenseRoot "$runtimePack-$($noticeFile.Name)")
        }
    }
    Copy-Item -LiteralPath (Join-Path $projectRoot 'LICENSE'), (Join-Path $projectRoot 'NOTICE'), (Join-Path $projectRoot 'THIRD-PARTY-NOTICES.txt'), (Join-Path $projectRoot 'README.md') -Destination $publishRoot
    Copy-Item -LiteralPath (Join-Path $projectRoot 'docs') -Destination (Join-Path $publishRoot 'docs') -Recurse
    if (Test-Path -LiteralPath $archivePath) { throw 'Package archive already exists; preserve it before republishing.' }
    Compress-Archive -Path (Join-Path $publishRoot '*') -DestinationPath $archivePath
    $packageHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash.ToLowerInvariant()
    [IO.File]::WriteAllText("$archivePath.sha256", "$packageHash  $([IO.Path]::GetFileName($archivePath))`n")
    Write-Output $archivePath
} finally { Pop-Location }
