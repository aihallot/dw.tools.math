# RS174 — Electrochemical Standard immutable calendar NuGet candidate

## Qualified source and historical non-mutation

RS173 succeeded at attempt 001 on GitHub main commit
`2befc6eb9ea4f54bf17256e91b3cbcb420806e54`, after one pre-attempt
refusal. Exactly twelve newly selected Math-owned source tests passed. The
`ElectrochemicalUnitProfile` remains explicitly opt-in, with six SI units,
eight-dimensional exact vectors, scale 1/1 and offset 0/1, and stable typed
`Ampere`, `Volt`, `Ohm`, `Watt`, `Mole`, `MolePerCubicMetre`,
`All` and `TryGetById` members.

This run preserves `InheritedStandardUnitCatalog` 0.2.0 (58 units),
`StandardUnitCatalog` 2026-10-exact-v1 (14 units), and source-project
`dw.quantities.standard` version `0.2.0-preview.1`. It links the six
Math-owned source files without copying any AURA implementation, tests or
algorithms. Historical AURA code reuse rights remain **not established**.

## New calendar candidate and actual package-dependent binary consumer

The candidate owns the existing `dw.quantities.standard` PackageId and
uses the canonical `20260.YYYYMMDD.NNNN` form. The actual local creation
date and next unoccupied ordinal from 1001–1999 select a candidate; no
unbuilt version is represented as reserved. The disposable packaging
projection replaces two source-only project dependencies **only inside the
projection** with exact private `PackageReference` dependencies:

- `dw.quantities [20260.20261102.1001]`: archive SHA-256
  `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`.
- `dw.quantities.expression [20260.20261102.1001]`: archive SHA-256
  `d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae`.

Old published `dw.quantities.standard [20260.20261103.1001]` remains
unchanged with SHA-256
`c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75`.
The pinned `Dw.Tools.NuGet [20260.20261026.1010]` reader independently
recognizes the freshly packed candidate. A complete ZIP member manifest
records every packaged file's path, size, compressed size and SHA-256.

A fresh `net10.0` consumer has one exact Standard PackageReference and
**no Math or AURA ProjectReference**. With explicit local feeds and a
separate empty NuGet package cache, it loads the new six-unit compiled
profile from the candidate archive and existing exact two-package closure.
Twelve independently named binary probes cover six definitions,
three physically loaded DLL hashes, catalogue preservation, two electrical
products, exact molar concentration/inverse and two refusal scenarios.
Neither the consumer nor the fixture reconstructs the six unit definitions.

The candidate evidence records actual asset-file restore sources (including
SDK-injected local sources if any), archive/full DLL fingerprints, source
file hashes, PackageId/version versus CLR assembly identity, TFM,
OS/architecture, exact commands and bounded logs. There is no source
rebuild equivalence claim to a future second archive: that would require
separate SHA comparison.

## Gates and rights

RS174 does not publish or reserve any identity in `NUGET_LOCAL`, access a
remote package registry, change shared NuGet sources, issue a new license
or SPDX expression, alter historical rights, qualify installed rollback,
modify AURA/another repository, or close AURA's U-N, host/worker/CLI or C01.
Only a subsequently qualified immutable publisher may publish the exact
candidate bytes without overwrite; independent re-reading of the actually
published archive and a package-only delivery consumer must follow.
