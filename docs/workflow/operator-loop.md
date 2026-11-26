---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.operator-loop"
documentVersion: 21
kind: "guidance"
title: "Operator Loop"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV031Draft.cs#OperatorLoop"
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

For controlled ordinary run completions, the final terminal line is the operator verdict. Guidance-only outcomes are separate and do not establish an ordinary RS result (see below):

- `pushed` in yellow means the exact result is durably delivered and remotely verified;
- `failed` in red means the command did not reach that durable success boundary.

When the operator reports `pushed`, the remote agent verifies the result commit, durable report, cleared `pendingRun`, and intended plan transition before preparing later work.

When the operator reports `failed`, the remote agent retrieves the newest durable `workflow/evidence/...` failure publication for the current run and corrects the same run. The operator should not copy the full terminal log when durable failure evidence was published successfully.

If failure evidence itself cannot be published because Git, the remote, or repository identity is unavailable, diagnostics explicitly report that exceptional transport failure before the final `failed` verdict. Only then may the missing terminal detail need to be supplied out of band.

`dwf run next --allow-dirty` and `dwf delivery retry` remain compatibility or diagnostic surfaces; they are not normal operator decisions.

## Mandatory operator handback

Whenever an agent returns control to the operator in a workflow conversation (including status, preparation, correction, failure, success, no prepared run, or handover), end with this mandatory six-field layout: one `Run terminé` line, one blank line, then five ordered operator fields. Nothing may follow `COMMAND`; do not duplicate the fields elsewhere.

```text
Run terminé : <run-id-or-aucun> — <outcome-or-n/a> — attempt <n-or-n/a> — first-pass <success|failure|n/a> — exécuteur <driver-version-or-n/a> — commit <verified-result-sha-or-none>.

DIRECTORY: <operator-checkout>
BRANCH: <operator-branch>
EXPECTED RESULT: pushed/failed [<next-run-id-or-aucun> retry <ordinal-or-n/a>] - <exact-result-commit-message-or-none>
STATUS: <next-run-id-or-aucun> · retry <ordinal-or-n/a> · attempts alloués <allocated-count-or-n/a>
COMMAND: <authority-derived-run-next-or-aucune>
```

Populate the completed-run line from durable report and verified result evidence, never a planned commit or assumption; use `n/a` and `none` when no completed run exists. `DIRECTORY` and `BRANCH` identify the operator's actual checkout/worktree; `EXPECTED RESULT` gives both pushed and failed verdicts with the exact next run ID, current retry ordinal and result commit message from its prepared request. Verify the prepared tip internally on GitHub, but omit it from the operator handback. Do not invent a run ID, retry ordinal or result subject when authority is missing. `STATUS` gives the next run, retry ordinal and cumulative attempts already allocated (zero before first invocation). `COMMAND` is the one authority-derived operator command (`dwf run next` or a qualified `dwfm run next`), or `aucune` if no next run is authorized. After `pushed` verify remote result and prepare one successor; after `failed` analyze durable evidence and correct the same run. A second failure requires incident review. This is agent education, not a chat-response runtime gate, repository-specific boilerplate, or an extra operator step.

Use the exact absolute operator checkout/worktree path for `DIRECTORY`, as resolved from the current prompt or verified operator execution authority. A repository name, relative path, guessed path or stale handover is not a substitute. Keep the existing six-field handback on plain-text lines without additional blocks or fields; nothing follows `COMMAND`.

## Optional project commands

Only for an explicitly selected project and a compatible installed tool, `dwf exec list [--json]` and `dwf exec validate [--json]` inspect optional project recipe metadata. `dwf exec run --id <id>` is a separate, explicit operator action, not a second step of the ordinary workflow. For index selection pin the listed absolute checkout and catalogue digest; for deferred ID selection pin both as well. A missing catalogue is not a run-next blocker. `dwf info` reports its availability, not launch readiness. See [Agent Playbook](agent-playbook.md#optional-project-command-recipes) for authoring and exact command forms.

Do not confuse a recipe exit code, timeout, cancellation, child output or metadata validation with an ordinary RS result. Continue to use `dwf run next` for normal controlled work, with the same mandatory operator handback.

## After installing a newer executable

`dwf info` and `dwf version` (or the qualified meta command) share a read-only projection. The original version/schema/compatibility lines are retained. Channel, embedded and repository guidance, and bundled/installed skill observations are evidence-based; unknown means unverified, not a reason to install or overwrite. The external latest version is not queried. A local channel observation is not a full installation certification.

Run:

```powershell
dwf version
dwf check
```

The executable, workflow schema, and repository guidance have independent versions. Installing a new executable does not rewrite the repository.

On a clean repository, use the normal `dwf run next` command for automatic convergence of an exact supported historical pack. `dwf guidance upgrade` remains an explicit maintenance command; it is not a routine prerequisite.

The supported 0.31 upgrade contract upgrades exact embedded guidance 0.22, 0.23, 0.24, 0.25, 0.26, 0.27, 0.28, 0.29 or 0.30 directly to 0.31. Unknown, divergent, partial or newer guidance is not overwritten or downgraded. Automatic convergence takes place after remote authority refresh in an eligible clean ordinary workspace. Corrections, selected or pending runs, pending delivery and self-replacement take precedence.

An eligible ordinary direct committed request is preserved and freshly prepared against the new authority. Specialized requests and explicit guidance dependencies defer optional convergence. Preserve legitimate guidance pins; when the migrated authority requires request review, revise the committed request before retrying rather than replaying the old request.

`Guidance upgraded; no run executed.` is a guidance-only success, not a delivered ordinary RS. `Guidance convergence failed; no run executed.` is not an allocated ordinary attempt failure. Inspect the diagnostic and guidance delivery evidence; never invent an ordinary attempt, retry or successor. Normal `dwf run next` recovers an existing pending result before later work.

Complete outstanding guidance delivery with its compatible driver before changing versions when possible. A newer driver can recover the original commit only for a unique exact supported target, including a recognized historical target; subsequent convergence is separate. Unknown or mismatched targets are refused. Do not delete or rewrite a receipt or force a push to bypass recovery. Older drivers may not recognize newer targets or metadata.

Divergent or unsupported guidance is refused without overwrite. Review the reported files and preserve repository-owned custom documentation outside `docs/workflow`.

Use `dwf prompt` to reproduce the bounded startup prompt for another agent conversation.
