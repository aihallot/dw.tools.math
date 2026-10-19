# dw.tools.math — compact DWF handover, 2026-10-19

> **Read this entire handover first.** It is a snapshot, not a substitute
> for newer GitHub `main` and durable DWF evidence. Read only the recent
> authorities named below; do not reconstruct older runs or load all history.

## Operator identity and current selection

- Repository: `aihallot/dw.tools.math`, execution **branch `main`**.
- Exact Windows operator checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`.
- DWF: `dw.tools.workflow 0.1.59`; engineering guidance `0.22`.
- **Verified GitHub main / RS103 prepared tip:** `22b40b01a0461bdec56b24661a139eca04531115`.
- Selected: **RS103, retry 0, zero attempts allocated, not executed**.
- Exact expected result commit message:
  `docs: qualify M6 scientific family pins and Decision reuse assessment`.
- Operator command: `dwf run next` (in the exact checkout above).
- The final selection commit changes **only** `.workflow/next-run/request.json`.
- RS103 selection: **80 verified preconditions, 3 staged documentation
  files, 11 declared mutation paths**. `pendingRun=null`.
  `.workflow/runs/RS103` and its attempt report are absent before execution.

## Last durable successful result

- **RS102 pushed and succeeded**, attempt **001**, **first-pass**,
  zero preceding invocation failures.
- Result main commit:
  `38b5e1022a3315078f6036201f98d3dc33b73608`.
- Evidence:
  `.aura/workflow/reports/RS102/attempt-001.json` and
  `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`.
- 1 compiled RED recovered, 90 GREEN, 904 retained scientific tests,
  four Math regressions, seven isolated eight-probe local NuGet consumers.
- Local scientific `1.1.0-preview.14` archive:
  SHA-256 `2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf`
  (37,270 bytes). No public package claim.
- **C01–C04 done**, including C03 and its three split children;
  **C05 planned**, **M6-W02 / M6 in-progress**. Backlog `0.1.101`,
  DWF native topology **523 nodes**.

## RS103 exact scope — documentary Q/O/D, not RED/GREEN

- Parent: `M6-W02-C05` (kind `document`); task `C05-T1` with
  native subtasks **T1-A Question, T1-B Observation, T1-C Decision**.
- Three staged documents:
  `docs/planning/decisions/m6-w02-c05.md`,
  `docs/distribution/m6-scientific-family-adoption.md`,
  `docs/planning/requests/decision-bounded-bracketed-root-assessment.md`.
- Payload verifies **seven independent historical family evidence/source
  SHA/version pins**, numerical oracles, and existing RS102 package
  receipts; writes `docs/planning/evidence/M6-W02-C05-T1-qualified.json`.
- **No new scientific tests, code change, package pack, external provider
  adoption or sibling repository change in RS103.** Historical family
  preview pins must NOT be presented as new preview.14 executions.
- Intended outcome: close **T1-A/B/C and C05-T1 only**, set C05
  `in-progress`; leave **C05-T2, C05, M6-W02, M6** open.
- Next after successful RS103: separately qualify
  `M6-W02-C05-T2` semver/notices, owner disposition and no automatic
  external cutover; do not claim Decision approval or close M6 early.

## First reads in the resumed conversation

1. This handover (in this separate handover branch).
2. Latest GitHub `main` HEAD, `.aura/workflow/state.json`,
   `.workflow/next-run/request.json`, `.workflow/next-run/payload.cs`
   **if still present**.
3. Latest relevant DWF terminal evidence/report and its handoff:
   start from RS102 and inspect RS103 if the user has run it.
   Search durable `workflow/evidence/` and `workflow/handoff/` refs
   if a `failed` is reported, including *preselection/pre-attempt*
   refusals that never allocated an attempt.
4. Only the pertinent backlog entry `docs/planning/backlog.json`
   (`M6-W02-C05`, index 50), native plan
   `.aura/workflow/plan/project.json` (M6/W02/C05 phase index 7)
   and latest qualification evidence. No historical repo sweep.

## Mandatory per-turn operating protocol

- **`pushed`:** verify pushed outcome, exact attempt, prior refusals,
  main commit, tests and native plan in durable evidence; then prepare
  **exactly one substantive next DWF run** within the accepted backlog.
- **`failed`:** find the exact latest error and retry allocation;
  **repair the same run**, never advance. Pre-attempt refusal may leave
  attempts allocated at zero, but forfeits first-pass; report accurately.
- Exceptions: a precise user question, or the end of an important cycle
  where the best continuation needs an explicit decision.
- Follow `dw.tools.workflow` authority, not guessed retries. No
  GitHub Actions/CI, no edits to workflow or sibling repositories.
  Implementation/validation is local DWF C#/.NET only; canonical source
  and documentation language English, conversation French.
- Do **not** manually precreate `.workflow/runs/RSxxx`. Stage files
  under `.workflow/next-run/files` or `checks`; use the exact
  `sourcePath` in the request. Preserve `pendingRun=null`.
- Commit `.workflow/next-run/request.json` **last**, changing no other
  path in that commit. Verify actual GitHub main SHA, all preconditions,
  staged inputs, mutation paths and absence of premature run artifacts.
- Keep contribution limits: summary <=500 chars, value <=1000,
  nextAction <=500; at most 20 unique debts <=300 chars each and
  20 unique achievements <=1000 chars each. Never invent first-pass,
  executed tests, product completion or outside provider acceptance.
- Use lowercase solution/project naming; build outputs under `/build`,
  minimal `/scripts`, no Python in repository implementation.

## Required end-of-response block — always LAST

After a short explanation, state how the operator reports
`pushed` or `failed`, then finish with this exact-shaped block
populated from the **latest verified GitHub main**, never stale memory:

```text
DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math
BRANCH: main
PREPARED TIP: <verified 40-character main SHA>
EXPECTED COMMIT: <exact selected request commitMessage>
STATUS: RSxxx — retry N prepared — attempts allocated N — not executed
COMMAND: dwf run next
```

If not actually prepared, use `COMMAND: DO NOT RUN` with an honest
status; do not show an executable run command for an unverified request.
