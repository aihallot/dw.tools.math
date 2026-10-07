---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.run-authoring-reference"
documentVersion: 11
kind: "guidance"
title: "Run Authoring Reference"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV021Draft.cs#RunAuthoringReference"
---
# Run Authoring Reference

A pending run is selected by `.aura/workflow/state.json` and lives under `.workflow/runs/<id>/`.

## Minimal direct manifest

```json
{
  "schemaVersion": 1,
  "id": "RS001",
  "title": "Implement one bounded outcome",
  "state": "prepared",
  "commitMessage": "feat: implement bounded outcome",
  "projectNodes": ["M001", "WP001", "P001", "T001", "ST001"],
  "contribution": {
    "summary": "Describe one bounded outcome.",
    "value": "Explain why the outcome matters.",
    "debtChanges": ["Record one bounded debt change."],
    "nextAction": "Name the next bounded action."
  },
  "delivery": { "mode": "direct" },
  "payload": {
    "kind": "csharp-file",
    "path": "payload.cs",
    "workingDirectory": "."
  },
  "mutationPaths": ["src/example.cs", "tests/example.tests.cs"],
  "validations": []
}
```

## Preparation preconditions

Committed preparation requests may declare source-authority preconditions. Text remains backward-compatible when `kind` is omitted:

```json
{
  "repositoryPath": "state.yaml",
  "expectedText": "status: ready",
  "expectedOccurrences": 1
}
```

Use semantic JSON authority explicitly:

```json
{
  "kind": "json-value",
  "repositoryPath": "config.json",
  "jsonPointer": "/status",
  "expectedValue": "ready"
}
```

JSON pointers use bounded RFC 6901 semantics. `expectedValue` is typed JSON, not serialized comparison text. Exact repository bytes remain part of the final preparation fingerprint after semantic preconditions pass.

## Staged validation helpers

Committed preparation requests may declare `stagedFiles` when a payload or preparation-safe validation needs bounded run-owned source material. Each entry's `sourcePath` is resolved relative to the committed request directory. Its `path` is relative to the generated run's `staged/` root and must not include a `staged/` prefix. A declared path `<path>` materializes as `.workflow/runs/<id>/staged/<path>`.

```json
{
  "validations": [
    {
      "id": "generated-csharp-check",
      "classification": "structural",
      "fileName": "dotnet",
      "arguments": [
        "run",
        "--file",
        ".workflow/runs/RS001/staged/check.cs"
      ],
      "workingDirectory": ".",
      "packs": []
    }
  ],
  "stagedFiles": [
    {
      "path": "check.cs",
      "sourcePath": "check.cs"
    }
  ]
}
```

Generated run file references used by PowerShell `-File` and `dotnet run --file` are checked against the generated files of the current run before selection. A generated C# helper invoked through `dotnet run` must use `--file`; a run-root path does not alias a `stagedFiles` entry. Missing, cross-run, or mislocated generated references are refused. The workflow reports the contract violation and does not rewrite the authored command or path.

## Contribution declaration

The `contribution` object carries agent-authored intent separately from observed execution facts. New controlled-progress run preparation validates it through the executable contribution contract; guidance does not replace that validation.

Controlled-progress preparation requires `projectNodes` to contain at least one entry. Every declared project node must exist in the project plan. Use the smallest truthful project scope that contains the contribution; an empty array is invalid.

Stable executable limits:

- `summary`: at most 500 characters, non-empty and single-line.
- `value`: at most 1000 characters, non-empty and single-line.
- optional `nextAction`: at most 500 characters, non-empty and single-line when present.
- `debtChanges`: at most 20 unique entries; each entry is non-empty, single-line, and at most 300 characters.
- optional `achievements`: at most 20 entries with unique project nodes; each `achievement` is non-empty, single-line, and at most 1000 characters.

### Strategic review

`contribution.strategicReview` is required for a comparable strategic target when the repository's configured no-movement threshold has already been reached by prior consecutive successful target-scoped runs, the current run produces no strategic movement for that same target, and no matching review has yet been declared. A run that produces strategic movement does not require a review for that target.

A declaration contains exactly the agent-authored target identity and decision context:

