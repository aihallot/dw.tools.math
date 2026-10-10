# Feedback — native DWF roadmap extension for agent Discovery

**Status:** feedback transmitted to the Workflow owner as [issue #139](https://github.com/aihallot/dw.tools.workflow/issues/139). No Workflow source change, implementation promise, or Math DWF run is implied.

## Product need

User-approved Math/Decision agent Discovery should be its own explicit roadmap scope, not an unrelated subtask hidden in the active numerical phase `M3-W01-C02`.

The proposed Math scope is `M3-W03` with phases `M3-W03-C01/C02/C03`, six accepted tasks and 18 RED/GREEN/verification subtasks. See `docs/planning/proposals/math-agent-discovery-work-package.json`. Existing M2 and M3 numerical definitions and successes must remain unchanged.

## Observed authoring limitation

The compiled Math DWF Payload SDK exposes `ProjectPlan.EnsurePhaseWithTasks(workPackageId, phaseId, title, state, tasks)` for **an existing work-package parent**. The published operation reference exposes no typed `EnsureWorkPackage` or `EnsureMilestone` operation and does not support a new nested subtask hierarchy through that method.

The Math native-plan validator `docs/planning/ValidateNativeDwfAdoption.cs` currently asserts fixed topology of **8 milestones, 17 work packages, 53 phases, 106 tasks, 312 actual subtasks, 496 total nodes**. Any roadmap extension must update accepted count checks along with the canonical mapping, without weakening historical structural checks.

The active DWF hierarchy following durable RS040 is `M3 / M3-W01 / M3-W01-C02`. Project state records only one active hierarchy. Starting a second independent work package needs an explicitly accepted branch-selection/continuation policy rather than an implicit parallel mutation.

## Desired controlled capability

Provide a bounded, typed workflow authoring operation to append an explicitly declared new work-package hierarchy to an existing milestone, including stable phase/task/subtask identities, dependency edges, completion criteria and exact source-vs-target reentry. It must refuse partial/ambiguous states, duplicate IDs, illegal dependencies and covert changes to previously qualified nodes. Progress transitions remain ordinary canonical DWF operations.

A safe alternative is a documented and validated **one-time native-plan re-baselining protocol** that can atomically reconcile an accepted product backlog extension and its canonical execution plan before any product run. Do not use arbitrary `Json.EditObject` or `Files.WriteComplete` to mutate `.aura/workflow/plan/project.json` in lieu of the ProjectPlan lifecycle contract.

## Acceptance

1. Existing Math 496-node plan and all durable RS001–RS040 outcomes remain untouched and valid.
2. Append exactly one new work package, three phases, six tasks and 18 subtasks to the accepted roadmap; preserve existing IDs and ordering.
3. Backlog, native DWF, generated `dwf-map.json`, projections and validators agree with the new topology. New nodes start `not-ready`; existing active hierarchy is unchanged.
4. A new Discovery run can safely select the new scope only when the workflow owns a legal readiness and activation transition, without claiming unrelated M3 numerical work done.
5. No changes to AURA, Decision, MCDM or existing product library packages are required for the planning operation.

**Ownership:** this feedback is intended for the maintainer of `dw.tools.workflow`; Math does not change that repository. No claim is made that the SDK extension already exists or has been accepted.
