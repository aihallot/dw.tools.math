$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'verify-helpers.ps1')
& (Join-Path $PSScriptRoot 'restore.ps1')
& (Join-Path $PSScriptRoot 'build.ps1')
& (Join-Path $PSScriptRoot 'test.ps1')
& (Join-Path $PSScriptRoot 'pack.ps1')
& (Join-Path $PSScriptRoot 'verify-consumer.ps1')

Write-Host 'M0-W01-C01 foundation chain verified.'
