---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.agent-playbook"
documentVersion: 11
kind: "guidance"
title: "Agent Playbook"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV024Draft.cs#AgentPlaybook"
---
# Agent Playbook

## Authority layers

Read the active workflow `mantra.md` and `constitution.md` before repository-state analysis. If repository `docs/workflow` differs from the executing tool's active embedded guidance, use the embedded active workflow policy rather than candidate or stale repository bytes. Then read repository-owned `PROJECT-MANTRA.md` and `PROJECT-CONSTITUTION.md` when present. Project policy may constrain product development but must not redefine workflow mechanics, and the workflow engine must not special-case repositories based on product identity.

Do not invent project policy when those files are absent. Product documentation, source, tests, and domain evidence remain distinct product authority.

## Reconstruct truth

Read the workflow state, project plan, pending run when present, latest relevant durable success report, product documentation, and exact source and tests affected by the active objective. Use `dwf check` evidence when compatibility or guidance integrity is uncertain. When the operator reports `failed`, retrieve the newest relevant durable remote failure evidence before asking for terminal text.

## Repository adoption mode

Before choosing work, classify the repository adoption mode from evidence: **greenfield** (no product history), **existing unmanaged** (product source/history exists without current workflow authority), or **legacy PowerShell** (a quiescent historical workflow is present). Preserve existing product authority and constraints. For a quiescent legacy PowerShell repository, the current tool qualifies dwf adoption cutover <project-plan.json> <workflow-state.json> [--what-if]; supply reviewed native project-plan and workflow-state contracts, use --what-if for a non-mutating preflight, and review the resulting uncommitted cutover before committing. Do not reconstruct fictitious history or treat an empty initialized plan as proof of a greenfield product.

The generic packaged-skill installer is available. Before non-trivial engineering work, use `dwf skill install engineering-guidance` (Codex default) or `dwf skill install engineering-guidance --target agent` when `.agent` is the explicit agent home. Exact repeat installation is a no-op; divergent target content is preserved and refused.

## Choose the path

- Initialized repository, no pending run, and `milestones` is empty: follow **Initial project plan authoring** below before preparing any run.
- No pending run, a truthful project scope exists, and no prior workflow result exists: prepare the first bounded run.
- No pending run and prior success is visible remotely: prepare one bounded run.
- Preparation failed before attempt allocation: correct the same run.
- Execution failed with a correction receipt: correct the same run while preserving its declared local mutation state.
- A successful result commit exists but delivery failed: do not alter or replay the run; preserve the result for automatic delivery recovery.
- A result or failure is not durably visible where expected: do not prepare later work until its authority is resolved.

For every recoverable run state above, the normal operator instruction is the same: `dwf run next`. Do not make the operator choose `git pull`, `--allow-dirty`, or `dwf delivery retry` in the normal loop.

## Initial project plan authoring

An initialized repository deliberately starts with a valid empty `.aura/workflow/plan/project.json`. A controlled run cannot bootstrap that authority because controlled-progress preparation already requires an existing project node. When `pendingRun` is `null`, `milestones` is empty, and product intent is sufficient to name a truthful first outcome, a repository-capable agent may author the initial project plan directly in Git. This is the one pre-run project-authority bootstrap; it is not a workflow execution and must not be delegated to manual operator editing.

Preserve the initialized `schemaVersion` and `project.id`/`project.title`. For schema 1, use the fixed hierarchy `milestones -> workPackages -> phases -> tasks -> subtasks`. IDs are non-empty and unique case-insensitively, titles are non-empty, and lifecycle states use the existing planning vocabulary. Keep the backlog as small as the known product intent allows. Do not pre-build speculative architecture simply because the repository is empty.

A minimal plan may omit subtasks. Mark the ancestor chain and first meaningful leaf `ready`; keep later work `planned` until it is actually eligible. Example:

