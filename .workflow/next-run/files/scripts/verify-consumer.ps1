. (Join-Path $PSScriptRoot 'common.ps1')

$sourceDirectory = Join-Path $RepositoryRoot 'tests/consumers/foundation-smoke'
$consumerRoot = Join-Path $RepositoryRoot 'build/artifacts/consumer-smoke'
$packageDirectory = Join-Path $RepositoryRoot 'publish/packages'

if (Test-Path -LiteralPath $consumerRoot) {
    Remove-Item -LiteralPath $consumerRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $consumerRoot -Force | Out-Null
Copy-Item -Path (Join-Path $sourceDirectory '*') -Destination $consumerRoot -Recurse -Force

$escapedPackageDirectory = [System.Security.SecurityElement]::Escape($packageDirectory)
$consumerNuGetConfig = Join-Path $consumerRoot 'NuGet.config'
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-package" value="$escapedPackageDirectory" />
  </packageSources>
</configuration>
"@ | Set-Content -LiteralPath $consumerNuGetConfig -Encoding utf8NoBOM

$consumerProject = Join-Path $consumerRoot 'foundation-smoke.csproj'
$consumerPackages = Join-Path $consumerRoot '.packages'

Invoke-DotNet -Arguments @(
    'restore',
    $consumerProject,
    '--configfile',
    $consumerNuGetConfig,
    '--packages',
    $consumerPackages
)

Invoke-DotNet -Arguments @(
    'build',
    $consumerProject,
    '-c',
    'Release',
    '--no-restore'
)

$consumerAssembly = Join-Path $consumerRoot 'bin/Release/net10.0/foundation-smoke.dll'
if (-not (Test-Path -LiteralPath $consumerAssembly -PathType Leaf)) {
    throw "Expected consumer assembly was not created: $consumerAssembly"
}

$output = @(Invoke-DotNetCapture -Arguments @($consumerAssembly))
$lines = @($output | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
if ($lines.Count -ne 1 -or $lines[0].Trim() -ne 'dw.tools.math.foundation/0.1') {
    throw "Isolated consumer returned unexpected output: $($lines -join ' | ')"
}

Write-Host 'Isolated local-package consumer verified.'
