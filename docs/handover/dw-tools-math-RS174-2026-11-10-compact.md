# Math DWF handover — 2026-11-10 — RS174 corrected, not rerun

Status: **compact continuation snapshot**, not an independent authority. Always prefer GitHub `main`, native DWF state and newest durable evidence.

## Operator identity and next run

- Repository: `aihallot/dw.tools.math`; execution branch: `main`; checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`.
- Last verified GitHub `main`: `f4bfbfbd44067a34f6e5bf467009edad9f4a6b96` — `test: correct RS174 preselection and retain exact electrochemical NuGet gates`.
- Current DWF runner: `0.1.66`, guidance `0.26`; `.aura/workflow/state.json` had `pendingRun: null` before run selection. Exactly one committed `.workflow/next-run/request.json` selects **RS174**.
- Expected RS174 result commit: `test: qualify electrochemical Standard NuGet candidate and twelve binary checks`.
- **Next single operator command: `dwf run next`.** RS174 has **0 allocated attempts**, 1 documented pre-attempt failure. If the next invocation fails, its expected CLI ordinal is **`failed [RS174 retry 1]`**. Do not confuse this with an allocated attempt.
- The corrected preparation has **6 staged source files**, **44 preconditions**, **5 structural validations**, and a bounded payload. The request was the final `main` write. Static inspection passed; no .NET execution of the correction has been claimed.

## Fresh RS174 failure and exact correction

- First RS174 invocation: `run-next-20261110T103020Z-755e0f9d7d4e9e41`; durable evidence ref `workflow/evidence/preselection-rs174/run-next-20261110T103020Z-755e0f9d7d4e9e41`; report `.aura/workflow/evidence/run-next-20261110T103020Z-755e0f9d7d4e9e41/report.json`.
- Outcome `failed`; stage `pre-attempt`, classification `workflow-refusal`, `attemptAllocated: false`, working tree unchanged. The candidate controller reported `nested_repository_refused` when inspecting `NUGET_LOCAL`.
- Root cause confirmed against shared `DotNetLocalPackageRepository.List` implementation in `aihallot/dw.tools` at `dw.tools.nuget/src/dw.tools.nuget/inspection/DotNetLocalPackageRepository.cs`: default policy refuses **child directories**, even when only top-level NuGet archives are wanted.
- Same-run correction: in staged `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs`, call `.List(PackageId, ignoreSubdirectories: true)`, require explicit `DirectoryPolicy == "ignore"`, bounded scanned inventory and matched packages. This ignores child directories without traversal; package/sha/old-version verification remains. Committed as `b48f27bc7e2dd9918c2f0f4b3f0f91c418ba2028`. Final request-correction commit: `f4bfbfbd44067a34f6e5bf467009edad9f4a6b96`. The diff relative to initial RS174 request changed **only** the staged controller and request JSON. No package publication.

## AURA priority and established Math facts

Owner-relayed AURA request: **`MATH-ELECTROCHEM-UNIT-PROFILE`**, need `A2-W02-C01-U-N`, AURA-side checkpoint `c818346f`. AURA local-only files were not accessible via this GitHub-only session. Work solely in Math; do not copy AURA code/tests or mutate AURA, DWF or sibling repositories. Historic AURA code reuse rights: **not established**; no public license/SPDX or global NuGet source change.

- RS172 `succeeded`, attempt 001 (one earlier pre-attempt failure), GitHub result `d403f9e73d4be8a65900b56320870b7ef120933c`; F38 Numerics evidence reconciled.
- RS173 `succeeded`, attempt 001 (one earlier pre-attempt failure), result `2befc6eb9ea4f54bf17256e91b3cbcb420806e54`. **12/12 source MSTest cases passed** under SDK `10.0.403`; receipt `docs/distribution/electrochemical-unit-profile-source-qualified.json`.
- Original opt-in `dw.quantities.standard.ElectrochemicalUnitProfile` has `ProfileId = math.si-electrochemical`, `ProfileVersion = 2026-11-electrochemical-exact-v1`, typed `Ampere, Volt, Ohm, Watt, Mole, MolePerCubicMetre, All, TryGetById`. Six exact 8D SI definitions with scale 1/1 and offset 0/1. See `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs` and `docs/distribution/electrochemical-unit-profile.md`; do not replay source campaign.
- Preserve `InheritedStandardUnitCatalog` 0.2.0 (58 entries), `StandardUnitCatalog` 2026-10-exact-v1 (14 entries), and source project version `0.2.0-preview.1`.
- Previously immutable local NuGet dependencies: `dw.quantities [20260.20261102.1001]` (SHA `e8ab26619c72...`), `dw.quantities.expression [20260.20261102.1001]` (SHA `d4eda8b77040...`), and older `dw.quantities.standard [20260.20261103.1001]` (SHA `c600644b960b...`).
- RS174 purpose: build a **new calendar-version** `dw.quantities.standard` from the 6 unchanged sources via disposable projection; two exact package dependencies; an independent **PackageReference-only**, no-ProjectReference consumer with a fresh disjoint NuGet cache, **12 binary probes**, all loaded DLL SHA-256, complete ZIP inventory, actual restore sources, CLR identities. Qualify **candidate only**, never claim published-feed qualification.

## Next agent operating loop

1. Read `docs/workflow/mantra.md`, then `docs/workflow/constitution.md`, then this compact handover in full. Read `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, `AGENTS.md`, the active .NET authoring checklist, backlog and only current authorities directly needed. GitHub `main` and newer durable DWF evidence always override this snapshot.
2. Await/report the single operator `dwf run next` result. On **`failed`**, inspect newest durable evidence and **correct RS174**; distinguish pre-attempt vs allocated attempt and give accurate next failure retry. On **`pushed`**, verify success receipt, GitHub main/result commit, native plan and cleared state, then prepare **one** bounded successor (RS175) on GitHub, with `request.json` written last.
3. After success: qualify an exact-archive, no-overwrite private publisher, publish new authorized Math-owned package only to `NUGET_LOCAL` using the existing shared mechanism, and perform independent readback of the **physically published** archive and three DLLs. Keep these as distinct DWF trust gates; never substitute a rebuilt candidate of the same version. The user's AURA-delivery request authorizes this bounded private-delivery objective; no repeated microapprovals, public release or AURA completion claim.
4. Minimize scope and context. No full suite or replay of prior tests; at most 12 newly selectable electrochemical consumer checks, targeted corrections only; .NET/C# project-owned logic, exact private package closures, immutable sources. No hand-authored workflow JSON parsing/hashing in payloads; prefer typed DWF SDK. Only `dwf run next` for operator.
5. Close operator handback with the exact required `Run terminé` line, then `DIRECTORY`, `BRANCH`, `PREPARED TIP`, `EXPECTED COMMIT`, `STATUS` (correct **next-failure** retry and allocated count), and `COMMAND` **last**.

This handover branch is a documentation snapshot and does not itself change GitHub `main`, the run request, any package, or the operator checkout.