```json
{
  "schemaVersion": 1,
  "project": {
    "id": "repository-id",
    "title": "Repository title"
  },
  "milestones": [
    {
      "id": "M001",
      "title": "Deliver the first useful product outcome",
      "state": "ready",
      "workPackages": [
        {
          "id": "WP001",
          "title": "Establish the first bounded capability",
          "state": "ready",
          "phases": [
            {
              "id": "P001",
              "title": "Build and verify the first slice",
              "state": "ready",
              "tasks": [
                {
                  "id": "T001",
                  "title": "Implement the first meaningful source-and-test slice",
                  "state": "ready"
                },
                {
                  "id": "T002",
                  "title": "Continue only after evidence from the first slice",
                  "state": "planned",
                  "dependsOn": ["T001"]
                }
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

Do not change `.aura/workflow/state.json` while authoring this initial plan: active pointers remain `null` and `pendingRun` remains `null`. Do not invent a fake completed result or a bookkeeping run. After committing the plan, the first `ready` leaf is legitimate repository authority and the agent may prepare the first bounded source-and-test run with existing project nodes. `dwf plan next` is a diagnostic view, not a required extra operator step. The operator returns to the normal `dwf run next` loop for controlled execution.

If product intent is not sufficient to choose a truthful first outcome, ask for that missing product direction instead of generating filler nodes.

## Ongoing roadmap planning

Treat the project plan as durable product authority, not as a ledger of run ids. Before preparing work after a successful run, consume or refine the existing roadmap first.

- A milestone names a durable product outcome expected to span multiple bounded changes unless the outcome is genuinely atomic.
- A work package names a coherent workstream within that outcome.
- A phase exists only when it represents a meaningful stage, gate, or change of evidence; it is not ceremony required merely to host a task.
- A task is a bounded deliverable and may require one or several runs.
- A subtask is the preferred place for progressive detail discovered while executing a task.

Author the thinnest coherent roadmap that preserves known product direction. Keep later work `planned` until dependencies and evidence make it eligible, and refine tasks or subtasks as knowledge improves instead of rebuilding milestones, work packages, and phases after each result.

A repeated one-run -> one-milestone -> one-work-package -> one-phase -> one-task chain is a planning smell, not an automatic schema error. Use it only when the product outcome is genuinely atomic and the structure adds durable meaning. Never create upper-level nodes solely because the next RS needs somewhere to attach.

When a newly accepted request does not fit the current roadmap, do not implement it as untracked work. Map it to existing authority, refine lower-level work, or explicitly defer or replan it. Do not invent a second backlog store; repository-owned backlog representation and projection are separate capabilities.
## Optional .NET NuGet local-feed inspection and planning

Only when the executing tool supports the `nuget` CLI and the repository voluntarily develops .NET packages, an agent may inspect an explicitly selected local feed or preview calendar versions. This is not a requirement for non-.NET repositories and not an additional step before normal `dwf run next`.

Qualified source-command forms (use the appropriate installed launcher for the operator's platform):

- `workflow nuget inspect <package-id> --feed <absolute-local-feed-directory>`
- `workflow nuget inspect <package-id> --project-root <absolute-project-root> [--feed <absolute-local-feed-directory>]`
- `workflow nuget plan <package-id> --date <yyyy-MM-dd> --feed <absolute-local-feed-directory>`
- `workflow nuget plan <package-id> --date <yyyy-MM-dd> --project-root <absolute-project-root>`

The exact, fully qualified project root must be supplied. Its optional `.aura/workflow/nuget-profile.json` declares `schemaVersion: 1`, `enabled: true` and a fully qualified `feedDirectory`. Ordinary workflow initialization and run-next never create or activate it. In the inspect form, explicit `--feed` overrides the project profile. There is no parent-project lookup, global NuGet settings lookup, or implicit local feed default. Current read-only commands never read ambient `NUGET_LOCAL`; the internal resolver's deliberately injected `NUGET_LOCAL` is not a supported implicit CLI fallback. `NUGET_PACKAGES` is a package cache, never the shared publication feed.

Inspection reports separate flat/hierarchical histories, byte hashes and exact overlaps. Planning requires an explicit `--date` and reports original version/layout plus blocker codes for noncanonical aliases, legacy packages and conflicting same-version bytes. `Candidate preview (informational only)` is not a selected/reserved release version or authorization to publish; `Candidate preview: none (blocked)` requires review. Neither command publishes packages or proves reproducible build-to-commit provenance.

Do not infer commands from newer repository source. Use only those available in the installed executable and its active embedded guidance. For eligible .NET projects, local NuGet publication is an optional, **mutating** product-domain operation; it is not part of `dwf run next` or an automatic result of `nuget plan`.

### Optional explicit local NuGet publication — writes to the selected feed

- `workflow nuget publish <package-id> --version <canonical-calendar-version> --feed <absolute-local-feed-directory> --package <absolute-prepared-nupkg> --source-root <absolute-git-worktree-root> --source-commit <lowercase-40-character-sha>`

The operator chooses all inputs explicitly. Publishing does not consult the opt-in project profile, ambient `NUGET_LOCAL`, `NUGET_PACKAGES`, global NuGet configuration, NuGet.org or remote feeds. The prepared `.nupkg` must already exist with matching root nuspec PackageId/PackageVersion; the command does not build it. Use only an explicitly authorized local feed. A newer source command is not installed in older stable/meta drivers.

Requested versions must be canonical and match the next ordinary calendar candidate for the observed PackageId. Legacy versions, normalized NuGet aliases, mixed-layout byte conflicts, already-present targets, skipped ordinals and ambiguous histories are refusals. The `nuget plan` candidate is informational only, not a reservation or authority to publish. The preflight is point-in-time; the publisher performs its final no-overwrite check, but unrelated cross-layout writers can race.

A successful publication writes a hierarchical `.nupkg`, extracted nuspec, SHA512 sidecar and a JSON manifest with package SHA256 and caller-declared Git HEAD. **Git HEAD equality is not reproducible build-to-commit package provenance.** Ordinary local collision evidence does not prove crash recovery or distributed-filesystem safety. Real shared-feed publication, tool promotion and independent consumer adoption require separate explicit owner authorization and release qualification.


## Prepare or correct

- Preserve the active project hierarchy unless the run intentionally advances it.
- Controlled-progress preparation requires `projectNodes` to contain at least one existing project-plan node. Never author an empty project scope; use the smallest truthful scope that contains the contribution.
- Declare complete and minimal mutation paths.
- Use complete staged source files.
- Select one proportionate test profile plus structural or preflight checks.
- Verify operation-specific limits, paths, commands, working directories, project nodes, staged membership, and producer-consumer contracts.
- Before another target-scoped run that is expected to make no strategic movement, inspect the repository strategy review threshold and recent durable target-scoped reports. If the prior no-movement streak already meets the threshold, declare the required `contribution.strategicReview` instead of discovering that contract at operator execution.
- Review idempotence and same-run correction behavior.
- Keep advanced delivery and workspace modes optional.

## Strategic review

A strategic review is an explicit agent-authored decision, not automatic replanning and not strategic movement by itself. When required, declare `projectNode`, `capabilityId`, and `stage` with the exact strategic target identity, plus a bounded `decision` and `rationale`. The review target must be inside the run's declared project scope. A matching declared review resets the target's no-movement streak; actual strategic movement also resets it.

Use `docs/workflow/run-authoring-reference.md` for the exact trigger and field bounds.

## Verify operator outcome

- `pushed`: verify the remote result message, durable report, zero boundary violations, changed paths, cleared `pendingRun`, and intended plan transition before preparing a successor.
- `failed`: inspect durable remote failure evidence, classify whether preparation, attempt, mutation boundary, delivery, or internal failure occurred, and correct the same run when correction is possible.

Distinguish preparation commits from the result commit created by `dwf`. Do not infer local execution, installation, publication, or provider state from source changes alone.
