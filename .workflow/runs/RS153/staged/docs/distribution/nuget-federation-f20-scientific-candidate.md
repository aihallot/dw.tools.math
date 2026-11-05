# RS153 — Independent Scientific calendar NuGet candidate (unpublished)

## Durable predecessor and selected package

RS152 succeeded on Math GitHub `main` at `ba7a33dd453c51332cebac1a3246197b8d193c2d` with DWF `0.1.63`, attempt 001, first-pass success and zero prior invocation failures. Its F19 receipt independently verifies exact physical Foundation private-feed bytes and five binary checks. Six Math packages now have independently qualified private local-feed publication: `dw.quantities`, `dw.quantities.expression`, `dw.quantities.standard`, `dw.tools.math.ir`, `dw.tools.math.analysis`, `dw.tools.math.foundation`.

The accepted F1 inventory describes thirteen Math source package projects. Among the seven not yet independently qualified as published, this bounded run selects `dw.tools.math.scientific`: a **source-independent** package with no original `ProjectReference` or external `PackageReference`. The original `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` has historical `Version 1.1.0-preview.14`, `PackageId dw.tools.math.scientific`, assembly identity `dw.tools.math.scientific` and framework `net10.0`. The original top-level C# source set contains **exactly sixteen** files, not a five-file or single-file subset. The original `packages.lock.json` has `version=2` and an empty `net10.0` dependency map.

## Bounded original candidate and immutable identities

The prospective **candidate-only** NuGet identity is `dw.tools.math.scientific [20260.20261105.1001]`, dated 2026-11-05. This candidate version does **not** replace the original source's `1.1.0-preview.14` version, reserve an identity in `NUGET_LOCAL`, or authorize publication. The original `csproj`, lockfile and all sixteen C# source files remain unchanged.

The Math-owned, non-packable `tools/dw.tools.math.nuget-scientific-candidate` controller checks F1, F19, the exact source project and each top-level original C# filename, verifies the zero-dependency lockfile, and records SHA-256 for **18 original files** (16 C#, one csproj, one lockfile). It uses the original project with a restore locked against an empty local source and an effective command-line `PackageVersion` property. It builds and packs a real `net10.0` archive, inspects its `.nuspec`, confirms zero package dependencies and hashes the packaged DLL.

The candidate is retained in ignored local output only: `build/artifacts/rs153-scientific-candidate/candidate/dw.tools.math.scientific.20260.20261105.1001.nupkg`. Exact bytes, archive length, DLL SHA-256, source fingerprints and command receipts will be known only after successful DWF execution. An existing candidate archive must not be overwritten, rebuilt with the same identity or silently repaired.

## Fifteen source-free numerical and refusal checks

A freshly restored and built independent consumer references only `dw.tools.math.scientific [20260.20261105.1001]` through one exact `PackageReference`, with a clean `NuGet.Config` restricted to that candidate folder and a private package cache. No original Math `ProjectReference` or sibling repository is used.

Its fifteen bounded independent oracles cover actual loaded assembly identity/SHA, GCD, factorial, valid/invalid binomial, Euclidean norm and invalid axis, discrete first differences and invalid moving-average window, linear convolution, real sine in radians, invalid real logarithm, one-period financial discount and invalid rate, and a cancelled exact computation. Exact integers and statuses are checked exactly; floating-point results use explicit tolerances and must not claim guaranteed numerical error bounds. These **fifteen examples do not certify all Scientific APIs or universal numerical correctness**.

All seven actual producer/consumer process commands, the .NET SDK, runtime, observed OS/architecture, archive SHA-256, packaged DLL SHA-256 and eighteen original source hashes become part of the bounded F20 receipt.

## DWF mutation scope and future gate

RS153 persists only this contract, an observational C# validator, Math-owned candidate tool and fixture, and the F20 candidate evidence. The 541 completed native Math nodes, original scientific source identities, previous six published packages and F1–F19 evidence remain unchanged. No shared `NUGET_LOCAL` publication, public licensing selection, installed rollback, feed-wide verification, other OS/RID qualification, Discovery migration, Decision repository mutation or AURA integration is permitted.

A future separate DWF run may rehearse immutable Scientific publisher behavior on a **disposable feed**. Only thereafter may an owner independently authorize publishing exactly the original F20 candidate bytes and verifying that physical shared-feed package. Physical-feed DWF verification must be a subsequent separate run.
