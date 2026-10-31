---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.operator-loop"
documentVersion: 16
kind: "guidance"
title: "Operator Loop"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV026Draft.cs#OperatorLoop"
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

## Mandatory operator handback

Whenever an agent returns control to the operator in a workflow conversation (including status, preparation, correction, failure, success, no prepared run, or handover), end with this mandatory seven-field layout: one `Run terminé` line, one blank line, then six ordered operator fields. Nothing may follow `COMMAND`; do not duplicate the fields elsewhere.

```text
Run terminé : <run-id-or-aucun> — <outcome-or-n/a> — attempt <n-or-n/a> — first-pass <success|failure|n/a> — exécuteur <driver-version-or-n/a> — commit <verified-result-sha-or-none>.

DIRECTORY: <operator-checkout>
BRANCH: <operator-branch>
PREPARED TIP: <prepared-tip-sha-or-none>
EXPECTED COMMIT: <exact-result-commit-message-or-none>
STATUS: <next-run-id-or-aucun> · retry <ordinal-or-n/a> · attempts alloués <allocated-count-or-n/a>
COMMAND: <authority-derived-run-next-or-aucune>
```

Populate the completed-run line from durable report and verified result evidence, never a planned commit or assumption; use `n/a` and `none` when no completed run exists. `DIRECTORY` and `BRANCH` identify the operator's actual checkout/worktree; `PREPARED TIP` is the exact remote commit containing the next prepared or corrected run, not the prior result. `EXPECTED COMMIT` comes from that run's request. `STATUS` gives the next run, retry ordinal and cumulative attempts already allocated (zero before first invocation). `COMMAND` is the one authority-derived operator command (`dwf run next` or a qualified `dwfm run next`), or `aucune` if no next run is authorized. After `pushed` verify remote result and prepare one successor; after `failed` analyze durable evidence and correct the same run. A second failure requires incident review. This is agent education, not a chat-response runtime gate, repository-specific boilerplate, or an extra operator step.

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

The supported 0.26 upgrade contract requires a clean repository and no pending run; it upgrades exact embedded guidance 0.22, 0.23, 0.24 or 0.25 directly to 0.26 in one result commit. Unknown, divergent or partial guidance is refused without overwrite; the operation replaces only `docs/workflow`, verifies the target pack, creates and pushes one deterministic result commit, and observes the remote result. No manual Git command follows a successful upgrade. If delivery fails after the guidance commit exists, restore connectivity and use normal `dwf run next`; pending-delivery recovery reuses that existing commit.

Divergent or unsupported guidance is refused without overwrite. Review the reported files and preserve repository-owned custom documentation outside `docs/workflow`.

Use `dwf prompt` to reproduce the bounded startup prompt for another agent conversation.
