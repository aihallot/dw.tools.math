# dw.tools.math — compact operator handover (2026-10-18)

## Authority and execution identity

- Repository: private `aihallot/dw.tools.math`; operator branch: `main`.
- Exact operator checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`. Never substitute a different worktree.
- Operator command: `dwf run next` **only after** one complete and verified request is selected on `main`.
- Communication: French. Repository source, documentation, evidence and commit messages: English.
- Observed DWF tool version: `0.1.59`; project guidance version: `0.22`. The executing workflow's active guidance takes precedence if different.
- Never modify `dw.tools.workflow`, AURA, Decision, MCDM, `dw.discovery`, or any sibling repository. No external CI assumptions, no public NuGet or licensing decision without owner authorization.

## Last verified durable result — RS092

- GitHub `main` HEAD, verified after operator's `pushed`: `8ba4b011cf59459840fc7d34c7fe359bc71709b7`.
- **RS092 succeeded, attempt 001**; outcome and changed paths in `.aura/workflow/reports/RS092/attempt-001.json`.
- **NOT first-pass**: report records **one pre-attempt preparation failure** (workflow refusal, no attempt allocated), then successful attempt 001. Do not misclassify first invocation success.
- Exact product/result commit message: `docs: split M6 W02 C03 scientific scope into three native DWF phases`.
- Backlog version `0.1.91`; native plan **523 nodes** (8 milestones / 17 work packages / 56 phases / 112 tasks / 330 subtasks). `M6` and `M6-W02` in progress; `M6-W01`, `M6-W02-C01` and `M6-W02-C02` done.
- RS092 made a **structural planning split only**, not scientific code delivery. Source: `docs/planning/decisions/m6-w02-c03-size-l-split.md`; durable qualification: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`.
- The original size-L `M6-W02-C03` remains planned/not-ready, preserving its T1/T2, dependencies and downstream gate. Three new size-M children are **planned/not-ready**:
  - `M6-W02-C03-S01`: finite 2D/3D Euclidean norms, distance, projection.
  - `M6-W02-C03-S02`: direct discrete Fourier transform and linear convolution.
  - `M6-W02-C03-S03`: uniformly sampled first differences and moving averages.
- No geometry, DFT or time-series implementation certified by RS092; no public release or downstream AURA/Decision/MCDM adoption claimed.

## Handover-time selection state: no run prepared

- `.aura/workflow/state.json`: `activeMilestone=M6`, `activeWorkPackage=M6-W02`, phase/task/subtask null; **`pendingRun=null`**.
- `.workflow/next-run/request.json` is **absent** on `main`. No RS093 attempt allocated or run selected.
- Last verified DWF tip is the above `main` SHA, **not** the handover branch/commit.
- This conversation-limit handover is a cycle boundary; it intentionally does not invent an unreviewed RS093 payload or tell the operator to run a nonexistent request. The next conversation's **first development interaction must prepare RS093**, not skip ahead. Re-check fresh GitHub evidence before acting.

## Exactly one next work item: prepare RS093

Target **`M6-W02-C03-S01-T1`**, finite Euclidean geometry contract and outcome, **retry 0 / allocated attempts 0** until fresh evidence says otherwise.

- Qualify a small, independently testable **.NET 10 C#** public numerical contract for finite 2D/3D vectors: Euclidean norm, distance and projection on a nonzero axis. Use explicitly bounded binary64 arithmetic, distinguish validated approximate outcomes from exact promises, and avoid implicit units or coordinate interpretations.
- Required independent oracles: norm(3,4)=5; distance((0,0),(3,4))=5; projection((3,4) onto (1,0))=(3,0). Design tolerance and refusal semantics from inspected existing Math contracts, not guessed signatures.
- **Before coding**, inspect the current scientific library, test project, NuGet versions and most recent C02 T1/T2 examples; avoid duplicate public APIs and helpers. One genuinely compiled missing-public-API RED, targeted independent GREEN tests, retained relevant regressions, and isolated NuGet-only consumer qualification consistent with active project standards. Do not claim outcomes until the controlled run proves them.
- Close only genuinely evidenced `S01-T1-R/G/V` and `S01-T1` in BOTH accepted backlog and native plan, with truthful unique achievements. Keep `S01-T2`, S01 phase, S02/S03, parent C03, M6-W02 and M6 unfinished. Keep all bounded/refusal/cancellation and source-free package integration audits for S01 T2 as separately accepted work.
- A proposed (not yet selected) result message is `feat: qualify finite Euclidean geometry norm distance and projection contracts`. The eventual **actual** expected commit must come from the verified selected request.
- Preserve the math package's locally qualified `1.1.0-preview.4` baseline until an explicitly justified new local preview is staged; no public package publication.

