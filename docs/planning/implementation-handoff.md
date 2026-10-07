# ChatGPT and DWF implementation handoff

## Current situation

The repository contains an accepted product roadmap, a .NET documentation validator, draft cross-repository requests, and an initialized DWF 0.1.58 / guidance 0.22 workflow.
No AURA extraction has been delivered and no package has been publicly published.
Canonical repository language is English. Existing French planning content is migration debt and must be normalized from canonical sources rather than hand-editing generated projections.

Read AGENTS.md, PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, architecture.md, existing-code-inventory.md, testing-and-quality.md, resource-cost-policy.md, and the backlog before product work.

## Product roadmap and native DWF plan

docs/planning/backlog.json is the accepted product roadmap. It owns product scope, stable IDs, dependencies, acceptance recipes, source coverage, and product-specific metadata.
.aura/workflow/plan/project.json is DWF execution authority. It must be a faithful executable projection of the accepted roadmap, not a second independently invented plan.

Use docs/planning/dwf-map.json as the stable level mapping, not as native DWF JSON to copy:
- release -> milestone
- work_package -> workPackage
- chunk -> phase
- task -> task
- subtask -> subtask

Preserve stable IDs and dependency meaning. Translate titles and developer-facing text to canonical English while preserving semantics.
Do not force backlog-only fields into unrelated DWF fields. Use native dependsOn and completion conditions only where the mapping is truthful.

The first bootstrap was intentionally thin so RS001 could start. That thin bootstrap is not the durable target. After durable RS001 success and before any successor implementation run, reconcile the accepted roadmap into the native DWF plan. The current native topology is 8 milestones, 17 work packages, 53 phases, 106 tasks, and 312 future/observed subtasks. The six accepted C01 RED/GREEN/verification subtasks are intentionally not backfilled because RS001 completed that phase before DWF observed those child transitions; see docs/planning/decisions/native-dwf-roadmap-reconciliation.md. Future refinements may split legitimate large product work without changing historical IDs or meaning.

## Run authoring rule

Before every run, derive and review the expected native-plan transition together with the product mutation:
- identify the exact scoped DWF nodes;
- record their baseline lifecycle states;
- decide which nodes remain active and which complete;
- ensure the payload performs those lifecycle transitions in the same controlled mutation;
- ensure target re-entry accepts the exact completed plan state;
- declare one contribution achievement for every node transitioned to done and no achievement for an incomplete node.

Do not execute product work first and repair DWF progress afterward.
A run may span multiple product tasks when the bounded outcome genuinely proves them, but the native plan must still contain those stable product task identities so the reported completion is not an aggregate that erases accepted planning detail.

## RS001 boundary

The first bounded product result is M0-W01-C01: reproducible .NET foundation, targeted tests, package creation, helper boundaries, and an isolated local consumer.
RS001 does not authorize AURA extraction.
Its technical chain has already demonstrated restore/build/test/pack/consumer success during failed attempts; the current correction must preserve retained mutation provenance and close only the progress-contract mismatch.

After RS001 is durably pushed, reconcile the full accepted roadmap into DWF before preparing RS002. M0-W01-C02 then validates DWF adoption and product/native mapping; it does not invent a second roadmap.

## Planning generation and validation

The planning validator is docs/planning/ValidatePlan.cs and uses .NET only.
Default validation is observational:

    dotnet run --file docs/planning/ValidatePlan.cs -- --check

Generation is intentionally mutating and belongs in payloads or deliberate authoring steps:

    dotnet run --file docs/planning/ValidatePlan.cs -- --write

Never place --write in a preparation-safe validation.
When backlog changes, declare every regenerated projection path in the mutation boundary and validate the resulting projection.

## Readiness

Before marking product scope ready, require completed internal dependencies, satisfied external gates with evidence, split size-L blocks where required, an independent oracle, exact mutation files, executable commands, bounded resource cost, rollback/failure behavior, edge tests, and no path outside this repository.
Proposed *Contract.cs names in the backlog are anchors, not mandatory class names.
Proposed commands are intentions until refined against the actual runner and project layout.

## Completion and reconciliation

A product chunk is done only when its accepted tasks/subtasks and acceptance evidence are satisfied.
A spike being done means a decision was produced, not that a product capability exists.
Release gates require their full accepted recipes.

DWF completion is explicit: every native node transitioned to done during a run needs a matching contribution achievement.
After a durable success, reconcile product status and generated documentation from the real DWF report/evidence before preparing dependent scope.
Do not manufacture historical success or mark future native nodes done merely because current code appears to satisfy them.

## Operator loop

Normal operator execution remains:

    dwf run next

On failure, inspect durable evidence and correct the same run. Do not erase retained local mutation state and do not prepare a successor to hide failure.
On success, verify the remote result, report, cleared pendingRun, native-plan transition, and product reconciliation before preparing later work.

## Session output

Report actual evidence and limits, the next admissible product scope, real validation results, and no invented billing estimate.
External prompts remain in Math until the owner transmits them.
Never write to AURA, Decision, MCDM, or another repository to satisfy a Math gate.
