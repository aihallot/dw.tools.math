---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.operator-loop"
documentVersion: 12
kind: "guidance"
title: "Operator Loop"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV022Draft.cs#OperatorLoop"
---
# Operator Loop

PowerShell operators use `dwf`; the package command `workflow` is reserved by the PowerShell parser.

## Normal command

```powershell
dwf run next
```

This is the normal command for a new run, a remotely corrected preparation, an authorized same-run dirty correction, and recovery of an existing result commit whose delivery is incomplete.

On a clean repository, `dwf run next` fast-forwards remote authority before resolving the committed request or pending run. When an exact correction receipt exists, it selects same-run correction automatically. When a pending delivery receipt exists, delivery recovery takes priority and reuses the existing result commit without replaying payload or validations.

A successful direct run executes the pending payload and targeted validations, verifies the mutation boundary, records durable evidence, commits, pushes, and observes the exact remote result. No manual pull, commit, push, `--allow-dirty`, or delivery-retry command belongs to the normal loop.

## Terminal verdict

For every controlled `dwf run next` completion, the final terminal line is the operator verdict:

- `pushed` in yellow means the exact result is durably delivered and remotely verified;
- `failed` in red means the command did not reach that durable success boundary.

When the operator reports `pushed`, the remote agent verifies the result commit, durable report, cleared `pendingRun`, and intended plan transition before preparing later work.

When the operator reports `failed`, the remote agent retrieves the newest durable `workflow/evidence/...` failure publication for the current run and corrects the same run. The operator should not copy the full terminal log when durable failure evidence was published successfully.

If failure evidence itself cannot be published because Git, the remote, or repository identity is unavailable, diagnostics explicitly report that exceptional transport failure before the final `failed` verdict. Only then may the missing terminal detail need to be supplied out of band.

`dwf run next --allow-dirty` and `dwf delivery retry` remain compatibility or diagnostic surfaces; they are not normal operator decisions.

## After installing a newer executable

Run:

```powershell
dwf version
dwf check
```

The executable, workflow schema, and repository guidance have independent versions. Installing a new executable does not rewrite the repository.

When `dwf check` reports an exact supported guidance upgrade, run:

```powershell
dwf guidance upgrade
```

The supported 0.22 upgrade contract requires a clean repository and no pending run, upgrades exact embedded guidance 0.21 to 0.22, replaces only `docs/workflow`, verifies the target pack, creates and pushes one deterministic result commit, and observes the remote result. No manual Git command follows a successful upgrade. If delivery fails after the guidance commit exists, restore connectivity and use normal `dwf run next`; pending-delivery recovery reuses that existing commit.

Divergent or unsupported guidance is refused without overwrite. Review the reported files and preserve repository-owned custom documentation outside `docs/workflow`.

Use `dwf prompt` to reproduce the bounded startup prompt for another agent conversation.
