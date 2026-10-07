---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.agent-playbook"
documentVersion: 9
kind: "guidance"
title: "Agent Playbook"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV022Draft.cs#AgentPlaybook"
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
