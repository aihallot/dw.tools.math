# dw.tools.math — compact handover after RS045

**Snapshot:** 2026-10-13 (Europe/Brussels). **Purpose:** restart one truthful DWF operator cycle without replaying conversation history. This handover is a checkpoint, **not** executable run material or an authorization to start `dwf run next`.

## Operator identity and authority

- **Repository:** `aihallot/dw.tools.math` (private); executable checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`; execution branch: **`main`**.
- **Verified GitHub `main` at creation:** `a5b886f4a624bc501d5ec0975298341ee0715319` — `feat: qualify frequency weighted statistics and close descriptive boundaries`.
- **Handover-only branch:** `handover/math-rs045-20261013-context-compact`, based on that exact `main` commit. **Do not use this handover branch as the operator execution branch.** New run preparation belongs on the verified `main` execution identity.
- **Workflow operating package:** observed DWF **0.1.59**, embedded guidance **0.22**. Start by reading active `docs/workflow/mantra.md`, then `constitution.md` and `bootstrap.md`; embedded executing-tool guidance prevails if these copies differ. Then read the project `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, `AGENTS.md`. Before authoring use `docs/workflow/run-authoring-reference.md`, `docs/workflow/payload-authoring.md` and `docs/engineering/dotnet-authoring-checklist.md`.
- Current source of truth: newest `main`, `.aura/workflow/state.json`, `.aura/workflow/plan/project.json`, `docs/planning/backlog.json`, last durable report and `workflow/handoff/current`. Newer remote evidence **overrides this snapshot**.

## Latest terminal run — verified success, no pending run

**RS045** — `M3-W02-C01-T2` frequency weights and explicit nullable-missing policy, **retry 0; attempt 1 succeeded; zero preparation/allocated-attempt failures**.

- Result commit: `a5b886f4a624bc501d5ec0975298341ee0715319`.
- Durable report: `.aura/workflow/reports/RS045/attempt-001.json`, `outcome=succeeded`, tool `0.1.59`, **no boundary violations**. Handoff: `.aura/workflow/handoff/current.json` at `workflow/handoff/current`.
- Passed: **23 new GREEN tests, 44 statistics tests total, 109 quantities tests, 81 numerics tests**; independent managed Math.NET 5.0.0 descriptive smoke; locked .NET 10 build and local `dw.tools.math.statistics` **0.4.0-preview.2** package. No public feed push.
- Qualified receipts: `docs/planning/evidence/M3-W02-C01-contract-qualified.json` (T1/RS044) and `docs/planning/evidence/M3-W02-C01-boundaries-qualified.json` (T2/RS045). Scientific policy: `docs/planning/decisions/m3-w02-c01-statistics-policy.md`.
- Canonical product backlog **`plan_version=0.1.44`**. `M3-W01` done; `M3-W02` in progress; `M3-W02-C01` **done**, both T1/T2 and their children done.
- Native DWF plan remains reconciled at **496 nodes** (8 milestones, 17 work packages, 53 phases, 106 tasks, 312 subtasks). `M3` in-progress.
- Current `.aura/workflow/state.json`: `activeMilestone=M3`, `activeWorkPackage=M3-W02`, `activePhase=null`, `activeTask=null`, `activeSubtask=null`, **`pendingRun=null`**.
- `.workflow/next-run/request.json` **absent** on `main` after success. **RS046 is NOT prepared, selected, or executed** in this handover.

## Immediate next product scope — prepare only ONE run

**Proposed RS046** (ID conditional on newest GitHub evidence): `M3-W02-C02 — Quantiles and correlations`, first accepted task **`M3-W02-C02-T1`**, with native children `-R/-G/-V`. Both backlog and native DWF nodes already exist; no topology insertion or sibling/Discovery migration required.

Grounded acceptance from `docs/planning/backlog.json`:

- Quantile **Type-7** for `[0,10]` at `p=.25` is **2.5**; **nearest-rank** on the same input is **0**. Conventions must be explicit rather than inferred.
- Correlation of increasing affine vectors is **1**; cover covariance/normalization as mathematically appropriate.
- Preserve separate treatment of exact rationals and finite binary64; declare interpolation precision/loss and admitted finite-input/size budgets. Avoid provider-dependent types.
- Reserve the accepted **`M3-W02-C02-T2`** for complete boundary/integration policy, including typed undefined correlation on constant input, NaN/empty/singleton, null/missing semantics, degenerate variance and Decision interoperability conventions. **Do not claim C02 is complete after T1.**
- Use independent hand/symmetry/invariance oracles and a controlled RED followed by GREEN; run exact affected tests first. Existing statistics package `0.4.0-preview.2` is the current qualified baseline; any successor version must be explicitly qualified, not guessed.

