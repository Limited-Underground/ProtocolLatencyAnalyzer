[CmdletBinding()]
param([ValidateSet('Debug', 'Release')][string]$Configuration = 'Release')
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'Build.ps1') -Configuration $Configuration
Push-Location -LiteralPath $projectRoot
try {
    & dotnet run --project tests/ProtocolLatencyAnalyzer.Tests --configuration $Configuration --no-build
    if ($LASTEXITCODE -ne 0) { throw 'Foundation tests failed.' }
} finally { Pop-Location }
