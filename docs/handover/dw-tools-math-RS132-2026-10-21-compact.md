# dw.tools.math — compact DWF handover, RS131 pushed / RS132 prepared (2026-10-21)

> **GitHub is the authority.** This Markdown is a restart aid on a separate `handover/` branch, **not** DWF run material. On the operator's `main`, **RS132 is selected but NOT executed**. Fetch newer DWF evidence before trusting any frozen status below.

## 1. Exact resume anchor — read once

- Repository: `aihallot/dw.tools.math`, operator branch `main`, checkout `D:\data.dev\github\ai@hallot.net\dw.tools.math`. GitHub `main` **prepared tip**: `33020632ac0e62f992ec2cac5a5007bb4acded32`, message `workflow: select RS132 compiled Math CLR signature qualification`.
- **Last pushed run**: `RS131`, GitHub result commit `8bc959a77c2915c8c5f17d2070945f52e213f605`, `.aura/workflow/reports/RS131/attempt-001.json` **`succeeded`**, attempt **001**, retry **0**, zero prior invocation failures, **first-pass YES**. DWF `0.1.59`. Its receipt `docs/discovery/qualification/RS131-portable-protocol-qualified.json` records the independent Math C# protocol gate: **13/13 Decision-pinned `dw.discovery/1.0.0-draft.1` schema-2 cases passed (2 accepted, 11 rejected)** on SDK `10.0.402` / `win-x64`. **No** generated skill, reflected CLR bindings, BrokerReady or cross-repo interoperability was claimed.
- `RS130` also pushed **attempt 001 / first-pass YES**, earlier than RS131: current repository README and local package guide truthfully summarize 13 locally packed packages, seven source-free consumer executions, 95 numbered probes and separate exact smoke, preserving historical source pins. Owner-approved sequencing is **A documentation (complete) → B Discovery/AURA qualification (underway) → D coordinated interproject review/transmission (future, authorized separately)**.
- `RS132` is **selected**, retry **0**, attempts allocated **0**, **NOT executed**; `.aura/workflow/reports/RS132/attempt-001.json` absent at preparation. Expected run-result commit message: `docs: qualify four compiled Math CLR Discovery signatures without skill`. Its first-pass result is **unknown** until durable report.
- Math accepted product roadmap `docs/planning/backlog.json` v`0.1.122` is `done`: **8 milestones, 17 work packages, 58 chunks**. Native `.aura/workflow/plan/project.json` **541/541 nodes done**. GitHub `.aura/workflow/state.json` is `initialized`, `pendingRun=null`, active cursors null before the next local DWF selection. **Do not reopen, fabricate M8, rewrite the native plan or fabricate a new M3-W03.**
- The selected `.workflow/next-run/request.json` was the **last GitHub write on main**; **71 source-checked preconditions**, **3 staged files**, **6 declared mutations**, **6 structural validations** (`planning`, `native-dwf`, `transfer`, `diff-check`, `portable-protocol`, `clr-seed`), `contribution.achievements=[]`, `projectNodes=["M7"]`, direct delivery, timeout 1800 sec. This run documents evidence on a completed M7 scope; **no new progress**.

