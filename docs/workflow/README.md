---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.readme"
documentVersion: 2
kind: "guidance"
title: "Repository Workflow Guidance"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV012Draft.cs#Readme"
---
# Repository Workflow Guidance

Start with `bootstrap.md`. This directory is the versioned operational guidance installed by `dwf init`; product requirements and architecture remain in the repository's own documentation.

## Independent identities

The installed `dwf` executable version, workflow schema version, and repository guidance version are independent versioned identities. Installing a newer executable does not silently rewrite an initialized repository.

Use:

- `dwf version` to inspect the executable and supported schema;
- `dwf check` to diagnose compatibility and guidance integrity without mutation;
- `dwf guidance upgrade` only when `dwf check` reports an exact supported upgrade;
- `dwf prompt` to reproduce the bounded agent startup prompt.

## Document map

- `bootstrap.md` — shortest repository reconstruction and action selection.
- `principles.md` — stable generic invariants.
- `engineering-guidelines.md` — implementation and validation choices.
- `payload-authoring.md` — preparation quality boundary.
- `lessons-learned.md` — recurring generic failure patterns.
- `powershell-boundary.md` — justified PowerShell use and parser preflight.
- `operator-loop.md` — operator commands, failures, delivery recovery, and guidance updates.
- `agent-playbook.md` — agent decision tree.
- `run-authoring-reference.md` — normative run contract.
- `payload-sdk-reference.md` — generated public Payload SDK catalogue.

The normal path is direct and small. Branch delivery, review requests, worktrees, publication, migration, and self-development are optional capabilities used only when the repository or run explicitly requires them.
