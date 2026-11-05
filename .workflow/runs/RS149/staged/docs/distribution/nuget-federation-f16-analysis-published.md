# RS149 — Independent physical-feed readback for Math Analysis

## Operator publication and distinct DWF authority

RS147 qualified the **original unpublished** `dw.tools.math.analysis [20260.20261104.1001]` archive with thirteen source-free numerical oracles and no dependencies. RS148 succeeded at `7da88c0903253b5695931b3a0234dd1a98578f1e`, attempt 001, first-pass YES, using DWF 0.1.63. The shared `Dw.Tools.NuGet` publisher was rehearsed on a **disposable feed only**. Its rehearsal bytes differ from the original RS147 archive and are not an authorized replacement.

On 2026-11-05, the owner separately reported two successful commands on clean Math `main` using the guarded release controller:

- `MATH-ANALYSIS-PUBLISHED|id=dw.tools.math.analysis|version=20260.20261104.1001|sha256=d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf|dependencies=0|feed=NUGET_LOCAL|verification=pending`
- `MATH-ANALYSIS-VERIFY|id=dw.tools.math.analysis|version=20260.20261104.1001|binary=13|archive=d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf|dependencies=0|feed=NUGET_LOCAL|PASS`

These are **operator-provided outputs**, not yet independent DWF evidence. RS149 establishes that separate physical-feed trust boundary. No publication is authorized in RS149.

## Exact immutable package

- Package ID: `dw.tools.math.analysis`.
- Version: `20260.20261104.1001` (candidate creation date 2026-11-04, not a mutable publication timestamp).
- Original archive size: **27,831 bytes**.
- Original archive SHA-256: `d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf`.
- Original packaged/loaded net10.0 DLL SHA-256: `b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7`.
- Original source project version: `1.1.0-preview.6`; seven original source files unchanged.
- NuGet dependencies: **zero**; no original Math source ProjectReference in the isolated binary consumer.

## Independent read-only verification

The Math-owned, non-packable tool `tools/dw.tools.math.nuget-analysis-feed-verify` uses .NET BCL file, ZIP, cryptographic hash and XML inspection, **not** the publisher implementation. It selects the one exact top-level physical nupkg from the owner-provided absolute existing `NUGET_LOCAL`; it does not enumerate unrelated packages. It checks the non-redirected regular file, exact size, SHA-256, canonical ZIP members, ID/version, zero .nuspec dependencies, packaged net10.0 DLL and DLL hash.

A fresh ignored workspace contains a byte-identical snapshot of **only** that archive, with a new source-cleared `NuGet.Config` and private package cache. A fresh compiled source-free consumer uses the unchanged RS147 `AnalysisBinaryConsumer.cs` fixture with **one direct** `PackageReference` to `dw.tools.math.analysis [20260.20261104.1001]`, no project references and no alternate feeds. It must load the exact package DLL and pass all thirteen prior numerical semantic/refusal oracles. The tool hashes the physical feed archive again after execution to detect drift. All four observed commands (restore, build, consumer, SDK) and actual OS, architecture, SDK and runtime are retained.

## Outcome and limits

Only the new verifier, this source contract, an observational validator and the F16 DWF evidence receipt are eligible as persistent product mutations. All 541 completed native Math nodes, historical source versions, previously published packages, F1–F15 evidence and four earlier package qualifications remain unchanged. RS149 cannot publish or repack, select public licensing, mutate another repo, or claim installed rollback, concurrent writer recovery, cross-OS/RID behavior, feed-wide integrity, Discovery, Decision or AURA adoption.

A successful RS149 would make `dw.tools.math.analysis` the **fifth independently qualified private local Math package**, within the exact observed platform and thirteen-oracle scope. Other unqualified Math packages remain separate future candidates.
