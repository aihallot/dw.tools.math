# RS151 — Immutable private Foundation NuGet publisher rehearsal

## Exact original candidate and durable predecessor

Math RS150 succeeded at GitHub main commit `a58222d25577d0f22d17d759c069b7d5f3cf8682`, DWF 0.1.63, attempt 001, **first-pass success**, with zero prior invocation failures. Its F17 receipt qualified a real, unpublished `dw.tools.math.foundation [20260.20261105.1001]` candidate with five source-free binary checks, one unchanged Foundation C# source file, and no dependencies. The original historical source project Version is `0.1.0-preview.1`.

The **original candidate** has exactly **3,863 bytes**, archive SHA-256 `0fc5499e40593d2e2c01b5d8228c3f731d7df9d54050200906e53faa33569f94` and packaged net10.0 DLL SHA-256 `8dd3af5cba6a277703307a972552e177feb22f3da5f42e220a01ce1ed64964d2`. It is retained only in the ignored local artifact path `build/artifacts/rs150-foundation-candidate/candidate/dw.tools.math.foundation.20260.20261105.1001.nupkg`; it has **not** been published into `NUGET_LOCAL`.

## Single DWF RS151 objective

The owner-side non-packable release controller `tools/dw.tools.math.nuget-foundation-release` depends on the previously qualified immutable `Dw.Tools.NuGet [20260.20261026.1010]` package, with loaded provider DLL SHA-256 `b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`. It does not duplicate or replace the shared publisher.

The **only mode executed by RS151 DWF** is `--rehearse`. It validates the F17 candidate receipt, original Foundation project version and SHA-256 for both the csproj and its single `FoundationContract.cs`, the independent F16 Analysis publication evidence and existing private feed selector. It builds a real temporary Foundation packaging project from exactly that original linked source, packs with the candidate calendar version, then runs the shared immutable publication and readback APIs against a fresh **disposable feed**. It checks actual package ID/version, archive and DLL SHA-256, zero NuGet dependencies, duplicate/collision refusal, changed-digest refusal, cancellation refusal, and unchanged published bytes.

A new isolated consumer has one direct exact Foundation `PackageReference`, restores **only from the disposable feed**, loads its real package DLL, verifies the loaded hash and runs all original **five** F17 source-free identity/refusal checks. The controller records the built rehearsal archive/DLL hashes **separately** from the original RS150 candidate. Rehearsal build bytes may differ, so successful rehearsal does not prove byte-for-byte reproducibility or grant permission to substitute an archive.

## Guarded modes outside DWF

After a successful RS151 result and a separate explicit owner decision, the guarded controller offers two distinct owner commands: `--publish` and `--verify`. They require clean `main`, exact successful RS151 DWF report, original source-owner private packaging/redistribution rights, selected existing absolute `NUGET_LOCAL` and exactly the **original 3,863-byte** F17 candidate with both matching SHA-256 identities. The no-overwrite shared publisher refuses an existing package identity; the owner publication receipt is stored only in ignored local build output. Any ambiguous side effect after a failed publish requires inspecting the physical feed before retry, not repacking.

A later RS152 independent DWF physical-feed readback may be prepared **only after** successful owner `--publish` and `--verify` outputs. RS151 itself must not call either mode or mutate shared feed archives.

## RS151 preselection incident review (two failures, no allocated attempt)

This is the same RS151 run, not a successor or a reset of failed evidence. The first invocation `run-next-20261105T120849Z-0bd406e2f24c6cca` failed before attempt allocation: payload source checks expected a Markdown heading that differed from this contract and the obsolete method name `RequireCleanOwnerAndRs148`. The second invocation `run-next-20261105T124248Z-0b922fb5f61ec9d5` also failed before attempt allocation: the read-only Foundation validator still expected the wrong RS150 result message `test: qualify independent Foundation calendar NuGet candidate without publication`. The actual RS150 report confirms `test: qualify Foundation calendar NuGet candidate without publication`, attempt 001, first-pass success.

The correction aligns the payload heading with this document, verifies the actual `RequireCleanOwnerAndRs151` method, aligns validator predecessor identity to the durable report, corrects the validator's inherited owner method token, and keeps Foundation's original archive size (3,863 bytes), SHA-256, candidate version and source count explicit. The controller still runs only `--rehearse` within DWF. A comparison audit covers F17 receipt fields, F16 predecessor, five original binary oracles, tool-generated rehearsal JSON versus validator expectations, targeted read-only validation markers, selected DWF project scope and the intended staged mutation files. No other repository, feed or publishing policy changes.

Both failures produced separate durable GitHub `workflow/evidence/preselection-rs151/...` branches. Each reports `attemptAllocated=false`, `workingTreeChanged=false`; the cumulative retry ordinal is 2 and allocated attempts remain 0. If the corrected run later succeeds, its `firstPassSucceeded` must remain false because these failures are retained provenance.

## Scope and unqualified claims

RS151 may persist this Markdown contract, its controller/project, a read-only validator and a durable F18 rehearsal receipt only. All 541 completed native roadmap nodes, source version, original Foundation contract, five previously published Math packages and F1–F17 receipts remain unchanged. This rehearsal does not select a public license, publish to NuGet.org, certify installed rollback, concurrent-writer recovery, other OS/RIDs, feed-wide integrity, Discovery migration, Decision integration or AURA admission.
