# dw.tools.math — compact handover after RS145

**Date:** 2026-11-04 (Europe/Brussels)  
**Repository:** `aihallot/dw.tools.math`  
**Operator checkout:** `D:\data.dev\github\ai@hallot.net\dw.tools.math`  
**Operator execution branch:** `main`  
**Verified GitHub `main` HEAD at handover:** `2306b261920a304189d8a21e31c5d0770f0f0cdc`  
**DWF executable:** `0.1.63` · **repository guidance:** `0.26`  
**Current selection:** RS145 pushed; **RS146 not prepared**, no `.workflow/next-run/request.json`, `.aura/workflow/state.json` `pendingRun=null`.

This document is a **read-only handover on a separate branch**. It is not part of the Math `main` payload or an instruction to replay an already delivered run. Reverify current GitHub authority before acting; this snapshot may become stale.

## 1. Last durable run — RS145

- **Outcome:** `succeeded`, `attempt-001`, **first-pass YES**.
- **Prior invocation failures:** 0; preparation failures: 0; allocated-attempt failures: 0.
- **Verified result commit:** `2306b261920a304189d8a21e31c5d0770f0f0cdc`.
- **Exact commit message:** `test: qualify IR immutable local NuGet publisher rehearsal`.
- **Report:** `.aura/workflow/reports/RS145/attempt-001.json`.
- **Owner evidence:** `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json`.
- **Observed rehearsal:** real linked-source IR build and NuGet pack, disposable-feed publication, SHA-256 readback, collision refusal, digest mismatch refusal and cancellation refusal, **15/15 source-free binary checks**. Four outer and seven inner commands succeeded.
- **Shared feed publication:** **NO**; owner receipt says `package_published=false`, `publishedToNugetLocal=false`, `release_authorized_by_this_run=false`. Successful RS145 is **not** proof that IR is published in `NUGET_LOCAL`.
- Historical source versions, six IR source files and the source project's only Quantities `ProjectReference` are unchanged. M0–M7 and all 541 native roadmap nodes remain `done`.

The **rehearsal's** archive SHA `ad0a611cb42149437a881802a0fea8e0475a47086a97e5e6900ad2b2f339d5d7` and DLL SHA `027452e5d36f2c0bb0c179a4850b58065ac1bc40490a75d8e0f64537bbf44a31` are **not** the qualified original RS144 candidate. Do not publish the rehearsal bytes as the original.

## 2. Qualified immutable IR candidate (RS144)

- **Package ID:** `dw.tools.math.ir`.
- **Package version:** `20260.20261104.1001` (creation date 2026-11-04; not a promise of same-day physical publication).
- **Exact original archive:** 21,926 bytes, SHA-256 `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480`.
- **Original packaged DLL SHA-256:** `5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af`.
- **Original candidate path (ignored local output):** `build/artifacts/rs144-ir-candidate/candidate/dw.tools.math.ir.20260.20261104.1001.nupkg`.
- **One exact NuGet dependency:** `dw.quantities [20260.20261102.1001]`, archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`, DLL SHA-256 `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.
- **Original IR project:** `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj`, source `Version 0.3.0-preview.1`, source project `ProjectReference` unchanged.
- **Candidate receipt:** `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`.
- **Binary oracle:** `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs` (15 cases).

Three other private Math packages were **previously published and independently qualified**, with receipts F4, F7 and F10: `dw.quantities [20260.20261102.1001]`, `dw.quantities.expression [20260.20261102.1001]`, and `dw.quantities.standard [20260.20261103.1001]`. Do **not** generalize this to the remaining Math packages.

## 3. Next action — external owner-side private publication, not a DWF run

**No RS146 request exists. Do not run `dwf run next` yet.** RS146's intended input is an independently observable **physical** IR publication, which RS145 deliberately did not perform.

The prepared, already compiled owner controller is:
`tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj`.
It offers three separate modes: `--rehearse` (already passed within RS145), guarded `--publish` and guarded `--verify`.

