Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$expected = '10.0.401'
$sdks = @(& dotnet --list-sdks)
if ($LASTEXITCODE -ne 0) {
    throw "dotnet --list-sdks failed with exit code $LASTEXITCODE."
}

$matches = @($sdks | Where-Object { $_ -match '^10\.0\.401\s+\[' })
if ($matches.Count -ne 1) {
    throw "Required .NET SDK $expected is not installed exactly once. Installed SDKs: $($sdks -join '; ')"
}

Write-Host "Required .NET SDK $expected is installed."
