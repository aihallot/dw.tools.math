# dw.tools.math — compact DWF handover (RS032, 2026-10-09)

**Authority:** `aihallot/dw.tools.math`, branch `main`. This handover is an
operator resume aid, not a second product roadmap. **Read it in full**, then
check GitHub `main` and only the current authorities listed below. More recent
durable evidence on GitHub takes precedence over this document.

## 1. Current verified situation

- **M1** exact quantities, inherited standard units/expressions and isolated
  local NuGet consumption is `done`; AURA adoption is not claimed. Its request
  `docs/coordination/requests/MATH-XR-002-aura-adoption.json` remains
  `draft` / `transmission: none`.
- **M2-W01** mathematical IR is `done`, including the immutable typed model,
  bounded DAG and matrices, and deterministic `math-ir/1` JSON transport.
  RS031 `attempt-001` succeeded with no boundary violations; canonical
  source/qualified evidence:
  `docs/planning/evidence/M2-W01-C03-boundary-qualified.json`.
- **RS032** targets `M2-W02-C01-T1` (first executable composition
  result/capability contract); `M2-W02` and `M2-W02-C01` are to become
  **in-progress**, not `done`, and `T2` remains **planned**.
- Accepted pre-RS032 plan version: **`0.1.30`**. Planned RS032 result:
  **`0.1.31`**. Current canonical backlog and native plan on `main`
  are still in their pre-run states; no RS032 attempt has committed a product
  mutation.
- RS032 expected result commit message:
  `feat: add inspectable math composition result and capability contracts`.

## 2. Exact latest RS032 failure and correction

Durable pre-attempt evidence:

- Invocation: `run-next-20261009T131505Z-8998af7ffcf27a70`
- Branch: `workflow/evidence/preselection-rs032/run-next-20261009T131505Z-8998af7ffcf27a70`
- Report: `.aura/workflow/evidence/run-next-20261009T131505Z-8998af7ffcf27a70/report.json`
- Handoff snapshot branch:
  `workflow/handoff/invocations/run-next-20261009T131505Z-8998af7ffcf27a70`
- Handoff file:
  `.aura/workflow/handoff/snapshots/run-next-20261009T131505Z-8998af7ffcf27a70.json`
- Terminal handoff ref at failure:
  `refs/heads/workflow/handoff/current` =
  `a10dc92ac3ac9c1a5a40f089e2536ce07339c4b3`.
- Report: `failureStage=pre-attempt`,
  `failureClassification=workflow-refusal`,
  `attemptAllocated=false`, `workingTreeChanged=false`;
  `run.retry=0`.

**Exact failure:** `classification=controlled-progress`:
`Controlled progress achievement targets project node 'M2-W02' which did
not complete in this run.`

The original RS032 request incorrectly recorded achievements for two *activated
but not completed* nodes: `M2-W02` and `M2-W02-C01`.
**Correction for retry 1:** remove those two entries from
`contribution.achievements`. Preserve only four actually completed nodes:
`M2-W02-C01-T1-R`, `M2-W02-C01-T1-G`,
`M2-W02-C01-T1-V`, and `M2-W02-C01-T1`.
Keep the work package and phase in `projectNodes` because they are legitimately
activated. Do **not** converge either ancestor to done or alter the accepted
scope to make the gate pass.

**Next operator status when retry is prepared:**
`RS032 · retry 1 · attempts alloués 0`.
All real compilation/test outcomes still require execution by the local DWF
runner; never invent a successful run.

## 3. Prepared RS032 functionality (do not reconstruct)

The preparation on `main` already contains all staged files and a C# typed
SDK payload in `.workflow/next-run/payload.cs`. Its `request.json` has:

- **16 mutation paths**, **8 staged product/documentation files**.
- New `dw.tools.math.composition` .NET 10, locked NuGet graph and local
  package check, provider-neutral typed results and matrix capability
  inspection; no provider execution or AURA permission follows from discoverability.
- Exact/approximate/unsupported/budget result statuses, no fabricated final
  value on failure, bounded provenance.
- One controlled semantic RED via temporarily misadvertised matrix capability,
  then **7 independent GREEN tests** and IR/quantities/foundation/architecture
  regressions.
- Planned durable proofs:
  `docs/planning/evidence/M2-W02-C01-contract-red.json` and
  `docs/planning/evidence/M2-W02-C01-contract-qualified.json`.
- Target node lifecycle: `M2-W02` in-progress; `M2-W02-C01` in-progress;
  `M2-W02-C01-T1` and its three subtasks done;
  `M2-W02-C01-T2` planned.
- Target-reentry preflight **passed** during the observed failed RS032 invocation,
  as did planning, native-DWF, transfer architecture and diff-check validation.
  The refusal was solely controlled-progress achievement ownership.
  Do not repeat large diagnostic exploration without fresh contrary evidence.

