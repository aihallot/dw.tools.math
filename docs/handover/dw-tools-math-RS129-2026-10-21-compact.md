# dw.tools.math — compact handover: RS128 pushed, RS129 prepared (2026-10-21)

> **Status:** GitHub `main` contains the selected **RS129** preparation. **RS129 has NOT been executed or allocated an attempt.** This handover is stored only on its dedicated `handover/` branch; it is **not** a DWF input or a second product authority.

## 1. Minimal bootstrap — read this first, then current authorities

- Repository: `aihallot/dw.tools.math`, canonical branch `main`; Windows operator checkout `D:\data.dev\github\ai@hallot.net\dw.tools.math`.
- **Prepared GitHub `main` tip:** `33097fc9c7424d485be14e4e4990a7b830c335f5`, commit `workflow: select RS129 source-pinned Math delivery portfolio`. Leave this commit intact for the next DWF run. Handover branch starts from this exact tip; this document exists **only on the handover branch**.
- **Latest confirmed push:** RS128 successful, commit `d99c770cd581430c089b12578359b0d832f9305b`, `.aura/workflow/reports/RS128/attempt-001.json`: attempt **001**, retry **0**, pre-attempt refusals **0**, `correctionMode=false`, **first-pass YES**. Four fresh local nupkgs, two offline source-free M4/M5 clients, **40** functional probes, `.NET 10.0.402 / win-x64`. No public release.
- **Selected run:** `RS129`, retry **0**, attempts allocated **0**, **NOT executed**. Expected result commit message: `docs: qualify post-M7 Math delivery evidence portfolio without publication`. First-pass of RS129 is **unknown**.
- `.aura/workflow/state.json`: `status=initialized`, `pendingRun=null`, all active cursors null at GitHub prepared tip. This is normal **before** the operator invokes DWF. `.aura/workflow/plan/project.json`: **541/541** native nodes `done`. Accepted product `docs/planning/backlog.json`: `plan_version=0.1.122`, `done`, **8 milestones / 17 work packages / 58 chunks / 106 requirements**. Do not reopen or invent historical progress.
- **Next local operator invocation:** from the checkout, `git pull --ff-only` then `dwf run next` using installed `dw.tools.workflow 0.1.59`. Do not run now from this handover process; await operator result `pushed` or `failed`.

**Authoritative read order in a new conversation:** (1) this document once; (2) `docs/workflow/mantra.md` and `docs/workflow/constitution.md` (active operating guidance wins when different), then `PROJECT-MANTRA.md` and `PROJECT-CONSTITUTION.md`; (3) latest **GitHub main**, DWF `.aura/workflow/state.json`, native plan, latest RS128 report, selected `.workflow/next-run/request.json` and corresponding staged source; (4) only focused current source/reports if needed. **Never reconstruct all RS001–RS128 history or fill context with old reports.** GitHub/DWF durable later evidence overrides this handover.

## 2. RS129 prepared, scope and files

RS129 is a **no-progress, read-only qualification review of the existing local delivery evidence** for the owner's previously authorized track **A — qualify delivery without public publication**. It does **not** build, pack, test consumers anew, update stale README, publish, send requests or approve M8. DWF `projectNodes=["M7"]` references the already-done node; **`contribution.achievements=[]`**.

- Final DWF selector **`.workflow/next-run/request.json`** was the **last GitHub write on main**. Its scope is `RS129`, **137 checked preconditions**, **three staged files**, **six declared mutation paths**, **five structural validations** (`planning`, `native-dwf`, `transfer`, `diff-check`, `portfolio`). `timeoutSeconds=1200`, payload limit `1000` seconds. Previous RS128 first-pass lineage must remain true.
- C# payload: `.workflow/next-run/payload.cs`, via typed DWF `PayloadContext`, `RequireCurrentRun("RS129",null,null,null,null,null)` and `RequirePriorSuccess("RS128",1,"docs: qualify fresh offline Math symbolics and codec consumers","0.1.59",0,0,0,true)`. Baseline/target reentry; stage three complete files; execute local read-only checker before/after receipt; `p.Files.WriteComplete` one receipt. **No `ConvergeNodeToDone`, no native/product state transitions**.
- Staged deliverables: `.workflow/next-run/files/docs/planning/decisions/post-m7-delivery-portfolio.md`, `.workflow/next-run/files/docs/distribution/post-m7-delivery-portfolio-matrix.md`, and `.workflow/next-run/files/docs/planning/ValidatePostM7DeliveryPortfolio.cs`. They install at matching paths without the `.workflow/next-run/files/` prefix.
- Expected receipt **only after real RS129 success**: `docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json` with source SHA pins, **five** reports, **13** packable projects, **7** independent post-M7 consumer executions, **95** numbered probes plus a separately passing unnumbered exact smoke; **3** first-pass YES, **2** first-pass NO, and **3** prior pre-attempt refusals (RS124 one, RS127 two). All 541 original native nodes remain done.
- **No successful RS129 attempt exists** at preparation. The C# code and selector were statically inspected against GitHub sources, **not locally compiled/tested by this assistant**. Only an actual DWF report can establish success or a new refusal. Never call RS129 first-pass until its `priorInvocationFailures` is inspected.
- External/owner negatives remain: no NuGet.org publication, no stable ABI, no another-RID certification, no transmitted AURA/Decision/MCDM requests, no generated Math discovery/agent skill, no advanced M7 precision/tensor/DAG/PlotModel/formal provider delivery, no new M8.

## 3. Minimal science/delivery truth to retain

