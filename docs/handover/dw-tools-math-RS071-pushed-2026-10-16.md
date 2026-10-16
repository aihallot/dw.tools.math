# dw.tools.math — compact handover after RS071 (2026-10-16)

**Purpose:** resume the operator loop without loading old chat history. This note is a snapshot, NOT a DWF execution receipt or an authority above fresh GitHub main. Latest durable evidence always wins.

## Identity and mandatory policy

- Private repository: **aihallot/dw.tools.math**; execution branch: **main**.
- Exact operator checkout: D:\data.dev\github\ai@hallot.net\dw.tools.math
- Normal operator command: **dwf run next**, ONLY after an actual next-run request has been prepared.
- Conversation FR; all canonical source, docs, tests, comments, evidence and commits EN.
- DWF owns workflow mechanics; do not modify dw.tools.workflow or sibling Math consumers (decision, mcdm, AURA, dw.tools/dw.discovery).
- Math is .NET 10 C# only in this repository, not Python; lowercase project/solution paths, compiled artifacts in build/artifacts, local packages only; no paid CI or public NuGet/stable-1.0 claim without owner approval.
- Read the complete current docs/workflow/mantra.md and docs/workflow/constitution.md BEFORE authoring; then PROJECT-MANTRA.md and PROJECT-CONSTITUTION.md. Read docs/engineering/dotnet-authoring-checklist.md before writing C#.
- Latest confirmed executor: **dw.tools.workflow 0.1.59**; refresh if later durable authority differs.

## Verified durable RS071 success

- GitHub main product commit: **0ad64922e27716162282b6da5e7c515e13ab0b14**.
- Message: **feat: qualify exact MathML XML boundaries and close C03**.
- Report: .aura/workflow/reports/RS071/attempt-001.json.
- Outcome: **succeeded, attempt 001, firstPassSucceeded=true, zero prior refusals**, executor 0.1.59.
- Science receipt: docs/planning/evidence/M5-W01-C03-T2-boundaries-qualified.json; exact status **exact-annotated-mathml-xml-security-and-loss-boundaries-qualified**.
- Accepted: one compiled absent-API RED recovered, **42 GREEN**, eight isolated NuGet-only consumer probes, three local Math archive inspections; dw.tools.math.codecs **1.0.0-preview.6**. This is not a published stable product.
- Scope: independently qualified exact annotated Content and Presentation MathML, typed original Math IR, provenance, DTD/entity/namespace/tampering/size/depth refusals. No semantic proof from input fingerprints and no inverse reconstruction from Presentation MathML alone.

## Exact repository state at handover

- Canonical backlog docs/planning/backlog.json: **0.1.70**. M5-W01-C01, C02 and C03 **done**; M5-W01-C04 **planned**. Work package M5-W01 and milestone M5 **in_progress**; M5-W02 planned.
- Native .aura/workflow/plan/project.json: M5 and M5-W01 in-progress, C01-C03 done, C04 and its children not-ready, M5-W02 not-ready.
- .aura/workflow/state.json: activeMilestone=M5, activeWorkPackage=M5-W01, activePhase=null, activeTask=null, activeSubtask=null, pendingRun=null.
- **RS072 HAS NOT BEEN PREPARED OR EXECUTED.** No .workflow/next-run/request.json exists. An old .workflow/next-run/payload.cs may remain from RS071; it is NOT a ready RS072 run.
- Do NOT ask the operator to run dwf run next until RS072 is staged and the final request-only commit is verified.

## Next exact accepted slice: RS072 / M5-W01-C04-T1

Authority: live docs/planning/backlog.json, chunk M5-W01-C04, T1 and native DWF plan; do not invent a replacement scope.

