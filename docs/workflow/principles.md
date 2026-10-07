---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.principles"
documentVersion: 2
kind: "guidance"
title: "Workflow Principles"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV012Draft.cs#Principles"
---
# Workflow Principles

- The project plan is progress truth; runs are bounded execution transactions.
- At most one normal pending run exists.
- A failure remains an attempt of the same run; it does not create a successor.
- The operator performs real local execution; durable repository evidence survives conversations and agents.
- Every run declares a complete and minimal mutation boundary.
- Payloads contain project work, not duplicated runner, Git, reporting, or delivery infrastructure.
- Typed C# is the default for durable logic. PowerShell is a thin, parser-checked integration boundary.
- Validation is targeted by default and broadens only at explicit confidence boundaries.
- Successful direct execution owns its result commit, push, and remote observation.
- Delivery recovery reuses a successful result commit and never replays payload or validation.
- Published identities and verified rollback evidence are immutable.
- Optional branch, review, worktree, migration, publication, and self-development capabilities must not burden the normal direct path.
- Generic product rules require generic evidence; repository-local practice remains repository-local.
- Simplicity is a product requirement, not the absence of controls.
