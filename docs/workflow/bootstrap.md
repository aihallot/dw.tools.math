---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.bootstrap"
documentVersion: 9
kind: "guidance"
title: "Workflow Bootstrap"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV022Draft.cs#Bootstrap"
---
# Workflow Bootstrap

Before repository-state analysis, read the active workflow `mantra.md` and use `constitution.md` as normative workflow-operating policy. When repository guidance differs from the executing tool's active embedded guidance, use the embedded active copies rather than candidate or stale repository bytes. Then read repository-owned `PROJECT-MANTRA.md` and `PROJECT-CONSTITUTION.md` when they exist; project policy may constrain product development but must not redefine workflow mechanics.

Reconstruct only the context needed for the current decision.

Before choosing work, classify the repository adoption mode from evidence: **greenfield** (no product history), **existing unmanaged** (product source/history exists without current workflow authority), or **legacy PowerShell** (a quiescent historical workflow is present). Preserve existing product authority and constraints. For a quiescent legacy PowerShell repository, the current tool qualifies dwf adoption cutover <project-plan.json> <workflow-state.json> [--what-if]; supply reviewed native project-plan and workflow-state contracts, use --what-if for a non-mutating preflight, and review the resulting uncommitted cutover before committing. Do not reconstruct fictitious history or treat an empty initialized plan as proof of a greenfield product.

1. Read `.aura/workflow/repository.json`, `.aura/workflow/state.json`, and `.aura/workflow/plan/project.json`.
2. If `pendingRun` names a run, read its manifest and material under `.workflow/runs/<id>/`.
3. Read the latest relevant durable success report under `.aura/workflow/reports/` and the latest relevant durable failure evidence from the repository remote when the operator reports `failed`.
4. When preparing or correcting a run, read `docs/workflow/run-authoring-reference.md` and `docs/workflow/payload-authoring.md` together with the product documentation, source and tests relevant to the active objective.
5. If executable access is available and a preparation request is already committed, an agent or CI may run `dwf run prepare --check <request.json>` to exercise the exact preparation authority in a disposable clone before operator handoff. This check never selects a run and is not a routine operator step.
6. Use `dwf check` when executable compatibility or guidance integrity is uncertain.

Choose one action:

- no pending run and the project plan has no milestones: if product intent is sufficient, author the initial project plan directly as repository authority using **Initial project plan authoring** in `docs/workflow/agent-playbook.md`; if product intent is insufficient, ask only for the missing product direction and do not invent backlog filler;
- no pending run, the project plan contains a truthful ready scope, and no prior workflow result exists: prepare the first bounded run from the smallest truthful project scope;
- no pending run and the prior result is visible remotely: consume the existing ready scope or refine the smallest truthful lower-level planned work before preparing one bounded run; do not create a new milestone-work-package-phase-task chain merely to represent the next run;
- a preparation gate failed before attempt allocation: correct the same prepared run;
- an execution failed with a matching correction receipt: correct the same run and preserve the failed local mutation state;
- a successful result commit exists but delivery failed: preserve that result and let the next `dwf run next` recover delivery without replaying project work;
- remote result or failure evidence is missing: do not prepare later work until the missing authority is resolved.

Initial project-plan authoring is the one pre-run authority bootstrap needed because a controlled run cannot truthfully reference project nodes before any exist. It is agent-authored repository planning, not local operator execution. Do not create a filler run to bootstrap the plan, do not mutate `.aura/workflow/state.json` to fake activation, and do not require a new routine operator command. Once the first truthful plan exists, return to the normal controlled-run path.

## Ongoing roadmap planning

After the initial plan exists, do not recreate the hierarchy for every run. Reuse durable milestones, work packages, and meaningful phases; select an existing ready task or subtask when possible, and progressively refine lower-level planned work when evidence changes the next bounded deliverable. Treat a repeated singleton hierarchy that exists only to mirror RS boundaries as a planning smell. If newly accepted work does not fit current authority, replan or explicitly defer it rather than implementing it untracked; do not invent a second backlog authority.
The normal operator command does not change across recoverable run states: use `dwf run next`. The runtime selects normal execution, same-run correction, or pending-delivery recovery from durable local evidence.

Treat committed repository state as authority. The operator owns local execution. Never claim an execution, installation, publication, or external observation that has not been reported or recorded.

Consult the remaining documents by responsibility rather than reading every file by default.
