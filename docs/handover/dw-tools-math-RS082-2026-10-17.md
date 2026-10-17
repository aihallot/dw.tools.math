# dw.tools.math — compact operator handover, 2026-10-17

## Authority and operator identity

- Repository: private `aihallot/dw.tools.math`, branch `main`.
- Operator checkout (do not substitute a worktree): `D:data.devgithubai@hallot.netdw.tools.math`.
- Operator command: **`dwf run next`** only, after the selected request is verified.
- DWF version observed: 0.1.59; repository guidance version: 0.22.
- Communication with owner: French. Canonical repository documents, product code, evidence and commits: English.
- Do not modify `dw.tools.workflow`, AURA, Decision, MCDM, dw.discovery or any sibling repository.

## Last authoritative completed operation

- **RS081 pushed**, attempt **001**, **first pass**: zero earlier invocation failures and no allocated failures.
- Product commit: `e5ab6b745cf5639cc467f2f788815e45865dfce0`.
- Durable report: `.aura/workflow/reports/RS081/attempt-001.json` (`succeeded`).
- Science: `docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json` — 47 GREEN, compiled missing-API RED recovered (48 targeted), eight independent local NuGet probes and a single BCL-only `dw.tools.math.analysis 1.1.0-preview.1`.
- Backlog version at this handover baseline: **0.1.80**. M5 done, M6 / M6-W01 / M6-W01-C01 in progress, T1 done, **T2 not-ready/planned**. Native cursor: M6 / M6-W01 / M6-W01-C01, no active task or pending run before RS082 selection.
- Product publication remains local only; no stable/public NuGet, cross-OS, agent/Decision/MCDM adoption or license choice.

## Exactly one prepared next run

**RS082 — M6-W01-C01-T2**, retry **0**, attempts allocated **0**, **not executed** when this handover is authored. After final selection, GitHub `main` must contain one authoritative `.workflow/next-run/request.json`; its HEAD SHA is the **prepared tip**. Do not use the handover commit as the prepared tip.

Expected product commit:
`feat: qualify Math curve unit conditioning boundaries and close M6 W01 C01`.

Accepted scope: independently qualify bounded binary64 curve unit labels,
relative X-span/rank-refusal policies, explicit linear-only extrapolation,
typed nonpartial failures and no inference guarantees. Preserve all T1
interpolation/spline/OLS APIs and oracles.

Prepared staged product includes `CurveBoundaryAudit64.cs` and its public
unit, policy and status types; package version becomes **1.1.0-preview.2**.
T2 calls for one genuinely compiled missing-public-API RED, **59 new GREEN**
MSTests (60 including recovered RED), **48 retained T1 targeted tests**,
four relevant existing Math regression projects, one independently
SHA-inspected local net10 NuGet archive and **two different eight-probe
NuGet-only consumers** restored from the same isolated local feed.
The earlier T1 consumer remains scientific and now accepts exactly the T1
or T2 local feed names while pinning the new preview.2 package.

On successful durable execution, close T2-R/G/V, T2 and the **C01 phase only**.
M6-W01 and M6 remain in progress; C02 numerical roots/quadrature follows.
No broad full-solution suite is required for this bounded T2.

## Minimal bootstrap authorities (read handover fully first)

1. Workflow policy `docs/workflow/mantra.md` and
   `docs/workflow/constitution.md`; project `PROJECT-MANTRA.md`,
   `PROJECT-CONSTITUTION.md` and
   `docs/engineering/dotnet-authoring-checklist.md`.
2. Current GitHub `main` HEAD and **only** current
   `.workflow/next-run/request.json`, `.workflow/next-run/payload.cs`
   and the T2 staged source/test/consumer files if needed.
3. `.aura/workflow/state.json`,
   `.aura/workflow/plan/project.json`,
   `docs/planning/backlog.json`.
4. The **latest** RS082 durable attempt report or failure-evidence ref, if
   an operator result exists; otherwise RS081 report and T1 evidence
   named above. Never reconstruct RS001–RS080 history from scratch.

## Mandatory operator loop and first-pass protection

- After **`pushed`**: inspect durable report, science, backlog,
  native plan and HEAD; prepare **one next substantive** DWF run on `main`.
- After **`failed`**: retrieve latest durable evidence ref and
  exact error/classification/attempt count, then correct **the SAME run**.
  Pre-attempt refusals consume no attempt but do count against first-invocation
  success; never misstate `first pass`.
- Each normal interaction prepares or corrects a run. Exception:
  owner asks a specific question or an important cycle decision.
- No scripts outside the DWF payload for product mutations. No direct
  untracked File.Copy/Directory.CreateDirectory mutations in payload.
- Audit .NET 10 C# static risks: CS0219, CS0819, CS0136, CS8321,
  MSTEST0032, exact public signatures, accurate locked restore/test/pack
  sequence, target re-entry and scientific oracle truth.
- Audit DWF contribution limits before selection: summary <=500 chars;
  value <=1000; nextAction <=500; unique <=20 debt items <=300 each;
  unique <=20 node achievements <=1000 each. Reserve margin.
- Stage code and payload first. Publish `request.json` **LAST**,
  as the **sole file changed in the final GitHub commit**, then verify
  selected HEAD, baseline preconditions, mutation/staged inventories and
  expected product commit. Do not claim DWF execution before owner verdict.

## Terminal response contract

End every routine run preparation/correction response with exactly:

```text
DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math
BRANCH: main
PREPARED TIP: <latest verified GitHub main SHA>
EXPECTED COMMIT: <exact request commitMessage>
STATUS: RSnnn — retry N prepared — attempts allocated X — not executed
COMMAND: dwf run next
```

At a conversation-limit handoff: update one compact Markdown handover,
give its exact GitHub path and a short bootstrap prompt. Future agents
must use fresh durable evidence instead of replaying old handover history.
