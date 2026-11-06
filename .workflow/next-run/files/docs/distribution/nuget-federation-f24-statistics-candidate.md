# RS157 — Statistics calendar NuGet candidate with an exact qualified Quantities dependency

## Qualified predecessor and dependency boundary

RS156 was durably delivered to GitHub `main` at `c279ad23a5268d38f487015d14746d655828d26f`, DWF 0.1.63, attempt 001, first-pass success and zero prior invocation failures. Its F23 receipt independently verifies the original Scientific archive physically in `NUGET_LOCAL` with all fifteen package-only binary oracles and without republication. This establishes seven independently qualified published Math packages.

The accepted F1 inventory contains thirteen Math source package projects. This run selects `dw.tools.math.statistics` because its sole historical `ProjectReference` is `../dw.quantities/dw.quantities.csproj` and no external `PackageReference` exists. Its original project has `PackageId dw.tools.math.statistics`, `AssemblyName dw.tools.math.statistics`, `RootNamespace Dw.Tools.Math.Statistics` and source `Version 0.4.0-preview.6`. Exactly eight top-level C# files define the source. The original `packages.lock.json` has one `net10.0` entry, `dw.quantities`, of type `Project`.

`dw.quantities [20260.20261102.1001]` has been independently qualified as physically published by RS137 in the F4 receipt. The exact required original archive is 14,747 bytes with SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`; its packaged net10.0 DLL SHA-256 is `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`. The pinned shared `Dw.Tools.NuGet [20260.20261026.1010]` provider DLL SHA-256 is `b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.

## Candidate build and original-byte preservation

The original Statistics project and its eight C# files remain untouched. The Math-owned `tools/dw.tools.math.nuget-statistics-candidate` controller requires the original metadata, F1/F4/F23 evidence, original lockfile's one-project dependency, pinned physical Quantities archive and shared provider DLL. It fingerprints the original Statistics `.csproj`, `packages.lock.json` and eight C# files separately.

The F1 preflight tool calculates a current-day calendar version from the physical private-feed inventory. On 2026-11-06, in the absence of a Statistics version already published on that date, the expected candidate is `20260.20261106.1001`; the exact selected version will be written in the F24 receipt at execution rather than presumed from a static old F1 snapshot. The candidate version must be unreserved and absent from `NUGET_LOCAL`.

Only a temporary packaging projection in ignored `build/artifacts/rs157-statistics-candidate` replaces the original Statistics source `ProjectReference` with a single exact `PackageReference` for `dw.quantities [20260.20261102.1001]`. It links the eight original C# source files byte-for-byte, restores only from `NUGET_LOCAL`, builds and packs a real .NET 10 archive, verifies `.nuspec` has precisely the pinned dependency, checks ZIP safety and hashes the exact packaged DLL. The candidate archive is retained in ignored local output; neither source `.csproj` nor project dependency graph is rewritten. Existing candidate archives must not be overwritten or repacked.

## Eighteen isolated package-only binary probes

A fresh consumer is built with **one direct `PackageReference` to the exact Statistics candidate version** and no original project references. Its NuGet.Config permits only the private dependency feed and the isolated candidate directory. The compiled consumer verifies the actually loaded Statistics DLL SHA-256, descriptive mean/variances and empty-input refusal, two quantile definitions and invalid probability, paired correlation and mismatched inputs, Bernoulli probabilities and quantiles, continuous uniform law and invalid bounds, frequency-weighted and missing-policy semantics, SplitMix64 checkpoint replay and canonical codec, and noncanonical checkpoint refusal.

The eighteen probes are bounded examples and do not certify all statistical APIs, universal numerical accuracy or cryptographic suitability. Eleven real internal receipts are retained, including the explicit restore and build of the existing F1 preflight executable in the disposable DWF checkout, its two read-only inventory observations, candidate and consumer commands, and SDK. F1 is built once from the exact private feed before either `--no-build --no-restore` observation; neither invocation assumes prior build artifacts. The controller refuses changes to the original source hashes, Quantities package, shared NuGet DLL or published-feed scoped inventory. The F24 receipt records the dynamically selected version, original project/lockfile SHA-256, all eight C# source SHA-256 values, archived package/DLL hashes, exact `dw.quantities` dependency and observed environment.

## DWF scope and explicit exclusions

RS157 only installs this contract, the read-only F24 validator, the candidate controller/project/props, the unchanged Statistics consumer fixture and the F24 qualification receipt. All 541 completed native DWF nodes, original Statistics version and reference, existing seven published packages and F1–F23 receipts remain unchanged.

RS157 does **not** publish or reserve `dw.tools.math.statistics` in `NUGET_LOCAL`, choose a license, push to NuGet.org, verify another OS/RID, qualify installed rollback, rewrite another repository, or authorize Decision/Discovery/AURA migration. A later run must rehearse immutable publication on a disposable feed, followed by a distinct owner-controlled release gate and independent physical-feed readback.