**Required cold-start read order:** read this compact handover fully, then current DWF `docs/workflow/mantra.md` and `docs/workflow/constitution.md` (active installed tool guidance wins if divergent), then `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, then **fresh GitHub main** + state + native plan + newest RS131 report/receipt + selected RS132 request + staged files. Consult only their narrow authorities; do not replay RS001–RS130 or carry entire conversation into context. Newer GitHub/DWF evidence supersedes this document.

## 2. RS132 objective, files and safety boundary

**Objective:** independently verify **four exact opt-in public static CLR signatures** against freshly compiled real Math Release DLLs, with source Git blob pins and canonical signature SHA-256 evidence. **This is not product Discovery work-package admission or skill generation.**

- Selected source: `docs/discovery/agent-surface-candidates.json`, kind `non-authoritative-public-API-classification-seed`, status `proposed-not-implemented-not-admitted`. Exact selected methods:
  1. `math.quantity.exact-pipeline.evaluate` → `ExactQuantityPipeline.Evaluate(IEnumerable<ExactPipelineStep>)`.
  2. `math.replay.try-evaluate` → `ExactReplayRunner.TryRun(IEnumerable<ExactPipelineStep>, ExactReplayContext, ExactReplayCache, CancellationToken)`.
  3. `math.ir.canonical.encode` → `IrCanonicalJsonCodec.Encode(IrNode)`.
  4. `math.ir.hash.semantic` → `IrCanonicalHashes.SemanticStructuralSha256(IrNode)`.
- Product assemblies `dw.quantities`, `dw.tools.math.ir` and `dw.tools.math.composition` are built from current source; the reflection checker loads actual DLLs in ignored `build/artifacts/bin/dw.tools.math.composition/Release/net10.0/`. Exact public static method, parameter types, return type and uniqueness required. Nonselected helpers and instance matrix/vector methods stay excluded.
- DWF payload `.workflow/next-run/payload.cs`: typed `PayloadContext`, `RequireCurrentRun("RS132",null,null,null,null,null)`, `RequirePriorSuccess("RS131",1,"docs: qualify CSharp Discovery portable protocol evidence without skill","0.1.59",0,0,0,true)`; stages three complete files and executes independent verifier `--check` baseline + target, plus **3 local successful commands** (locked solution restore, composition Release build, actual C# `--reflect`). It then records one receipt through typed `p.Files.WriteComplete`. No direct filesystem mutation in the payload, no plan transitions.
- Staged sources:
  - `.workflow/next-run/files/docs/discovery/qualification/RS132-clr-seed-boundary.md` → decision;
  - `.workflow/next-run/files/docs/discovery/qualification/RS132-clr-seed-matrix.md` → four-method matrix;
  - `.workflow/next-run/files/docs/discovery/qualification/ValidateClrSeedReflection.cs` → read-only BCL verifier.
- Expected **only after actual success**: `docs/discovery/qualification/RS132-clr-seed-qualified.json`, status `rs132-four-candidate-static-clr-signatures-reflected-no-agent-skill`. Four observed reflected signatures/hashes, RS131 13 cases retained, 541 nodes retained, all release/external flags false.
- Critical **nonclaims**: **no compiled invocation examples**, no Math-owned generated manifest or skill, no JSON broker adapter, no BrokerReady, no AURA authorization, no direct Decision runtime integration, no public NuGet package, no cross-version ABI/other RID, no `M3-W03` admission. Reflection alone does not create runnable agent tools.
- Static author review completed (exact staged markers, target receipt fields, four candidates, 3 matching execution labels, C# delimiter balance, no forbidden payload local mutation, DWF contribution length checks). **No local .NET SDK is available to the assistant**; only operator DWF can certify compilation/preflight/execution. On `failed`, inspect **newest durable failure evidence** and correct **the same RS132**, without weakening source/broker/approval gates.

## 3. B-stage dependency and D-stage boundaries

- `docs/discovery/README.md` and `docs/planning/proposals/math-agent-discovery-work-package.json` remain **candidate/historical scope**, with a draft `M3-W03-C01/C02/C03` new work package **not merged** into backlog/native DWF. Installed Math DWF `0.1.59` has no documented typed new-work-package authoring mechanism; Workflow [issue #139](https://github.com/aihallot/dw.tools.workflow/issues/139) is feedback, **not proof of delivered capability**. Do not rewrite DWF native plan by hand or hide new product code under a completed M3 numerics task.
- On successful RS132, **investigate whether the current executable DWF workflow supports approved native topology admission** before implementing compiled invocation examples, deterministic generator, manifest and skill. If genuinely blocked, report the exact blocker and request an owner/workflow decision at this major gate. Do **not** invent RS133/M8 merely for progress. The owner already approved B as the next direction, not unsafe edits of workflow authority.
- `docs/discovery/README.md` itself remains source-SHA-pinned by the RS131 evidence; **do not casually rewrite it** during subsequent runs without a proper provenance reconciliation.
- Stage D is **after qualified B**: AURA, Decision, MCDM request drafts under `docs/coordination/requests/` are **unsent**. A conversation preference for D does **not** authorize transmitting requests, modifying a sibling repo, performing an AURA cutover or publishing a package without an explicit action-level owner approval.

## 4. Operator loop, refusal accounting and response convention

All repo artifacts canonical **English**, user conversation **French**. C#/.NET 10, no Python, no GitHub Actions/paid CI, no opportunistic provider/network installation, no cross-repository source changes, no public publishing. DWF owns workflow mechanics. The normal operator command remains:

```powershell
cd D:\data.dev\github\ai@hallot.net\dw.tools.math
git pull --ff-only
dwf run next
```

After **`pushed`**, independently verify live `main`, RS132 `attempt-001.json`, evidence receipt, latest `workflow/handoff/current` and honest `priorInvocationFailures/firstPassSucceeded`. Proceed only with an authorized supported B continuation, stopping for topology decision if required. After **`failed`**, read latest durable preselection/allocated attempt evidence (not guessed history), keep exact retry/attempt counters, correct **RS132 itself**, and write `.workflow/next-run/request.json` **LAST**. No generic shell repairs transferred to operator. After every ordinary `pushed`, prepare a bounded authorized successor unless an actual decision gate has been reached; after `failed`, repair same run unless a direct question takes precedence.

At the end of each answer, give the **compact operator block** (checkout, `main`, prepared exact SHA, expected result commit, retry, allocated attempts, real first-pass and `git pull --ff-only; dwf run next`). Never show a runnable `dwf run next` command unless a next run is genuinely prepared. `$pcl` = concise hierarchical completed/active/future check-list by solution; `$phof` = verify pushed, prepare authorized next run, create compact durable handover and bootstrap; `$fhof` = failed, correct same run and handover.

## 5. Minimal bootstrap prompt

```text
$hop @GitHub

Resume aihallot/dw.tools.math, operator branch main, strictly under dw.tools.workflow (DWF).

Read FIRST and in full the compact handover on:
Branch: handover/dw-tools-math-rs131-pushed-rs132-prepared-20261021-compact
File: docs/handover/dw-tools-math-RS132-2026-10-21-compact.md
Commit: use the handover branch's verified HEAD; the text intentionally contains no self-referential commit hash.

Last verified GitHub main prepared tip: 33020632ac0e62f992ec2cac5a5007bb4acded32.
RS131 pushed first-pass YES, attempt 001, GitHub commit 8bc959a77c2915c8c5f17d2070945f52e213f605.
RS132 prepared, retry 0, 0 allocated attempts, not executed.
Expected result commit: docs: qualify four compiled Math CLR Discovery signatures without skill.
Accepted Math plan 541/541 done, backlog 0.1.122 done, installed DWF 0.1.59.

After the handover, inspect only newer GitHub/DWF source authorities and the current staged RS132 material. Never reconstruct old run history. On pushed, verify durable truth and prepare only the authorized supported B continuation; on failed, correct RS132 from newest evidence. No direct native-plan mutations, fake M3-W03, skill/broker/publication/adoption claims, Python, CI or sibling repository changes. The owner approved A→B→D; actual outbound transmission still requires explicit permission. Always finish with the compact operator block.
```
