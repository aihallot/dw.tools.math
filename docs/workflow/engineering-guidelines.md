---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.engineering-guidelines"
documentVersion: 2
kind: "guidance"
title: "Engineering Guidelines"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV012Draft.cs#EngineeringGuidelines"
---
# Engineering Guidelines

## Before changing a repository

- Inspect the active objective, pending run, latest relevant report, affected source, tests, and repository conventions.
- Reuse an existing responsibility or primitive before adding a near-duplicate abstraction.
- Define the smallest coherent outcome, invariants, mutation paths, evidence, and rollback behavior.
- Verify every path, command, working directory, project node, and staged file from repository evidence.

## Implementation

- Prefer typed, testable C# for non-trivial behavior.
- Stage complete PowerShell files; do not generate or repair scripts from fragments.
- Keep source mutation and validation in separate processes when the changed project may be loaded by the payload host.
- Write repository text with explicit encoding, line-ending, and final-newline policies.
- Treat exact prose, capitalization, whitespace, order, and occurrence counts as contracts only when explicitly justified.

## Validation

- Review producers and assertions together.
- Run the narrowest conclusive validation for the change.
- Compile or parse staged languages before the operator's real execution.
- Exercise disposable, rollback, delivery, or full-profile boundaries only when their risk is present.
- Record limitations honestly; a successful internal fixture is not independent adoption evidence.

After repeated structural corrections to one mechanism, stop stacking repairs and replace the faulty mechanism with the smallest direct implementation.