| Report | Outcome/first-pass | Durable scoped observation |
|---|---|---|
| `RS124` | succeeded attempt 001; **NO**, 1 pre-attempt refusal | Locked restore, full Release solution build/tests, three representative local preview archives; no source-free consumer rerun |
| `RS125` | succeeded attempt 001; **YES** | Two offline independent analysis/scientific NuGet consumers, 16 numbered probes |
| `RS126` | succeeded attempt 001; **YES** | Seven-package distribution closure, one eight-probe source-free consumer, current-only API and assembly SHA fingerprints |
| `RS127` | succeeded attempt 001; **NO**, 2 pre-attempt refusals | All **13** local preview archives repacked; M3 statistical/numerical consumer with 31 numbered probes and separately successful unnumbered exact-quantity smoke |
| `RS128` | succeeded attempt 001; **YES** | Four fresh symbolic/codec archives, M4 source-free 32 probes and M5 Markdown/OMML source-free 8 probes |

The four independent consumer qualification runs RS125–128 total **7 executions and 95 numbered probes**, **not** seven distinct independently certified public APIs, 47 fresh clients, a coverage percentage, ABI proof or a single end-to-end suite. `tests/consumers/` still contains **47 historical directories**, not 47 currently rerun clients.

`README.md` still says `Status on 2026-10-07: initial planning`. This is user-facing debt, **not corrected by RS129**, because a historical source-pinned validator explicitly checks the old sentence. Correcting it later needs a separate evidence-conscious task. `docs/discovery/README.md` is candidate-only; `docs/coordination/requests/README.md` lists three **unsent** recipient drafts. `global.json` declares SDK `10.0.401` with `latestPatch` and observed local SDK `10.0.402` worked; do **not** demand manual reinstall of 10.0.401.

## 4. Mandatory workflow and owner/operator contract

**Language:** French owner conversation, English repository artifacts. **Engineering:** C#/.NET 10 and BCL; lowercase solution/project identifiers; build outputs under `/build`, minimal `/scripts`, no Python, no GitHub Actions/paid CI, no opportunistic web scraping or provider installation, no changes to `dw.tools.workflow` or sibling repositories. DWF 0.1.59 active guidance is workflow authority.

The owner provides minimal local terminal verdicts:

- On **`pushed`**: first independently verify current GitHub main commit, the new DWF `reports/RS129/attempt-001.json`, the receipt, native plan and accurate first-pass/preparation counts. **RS129 is the end of owner-selected A delivery evidence review**; treat it as a major decision checkpoint. Explain qualifications and ask the owner which new gate to authorize. **Do not manufacture RS130/M8, publication or external communication simply to keep the loop going.**
- On **`failed`**: inspect the *newest* durable `workflow/handoff/current` and its referenced preselection or allocated-attempt evidence. Differentiate a pre-attempt refusal (no allocated attempt) from allocated failure, mutation-boundary/delivery/internal failures. Correct **the same RS129** under DWF; keep exact retry and attempt counters, retain scope and prior provenance, do not weaken validations to obtain green. Prepare the corrected `.workflow/next-run/request.json` as the **last GitHub write** and give the operator only `git pull --ff-only; dwf run next`.
- In ordinary development after `pushed` prepare only the authorized next run; `failed` repairs the same run, except a precise user question or major milestone decision. Never claim a run executed without durable report, never backfill first-pass.
- End with a **compact operator block only**, not a duplicate full script: directory, branch, exact prepared SHA, expected result commit, RS id, retry, allocated attempts, observed first-pass and local command. Only when a new run has actually been prepared do you instruct `dwf run next`.
- `$pcl`: requested hierarchical compact checklist grouped by solution/milestone, showing done/in progress/to do with grounded evidence. `$phof`: **verify pushed → prepare authorized next run → publish a compact `$hof` handover on a separate branch and give the ready bootstrap prompt**. `$fhof`: verify failed → correct same run → handover. Do not use either handover command to rewrite prepared `main`.

**Do not treat handover branch as a source for DWF input.** The next local `git pull --ff-only` must pull **main**. Handover branch is solely for the new assistant's cold start.

## 5. Optimal restart prompt (copy as the next user message)

```text
$hop @GitHub

Resume aihallot/dw.tools.math on main strictly via dw.tools.workflow (DWF).

FIRST read in full this compact handover, on the dedicated branch:
- Branch: handover/dw-tools-math-rs128-pushed-rs129-prepared-20261021-compact
- Path: docs/handover/dw-tools-math-RS129-2026-10-21-compact.md
- Handover commit: use the branch's currently verified HEAD; do not infer any handover SHA.

Verified source of truth at handover:
- main prepared tip: 33097fc9c7424d485be14e4e4990a7b830c335f5
- RS128 pushed: d99c770cd581430c089b12578359b0d832f9305b; attempt 001, first-pass YES.
- RS129 selected: retry 0, attempts allocated 0, NOT executed; expected commit "docs: qualify post-M7 Math delivery evidence portfolio without publication".
- DWF 0.1.59; accepted 541/541 native nodes done; backlog 0.1.122 done.

After reading, verify current GitHub main and only the relevant newer authorities named by the handover. Newer durable GitHub/DWF evidence wins. Do not reconstruct old history or overload context.

Methodology: pushed -> verify and prepare authorized next run; failed -> inspect evidence and correct the SAME run, preserving truthful prior refusal and first-pass counters. RS129 is an owner-decision checkpoint on success: do not invent RS130/M8 or publish/communicate without authorization. Canonical repo language English; owner conversation French. C# only, no Python, no GitHub Actions/paid CI, no sibling/DWF source edits. Every preparation writes request.json LAST. Operator uses only git pull --ff-only; dwf run next.

Always conclude with the compact operator block, and return exactly the prepared result or the decision needed.
```
