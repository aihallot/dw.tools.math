. (Join-Path $PSScriptRoot 'common.ps1')

if ([Console]::OutputEncoding.WebName -ne 'utf-8') {
    throw "Expected UTF-8 console output; observed $([Console]::OutputEncoding.WebName)."
}

$nonZeroWasPropagated = $false
try {
    Invoke-DotNet -Arguments @('--definitely-not-a-real-dotnet-option')
}
catch {
    $nonZeroWasPropagated = $true
}

if (-not $nonZeroWasPropagated) {
    throw 'Invoke-DotNet did not propagate a non-zero dotnet exit code.'
}

Write-Host 'PowerShell helper UTF-8 and non-zero propagation contracts verified.'
