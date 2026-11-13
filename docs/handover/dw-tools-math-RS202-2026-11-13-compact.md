# dw.tools.math — compact DWF handover (RS201 pushed, RS202 unprepared)

Date: 2026-11-13 (Europe/Brussels). Repository: `aihallot/dw.tools.math`; canonical execution branch: `main`. Operator checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`. Installed DWF: **0.1.66**, guidance **0.26**.

## Authority and verified checkpoint

- GitHub `main` at handover preparation: `cf541fa192825d85ee9cec1524c84990bcc8fad6`, commit **`feat: bundle qualified Math Discovery manifest byte-for-byte`**.
- RS201: **succeeded and pushed**, allocated **attempt 001**, first-pass **false**, **two previous preselection failures** and no earlier allocated attempt. Durable report: `.aura/workflow/reports/RS201/attempt-001.json`; verdict and delivery projection: `workflow/handoff/current`, `.aura/workflow/handoff/current.json`.
- RS201 refusal 0: newline/byte comparison of qualified source manifest versus staged bundle on Windows; corrected to canonical LF comparison. Refusal 1: `contribution.debtChanges` entry >300 characters; corrected request metadata. Both refusals are retained on evidence branches, **not silently erased**.
- Verified `.aura/workflow/state.json`: `pendingRun:null`. `.workflow/next-run/request.json` absent on `main`. Therefore **RS202 is not prepared and must be prepared**; do not tell the operator to execute `dwf run next` before preparation.
- GitHub `main`, workflow state, plan, latest reports/evidence always take precedence over this document. Handover branch is documentation only; **do not merge it to main** just to resume.

## Mandatory first reads in resumed conversation

Read fully in this order:
1. `docs/workflow/mantra.md`.
2. `docs/workflow/constitution.md`.
3. This handover on branch `handover/dw-tools-math-rs201-pushed-rs202-unprepared-20261113-compact`, path `docs/handover/dw-tools-math-RS202-2026-11-13-compact.md`.

Then use only relevant current authorities: `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, `AGENTS.md`, `docs/planning/backlog.json`, `.aura/workflow/plan/project.json`, `.aura/workflow/state.json`, newest durable report, `docs/discovery/qualification/m8-discovery-manifest.json`, and affected implementation/generators. Consult `docs/workflow/payload-authoring.md` only where authoring syntax needs confirmation. Do not reconstruct old campaign history or replay old suites.

## Product truth and work direction

- Canonical backlog `docs/planning/backlog.json` version `0.1.123`; milestone **M8 Foundation and Discovery** and `M8-W01` / `M8-W01-C01` / `M8-W01-C02` remain DWF **not-ready**. Do not invent completion or lifecycle transitions. Earlier milestones and immutable packages are preserved.
- Four explicitly selected **AgentCallable** CLR operations, **zero BrokerReady**, **runtime-only** authorization: `math.quantity.exact-pipeline.evaluate`, `math.replay.try-evaluate`, `math.ir.canonical.encode`, `math.ir.hash.semantic`.
- Schema-2 source manifest: `docs/discovery/qualification/m8-discovery-manifest.json`; source version `0.3.0-preview.1`. Distinct qualified private NuGet calendar versions (immutable): Composition `20260.20261107.1001`, IR `20260.20261104.1001`, Quantities `20260.20261102.1001`, feed `NUGET_LOCAL`. Do not conflate source/package versions, claim public release, or modify existing packages.
- RS186–RS200 generated and qualified manifest, source bindings, skill, selected reference, version matrix, `--list/--explain` CLI, private package SHA-256 reflection and four bounded packaged method invocations, then navigation and six navigation refusals. RS201 now additionally generates/checks the canonical-LF bundled machine-readable `.agents/skills/dw-tools-math/references/manifest.json` via `docs/discovery/qualification/GenerateM8MathSkill.cs`.
- The next useful bounded step is an **independent refusal/consistency qualification of the bundled manifest against the canonical source** (including byte drift and authorization/refusal boundaries), then inspect remaining genuinely missing runnable examples and M8 acceptance criteria. Do not gratuitously repeat the prior validator campaigns. A targeted consumer or skill delivery gate is preferable to an artificial broad test.
- New development policy: .NET 11 / C# 15, with already published .NET 10 binaries immutable; avoid speculative framework migration inside the Discovery slice. No sibling repository modifications, AURA adoption, NuGet publication or BrokerReady promotion without explicit evidence/authority.

## Operator loop and remote preparation mechanics

- The assistant authors exactly one DWF run remotely on GitHub `main`; the user runs only `dwf run next` on the Windows checkout and replies `pushed` or `failed`.
- On `pushed`: verify actual result commit, full durable report, attempt and prior failure count; **immediately prepare the next run** before responding unless owner choice or major milestone boundary genuinely requires discussion.
- On `failed`: inspect latest durable failure evidence (including `workflow/handoff/current` and `workflow/evidence/...`), distinguish pre-attempt vs allocated-attempt, correct **the same run**, retain retry count and provenance; never start RS+1 to hide a failure. First failed invocation is retry 0; next prepared retry 1.
- DWF 0.1.66 remote next-run preparation: stage `.workflow/next-run/files/<relative target>` and `.workflow/next-run/payload.cs`, then write **`.workflow/next-run/request.json` LAST**, and verify GitHub `main` tip and file contents. Never commit generated `.workflow/runs/RSxxx` to GitHub; the DWF executor produces those.
- For `request.json`, all `contribution.debtChanges` entries must be single-line, nonempty, **at most 300 characters**. Avoid repeatedly triggering elementary C# compile errors. Validation should be differential and bounded. Prefer compact, factual preparation commits, and always ensure the request is last write.
- Never claim `first-pass success` just because attempt 001 succeeded; check `priorInvocationFailures.firstPassSucceeded` and the actual count.
- Operator handback format: a short `Run terminé : RS... — ... — attempt ... — first-pass ... — exécuteur 0.1.66 — commit ...` followed by six ordered lines `DIRECTORY`, `BRANCH`, `PREPARED TIP`, `EXPECTED COMMIT`, `STATUS` (including correct retry/allocated attempts), `COMMAND: dwf run next`. No redundant prose after the handback.

## Handover disposition

The handover intentionally does **not** prepare RS202. The next assistant must verify `main` and prepare **RS202** as the very first operational step, preserving `main` exactly until it is ready to write the bounded staged payload. The handover branch is a resumable context artifact only.
