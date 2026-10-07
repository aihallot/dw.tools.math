. (Join-Path $PSScriptRoot 'common.ps1')

Invoke-DotNet -Arguments @(
    'test',
    'tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj',
    '-c',
    'Release',
    '--no-restore',
    '--no-build',
    '--filter',
    'FullyQualifiedName~M0W01C01Tests',
    '--logger',
    'console;verbosity=minimal'
)
