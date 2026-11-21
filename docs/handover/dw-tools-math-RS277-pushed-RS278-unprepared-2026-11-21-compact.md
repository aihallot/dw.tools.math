# dw.tools.math — Compact DWF handover after RS277 pushed

Date: 2026-11-21 (Europe/Brussels)  
Operator repository: `aihallot/dw.tools.math`; branch `main`  
Windows checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`  
Workflow: executor **0.1.66**, guidance **0.26** (verify active versions on resumption).

## Exact GitHub authority at handover

- **`main` HEAD verified:** `aed67c6e5b2a4ccceb87e9737a74bbf8e1f0ff6b`
- **RS277:** `succeeded`, attempt `001`, **first-pass true**, `priorInvocationFailures.count=0`.
- RS277 delivery commit message: `test: qualify published bounded Statistics binaries on Linux net11`.
- Durable report: `.aura/workflow/reports/RS277/attempt-001.json`.
- Changed product artifact: `docs/engineering/ProbeM9WslNet11PublishedBoundedStatistics.cs`.
- `.aura/workflow/state.json` has **`pendingRun=null`**. **RS278 has NOT been prepared** on the checked `main`; `.workflow/next-run/request.json` was absent.
- No follow-up operator command is valid until RS278 is prepared. A bare `dwf run next` currently would say `Next run has not been prepared yet.`
- This handover is on a **separate branch** only; do not merge it or alter `main` as part of handover. GitHub `main` and fresh durable receipts always supersede these recorded facts.

## Product delivery and consumer handoff

Math has delivered generic unbiased bounded integer sampling on the **existing** `SplitMix64Random`:
- `ulong NextBoundedUInt64(ulong exclusiveUpperBound, int maxRawDraws)`
- `long NextInt64Inclusive(long lowerInclusive, long upperInclusive, int maxRawDraws)`
- `splitmix64/v1` raw stream and checkpoints preserved; separate bounded transform identifier `splitmix64-bounded-rejection/v1`.
- Exact rejection threshold `(2^64 mod bound)`, positive raw-draw budget including rejected words, invalid-argument/budget/counter refusals, replay, independent reduced-domain proof. Noncryptographic.

**Physically published and independently qualified in private `NUGET_LOCAL`:**
- `dw.tools.math.statistics 20260.20261120.1001`
- Archive SHA-256: `feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1`
- Embedded DLL SHA-256: `684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea`
- Exact immutable dependency `dw.quantities [20260.20261102.1001]`, archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`.
- Older `dw.tools.math.statistics 20260.20261106.1001` remains immutable and does **not** contain bounded sampling.

Evidence chronology:
- RS266 Assessa intake; RS267 native/canonical M9 admission (2 preselection refusals).
- RS268 source behavior (1 refusal), RS269 candidate (1 refusal), RS270 Windows net11 package-only consumer (1 refusal).
- RS271 private readiness first-pass; RS272 authorized **disposable-feed** publisher rehearsal (1 refusal); owner separately ran guarded `--publish`; RS273 physical private-feed readback and twelve-check independent Windows net11 consumer, **first-pass**.
- RS274 Assessa package handover (1 refusal); RS275 reconciled MATH-XR-004 request first-pass; RS276 evidence-backed lifecycle assessment first-pass; **RS277 Linux/WSL net11 published-package twelve-check consumer first-pass**.
- RS277 is **not** proof of Assessa application adoption.

Assessa was informed by an operator-sent bootstrap prompt. Math handoff is committed in `docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md` and source request in `docs/coordination/requests/MATH-XR-004-assessa-bounded-integers.md`. Assessa owns `GENERATE-001 / EG-A-01 / T002`, fractions F0/F2, recipe/session/identifier/answer-protection semantics. **Do not mutate sibling repositories.** Do not presume Assessa has finished its work.

## M9 planning integrity / next work

- Canonical `docs/planning/backlog.json`: `plan_version=0.1.124`, M9 release/work-package/chunk/tasks remain **planned**.
- Native `.aura/workflow/plan/project.json`: M9, M9-W01, M9-W01-C01, both tasks and six RED/GREEN/verify subtasks remain **not-ready**.
- `docs/planning/decisions/m9-w01-c01-evidence-lifecycle-assessment.md` (RS276) explicitly rejects retroactive `done` transitions: previous runs did not perform native transitions/one achievement per completed node. Keep product history honest.
- `docs/planning/ValidateNativeDwfAdoption.cs` and `docs/planning/ValidateAssessaM9Admission.cs` assert the current M9 state, so any future lawful progression requires deliberate validator/projection reconciliation.
- Candidate RS278 priority: **one substantive, bounded Math-owned follow-up**, ideally update the consumer delivery documentation to include the new RS277 Linux/WSL evidence, then return to accepted general Math backlog. Avoid endless ceremonial qualification, arbitrary history rewrites, or work depending on unverified Assessa output. If selecting a different task, inspect current plan and choose based on actual dependencies.

## Workflow and response contract

1. Read in full and in this order: `docs/workflow/mantra.md`, `docs/workflow/constitution.md`, `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, **this compact handover**.
2. Consult only fresh current authority and the specific documents/receipts above; do not reload prior conversational history. Verify actual `main` and DWF pending state before authoring.
3. On `pushed`: inspect actual durable attempt report, prepare **one next run**; on `failed`: inspect newest failure evidence and **correct the same run** using true pre-attempt/allocated-attempt distinction and accurate retry number. Never invent first-pass status or silently advance a failed run.
4. Keep preparation as GitHub writes; **`.workflow/next-run/request.json` must be the last GitHub write** on preparation. Do not edit other repositories or published archives.
5. Operator generally runs only **`dwf run next`** after you have prepared one pending run; do not paste extensive scripts into CLI history. External irreversible publication should have authorization and replay/recovery boundaries, not be placed into preparation-safe validation.
6. Final answer in French, compact, with exact operator handback: `DIRECTORY`, `BRANCH`, `PREPARED TIP`, `EXPECTED COMMIT`, `STATUS` including retry and allocated attempts, `EXPECTED NEXT FAILURE` (**the failure label of the very next invocation**, not the subsequent retry), `COMMAND: dwf run next`.
7. Never tell the operator to run a command until the pending run exists on verified `main`. If no run prepared, say so.
