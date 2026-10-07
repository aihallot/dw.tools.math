# Native DWF roadmap reconciliation

## Decision

The accepted product roadmap in `docs/planning/backlog.json` remains product authority for scope, stable IDs, dependencies, acceptance recipes, and product meaning. The native DWF plan in `.aura/workflow/plan/project.json` is its executable projection for work selection and lifecycle progress.

The durable mapping is:

- product release -> DWF milestone
- product work package -> DWF work package
- product chunk -> DWF phase
- product task -> DWF task
- product subtask -> DWF subtask

This reconciliation preserves product IDs and the explicit release/chunk dependency graph. It does not copy backlog-only metadata into unrelated DWF fields.

## Resulting native topology

The reconciled plan contains:

- 8 milestones
- 17 work packages
- 53 phases
- 106 tasks
- 312 subtasks

The accepted backlog contains 318 subtasks. The six-subtask difference is deliberate and historical, not data loss.

## RS001 historical exception

`M0-W01-C01` was completed by RS001 while the initial native DWF bootstrap still represented that phase at coarse granularity. Durable RS001 evidence proves both accepted task responsibilities: the reproducible toolchain/package/consumer chain and the helper/provider-default boundaries.

The reconciliation therefore projects the two accepted C01 tasks as `done` with explicit RS001 evidence, but does **not** synthesize the six historical RED/GREEN/verification subtask lifecycle transitions. DWF never observed those individual transitions, and inventing them after the fact would manufacture history.

All accepted future subtasks are represented natively. The exception is limited to those six already-historical C01 process subtasks.

## Current continuation

`M0` and `M0-W01` remain `in-progress`.
`M0-W01-C01` is `done`.
`M0-W01-C02` is the next `ready` phase, with `M0-W01-C02-T1` and its first decision subtask ready for bounded continuation.

RS002 must validate this mapping and evolve its DWF progress in the payload itself. Every native node completed by that run must have one matching contribution achievement.

## Language

Native DWF titles and developer-facing planning text are canonical English. The older French product planning corpus remains source-preserving migration debt until it is translated at its canonical source and regenerated; generated pages are not hand-translated independently.