After the next `pushed`, inspect
`.aura/workflow/reports/RS032/attempt-001.json`, validate result and
boundary evidence, and prepare **RS033** for
`M2-W02-C01-T2`: public provider-visibility, permission independence,
absence of final value, result/provenance boundaries, then close C01 if
qualified. Be substantive but avoid false claims of execution or AURA adoption.

## 4. Authoritative files to read on a new chat

Read **only these first**; expand specific referenced content only if needed:

1. `docs/handover/dw-tools-math-RS032-2026-10-09.md` (this file).
2. `.workflow/next-run/request.json` and `.workflow/next-run/payload.cs`
   on current GitHub `main`.
3. `docs/planning/backlog.json` and
   `.aura/workflow/plan/project.json`.
4. `.aura/workflow/state.json`, most recent
   `.aura/workflow/reports/RS031/attempt-001.json`, and
   `docs/planning/evidence/M2-W01-C03-boundary-qualified.json`.
5. The latest **RS032** evidence branch report and terminal handoff above.
   If a newer invocation exists, its durable evidence and retry count win.
6. `docs/workflow/payload-sdk-reference.md` **only the relevant operation
   sections**, not the entire 55 KB document.
7. Staged sources/tests **only when a new failure requires a targeted repair**.

No speculative local files, sibling repositories, old attempt reconstructions
or long transcript replay.

## 5. Strict authoring/operator discipline

1. User reports **`pushed`**: verify the exact newest durable succeeded
   attempt report, changed paths and boundary violations; inspect current
   backlog/native plan; then prepare the **next substantial run**.
2. User reports **`failed`**: fetch newest durable failure report **and**
   terminal handoff; use the exact `run.retry` and allocated-attempt state;
   repair **the same RS###**, preserving qualified scope. A pre-attempt
   `run.retry=0` failure means next prepared `retry 1`, attempts 0.
3. A user question that demands a precise answer or a major-cycle decision
   is an exception to automatic next-run preparation.
4. Treat `docs/planning/backlog.json` as product authority and
   `.aura/workflow/plan/project.json` as native DWF lifecycle authority.
   Change product files with typed Payload SDK; run
   `ValidatePlan.cs --write` **inside the payload**, not a
   preparation-safe structural validator. Keep target re-entry deterministic.
5. **Controlled progress achievements must reference nodes completed in this
   run**, not merely activated or remaining in progress. A completed task can
   have its parent in `projectNodes` without awarding an achievement to
   that parent. Do not use false `done` transitions to satisfy a request.
6. Account for previous failure patterns: unused C# constants (CS0219),
   MSTEST0032 always-true assertions, off-by-one expanded-DAG budgets,
   incorrect active-hierarchy assumption after a milestone close, and
   release validators pinned to prior plan versions.
7. **Publish `.workflow/next-run/request.json` LAST** on GitHub
   `main`; immediately verify it is the last commit and report the exact
   SHA, request commit message, `RS###`, retry and attempts. Never edit
   `dw.tools.workflow`, AURA or any sibling repository.
8. Every response that prepares a run finishes with these **exact six lines**,
   with no extra text after them:

```text
**DIRECTORY:** `D:\data.dev\github\ai@hallot.net\dw.tools.math`
**BRANCH:** `main`
**PREPARED TIP:** `<exact final GitHub main SHA>`
**EXPECTED COMMIT:** `<request.commitMessage>`
**STATUS:** `RS### · retry N · attempts alloués M`
**COMMAND:** `dwf run next`
```

Keep the prose compact. Document external requests as draft/untransmitted
until real AURA evidence exists.

## 6. Suggested new-conversation bootstrap

```text
$hop @GitHub

Reprends aihallot/dw.tools.math, branche main, depuis
docs/handover/dw-tools-math-RS032-2026-10-09.md.

Lis intégralement le handover, puis vérifie uniquement les autorités récentes
qu'il indique (main, backlog, plan DWF natif, preuves terminales et handoff).
La dernière preuve RS032 est un failed pré-attempt controlled-progress :
deux achievements visaient des ancêtres seulement in-progress.
La correction attendue retire ces deux achievements, sans changer le scope,
pour RS032 retry 1, attempts alloués 0. Vérifie d'abord si la correction
et une preuve plus récente sont déjà publiées.

Respecte notre protocole : à chaque pushed, vérifie les preuves durables puis
prépare le prochain run substantiel ; à chaque failed, récupère l'erreur et
le retry exact, puis corrige le même run. Pas de reconstruction historique ;
pas d'écriture dans les dépôts frères. Publie request.json en dernier.
Termine chaque réponse d'opérateur par les six lignes exactes DIRECTORY,
BRANCH, PREPARED TIP, EXPECTED COMMIT, STATUS, COMMAND.
```

**Resume tip:** use the exact current GitHub `main` commit, not the older
pre-failure authority baseline SHA embedded in the report.