Author a bounded, nontrivial C#/.NET 10 payload and only necessary staged files on `main`. Keep `M3-W02` and C02 **in-progress** if T2 remains, mark only genuinely completed T1 and `-R/-G/-V` done, and supply exactly matching contribution achievements. Maintain product backlog/native DWF/projections and evidence together. `request.json` is the **final GitHub write** in a multi-file preparation; verify its commit touches only that path. **No normal operator command until that request exists and its exact preparation tip is verified.**

## External dependencies — observe, do not block normal Math work

- **Workflow issue #139**: <https://github.com/aihallot/dw.tools.workflow/issues/139>. The last observed Workflow development success was **RS710** on `f8e61a64fe46b9151c193216e9a1f5bec521e715` (2026-10-13), concerning immutable-generation/sibling reader semantics. **Do not infer stable consumer delivery** of new native work-package authoring plus sibling switching: Math's installed SDK remains `0.1.59` without that verified adoption. No Math DWF mutation may edit Workflow.
- **Neutral `dw.discovery`** is under `aihallot/dw.tools/dw.discovery/` (last observed portfolio `main` `c0622b95cfcb9f00b77fa78f6f2b0c81d6eef50f`, RS007). It remains an independent documented/planned extraction, **not** a qualified neutral Math dependency, agent skill, or Aura broker. Decision keeps its own current implementation meanwhile.
- **Pinned Decision interoperability request:** Decision commit `5d3af8580ea984425b314d589def18e031172c2c`; protocol `dw.discovery/1.0.0-draft.1`, manifest schema 2, **13 fixtures** (2 accept/11 reject). Raw schema SHA-256 `678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170`; raw fixture SHA-256 `5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e`.
- Math's `docs/discovery/qualification/portable-protocol-preflight.json` reports an independently executed **JavaScript-only 13/13 preliminary semantic check**. The Math-owned `ValidatePortableProtocol.cs` is staged source **not proven compiled/executed** by a Math DWF receipt. **Do not claim Math/Decision implementation conformity, generated Math skill or BrokerReady**. The `docs/planning/decisions/agent-discovery-interoperability.md` ADR has historical references to M3-W01/C02 being outstanding; current durable 2026-10-13 evidence supersedes those portions.
- When a neutral package and appropriate DWF support actually become qualified and installed, consider the separately approved draft `M3-W03` Discovery work package. Do not edit native `.aura/workflow/plan/project.json` with raw JSON, fabricate plan nodes or create an artificial gated run.

## Failure-prevention checklist learned from last cycles

- Run/project metadata: `contribution.summary <= 500` characters; each debt note `<= 300`; single-line. Check all other active SDK limits from the repository reference before final selection.
- MSTest 4 with warnings as errors: avoid `MSTEST0032` constant-vs-constant assertions; prefer actual runtime boundary oracles and verified assertion signatures.
- In `Dw.Tools.Math.*` namespaces, qualify CLR arithmetic helpers as `global::System.Math.Abs/Min/Max/Sqrt` to avoid `CS0234` shadowing.
- DWF authoring: no hand-written JSON traversal of workflow authority or manual workflow SHA hashing. Use bounded `p.Require`, `p.Json.EditObject`, `p.ProjectPlan` and SDK-reported `AfterSha256`.
- Every product node transitioned to `done` needs **existing, nonempty evidence arrays** (including a completed work package). Validate projected backlog using `docs/planning/ValidatePlan.cs` in `--write` mode **inside the payload only**, then `--check`.
- Re-entry must accept the exact baseline and applied target. When a phase/work-package completes, reconcile the active DWF cursor in the same controlled payload (declare `.aura/workflow/state.json` in mutations), and align `Workflow.RequireCurrentRun` with both cursor states. Do not alter execution history or attempt counters.
- Avoid redundant full suites and repeated operator ceremony; substantial run slices and small independent falsifying tests first. Canonical project language **English**, conversation French. Names lowercase for project/solution files; compiled artifacts outside sources under root `build/`; no Python build/validator, no paid CI assumption, no sibling repository edits.

## Operator response convention

Every normal `pushed`: inspect **new** durable report, state, GitHub `main` and latest evidence, then prepare **one** substantial successor. Every `failed`: read the latest failure evidence branch, distinguish pre-attempt from allocated attempt, preserve the same RS and exact retry/attempt semantics, and publish a corrected `request.json` last. Exception: an explicit question or meaningful cycle-end decision/handover.

End the operator-facing reply with:

```text
DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math
BRANCH: main
PREPARED TIP: <verified exact GitHub commit, or none>
EXPECTED COMMIT: <run's exact expected result commit, or none>
STATUS: <run id, exact retry/attempts, and durable state>
COMMAND: dwf run next   # only once an executable request is truly prepared
```

**Resume instruction:** verify current `main` before acting, then author **RS046** on the admitted C02-T1 scope (unless newer durable evidence changes the decision), with `request.json` last. Do not re-run RS045, reconstruct old cycles, or treat this handover branch as the working checkout.
