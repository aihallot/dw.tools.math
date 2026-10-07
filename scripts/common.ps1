Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$utf8 = [System.Text.UTF8Encoding]::new($false)
[Console]::InputEncoding = $utf8
[Console]::OutputEncoding = $utf8
$OutputEncoding = $utf8

$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Invoke-DotNet {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string[]] $Arguments
    )

    Push-Location $RepositoryRoot
    try {
        & dotnet @Arguments
        $exitCode = $LASTEXITCODE
        if ($exitCode -ne 0) {
            throw "dotnet $($Arguments -join ' ') failed with exit code $exitCode."
        }
    }
    finally {
        Pop-Location
    }
}

function Invoke-DotNetCapture {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string[]] $Arguments
    )

    Push-Location $RepositoryRoot
    try {
        $output = @(& dotnet @Arguments 2>&1)
        $exitCode = $LASTEXITCODE
        if ($exitCode -ne 0) {
            $output | ForEach-Object { Write-Host $_ }
            throw "dotnet $($Arguments -join ' ') failed with exit code $exitCode."
        }

        return @($output | ForEach-Object { $_.ToString() })
    }
    finally {
        Pop-Location
    }
}