On the actual clean `main` checkout, once the owner elects to publish the exact private package into the existing `NUGET_LOCAL` feed, run these **two separate commands**:

```powershell
dotnet run --project tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj -c Release --no-build --no-restore -- --publish

dotnet run --project tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj -c Release --no-build --no-restore -- --verify
```

Expected success signatures:
- `MATH-IR-PUBLISHED|id=dw.tools.math.ir|version=20260.20261104.1001|sha256=d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480|dependency=20260.20261102.1001|feed=NUGET_LOCAL|verification=pending`.
- `MATH-IR-VERIFY|id=dw.tools.math.ir|version=20260.20261104.1001|binary=15|archive=d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480|dependency=20260.20261102.1001|feed=NUGET_LOCAL|PASS`.

Actual commands are **not yet reported** in this handover. Never claim they ran. The controller requires the successful RS145 report, private owner rights, an existing absolute `NUGET_LOCAL`, clean `main`, the identical **original** 21,926-byte RS144 archive and exact dependency. It refuses collision and changed archive/DLL bytes. The local `build/artifacts` file is not guaranteed by GitHub: if it is missing, **do not rebuild, rename or reversion under the same ID/version**. If `--publish` fails after a possible side effect, inspect actual destination bytes before attempting anything again.

## 4. RS146 after successful publication

Only after the operator provides real `--publish` **and** `--verify` outputs:

1. Re-read `docs/workflow/mantra.md`, `docs/workflow/constitution.md`, `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, `AGENTS.md` and the applicable current DWF guidance. GitHub `main` / native DWF state and durable evidence override this handover.
2. Reconfirm RS145 `pushed`, source/dll/rights and that no other next run was prepared meanwhile.
3. Prepare a single bounded **RS146** on `main` via the normal DWF preparation mechanism: **independent read-only physical-feed** verification of exact IR and Quantities package archives/DLLs; one fresh consumer with only `dw.tools.math.ir [20260.20261104.1001]` as direct PackageReference, no Math ProjectReference; execute the unchanged 15-case binary oracle. Record exact source, actual SHA-256, SDK/runtime and bounded command receipts.
4. Preserve F1–F12 source/evidence, all completed roadmap nodes, existing package bytes and project source versions. No DWF publication, broad feed-wide inventory, public license, other-repo edits or unqualified rollback claim.
5. As required, `.workflow/next-run/request.json` is the **final GitHub write** of a preparation. Report exact `PREPARED TIP`, `EXPECTED COMMIT`, `retry 0` and `attempts alloués 0` only if the new request is actually prepared.

If the operator publishes unsuccessfully, **do not prepare RS146** against assumed state; investigate the physical source/destination/receipt first. If a DWF run fails, inspect newest durable evidence and correct the **same** run, with truthful retry and allocation counts. Never replace GitHub authority with remembered history.

## 5. Workflow conventions and handback

- Project scope is **Math only**; never modify Decision, AURA or sibling repositories on Math's behalf.
- English is canonical for committed docs, code and evidence; conversation is French.
- DWF is the sole execution authority: one pending run, read-only validations, no unrequested source version changes, no implicit publication.
- For normal prepared work, the only operator command is `dwf run next`; a reported `pushed` requires verification of result commit, report and cleared pending run before preparing a successor. A `failed` requires durable-evidence review and same-run correction.
- End operator responses with the exact ordered handback layout in `docs/workflow/operator-loop.md`, including actual first-pass, attempt allocation, next retry and `COMMAND`. If no next DWF run exists, use `PREPARED TIP: none`, `EXPECTED COMMIT: none`, `STATUS: aucun · retry n/a · attempts alloués n/a`, `COMMAND: aucune`.
- Do not reproduce the entire RS001–RS144 history at restart. Read only this handover and the recent authoritative files it names.

**Handover baseline:** `main` commit `2306b261920a304189d8a21e31c5d0770f0f0cdc`; RS145 `succeeded`, attempt 001, first-pass YES; RS146 **unprepared**, owner publication **not yet observed**.
