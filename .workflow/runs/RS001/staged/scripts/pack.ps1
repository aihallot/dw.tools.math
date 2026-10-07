. (Join-Path $PSScriptRoot 'common.ps1')

$packageDirectory = Join-Path $RepositoryRoot 'publish/packages'
New-Item -ItemType Directory -Path $packageDirectory -Force | Out-Null

Invoke-DotNet -Arguments @(
    'pack',
    'src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj',
    '-c',
    'Release',
    '--no-restore',
    '--no-build',
    '-o',
    $packageDirectory
)

$packagePath = Join-Path $packageDirectory 'dw.tools.math.foundation.0.1.0-preview.1.nupkg'
if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
    throw "Expected package was not created: $packagePath"
}

$archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    $assemblyEntry = $archive.GetEntry('lib/net10.0/dw.tools.math.foundation.dll')
    if ($null -eq $assemblyEntry) {
        throw 'Package does not contain the expected net10.0 assembly.'
    }

    $nuspecEntries = @($archive.Entries | Where-Object { $_.FullName -like '*.nuspec' })
    if ($nuspecEntries.Count -ne 1) {
        throw "Expected exactly one nuspec in the package; found $($nuspecEntries.Count)."
    }

    $reader = [System.IO.StreamReader]::new($nuspecEntries[0].Open())
    try {
        [xml] $nuspec = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }

    $dependencies = @($nuspec.SelectNodes("//*[local-name()='dependency']"))
    if ($dependencies.Count -ne 0) {
        throw "Foundation package must not install providers or other NuGet dependencies by default; found $($dependencies.Count) dependency entries."
    }
}
finally {
    $archive.Dispose()
}

Write-Host "Package contract verified: $packagePath"
