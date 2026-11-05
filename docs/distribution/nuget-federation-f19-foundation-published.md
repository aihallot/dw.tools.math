# RS152 — Independent physical-feed readback for Foundation

## Publication provenance and separate DWF authority

Math RS150 produced the original unpublished `dw.tools.math.foundation [20260.20261105.1001]` archive from unchanged source and independently verified five binary identity contracts. RS151 was durably delivered at `63b2925234e3c8f27f6da704707084c177b91234`, DWF `0.1.63`, attempt 001, **successful after two failed pre-attempt invocations**; `firstPassSucceeded=false`, both failures are retained in its native report. RS151 rehearsed immutable publication only in a disposable feed and did not publish the shared private package.

On 2026-11-05 at 15:52 Europe/Brussels, the owner separately reported these commands succeeding in a clean `main` PowerShell session on MSI-02:

- `dotnet restore tools/dw.tools.math.nuget-foundation-release/dw.tools.math.nuget-foundation-release.csproj --source "$env:NUGET_LOCAL" -p:NuGetAudit=false` → `Restore complete` and `Build succeeded`. A previous automatic restore without the private feed failed `NU1101`; the explicit feed source resolved that dependency.
- `dotnet run --project $project -c Release --no-restore -- --publish` → `MATH-FOUNDATION-PUBLISHED|id=dw.tools.math.foundation|version=20260.20261105.1001|sha256=0fc5499e40593d2e2c01b5d8228c3f731d7df9d54050200906e53faa33569f94|dependencies=0|feed=NUGET_LOCAL|verification=pending`.
- `dotnet run --project $project -c Release --no-build --no-restore -- --verify` → `MATH-FOUNDATION-VERIFY|id=dw.tools.math.foundation|version=20260.20261105.1001|binary=5|archive=0fc5499e40593d2e2c01b5d8228c3f731d7df9d54050200906e53faa33569f94|dependencies=0|feed=NUGET_LOCAL|PASS`.

Those are **owner-supplied terminal outputs**, not independent DWF publication evidence. RS152 independently inspects the physical private-feed archive and may not publish or repack it.

## Exact original archive and independent inspection

- Package: `dw.tools.math.foundation`; version: `20260.20261105.1001` (date segment is original candidate date).
- Original F17 archive: **3,863 bytes** and SHA-256 `0fc5499e40593d2e2c01b5d8228c3f731d7df9d54050200906e53faa33569f94`.
- Original packaged and loaded `net10.0` DLL SHA-256: `8dd3af5cba6a277703307a972552e177feb22f3da5f42e220a01ce1ed64964d2`.
- Historical source Version `0.1.0-preview.1`, single `FoundationContract.cs` original source, and **zero NuGet dependencies**.

The new Math-owned BCL-only tool `tools/dw.tools.math.nuget-foundation-feed-verify` opens only that exact named archive at the top level of an existing absolute `NUGET_LOCAL` directory. It checks a physical non-redirected file, size, archive SHA-256, bounded and unique ZIP members, `.nuspec` package ID and version, absence of dependency declarations, actual `lib/net10.0` DLL bytes and DLL hash. It does **not** enumerate unrelated shared-feed packages or use publisher routines.

The verifier copies those unchanged physical bytes into a **fresh ignored snapshot**, then builds an isolated new .NET 10 consumer with a single direct `PackageReference` to the exact Foundation version, no Math source `ProjectReference`, a source-cleared `NuGet.Config`, and a new private package cache. It runs the unchanged five-case `FoundationBinaryConsumer.cs` fixture, compares the loaded assembly SHA to original F17, and checks all five binary identities/refusals. Physical and snapshot archives are rehashed after the run. Four observed process receipts (restore, build, binary, SDK), selected SDK/runtime and observed Windows architecture are recorded in the new F19 qualification receipt.

## Outcome and exclusions

Successful RS152 qualifies only the observed **sixth** locally published Math package, with exact physical bytes and five scoped binary identity tests. The previous five immutable local packages, historical Foundation source, all 541 completed native Math nodes, DWF history and F1–F18 evidence remain unchanged. No shared-feed write, public registry or license, installed downgrade/rollback claim, alternative OS/RID support, feed-wide integrity, concurrent-writer recovery, Decision, Discovery or AURA migration is authorized.

The next package candidate, if any, must be selected and qualified separately against the accepted 13-project inventory and exact dependency closure.
