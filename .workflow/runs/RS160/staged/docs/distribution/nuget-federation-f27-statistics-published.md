# RS160 — Independent physical Statistics private-feed verification

## Owner-observed private publication, separate from DWF evidence

After RS159 succeeded first-pass on GitHub `main` at `c84e7647b5a75067535feec1085ec66f0a27688c` (DWF `0.1.66`, attempt 001), the owner separately executed guarded `--publish` followed by `--verify` on MSI-02 `main` at **2026-11-07 00:29 Europe/Brussels**. The local transcript returned:

`MATH-STATISTICS-PUBLISHED|id=dw.tools.math.statistics|version=20260.20261106.1001|sha256=9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8|dependency=20260.20261102.1001|feed=NUGET_LOCAL|verification=pending`

`MATH-STATISTICS-VERIFY|id=dw.tools.math.statistics|version=20260.20261106.1001|binary=18|archive=9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8|dependency=20260.20261102.1001|feed=NUGET_LOCAL|PASS`

These are **owner-observed terminal outputs**, not independent DWF qualification of physical package bytes. The immutable candidate calendar version encodes **2026-11-06**, its qualification date, despite the owner's private publication occurring after midnight on 2026-11-07.

## Four independent authorities and immutable package identities

F24/RS157 qualified the **unpublished original** `dw.tools.math.statistics [20260.20261106.1001]`: exactly **17,451 bytes**, archive SHA-256 `9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8`, net10.0 DLL SHA-256 `75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96`, source Version `0.4.0-preview.6`, eight original Statistics C# sources, project and lockfile intact. Its single and exact package dependency is `dw.quantities [20260.20261102.1001]`, from the F4-qualified physical private-feed archive: **14,747 bytes**, archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`, DLL SHA-256 `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.

F25/RS158 independently qualified seven immutability, collision, digest and cancellation controls against a **different disposable rehearsal archive**, with 18 package-only binary probes. It did not publish to `NUGET_LOCAL`. F26/RS159 compiled the owner release controller after correcting the single inherited `irDllSha256` receipt key to `statisticsDllSha256`. F26 retains two separate source fingerprints: historical RS158 source as attested by F25, and its one-field-corrected RS159 source. Neither F25's archived historical controller fingerprint nor the F26 corrected fingerprint may be silently rewritten.

## Independent physical readback

RS160 installs a Math-owned BCL-only tool `tools/dw.tools.math.nuget-statistics-feed-verify`, then uses the local `NUGET_LOCAL` environment variable only as a **read-only** physical package source. The verifier inspects the exact two named archives (Statistics and Quantities), requiring ordinary nonredirected physical files, expected sizes and SHA-256, bounded unique safe ZIP entries, correct `.nuspec` identities, exactly one declared Statistics dependency `dw.quantities [20260.20261102.1001]`, no dependencies of Quantities, and net10.0 DLL SHA-256 matching both pinned authorities.

The verifier copies both physical archives into a new ignored `build/artifacts` snapshot. It restores a **fresh isolated package consumer** using only that snapshot, with one direct exact Statistics `PackageReference`, no original Math project references and no online package sources. The consumer compiles and executes all **18 original Statistics binary oracles**, including the actual loaded Statistics DLL SHA check, descriptive/weighted statistics, quantiles, correlation, distributions, deterministic sampling and typed refusals. The verifier re-inspects the source feed's two physical archives after testing to establish no change. Four actual child process receipts (consumer restore/build/run, SDK) and four outer DWF receipts (tool restore/build, readback, SDK) are saved as an F27 evidence document.

F27 binds the exact prior RS159 first-pass report, F24 candidate, F25 historical rehearsal, F26 corrected controller, F4 independent base-package receipt, current Math project scopes and all retained original source hashes. It verifies F25's source-file hashes using the **historical** controller copy in `.workflow/runs/RS158/staged`, and checks F26's hashes against the **corrected** installed controller. Original Statistics code, all 541 completed native nodes, earlier F1–F26 receipts and seven previously qualified packages remain unchanged.

## Explicit exclusions

RS160 does **not** publish or overwrite a package, invoke the private publisher, scan unrelated packages, certify feed-wide integrity, choose public licensing, qualify installed rollback, certify another OS/RID, modify another repository or authorize Discovery/Decision/AURA migration. Successful F27 qualification describes only the two physically inspected private package identities, the observed Windows/.NET environment, the isolated consumer and explicitly bounded binary oracles. It qualifies Statistics as the **eighth independently inspected published Math package**, not a general release of all Math sources.
