# dw.tools.math — compact DWF handover (RS187 pushed / RS188 unprepared)

Date: 2026-11-12. This is a compact index, not a replacement for current repository authority.

## Exact operator identity

- Repository: `aihallot/dw.tools.math`
- Branch of execution: `main`; Windows checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`.
- Remote `main` verified at `33ebae6ac23ac09170d2671ab77a507450a451e7`.
- DWF executor `0.1.66`, guidance `0.26`.
- Native DWF state: `initialized`, `pendingRun: null`. No `.workflow/next-run/request.json` exists on this verified main.
- This handover exists on a **separate branch**; do not check out or run DWF from it.

## Read in this order

1. `docs/workflow/mantra.md` and `docs/workflow/constitution.md` **fully, before repository state analysis**.
2. `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, `AGENTS.md`, and the current workflow authoring/operator guidance.
3. This handover; then **only** the recent authorities referenced below and latest GitHub main/report. If a newer commit exists, it wins.
4. Relevant `docs/planning/backlog.json` M8 entries, `.aura/workflow/plan/project.json`, current `docs/discovery` sources and existing source/consumer code before preparing a run.

## Durable result

- RS187: `pushed`, `succeeded`, attempt 001, **first-pass success**, DWF 0.1.66, no prior invocation failure and zero boundary violations.
- Result commit: `33ebae6ac23ac09170d2671ab77a507450a451e7`.
- Exact result message: `test: qualify generated Math Discovery manifest and eleven refusals`.
- Evidence: `.aura/workflow/reports/RS187/attempt-001.json`; installed `docs/discovery/qualification/ValidateM8GeneratedManifest.cs`.
- State cleared; **RS188 has not been authored or prepared** at this checkpoint.

## Current product and retained evidence

- Canonical backlog `0.1.123`, `in_progress`; M8 present but its W01/C01/C02 and W02/C01/C02 remain `planned` / native nodes `not-ready`. M0–M7 completed history is preserved.
- M8 roadmap: independent Math Discovery first, then .NET 11/C# 15 modernization. Later M9–M12 approved in order, not yet admitted. Nine .NET 11 numerical research candidates at `docs/planning/proposals/net11-numerics-impact-matrix.json`, **not implemented**.
- RS131: qualified 13 pinned portable `dw.discovery/1.0.0-draft.1` schema-2 fixtures (2 accepted/11 refused), documented in `docs/discovery/qualification/RS131-portable-protocol-qualified.json`. Do not replay.
- RS184: compiled source-project consumer `tests/consumers/m8-discovery-clr-seed/` with four selected static Math CLR signatures and bounded IR examples. Not a package-installed consumer.
- RS185: four actual assembly fingerprints in `docs/discovery/qualification/m8-selected-clr-bindings.json`; succeeded attempt 001 after one pre-attempt nullability refusal, not first-pass.
- RS186: deterministic generated `docs/discovery/qualification/m8-discovery-manifest.json`, 4 `AgentCallable`, zero `BrokerReady` and broker candidates; `GenerateM8DiscoveryManifest.cs` supports write/check. First-pass.
- RS187: independently checked real manifest and 11 targeted refusal mutations; reuse its receipt rather than rerunning earlier campaigns.
- Four explicit capabilities: `math.quantity.exact-pipeline.evaluate`, `math.replay.try-evaluate`, `math.ir.canonical.encode`, `math.ir.hash.semantic`.
- No skill bundle delivered or installed; no broker authorization, no AURA adoption, no public NuGet publication. Preserve immutable packages and proofs.

## Next bounded run

Prepare **RS188** on `main`: begin deterministic, progressive Math-owned agent skill generation from qualified RS186/RS187 manifest. Inspect existing project skills/generators/templates and avoid duplicate functionality. A reasonable bounded first slice: generator `generate/check`, compact `.agents/skills/dw-tools-math/SKILL.md`, version/index/capability reference pages and source-based C# usage, all derived from exact selected capabilities and their approved numeric limits. Split if too large. Do not infer a runtime broker/permissions or public distribution; stage source files completely, qualify deterministic bytes and drift refusals. No native M8 completion without a truthful corresponding lifecycle transition and achievement.

## DWF methodology and handback

- One run / one pending request. Read active run authoring, Payload SDK and cost-policy sources; use C# for product logic, minimal targeted validations, no unnecessary replays/full suites.
- After `pushed`, verify GitHub main, durable report, cleared pendingRun and actual transition, then prepare the next run.
- After `failed`, inspect latest durable evidence, distinguish pre-attempt vs allocated failure, correct **the same run** and report retry accurately. After repeated failures re-audit the full topology; never weaken guards to obtain green.
- Last GitHub write of preparation must be `.workflow/next-run/request.json`; preserve previous staged corrections by basing subsequent Git trees on the **latest tree**, not an old base.
- External coordination remains draft until owner explicitly transmits it.
- Canonical repository code/docs/commits in English; conversation with owner in French.
- End each operator answer with exactly one seven-field handback and nothing after COMMAND:

`Run terminé : <id> — <outcome> — attempt <n/n/a> — first-pass <success|failure|n/a> — exécuteur <version/n/a> — commit <verified-result-SHA/none>.`

Blank line, then six ordered lines:

`DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math`
`BRANCH: main`
`PREPARED TIP: <verified SHA/none>`
`EXPECTED COMMIT: <exact expected result commit message/none>`
`STATUS: <id/aucun> · retry <ordinal/n/a> · attempts alloués <number/n/a>`
`COMMAND: dwf run next` or `COMMAND: aucune`.