- Name: **Markdown, Unicode, and Office**. Dependency: completed C03.
- T1 accepted behavior: render Markdown/Unicode math wrappers and qualify a **separate small Office OMML codec subset**.
- Independent acceptance: **a simple OMML equation and an exact fraction traverse the codec without semantic loss**, and Markdown explicitly distinguishes inline from block math.
- T2 still owns unsupported inputs, absence of Office installation, whole-document conversion boundaries, fonts/positioning and loss refusals. Do not pre-complete C04, W01 or M5.
- Reuse qualified Math IR, ExactRational, existing codecs; no arbitrary Office document parsing, TeX execution, full Office runtime or broad OMML interpreter. Confirm namespace and admitted OMML constructs before coding; preserve original typed IR and explicit unsupported statuses.
- Expected preparation approach: one real compiled missing-public-API RED, independent useful GREEN tests, focused codecs/IR/quantities regressions, pinned package-only NuGet consumer, three locally rebuilt/inspected archives and actual hash receipts. Select the next codec preview coherently from preview.6; no claim of tests having run before DWF.
- Activate C04 and complete ONLY C04-T1 R/G/V and T1, keeping the phase C04 in-progress and M5-W01/M5 open. Any state/plan/backlog transition occurs inside the controlled payload.

## Minimal fresh authority reads in the next conversation

1. Mandatory workflow/project governance files above.
2. Live GitHub main HEAD, .aura/workflow/state.json and any NEWER RS072 durable result or preselection evidence. Never assume this snapshot is still the newest evidence.
3. Only M5-W01-C04/T1 in docs/planning/backlog.json and the corresponding nodes in .aura/workflow/plan/project.json.
4. RS071 attempt-001 report and C03-T2 science receipt; relevant current codecs/IR sources, project/test csproj and solution, global.json and the C# checklist.
5. Last accepted DWF next-run payload/request patterns as reference ONLY; do not reuse stale run IDs, file paths, states, scientific status literals or package versions.

## Operating loop and failure discipline

Every normal interaction prepares the next run after **pushed** or corrects the SAME run after **failed**. Exceptions: a specific owner question, a genuine milestone decision, or this explicit conversation handover. Exactly one bounded run at a time; never stack successors.

- On pushed: verify actual GitHub product HEAD, relevant attempt report and scientific evidence, native DWF plan, backlog and state; only then prepare the next accepted task.
- On failed: inspect newest preselection/unresolved/allocated-attempt evidence first; classify pre-attempt, allocated attempt, mutation, delivery or internal correctly; report exact retry and attempts already allocated; repair the SAME run. Do not reconstruct ancient history or modify DWF/sibling repos.
- If failures repeat, audit the whole payload, target re-entry, scientific assertions, package helpers and consumer. GIGO education first; no workflow guard proliferation.
- C# pitfalls from recent genuine failures: MSTEST0032 includes compile-time constant equalities as well as trivial IsTrue comparisons; CS0136 nested local name collisions; CS8321 unused local helpers; IrExactScalar requires ExactRational, not BigInteger; canonical rationals must be reduced; TeX control words absorb adjacent letters. Do not hide errors by disabling analyzers.
- DWF target replay: EACH baseline/target precondition must match actual JSON written, including status via ONE shared C# constant, backlog version, node states and active cursor.
- Verify contribution authoring limits (summary max 500 chars; value/nextAction max 1000/500; up to 20 unique debtChanges max 300 each; achievements unique and ONLY for nodes transitioning to done, max 1000 each). No blank/CR/LF values.
- Run smallest likely-to-fail checks first. Never advertise preflight or scientific completion before durable DWF evidence.
- Publish .workflow/next-run/request.json as the VERY LAST GitHub write, in a commit whose ONLY changed file is that request. Verify main HEAD 40-char SHA, stagedFiles, mutationPaths, preconditions and request commitMessage.

When RS072 is actually prepared or corrected, end every response with EXACTLY:

    DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math
    BRANCH: main
    PREPARED TIP: <verified current GitHub main SHA>
    EXPECTED COMMIT: <actual request.commitMessage>
    STATUS: RSnnn — retry N prepared — attempts allocated X — not executed
    COMMAND: dwf run next

For the current snapshot, since RS072 is **not yet prepared**, do not manufacture this execution-ready contract or tell the operator to run prematurely.