## Bootstrap reading order — minimal fresh authorities

1. Read this entire handover once. Then read `docs/workflow/mantra.md`, `docs/workflow/constitution.md`, `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, and `docs/engineering/dotnet-authoring-checklist.md`. Treat the active executable DWF policy as authority over stale repository guidance.
2. Check current GitHub `main` HEAD, `.aura/workflow/state.json`, `.workflow/next-run/request.json` (if any), `.aura/workflow/plan/project.json`, and `docs/planning/backlog.json` (focus on `M6-W02-C03-S01` only).
3. Check **latest** RS092 durable report and RS092 split decision/evidence above; if newer durable proof exists, newer GitHub `main` and that proof override this snapshot. Never reconstruct RS001–RS091 history.
4. For RS093 authoring only, inspect relevant recent scientific source/tests/project/package/consumer fixtures and active DWF `run.json`/`preparation-request.json` examples. Do not scan the whole repository or multiply run scope.

## Non-negotiable DWF operator loop

- **After `pushed`**: verify latest durable report, invocation failure history (first-pass separately), backlog, native plan, GitHub result HEAD and scientific receipts, then prepare **exactly one** substantive successor.
- **After `failed`**: inspect latest durable failure/refusal reference, failure stage/class, attempted allocation, retry index and correction provenance, then correct **the SAME run**, not a new successor. Pre-attempt refusals use no allocated attempt, but disqualify first-invocation success.
- Every normal interaction prepares/corrects a run. Exceptions: focused owner questions and major cycle/architecture decisions such as this handover.
- Make no direct product mutations outside the controlled DWF payload. Plan/version projections must agree with native transitions; no historical synthetic achievements.
- Risk-based validation: smallest failing surface -> exact tests -> relevant regressions -> broad certification only at actual gate. Prefer planned validation; explicit only for a genuine gap.
- Prevent repeat mistakes: inspect exact public signatures and file paths; static-check warnings-as-errors (`CS0219`, `CS0136`, `CS8321`, `MSTEST0032`), re-entry baseline/target truth, test restore/build/pack sequence, C# payload scope, and exact JSON statuses.
- Contribution contract: `summary <= 500`, `value <= 1000`, `nextAction <= 500` characters; <=20 unique debts (<=300 each); <=20 unique node achievements (<=1000 each). Reserve margin.
- Stage all source/tests/payload/validations and validate inventories first. Publish `.workflow/next-run/request.json` **LAST**, with **that file alone in the last GitHub commit**, then inspect `main` HEAD, request content, mutation paths, staged files, preconditions, expected commit, retry and allocated attempts.
- Only after fully verified selected request may the operator receive `COMMAND: dwf run next`.

## Response footer contract (mandatory)

End every routine operator reply with exactly these six labeled fields, with truthful live values:

DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math
BRANCH: main
PREPARED TIP: [latest verified main SHA, or NOT PREPARED]
EXPECTED COMMIT: [exact selected request commitMessage, or NOT SELECTED]
STATUS: [RSnnn — retry N prepared — attempts allocated X — not executed; or accurate unprepared state]
COMMAND: [dwf run next ONLY if request is selected and verified; otherwise DO NOT RUN]

Do not present the handover-branch commit as a prepared run tip. No need to read historical handover files in the next session.