- `projectNode`: non-empty, trimmed, single-line, at most 100 characters; it must name the milestone strategic target.
- `capabilityId`: non-empty, trimmed, single-line, at most 100 characters and must exactly match the target capability id.
- `stage`: non-empty, trimmed, single-line, at most 80 characters and must exactly match the target stage.
- `decision`: non-empty, trimmed, single-line, at most 200 characters.
- `rationale`: non-empty, trimmed, single-line, at most 1000 characters.

The review target must be inside the run's declared `projectNodes` scope. A matching declared review resets the consecutive no-movement sequence to zero. Strategic movement also resets that sequence. Declaring a review does not itself claim project completion or strategic movement.

## Payload re-entry cross-check

Before handoff, inspect every payload-side requirement for facts the payload itself changes. Source-only facts belong in preparation preconditions; payload guards for changing values must accept the declared baseline or completed target and reject third states.

For `Text.ReplaceExact`, the safest normal form is `payload.Text.ReplaceExact(path, oldText, newText)` because the complete `newText` is the default already-applied marker. Do not narrow `alreadyAppliedText` merely because one distinctive substring looks sufficient; when `newText` retains `oldText`, a narrow marker can leave the retained baseline outside the applied range and fail target re-entry.

Prefer `payload.Json.EditObject(...)` for ordinary semantic object mutation instead of manually parsing, editing, serializing and rewriting the same JSON document.

## Rules

- The directory, pending state, and manifest id match exactly.
- `projectNodes` contains at least one project-plan node and every declared node exists in the project plan.
- `mutationPaths` are repository-relative, complete, and minimal.
- The stable payload kind is `csharp-file`; the runner supplies the executing Payload SDK reference unless an explicit project or package reference is declared.
- Working directories already exist and timeouts are bounded.
- Validation classification is `preflight`, `structural`, `focused-preparation`, `focused`, `integrated-preparation`, `integrated`, `full-preparation`, or `full`.
- `preflight`, `structural`, `full-preparation`, and `full` declare no packs. `focused-preparation`, `focused`, `integrated-preparation`, and `integrated` declare named packs.
- A run may declare an ordered validation ladder. Test-profile entries must strictly widen scope: focused < integrated < full; preparation variants share the same breadth as their post-attempt counterpart.
- Preparation-safe test profiles (`focused-preparation`, `integrated-preparation`, `full-preparation`) must precede post-attempt profiles.
- In `planned` mode, when current-run `mutationPaths` produce a narrower differential plan than an explicitly broader certification profile, the workflow applies differential-first ordering by prepending the repository-rendered differential selection. The broader certification remains mandatory.
- Bounded replay of targeted tests inside a later broader certification is intentional: fail fast on the highest-probability current-change surface before paying for broader certification without sacrificing the broader gate.
- `explicit` mode preserves authored validation selection and does not synthesize a differential prefix.
- Preparation-safe `preflight`, `structural`, `focused-preparation`, `integrated-preparation`, and `full-preparation` validations execute before attempt allocation and are reused at the real execution boundary.
- Preparation-safe validations are repository-observational during preparation: each must leave the disposable Git working-tree identity unchanged. Generators, canonical renderers, migrations, and intended repository mutations belong in the payload; mutation is refused before selection even when the changed path is inside the declared mutation boundary.
- Plain `focused`, `integrated`, and `full` remain post-attempt and may contain external side effects; never move them into preparation merely for speed.
- The commit message describes the result, not preparation mechanics.

## Preparation and inspection

`dwf run prepare <request.json>` creates and selects deterministic run material from an operator request. `dwf run prepare --check <request.json>` is a non-selecting agent/CI check for an already committed request: it runs the exact preparation path in a disposable clone, reports the predicted result, and leaves source HEAD, status, workflow state and run material unchanged. It is optional authoring support, not a routine operator command. `dwf run verify-mutations <run-id>` compares declared and observed mutation paths. `dwf run rehearse <run-id>` is an explicit disposable rehearsal for selected high-risk boundaries, not a mandatory step for normal work.

## Correction and delivery

Correct the same run after failure. Exact provenance-bound correction receipts authorize dirty correction, and `dwf run next` selects that mode automatically. Pending delivery receipts cause `dwf run next` to recover the existing result commit without replaying project work. Explicit `--allow-dirty` and `dwf delivery retry` remain compatibility or diagnostic surfaces, not normal operator choices.

Branch delivery, review declarations, and worktree isolation are optional and must be explicitly declared together with their required identities. They do not alter the minimal direct contract.
