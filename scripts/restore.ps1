. (Join-Path $PSScriptRoot 'common.ps1')

Invoke-DotNet -Arguments @(
    'restore',
    'dw.tools.math.slnx',
    '--locked-mode'
)
