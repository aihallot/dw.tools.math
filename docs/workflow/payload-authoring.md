---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.payload-authoring"
documentVersion: 6
kind: "guidance"
title: "Payload Authoring"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV021Draft.cs#PayloadAuthoring"
---
# Payload Authoring

A pending run is an executable change contract. Preparation quality is required even when the runner provides strong guards.

## Mandatory sequence

1. Read the active plan, workflow state, relevant reports, source, tests, and applicable generic lessons.
2. State the intended result, invariants, mutation boundary, failure behavior, and evidence.
3. Use an existing Payload SDK operation when it directly expresses the need; use ordinary C# for project-specific logic.
4. Stage complete source files for language changes.
5. Verify paths, commands, working directories, project nodes, staged membership, and operation-specific limits against current references.
6. Review every validation expectation with the code or artifact that produces it.
7. Compile or parse the final payload topology before real execution.
8. Make the payload idempotent: accept the declared baseline and completed target, reject third states, and converge using complete sources.
9. Preserve the current run id in `pendingRun`; only the runner clears it after successful validation.
10. Stop at the smallest coherent increment.

## Preparation preconditions

Preparation-request `preconditions` describe repository authority before the run is selected. Keep exact text cardinality for genuinely textual authority. For JSON authority, prefer `kind: "json-value"` with a canonical repository path, bounded RFC 6901 `jsonPointer`, and typed `expectedValue`; JSON object property order and formatting are not authority.

Omitting `kind` preserves the historical text contract. A JSON value precondition must not also declare text fields, and a text precondition must not declare JSON fields. The final selection lock still fingerprints exact repository input bytes after semantic validation.

## Source preconditions and target re-entry

Source preparation preconditions may assert facts that the payload intentionally changes. Payload mutation guards have a different responsibility: they must be safe when the payload is evaluated again against its already-applied target.

If a mutation changes source value `A` to target value `B`, keep the source-only assertion in preparation and use `payload.Require.JsonValueBaselineOrTarget(...)` or `payload.Require.TextOccursBaselineOrTarget(...)` only when the payload itself needs a guard. These operations accept only the declared baseline or target and reject third states. A payload-side `JsonValueEquals(A)` for a fact the same payload changes to `B` is a target-reentry defect even when the preparation request may truthfully require `A`.

For `payload.Text.ReplaceExact(...)`, prefer the default already-applied recognition: omit `alreadyAppliedText` when the complete `newText` is the converged target marker. If a custom marker is genuinely required, it must cover the complete converged target region that contains any retained baseline anchor. A narrower custom marker can leave the old text outside the applied range and correctly trigger an ambiguous target-reentry refusal.

For ordinary semantic edits to a JSON object, prefer `payload.Json.EditObject(...)` instead of custom parse/re-emit code. It performs a deterministic indented LF write and returns no-change when the semantic target already matches. Use custom JSON construction only when the project-specific mutation cannot be expressed by the existing helper.

Executable payload compilation, target-reentry rehearsal, and mutation validation remain authoritative. These surfaces improve authoring before operator invocation; they do not authorize automatic repair or weaker gates.

## Mirrored staged files

When a `stagedFiles` entry intentionally uses the same repository-relative destination path below the run's `staged/` directory, express that relationship once with the typed mirror helper:

```csharp
var destination = PayloadPath.RepositoryFile("docs/example.json");
payload.Files.ReplaceFromStagedMirror(destination);
```

The helper deterministically derives the run-root source as `staged/<destination>` and delegates to the existing byte-idempotent staged-file installation contract. Prefer it when source and destination are mirrors so an agent cannot independently mistype the staged prefix or nested destination path.

Use the explicit two-root `ReplaceFromStaged(PayloadPath.RunFile(...), destination)` form only when the staged source and repository destination intentionally differ. The mirror helper is authoring simplification, not automatic repair: a missing derived staged source is still refused before mutation.

## AOT-safe JSON arrays

The strict compilation preflight treats trimming and AOT diagnostics as failures when the payload project enables those analyzers. Do not call `JsonArray.Add<T>(T)` with primitive values; that generic overload carries `IL2026` and `IL3050` hazards.

Use `payload.Json.StringArray(...)` for string arrays or `payload.Json.Array(...)` for already-created `JsonNode` values. Both helpers construct arrays through explicit `JsonNode` insertion and avoid the generic `JsonArray.Add<T>` family. Compilation diagnostics for the known `JsonArray.Add<T>` IL2026/IL3050 pattern point to these helpers without weakening the analyzer gate.

## Assertions

Test the strongest stable contract and no stronger. Exact comparison is appropriate for identifiers, protocol markers, hashes, byte preservation, machine-readable values, and deliberate external text contracts. Prose usually requires semantic markers or explicitly tolerant comparison.

## Topology

A payload must not rebuild or overwrite the project assembly loaded by its own host. Finish mutation first, then validate the changed project in a separate process with isolated outputs.

Inspection and mutation helpers have bounded public contracts. Treat limits documented in `payload-sdk-reference.md` as preparation preconditions; do not rely on a guard to discover an invalid request for the operator.
