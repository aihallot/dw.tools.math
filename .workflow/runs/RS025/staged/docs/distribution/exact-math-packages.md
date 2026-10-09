# Independent exact Math package consumer

## Distribution boundary

This is a source-controlled recipe for building three local-only preview NuGet
packages and executing an isolated `.NET 10` consumer. The published package IDs are:

- `dw.quantities` at `0.2.0-preview.1`
- `dw.quantities.expression` at `0.2.0-preview.1`
- `dw.quantities.standard` at `0.2.0-preview.1`

The consumer source lives in `tests/consumers/exact-math-smoke/`. It has
**PackageReference only**: no `ProjectReference`, source include or reference
to the AURA checkout. Its NuGet configuration clears other package sources.
The run's `dotnet restore` also supplies the absolute local-only source and
an isolated packages directory under `build/artifacts/`.

## Reproduce from the Math checkout

After building `dw.tools.math.slnx` in Release configuration with a
locked solution restore, pack all three projects:

```pwsh
dotnet pack src/projects/dw.quantities/dw.quantities.csproj -c Release --no-build --no-restore -o build/artifacts/math-package-feed
dotnet pack src/projects/dw.quantities.expression/dw.quantities.expression.csproj -c Release --no-build --no-restore -o build/artifacts/math-package-feed
dotnet pack src/projects/dw.quantities.standard/dw.quantities.standard.csproj -c Release --no-build --no-restore -o build/artifacts/math-package-feed
```

Restore the consumer from that feed only and run it:

```pwsh
dotnet restore tests/consumers/exact-math-smoke/exact-math-smoke.csproj --source build/artifacts/math-package-feed --configfile tests/consumers/exact-math-smoke/NuGet.Config --packages build/artifacts/math-isolated-packages
dotnet run --project tests/consumers/exact-math-smoke/exact-math-smoke.csproj -c Release --no-restore
```

The executable must print exactly:
`dw.tools.math/exact-consumer/0.2 qualified`.

## Rights, provenance and limitations

Selected mathematical source was copied, adapted and packaged under the
owner authorization recorded in
`docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`, for the
pinned AURA revision `82a6b435a387a7e116b47a6b2c433ae9e067bf21`.
The authorization describes **owner-authorized proprietary code**; it does not
assert an open-source license. No separately issued owner notice is required
by that attestation. Third-party materials, if identified, need their own
applicable assessment before redistribution.

The RS025 evidence includes SHA-256 values of the package artifacts actually
built and inspected, together with dependency IDs and consumer checks.
The `.nupkg` outputs are local, ignored build artefacts; this procedure does
**not** publish binaries to NuGet.org, a public feed, or any sibling repository.
A hash in a run report identifies that attempt's specific binary and is
not a promise of bit-for-bit reproducibility across SDKs or build environments.

Intentional source divergences are documented and accepted under M0 policy:
Math requires explicit unit profiles instead of AURA's implicit host-culture
priority, and its strict 14-unit catalogue and 58-unit inherited catalogue
remain distinguishable. Historical defects are not silently frozen as oracles.
External AURA adoption requires a separate documented request and verification.
