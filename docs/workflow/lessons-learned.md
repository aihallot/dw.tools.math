---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.lessons-learned"
documentVersion: 2
kind: "guidance"
title: "Generic Failure Patterns"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV012Draft.cs#LessonsLearned"
---
# Generic Failure Patterns

Use these patterns only when relevant to the current work.

## Ambiguous shell interpolation

Punctuation can become part of a PowerShell variable reference. Delimit variables explicitly and parse complete scripts before execution.

## Fragile textual mutation

Line endings, indentation, casing, and source variation make ad hoc replacement unreliable. Prefer complete staged sources or structured transformations. Text replacement needs a unique anchor, an accepted already-applied state, and ambiguity refusal.

## Assertions stronger than the contract

Incidental capitalization, ordering, counts, or preferred prose can reject valid behavior. Assert semantic or machine contracts at the appropriate strength.

## Guessed paths and commands

Plausible names are not evidence. Resolve paths, commands, working directories, staged files, and project nodes from the repository before preparation.

## Guards used as authoring substitutes

Early refusal protects the repository but still costs operator time. Validate operation limits, producer-consumer alignment, and final staged topology before selecting a run.

## Recursive build and loaded assemblies

A payload that rebuilds the assembly hosting it can create deterministic locks and topology failures. Separate mutation from project validation.

## Correction stacks

Repeated wrappers and repairs can conceal the faulty abstraction. After repeated structural failure, replace the mechanism directly.

## Pending-run ownership

Payloads preserve the active run id. The runner clears it only after all validations succeed so same-run correction remains reachable.
