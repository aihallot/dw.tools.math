. (Join-Path $PSScriptRoot 'common.ps1')

Invoke-DotNet -Arguments @(
    'build',
    'dw.tools.math.slnx',
    '-c',
    'Release',
    '--no-restore'
)
