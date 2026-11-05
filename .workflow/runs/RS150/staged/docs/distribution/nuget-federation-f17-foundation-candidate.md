# RS150 — Independent Foundation NuGet calendar candidate (unpublished)

## Source authority and bounded purpose

RS149 was durably delivered to Math `main` as `f37f9269bcaf17080e2ee9efea009454c850190f`. Its report records attempt 001, first-pass success, DWF `0.1.63`, and a physically verified `dw.tools.math.analysis [20260.20261104.1001]` private NuGet package with 13/13 source-free binary checks.

The original F1 inventory contains thirteen Math source package identities. Five packages have independently qualified immutable local-feed publication: `dw.quantities`, `dw.quantities.expression`, `dw.quantities.standard`, `dw.tools.math.ir`, and `dw.tools.math.analysis`. This bounded run chooses **only** the still-unpublished `dw.tools.math.foundation` as the next leaf package candidate. F1 declares no source ProjectReferences or external PackageReferences for Foundation. The original `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj` has `Version 0.1.0-preview.1`, and the package's entire C# source set is one file: `FoundationContract.cs`.

## Candidate identity and boundaries

- New candidate ID and version: `dw.tools.math.foundation [20260.20261105.1001]`. The date segment records candidate creation on 2026-11-05, not a promise of same-day publication or a renumbering of historical source.
- Original source version: `0.1.0-preview.1`, unchanged.
- Original source: `src/projects/dw.tools.math.foundation/FoundationContract.cs`, unchanged.
- Target: `net10.0`; exact NuGet dependency count: **zero**.
- Candidate path (ignored local output): `build/artifacts/rs150-foundation-candidate/candidate/dw.tools.math.foundation.20260.20261105.1001.nupkg`.
- Package ID/version is not reserved or published into `NUGET_LOCAL` by this run.

The Math-owned source-free candidate controller `tools/dw.tools.math.nuget-foundation-candidate` audits F1, RS149 evidence, the source project, its one C# file, rights boundaries and existence of the selected private feed. It restores the existing original source project using its lock file and an empty disposable package source. It passes `PackageVersion` only at command invocation, without altering `Version` or source files. It builds and packs a real archive, records its size/SHA-256, inspects the actual `.nuspec` ID/version, verifies no NuGet dependencies, and hashes the packaged net10.0 DLL. Existing original candidate paths must not be overwritten.

## Independent binary-only Foundation consumer

A new isolated consumer has exactly one direct `PackageReference` (`dw.tools.math.foundation [20260.20261105.1001]`), with no Math source ProjectReference and no alternate package source. The consumer uses a fresh NuGet.Config restricted to the actual candidate folder and a private cache.

The five independent checks verify the loaded assembly identity/SHA-256, package ID, source contract version, exact public identity `dw.tools.math.foundation/0.1`, and absence of AURA/Decision/MCDM or foreign CAS provider references. These are narrow distribution-identity contracts; Foundation is a bootstrap/smoke package, **not** a mathematical engine or a scientific accuracy claim. The tool records actual restore/build/pack/consumer commands, SDK/runtime/OS/architecture, original source fingerprints, and refusal/unchanged-feed boundaries.

## Release separation and next gate

RS150 persists only a Math-owned candidate verifier/fixture, observational validator, this contract, and a durable F17 candidate receipt. All 541 completed native Math nodes, original source Version, existing five published package archives and F1–F16 evidence remain unchanged. No public license, NuGet.org publication, rollback guarantee, package replacement, other OS/RID evidence, Discovery/Decision migration or AURA admission is permitted.

If the original candidate is later published, first qualify the shared immutable publisher on a **fresh disposable feed** in a separate DWF run; only then may an explicit owner-controlled `--publish` / `--verify` act on the **exact original candidate bytes**, followed by a separate independent physical-feed DWF readback. Do not recreate a missing candidate under the same ID/version.
