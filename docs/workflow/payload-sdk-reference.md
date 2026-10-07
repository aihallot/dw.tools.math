---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.payload-sdk-reference"
documentVersion: 3
kind: "guidance"
title: "Payload SDK Reference"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/Payloads/PayloadOperationCatalog.cs"
---
# Payload SDK Reference

Generated deterministically from compiled `PayloadOperationAttribute` metadata, the compiled self-audited operation contract registry, and the compiled public-surface registry. This file must not be edited as an independent contract or signature list.

## PayloadContext properties

These properties are the complete public context surface available to a payload after `PayloadContext.Create` succeeds.

- `PayloadFiles Files` — Bounded complete-file deletion, installation, and deterministic text-writing operations.
- `PayloadInspect Inspect` — Bounded repository inspections that publish Markdown artifacts.
- `PayloadJson Json` — Typed semantic JSON-object mutation and AOT-safe array construction operations.
- `PayloadManagedMarkdown ManagedMarkdown` — Typed creation, versioned mutation, lifecycle, requirement, and inspection operations for canonical Managed Markdown.
- `PayloadProjectPlan ProjectPlan` — Bounded typed requirements, target-aware transitions, and exact phase/task structural authoring over the canonical workflow project plan.
- `string RepositoryRoot` — Absolute root that contains every repository mutation and repository inspection path.
- `PayloadRequire Require` — Read-only precondition operations.
- `IReadOnlyList<PayloadOperationResult> Results` — Ordered immutable view of operation results recorded by this context instance.
- `PayloadRevision Revision` — Immutable commit-bound text-blob reads and explicit deterministic revision-surface installation.
- `string RunRoot` — Absolute root of the selected run; staged inputs must remain below this root.
- `PayloadText Text` — Exact ordinal text mutation operations.
- `PayloadWorkflow Workflow` — Strict read-only requirements over canonical workflow execution state and prior successful run evidence.
- `PayloadWorkspace Workspace` — Bounded exact requirements and baseline convergence over persistent named workspace descriptors.

## Support types

These are the complete Payload SDK types reachable from operation signatures, context properties, and nested result metadata.

### `ManagedMarkdownKind`

Typed classification serialized by the canonical Managed Markdown codec.

**Values**

- `Guidance` — Operational or engineering guidance.
- `Audit` — Audit findings or review evidence.
- `Evidence` — Durable validation evidence.
- `Decision` — Accepted decision and rationale.
- `Plan` — Project, implementation, or rollout plan.
- `Handoff` — Conversation or operator transition state.
- `GeneratedReference` — Reference generated from another canonical source.
- `Specification` — Normative product or technical specification.
- `Other` — Explicit fallback classification.

### `ManagedMarkdownMetadata`

Typed canonical metadata supplied to and verified by Managed Markdown operations.

**Public properties**

- `string DocumentId` — Stable logical document identity.
- `int DocumentVersion` — Positive logical revision owned and advanced by managed mutations.
- `ManagedMarkdownKind Kind` — Typed document classification.
- `string ManagedBy` — Stable helper/schema identity that owns serialization.
- `string Owner` — Canonical subsystem or responsibility that governs the document.
- `int SchemaVersion` — Version of the Managed Markdown metadata contract.
- `string? SourceOfTruth` — Optional canonical producer when the document is generated or mirrored.
- `ManagedMarkdownStatus Status` — Typed lifecycle state.
- `string Title` — Human-readable document title.

### `ManagedMarkdownStatus`

Typed lifecycle state serialized by the canonical Managed Markdown codec.

**Values**

- `Draft` — Document is not yet authoritative.
- `Active` — Document is current and authoritative for its declared scope.
- `Superseded` — Document has been replaced by newer authority.
- `Archived` — Document is retained only for history.
- `Generated` — Document is regenerated from its declared source of truth.

### `PayloadArtifact`

Metadata for one bounded artifact written under the runner-supplied artifact root.

**Public properties**

- `long Bytes` — Persisted UTF-8 byte count.
- `string MediaType` — Artifact media type; current inspection artifacts use text/markdown.
- `string Path` — Artifact-root-relative normalized path.
- `PayloadArtifactPurpose Purpose` — Evidence or next-run-context classification.
- `bool Truncated` — Whether an explicit operation bound truncated the artifact.

### `PayloadArtifactPurpose`

Durable role assigned to an inspection artifact.

**Values**

- `Evidence` — Evidence retained for review of the current run.
- `NextRunContext` — Bounded context intended to help author or correct a later run.

### `PayloadFinalNewline`

Explicit final-newline policy accepted by Files.WriteText.

**Values**

- `Preserve` — Preserve whether the supplied normalized content ends with a newline.
- `Ensure` — Persist exactly one final newline.
- `Remove` — Persist no final newline.

### `PayloadLineEnding`

Explicit line-ending policy accepted by Files.WriteText.

**Values**

- `Lf` — Persist LF line endings.
- `CrLf` — Persist CRLF line endings.
- `PreserveExisting` — Use CRLF only when the existing destination contains CRLF; otherwise use LF.

### `PayloadOperationResult`

Immutable record returned by repository, requirement, and inspection operations and persisted in payload observation reports.

**Public properties**

- `string? AfterSha256` — Optional SHA-256 of the resulting complete content.
- `string Area` — Lower-case reporting area used in terminal and durable operation output.
- `IReadOnlyList<PayloadArtifact>? Artifacts` — Optional bounded artifact metadata emitted by inspection operations.
- `string? BeforeSha256` — Optional SHA-256 of the previous complete content.
- `string? Details` — Optional deterministic detail string.
- `string Operation` — Lower-case operation name used in terminal and durable operation output.
- `string Path` — Normalized repository, run, or inspection input path associated with the operation.
- `PayloadOperationStatus Status` — Verified, changed, or no-change result classification.
- `string Summary` — Compact deterministic result summary.

### `PayloadOperationStatus`

Result classification recorded for every successful high-level operation that returns PayloadOperationResult.

**Values**

- `Verified` — A read-only requirement or inspection completed successfully.
- `Changed` — A repository mutation changed complete persisted content.
- `NoChange` — The requested final state already existed and no repository bytes changed.

### `PayloadProjectPlanTaskSpec`

Exact direct-task specification accepted by bounded ProjectPlan structural authoring.

**Public properties**

- `string Id` — Exact globally unique project task id.
- `string State` — Valid persisted task lifecycle state.
- `string Title` — Exact task title persisted in the target phase.

### `PayloadRevisionSnapshot`

Immutable canonical Git commit identity returned by Revision.Snapshot and accepted by commit-bound revision operations.

**Public properties**

- `string Commit` — Canonical lowercase 40-character commit id that binds all snapshot reads and installations.

## Context

### `int Context.Complete()`

Writes the bounded operation summary and returns a successful process exit code.

- **Effect:** `Lifecycle`
- **Idempotence:** Does not mutate repository state; unchanged recorded results produce the same summary.

**Guarantees**

- Prints deterministic changed, no-change, verified, and artifact counts.
- Returns process exit code zero.

**Refusals**

- Does not suppress failures raised before completion.

**Limits and boundedness**

- Summarizes only results recorded by the current context instance.

**Example**

```csharp
return payload.Complete();
```

### `PayloadContext Context.Create(string? repositoryRoot = null, string? runRoot = null, string? operationLogPath = null, string? artifactRoot = null)`

Creates a payload context rooted in the current initialized repository and its pending run unless explicit roots are supplied.

- **Effect:** `Lifecycle`
- **Idempotence:** Side-effect free; unchanged inputs and workflow state resolve equivalent roots.

**Guarantees**

- Normalizes all roots to full paths.
- Defaults to the current initialized repository and its one pending run.

**Refusals**

- Refuses invalid explicit roots and implicit resolution when repository state or the pending run is missing; every refusal includes the exact Context.Create signature.

**Limits and boundedness**

- All later paths remain contained by the resolved repository, run, or artifact root.

**Example**

```csharp
var payload = PayloadContext.Create();
```

## Require

### `PayloadOperationResult Require.DirectoryExists(string relativePath)`

Requires one repository-relative directory to exist.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the exact directory remains present.

**Guarantees**

- Checks one repository-contained directory path.

**Refusals**

- Refuses invalid, escaping, or missing directory paths.

**Limits and boundedness**

- Does not enumerate directory contents.

**Example**

```csharp
payload.Require.DirectoryExists("src");
```

### `PayloadOperationResult Require.FileExists(string relativePath)`

Requires one repository-relative file to exist.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the exact file remains present.

**Guarantees**

- Checks one repository-contained file path.

**Refusals**

- Refuses invalid, escaping, or missing file paths.

**Limits and boundedness**

- Does not inspect file contents.

**Example**

```csharp
payload.Require.FileExists("README.md");
```

### `PayloadOperationResult Require.FileMissing(string relativePath)`

Requires a repository-relative file or directory path to be absent.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while neither a file nor directory occupies the path.

**Guarantees**

- Treats files and directories as conflicting existing paths.

**Refusals**

- Refuses invalid or escaping paths and any occupied path.

**Limits and boundedness**

- Checks one repository-contained path without deleting content.

**Example**

```csharp
payload.Require.FileMissing("generated.txt");
```

### `PayloadOperationResult Require.JsonValueBaselineOrTarget(string relativePath, string jsonPointer, JsonNode? baselineValue, JsonNode? targetValue)`

Requires a JSON value selected by RFC 6901 pointer to equal exactly one declared baseline or target value.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the selected JSON value remains one of the two explicitly declared states.

**Guarantees**

- Uses RFC 6901 pointer traversal and structural JsonNode equality.
- Reports whether the observed value is the baseline or target without mutating it.

**Refusals**

- Refuses equal baseline/target values, invalid paths, missing or malformed JSON, invalid pointers, missing targets, or any third state.

**Limits and boundedness**

- Accepts RFC 6901 only; arbitrary JSONPath and automatic repair are excluded.

**Example**

```csharp
payload.Require.JsonValueBaselineOrTarget("data.json", "/status", JsonValue.Create("old"), JsonValue.Create("ready"));
```

### `PayloadOperationResult Require.JsonValueEquals(string relativePath, string jsonPointer, JsonNode? expectedValue)`

Requires a JSON value selected by a bounded RFC 6901 pointer to be structurally equal to the expected JsonNode value.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the JSON document and expected node remain unchanged.

**Guarantees**

- Uses RFC 6901 pointer traversal and structural JsonNode equality.

**Refusals**

- Refuses invalid paths, missing or malformed JSON, invalid pointers, missing targets, or unequal values.

**Limits and boundedness**

- Accepts RFC 6901 only; arbitrary JSONPath is excluded.

**Example**

```csharp
payload.Require.JsonValueEquals("data.json", "/status", JsonValue.Create("ready"));
```

### `PayloadOperationResult Require.TextContains(string relativePath, string value, StringComparison comparison = StringComparison.Ordinal, bool normalizeLineEndings = false, bool trimOuterWhitespace = false)`

Requires semantic text presence with only the explicitly selected ordinal comparison, line-ending normalization, and outer-trim behavior.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while file bytes and explicit comparison options remain unchanged.

**Guarantees**

- Supports only ordinal or ordinal-ignore-case comparison.
- Normalizes line endings or trims outer whitespace only when requested.

**Refusals**

- Refuses unsupported comparison modes, empty values, invalid paths, missing files, or absent text.

**Limits and boundedness**

- Reads one complete text file and performs one containment check.

**Example**

```csharp
payload.Require.TextContains("README.md", "workflow", StringComparison.OrdinalIgnoreCase, normalizeLineEndings: true, trimOuterWhitespace: true);
```

### `PayloadOperationResult Require.TextOccursBaselineOrTarget(string relativePath, string baselineText, string targetText, int expectedOccurrences = 1)`

Requires one exact ordinal baseline or target text cardinality while rejecting third or ambiguous states.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the file remains exactly in the declared baseline or target cardinality state.

**Guarantees**

- Uses ordinal non-overlapping matching and overlap-safe target ranges.
- Reports baseline or target state without mutating repository bytes.

**Refusals**

- Refuses empty or equal markers, counts below one, invalid paths, missing files, ambiguous mixed states, and undeclared third states.

**Limits and boundedness**

- Reads one complete text file; exactly one declared state must match expectedOccurrences.

**Example**

```csharp
payload.Require.TextOccursBaselineOrTarget("state.yaml", "status: old", "status: ready");
```

### `PayloadOperationResult Require.TextOccursExactly(string relativePath, string value, int expectedOccurrences = 1)`

Requires an exact ordinal occurrence count for a machine-significant text fragment.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while file bytes and the expected count remain unchanged.

**Guarantees**

- Uses ordinal non-overlapping occurrence counting.

**Refusals**

- Refuses negative counts, empty values, invalid paths, missing files, or count mismatches.

**Limits and boundedness**

- Reads one complete text file without normalization.

**Example**

```csharp
payload.Require.TextOccursExactly("state.yaml", "status: active");
```

## Files

### `PayloadOperationResult Files.ConvergeFromStaged(string stagedRelativePath, string destinationRelativePath, string expectedBaselineSha256)`

Atomically converges one existing repository file to complete staged bytes only from an explicit baseline SHA-256 or an already-applied target.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when destination bytes already equal the staged target; otherwise writes only from the exact declared baseline SHA-256.

**Guarantees**

- Reads one complete staged target and one complete existing destination before mutation.
- Persists exact staged bytes through the existing atomic writer and records complete before/after SHA-256 values.

**Refusals**

- Refuses invalid or escaping paths, missing staged sources or destinations, directory destinations, invalid baseline hashes, baseline identities equal to the target, undeclared third states, and surfaced filesystem failures.

**Limits and boundedness**

- Converges one complete existing repository file only; implicit creation, partial merge, semantic repair, deletion, and multi-file transactions are excluded.

**Example**

```csharp
payload.Files.ConvergeFromStaged("staged/Program.cs", "src/Program.cs", baselineSha256);
```

### `PayloadOperationResult Files.Delete(string destinationRelativePath)`

Deletes one repository-contained file idempotently without recursive directory deletion.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the path is already absent; otherwise deletes exactly one file and converges to absence.

**Guarantees**

- Records the deleted file's complete SHA-256 before mutation and verifies that the target is absent after deletion.

**Refusals**

- Refuses invalid or escaping paths, directory targets, and surfaced filesystem deletion failures.

**Limits and boundedness**

- Deletes one repository-contained file only; directory and recursive deletion are excluded.

**Example**

```csharp
payload.Files.Delete("scripts/obsolete.ps1");
```

### `PayloadOperationResult Files.ReplaceFromStaged(string stagedRelativePath, string destinationRelativePath)`

Atomically installs a complete staged file from the current run and reports byte-level idempotence.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when bytes already match; otherwise one atomic write converges to staged bytes.

**Guarantees**

- Reads one complete staged file and records before/after SHA-256 values.

**Refusals**

- Refuses invalid or escaping paths and a missing staged source.

**Limits and boundedness**

- Replaces one complete repository file; no partial merge is performed.

**Example**

```csharp
payload.Files.ReplaceFromStaged("staged/Program.cs", "src/Program.cs");
```

### `PayloadOperationResult Files.WriteComplete(string destinationRelativePath, string content, bool requireExisting = false)`

Atomically writes exact UTF-8 content without implicit line-ending or final-newline normalization.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when exact UTF-8 bytes match; otherwise one atomic write converges to supplied content.

**Guarantees**

- Writes UTF-8 without BOM and performs no implicit text normalization.

**Refusals**

- Refuses invalid paths and, when required, a missing destination.

**Limits and boundedness**

- Writes one complete repository file.

**Example**

```csharp
payload.Files.WriteComplete("generated.txt", content);
```

### `PayloadOperationResult Files.WriteText(string destinationRelativePath, string content, PayloadLineEnding lineEnding = PayloadLineEnding.Lf, PayloadFinalNewline finalNewline = PayloadFinalNewline.Ensure, bool requireExisting = false)`

Atomically writes UTF-8 text using explicit line-ending and final-newline policies.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when policy-normalized bytes match; otherwise one atomic write converges to them.

**Guarantees**

- Applies only explicit line-ending and final-newline policies.

**Refusals**

- Refuses invalid policies, invalid paths, and, when required, a missing destination.

**Limits and boundedness**

- PreserveExisting selects CRLF only when existing content contains CRLF; otherwise LF.

**Example**

```csharp
payload.Files.WriteText("generated.md", content, PayloadLineEnding.Lf, PayloadFinalNewline.Ensure);
```

## Text

### `PayloadOperationResult Text.ReplaceBetweenMarkers(string relativePath, string beginMarker, string endMarker, string replacement)`

Replaces the exact content between one unique ordered begin marker and one unique ordered end marker while preserving both markers.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the unique bounded section matches; otherwise one atomic write converges to replacement.

**Guarantees**

- Preserves one unique ordered begin marker and one unique ordered end marker.

**Refusals**

- Refuses empty, equal, duplicated, reversed, or replacement-contained markers and missing files.

**Limits and boundedness**

- Requires exactly one begin and one end marker in one complete text file.

**Example**

```csharp
payload.Text.ReplaceBetweenMarkers("README.md", "<!-- BEGIN -->", "<!-- END -->", "\nNew body\n");
```

### `PayloadOperationResult Text.ReplaceExact(string relativePath, string oldText, string newText, int expectedOccurrences = 1, string? alreadyAppliedText = null)`

Replaces an exact ordinal fragment with strict ambiguity refusal and overlap-safe recognition of an already-applied final marker.

- **Effect:** `RepositoryMutation`
- **Idempotence:** Recognizes the explicit applied marker as no-change; otherwise converges through one atomic exact replacement.

**Guarantees**

- Uses ordinal matching and refuses ambiguous old/applied states before writing.

**Refusals**

- Refuses invalid counts, empty markers, invalid paths, missing files, or ambiguous occurrences.

**Limits and boundedness**

- Replaces exactly expectedOccurrences old fragments outside applied-marker ranges.

**Example**

```csharp
payload.Text.ReplaceExact("README.md", oldText, newText);
```

### `PayloadOperationResult Text.ReplaceExactMany(string relativePath, IReadOnlyList<KeyValuePair<string, string>> replacements)`

Atomically applies an ordered set of independent one-occurrence exact text replacements to one file after validating the complete converged target in memory.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when every pair is already at its exact target; otherwise writes once only after every pair and the combined target validate in memory.

**Guarantees**

- Evaluates every pair against the same original file, accepts exactly one baseline or one target occurrence per pair, rejects overlapping baseline ranges, and validates the complete combined target before writing.

**Refusals**

- Refuses null or empty replacement sets, empty or equal markers, invalid paths, missing files, ambiguous occurrences, overlapping baseline ranges, and combined targets that do not converge.

**Limits and boundedness**

- Mutates one complete text file; each pair is bounded to one occurrence and advanced cardinality or custom applied markers remain the responsibility of Text.ReplaceExact.

**Example**

```csharp
payload.Text.ReplaceExactMany("state.yaml", new[] { KeyValuePair.Create("status: old", "status: ready"), KeyValuePair.Create("next: old", "next: new") });
```

## Json

### `JsonArray Json.Array(JsonNode[] nodes)`

Constructs an AOT-safe in-memory JsonArray from explicit JsonNode values without generic JsonArray.Add<T>.

- **Effect:** `Lifecycle`
- **Idempotence:** Side-effect free; equivalent JsonNode inputs produce equivalent in-memory array structure.

**Guarantees**

- Copies each supplied JsonNode through DeepClone and inserts it through the non-generic JsonNode path.
- Avoids the JsonArray.Add<T> trimming and AOT warning family.

**Refusals**

- Refuses a null values array or null entries instead of inferring serialization semantics.

**Limits and boundedness**

- Constructs one in-memory array only; it does not mutate repository files or record a payload operation result.

**Example**

```csharp
var items = payload.Json.Array(new JsonObject { ["id"] = "alpha" });
```

### `PayloadOperationResult Json.EditObject(string relativePath, Action<JsonObject> edit)`

Applies a typed semantic edit to a JSON object root and persists deterministic indented UTF-8/LF text.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when semantic object JSON is unchanged; otherwise converges to deterministic indented UTF-8/LF JSON.

**Guarantees**

- Requires an object root and persists one final LF.

**Refusals**

- Refuses null delegates, invalid paths, missing or malformed JSON, non-object roots, and surfaced delegate failures.

**Limits and boundedness**

- Edits one complete JSON object document; array and scalar roots are excluded.

**Example**

```csharp
payload.Json.EditObject("data.json", root => root["status"] = "ready");
```

### `JsonArray Json.StringArray(String[] values)`

Constructs an AOT-safe in-memory JsonArray from string values without generic JsonArray.Add<T>.

- **Effect:** `Lifecycle`
- **Idempotence:** Side-effect free; equivalent string inputs produce equivalent in-memory array structure.

**Guarantees**

- Creates explicit JsonValue string nodes and inserts them through the non-generic JsonNode path.
- Avoids the JsonArray.Add<T> trimming and AOT warning family.

**Refusals**

- Refuses a null values array or null entries.

**Limits and boundedness**

- Constructs one in-memory string array only; it does not mutate repository files or record a payload operation result.

**Example**

```csharp
var names = payload.Json.StringArray("alpha", "beta");
```

## ProjectPlan

### `PayloadOperationResult ProjectPlan.ActivateReadyContinuation(string leafId)`

Activates one ready descendant leaf beneath the existing active project hierarchy while preserving already-active ancestors and accepting exact target re-entry.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the requested leaf already terminates the current active hierarchy; otherwise activates only the explicitly requested effectively-ready descendant suffix.

**Guarantees**

- Requires an existing active hierarchy that is an exact ancestor prefix of the requested leaf chain.
- Prevalidates every suffix node as declared and effectively ready before mutation, validates the complete plan transactionally, persists canonical ProjectTracking JSON, and records complete before/after SHA-256 values.

**Refusals**

- Refuses empty leaf ids, invalid or missing project plans, unknown or non-leaf targets, missing active hierarchy, divergent or longer active hierarchy, any non-ready suffix node, and full-plan validation failures.

**Limits and boundedness**

- Activates one ancestor-to-leaf continuation suffix only; branch selection, readiness transitions, workflow-state reconciliation, completion propagation, arbitrary JSON mutation, and intent repair are excluded.

**Example**

```csharp
payload.ProjectPlan.ActivateReadyContinuation("ST001");
```

### `PayloadOperationResult ProjectPlan.ConvergeNodeToDone(string nodeId)`

Converges one exact in-progress project node through the canonical implemented and validated lifecycle states to done, accepting exact done target re-entry.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the exact done target already exists; otherwise converges only from canonical in-progress through implemented and validated to done.

**Guarantees**

- Delegates every lifecycle transition and full-plan validation to the existing project-planning contract.
- Prevalidates the source state before mutation, persists one canonical ProjectTracking JSON write after the complete transition chain, and records complete before/after SHA-256 values.

**Refusals**

- Refuses empty ids, invalid or missing project plans, unknown nodes, any source state other than canonical in-progress or exact done target re-entry, illegal lifecycle transitions, and full-plan validation failures.

**Limits and boundedness**

- Completes one exact canonical project-plan node only; readiness activation, parent completion propagation, workflow-state mutation, arbitrary JSON mutation, automatic repair, and intermediate-state recovery are excluded.

**Example**

```csharp
payload.ProjectPlan.ConvergeNodeToDone("T001");
```

### `PayloadOperationResult ProjectPlan.EnsurePhaseWithTasks(string workPackageId, string phaseId, string phaseTitle, string phaseState, PayloadProjectPlanTaskSpec[] tasks)`

Ensures one exact phase with an explicitly declared ordered task list under one existing work package, accepting only missing-or-exact-target state.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the exact declared phase/task target already exists; otherwise creates only when every declared target id is absent.

**Guarantees**

- Requires one existing direct work-package parent and valid persisted phase/task lifecycle states.
- Creates one exact phase with the explicitly ordered direct task list, serializes through ProjectTracking, and records complete before/after SHA-256 values.
- Reuses ProjectPlanningContract validation through canonical ProjectTracking serialization before persisted bytes are written.

**Refusals**

- Refuses empty identities or titles, null or empty task arrays, duplicate declared ids, invalid project plans, unknown or non-work-package parents, unsupported states, the legacy active alias for new phase/task targets, partial targets, wrong-parent targets, conflicting titles/states/order/metadata, and full-plan validation failures.

**Limits and boundedness**

- Creates one phase with direct task children under one existing work package only; arbitrary subtree shapes, dependencies, checklists, completion conditions, strategic metadata, automatic inference, and generic JSON editing are excluded.

**Example**

```csharp
payload.ProjectPlan.EnsurePhaseWithTasks("WP001", "P001", "Phase", "ready", new PayloadProjectPlanTaskSpec[] { new("T001", "Task", "ready") });
```

### `PayloadOperationResult ProjectPlan.RequireDirectChild(string parentNodeId, string childNodeId)`

Requires one exact direct parent-child identity in the canonical workflow project plan.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the canonical project plan and declared direct-child relationship remain unchanged.

**Guarantees**

- Loads and validates the canonical project plan through the existing project-planning contract.
- Records the complete current project-plan SHA-256 in AfterSha256.

**Refusals**

- Refuses empty ids, invalid or missing project plans, unknown parents, or a child that is not directly owned by the declared parent.

**Limits and boundedness**

- Checks one exact parent-child relationship; recursive inference and arbitrary JSON traversal are excluded.

**Example**

```csharp
payload.ProjectPlan.RequireDirectChild("M001", "WP001");
```

### `PayloadOperationResult ProjectPlan.RequireNodeState(string nodeId, string expectedState)`

Requires one exact project node id to have one explicitly declared lifecycle state in the canonical workflow project plan.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the exact node and explicitly declared state remain unchanged.

**Guarantees**

- Uses the canonical project-plan parser and lifecycle vocabulary.
- Records the complete current project-plan SHA-256 in AfterSha256.

**Refusals**

- Refuses empty ids or states, invalid or missing project plans, unknown nodes, unsupported states, or any state mismatch.

**Limits and boundedness**

- Checks one exact node id and declared state; semantic inference and arbitrary JSON traversal are excluded.

**Example**

```csharp
payload.ProjectPlan.RequireNodeState("ST001", "in-progress");
```

### `PayloadOperationResult ProjectPlan.TransitionNode(string nodeId, string baselineState, string targetState)`

Transitions one exact project node from an explicit baseline lifecycle state to an explicit target lifecycle state with strict target re-entry.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the exact target state already exists; otherwise mutates only from the explicit baseline state.

**Guarantees**

- Delegates transition legality and full-plan validation to the existing project-planning contract.
- Persists deterministic indented UTF-8/LF project-plan JSON and records complete before/after SHA-256 values.

**Refusals**

- Refuses empty, equal, semantically equivalent, unsupported or non-canonical target states, invalid project plans, unknown nodes, undeclared third states, and illegal lifecycle transitions.

**Limits and boundedness**

- Transitions one exact canonical project-plan node only; automatic node inference, workflow-state mutation, arbitrary JSON mutation, and semantic repair are excluded.

**Example**

```csharp
payload.ProjectPlan.TransitionNode("ST001", "ready", "in-progress");
```

## Workspace

### `PayloadOperationResult Workspace.ConvergeBaseline(string workspaceId, string baselineCommit, string baselineProjectPlanSha256, string targetCommit, PayloadOperationResult projectPlanOperation, String[] assignedProjectNodes)`

Converges one named workspace descriptor from an explicit baseline pair to a target base commit and the SHA-256 recorded by one current ProjectPlan operation.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the exact target commit/project-plan pair already exists; otherwise mutates only from the explicit baseline pair.

**Guarantees**

- Requires a current ProjectPlan operation result whose AfterSha256 still matches the canonical project plan.
- Preserves the exact assigned-project-node surface and existing strategic target snapshots while changing only baseCommit and baseProjectPlanSha256.
- Serializes through the existing workspace authority and records complete descriptor before/after SHA-256 values.

**Refusals**

- Refuses invalid workspace, commit or hash identities, stale or non-ProjectPlan operation results, missing or invalid descriptors, assignment mismatch, equal baseline/target pairs, and undeclared third states.

**Limits and boundedness**

- Converges one existing named workspace descriptor only; workspace creation, assignment inference, strategic-target reconstruction and arbitrary JSON mutation are excluded.

**Example**

```csharp
payload.Workspace.ConvergeBaseline("cooking", oldCommit, oldPlanSha256, newCommit, planResult, "M004");
```

### `PayloadOperationResult Workspace.RequireDescriptor(string workspaceId, string expectedBaseCommit, string expectedBaseProjectPlanSha256, String[] assignedProjectNodes)`

Requires one named workspace descriptor to match an explicit baseline identity and exact assigned project-node surface.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the exact descriptor baseline identity and assigned project-node surface remain unchanged.

**Guarantees**

- Loads and validates one persistent descriptor through the existing workspace authority.
- Requires exact baseCommit, baseProjectPlanSha256 and ordinal assigned-project-node sequence while recording the complete descriptor SHA-256 in AfterSha256.

**Refusals**

- Refuses invalid workspace, commit or hash identities, missing or invalid descriptors, baseline mismatch, empty expected assignments, or assignment-surface mismatch.

**Limits and boundedness**

- Checks one existing named workspace descriptor; strategic targets are validated by authority but are not inferred or rewritten.

**Example**

```csharp
payload.Workspace.RequireDescriptor("cooking", baseCommit, baseProjectPlanSha256, "M004");
```

## Revision

### `PayloadOperationResult Revision.InstallTextSurface(PayloadRevisionSnapshot snapshot, String[] relativePaths)`

Installs an explicit deterministic set of text blobs from one immutable revision snapshot to the same repository-relative paths.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when every explicit destination already equals its snapshot text blob; otherwise converges only the declared surface after full prevalidation.

**Guarantees**

- Resolves every declared path as a blob from one immutable canonical commit before the first repository write.
- Normalizes and ordinal-sorts the explicit path surface and records deterministic composite before/after SHA-256 identities.

**Refusals**

- Refuses null snapshots, empty or duplicate path surfaces, invalid or escaping paths, missing or non-blob source paths, directory destinations, and surfaced Git or filesystem failures.

**Limits and boundedness**

- Installs UTF-8 text blobs to the same explicit repository-relative paths only; wildcards, tree enumeration, deletion, binary transport, cross-file atomic transactions, and generic Git execution are excluded.

**Example**

```csharp
payload.Revision.InstallTextSurface(source, "docs/a.md", "data/b.json");
```

### `string Revision.ReadText(PayloadRevisionSnapshot snapshot, string relativePath)`

Reads one repository-contained text blob from an immutable revision snapshot and records its complete SHA-256.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the immutable snapshot commit and selected blob remain unchanged.

**Guarantees**

- Requires one repository-contained normalized path and verifies that the commit entry is a blob.
- Returns the committed text and records its complete UTF-8 SHA-256 plus Git object id in the operation ledger.

**Refusals**

- Refuses null snapshots, invalid or escaping paths, missing paths, non-blob entries, and surfaced Git failures.

**Limits and boundedness**

- Reads one UTF-8 text blob only; binary blob transport, working-tree fallback, tree enumeration, and generic Git execution are excluded.

**Example**

```csharp
var reportText = payload.Revision.ReadText(source, ".aura/workflow/reports/RS001/attempt-001.json");
```

### `PayloadRevisionSnapshot Revision.Snapshot(string revision)`

Resolves one Git revision to an immutable canonical commit-bound repository snapshot.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only; the returned snapshot remains bound to the resolved canonical commit even if refs move later.

**Guarantees**

- Resolves one bounded revision token through the existing Git client to a canonical lowercase 40-character commit id.

**Refusals**

- Refuses blank, untrimmed, multiline, overlong, option-like, unresolvable, or non-commit revisions and surfaced Git failures.

**Limits and boundedness**

- Resolves commit identity only; it does not expose arbitrary Git commands, mutate refs, enumerate trees, or inspect the working tree.

**Example**

```csharp
var source = payload.Revision.Snapshot(sourceCommit);
```

## Workflow

### `PayloadOperationResult Workflow.RequireCurrentRun(string runId, string? expectedMilestoneId, string? expectedWorkPackageId, string? expectedPhaseId, string? expectedTaskId, string? expectedSubtaskId)`

Requires the canonical default workflow state to name one exact pending run and exact active project hierarchy.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the canonical default workflow state preserves the exact pending run and declared hierarchy.

**Guarantees**

- Delegates workflow-state schema and structural validation to the existing workspace execution authority.
- Compares pending run and every declared active hierarchy pointer exactly, including explicit nulls, and records the complete state SHA-256.

**Refusals**

- Refuses invalid run ids, missing or invalid workflow state, pending-run mismatch, any hierarchy mismatch, and surfaced filesystem failures.

**Limits and boundedness**

- Checks the canonical default-workspace state only; project inference, pointer reconciliation, state mutation, and arbitrary JSON traversal are excluded.

**Example**

```csharp
payload.Workflow.RequireCurrentRun("RS100", "M001", "WP001", null, null, null);
```

### `PayloadOperationResult Workflow.RequireMetaCandidateIdentity(string developmentCycleRelativePath, string liveInstallationRelativePath, string expectedSourceCommit, string expectedPackageVersion, string expectedPackageSha256, string expectedCoreAssemblySha256, string expectedInstalledExecutableSha256, string expectedQualificationStatus = "candidate")`

Requires the development-cycle and live-installation records to project one exact isolated meta candidate identity while stable promotion and ordinary-consumer switching remain disabled.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the development-cycle and live-installation records preserve one exact declared meta candidate identity and disabled promotion boundaries.

**Guarantees**

- Requires exact source commit, package version, package/core/executable SHA-256 identities and qualification status in both canonical projections.
- Requires stable promotion to remain disabled in both records and ordinary-consumer switching to remain disabled in the live-installation record.
- Records deterministic complete-file hashes for both projections.

**Refusals**

- Refuses empty declared identities, invalid or missing paths, malformed or non-object JSON, missing identity fields, any identity mismatch, enabled stable promotion, enabled ordinary-consumer switching, and surfaced filesystem failures.

**Limits and boundedness**

- Checks only the two explicitly supplied canonical JSON projections; it does not inspect external installation bytes, mutate files, infer candidate identity, or authorize promotion.

**Example**

```csharp
payload.Workflow.RequireMetaCandidateIdentity("docs/0.1.53-development-cycle.json", "docs/0.1.53-meta-live-installation.json", sourceCommit, packageVersion, packageSha256, coreSha256, executableSha256);
```

### `PayloadOperationResult Workflow.RequirePriorSuccess(string runId, int attempt, string commitMessage, string executorToolVersion, int priorInvocationFailureCount, int preparationFailureCount, int allocatedAttemptFailureCount, bool firstPassSucceeded, bool expectedCorrectionMode = false, Nullable<int> expectedCorrectionSourceAttempt = null)`

Requires one exact prior default-workspace run attempt to satisfy the canonical success-report contract and declared invocation-lineage facts.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the exact prior attempt report and declared success-lineage facts remain unchanged.

**Guarantees**

- Derives the exact default-workspace attempt report path through existing execution authority and delegates success semantics to ReportContractValidator.
- Requires exact executor tool version and prior-invocation failure counts/first-pass truth and records the complete report SHA-256.

**Refusals**

- Refuses invalid run ids or attempts, empty declared identities, negative failure counts, missing or invalid reports, failed payload/validations, boundary violations, commit/correction mismatches, and lineage mismatches.

**Limits and boundedness**

- Checks one explicitly named run attempt only; latest-attempt inference, arbitrary report paths, arbitrary JSON pointers, mutation, and intent repair are excluded.

**Example**

```csharp
payload.Workflow.RequirePriorSuccess("RS099", 1, "feat: prior result", "0.1.53", 0, 0, 0, true);
```

## ManagedMarkdown

### `PayloadOperationResult ManagedMarkdown.FlattenSet(string setDirectoryRelativePath, string destinationRelativePath)`

Validates and reconstructs one complete canonical Managed Markdown document from a deterministic document set without replacing divergent content.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change only when the exact reconstructed destination already exists; otherwise creates one document or refuses divergent replacement.

**Guarantees**

- Orders parts from canonical manifest metadata, validates complete identities, hashes, bounds, and set membership, and reconstructs the exact parent document bytes.

**Refusals**

- Refuses missing or malformed manifests, gaps, duplicates, mixed or altered parts, unexpected entries, invalid UTF-8, hash mismatches, unsafe paths, and divergent destinations.

**Limits and boundedness**

- Consumes one manifest plus 1..256 canonical part files and never deletes or rewrites the source set.

**Example**

```csharp
payload.ManagedMarkdown.FlattenSet("docs/long.parts", "docs/long-restored.md");
```

### `PayloadOperationResult ManagedMarkdown.InspectMetadata(string relativePath, string outputRelativePath, PayloadArtifactPurpose purpose = PayloadArtifactPurpose.NextRunContext)`

Writes a bounded deterministic Markdown artifact describing canonical metadata and body size without copying the body.

- **Effect:** `ArtifactWrite`
- **Idempotence:** Repository-read-only; unchanged document metadata and body size deterministically rewrite the same artifact.

**Guarantees**

- Emits all typed metadata plus body line and UTF-8 byte counts without copying the body.

**Refusals**

- Refuses invalid artifact purposes, invalid or missing files, malformed Managed Markdown, invalid paths, and missing artifact roots.

**Limits and boundedness**

- Writes one bounded Markdown artifact and never copies body content.

**Example**

```csharp
payload.ManagedMarkdown.InspectMetadata("docs/decision.md", "decision-metadata.md");
```

### `PayloadOperationResult ManagedMarkdown.ReplaceBody(string relativePath, int expectedDocumentVersion, string body)`

Replaces the complete body under an expected document-version precondition and advances the version exactly once when content changes.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the expected current version and complete body already match; otherwise advances documentVersion exactly once and atomically replaces the document.

**Guarantees**

- Preserves all metadata except codec-owned documentVersion and validates the complete replacement body.

**Refusals**

- Refuses null bodies, invalid or missing files, malformed Managed Markdown, stale versions, invalid body representation, and exhausted document versions.

**Limits and boundedness**

- Replaces one complete body; partial Markdown editing and implicit whitespace repair are excluded.

**Example**

```csharp
payload.ManagedMarkdown.ReplaceBody("docs/decision.md", 1, "# Revised decision\n");
```

### `PayloadOperationResult ManagedMarkdown.RequireMetadata(string relativePath, ManagedMarkdownMetadata expectedMetadata)`

Requires one canonical Managed Markdown document to have exactly the supplied metadata.

- **Effect:** `ReadOnly`
- **Idempotence:** Read-only and stable while the complete canonical metadata remains equal to the supplied record.

**Guarantees**

- Parses canonical Managed Markdown and compares every metadata field by typed record equality.

**Refusals**

- Refuses null expected metadata, invalid or missing files, malformed Managed Markdown, and any metadata difference.

**Limits and boundedness**

- Does not compare or expose body content.

**Example**

```csharp
payload.ManagedMarkdown.RequireMetadata("docs/decision.md", expectedMetadata);
```

### `PayloadOperationResult ManagedMarkdown.SetStatus(string relativePath, int expectedDocumentVersion, ManagedMarkdownStatus status)`

Changes only the lifecycle status under an expected document-version precondition and advances the version exactly once when status changes.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change when the expected current version already has the requested status; otherwise advances documentVersion exactly once and atomically replaces the document.

**Guarantees**

- Changes only status and codec-owned documentVersion while preserving body bytes and all other metadata.

**Refusals**

- Refuses invalid statuses, invalid or missing files, malformed Managed Markdown, stale versions, and exhausted document versions.

**Limits and boundedness**

- Updates one lifecycle field; arbitrary metadata editing is excluded.

**Example**

```csharp
payload.ManagedMarkdown.SetStatus("docs/decision.md", 2, ManagedMarkdownStatus.Active);
```

### `PayloadOperationResult ManagedMarkdown.SplitDocument(string relativePath, int expectedDocumentVersion, string outputDirectoryRelativePath, int maxPartBodyBytes = 65536)`

Creates one deterministic immutable Managed Markdown document set under explicit body-byte and part-count bounds.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change only when the exact deterministic set already exists; otherwise creates one immutable set or refuses divergence.

**Guarantees**

- Requires the expected parent document version, preserves exact body bytes across ordered canonical parts, and records parent/part identities and SHA-256 values in one canonical manifest.

**Refusals**

- Refuses stale versions, invalid or non-canonical parents, unsafe paths, bounds outside 64..1000000 bytes, oversized lines, more than 256 parts, and conflicting destinations.

**Limits and boundedness**

- Splits only at LF line boundaries; each part body remains canonical Managed Markdown and the output directory is created through a temporary-directory move.

**Example**

```csharp
payload.ManagedMarkdown.SplitDocument("docs/long.md", 3, "docs/long.parts", 65536);
```

### `PayloadOperationResult ManagedMarkdown.WriteNew(string destinationRelativePath, ManagedMarkdownMetadata metadata, string body)`

Creates one new canonical Managed Markdown document without replacing divergent existing content.

- **Effect:** `RepositoryMutation`
- **Idempotence:** No-change only when an existing file already has the exact canonical bytes; otherwise creates one new file or refuses divergence.

**Guarantees**

- Requires documentVersion 1 and serializes through the canonical Managed Markdown codec.
- Never replaces divergent existing content.

**Refusals**

- Refuses null metadata or body, invalid metadata or body, invalid paths, directory destinations, non-initial versions, and conflicting existing files.

**Limits and boundedness**

- Creates one complete repository-contained Markdown file using UTF-8 without BOM and canonical LF representation.

**Example**

```csharp
payload.ManagedMarkdown.WriteNew("docs/decision.md", metadata, "# Decision\n");
```

## Inspect

### `PayloadOperationResult Inspect.CompleteContext(IEnumerable<string> paths, string outputBaseRelativePath, IReadOnlyCollection<string>? extensions = null, int maxPartBytes = 1000000, PayloadArtifactPurpose purpose = PayloadArtifactPurpose.NextRunContext)`

Writes complete selected repository text as one Markdown artifact or the minimum deterministic multipart set without silent truncation.

- **Effect:** `ArtifactWrite`
- **Idempotence:** Repository-read-only; unchanged selected source bytes and output identity reproduce the exact single artifact or multipart set.

**Guarantees**

- Includes every file in the explicit path and extension universe after only documented generated/local exclusions.
- Emits one Markdown file when possible or the minimum deterministic numbered parts plus a manifest when splitting is required.
- Records every artifact as non-truncated and preserves exact source bytes, order, ranges, byte counts, and SHA-256 evidence.

**Refusals**

- Refuses null or empty paths, missing or escaping inputs, invalid extensions, invalid UTF-8, NUL-bearing content, reparse points, absent artifact roots, unsafe output paths, invalid part bounds, excessive file/source/part counts, and conflicting or divergent artifact output.

**Limits and boundedness**

- maxPartBytes is 1000..1000000; at most 100000 files, 250000000 selected source bytes, and 256 parts are accepted; excluded directories are .git, artifacts, bin, node_modules, and obj, plus .aura/workflow/local, .workflow/local, and the active artifact root.

**Example**

```csharp
payload.Inspect.CompleteContext(["src", "tests"], "complete-context", extensions: [".cs"]);
```

### `PayloadOperationResult Inspect.FileWindow(string relativePath, int startLine, int endLine, string outputRelativePath, PayloadArtifactPurpose purpose = PayloadArtifactPurpose.NextRunContext)`

Writes a bounded numbered line window from one repository text file.

- **Effect:** `ArtifactWrite`
- **Idempotence:** Repository-read-only; unchanged input deterministically rewrites the same artifact.

**Guarantees**

- Uses one-based inclusive line numbers and stable numbered output.

**Refusals**

- Refuses invalid ranges, missing files, invalid paths, or invalid artifact roots.

**Limits and boundedness**

- At most 500 requested lines are accepted.

**Example**

```csharp
payload.Inspect.FileWindow("README.md", 1, 120, "readme-window.md");
```

### `PayloadOperationResult Inspect.Flatten(IEnumerable<string> paths, string outputRelativePath, IReadOnlyCollection<string>? extensions = null, int maxFiles = 50, int maxBytes = 100000, PayloadArtifactPurpose purpose = PayloadArtifactPurpose.NextRunContext)`

Writes bounded readable repository files into one Markdown context artifact with optional extension filtering.

- **Effect:** `ArtifactWrite`
- **Idempotence:** Repository-read-only; unchanged inputs deterministically rewrite the same artifact.

**Guarantees**

- Aggregates readable text in deterministic order with optional extension filtering and explicit truncation.

**Refusals**

- Refuses null/empty inputs, invalid paths, invalid artifact roots, or invalid file/byte limits.

**Limits and boundedness**

- maxFiles is 1..500; maxBytes is 1000..2000000; oversized or sampled-binary files are skipped.

**Example**

```csharp
payload.Inspect.Flatten(["src"], "source-context.md", extensions: [".cs"]);
```

### `PayloadOperationResult Inspect.Search(string pattern, IEnumerable<string> paths, string outputRelativePath, int maxMatches = 200, bool ignoreCase = false, PayloadArtifactPurpose purpose = PayloadArtifactPurpose.NextRunContext)`

Searches bounded readable repository text with explicit ordinal case behavior and writes Markdown matches.

- **Effect:** `ArtifactWrite`
- **Idempotence:** Repository-read-only; unchanged inputs deterministically rewrite the same artifact.

**Guarantees**

- Searches readable text in deterministic path order with explicit ordinal case behavior.

**Refusals**

- Refuses empty patterns, null/empty inputs, invalid paths, invalid artifact roots, or invalid maxMatches.

**Limits and boundedness**

- maxMatches is 1..2000; files above 1000000 bytes or sampled as binary are skipped.

**Example**

```csharp
payload.Inspect.Search("TODO", ["src"], "todo-search.md", ignoreCase: true);
```

### `PayloadOperationResult Inspect.Tree(IEnumerable<string> paths, string outputRelativePath, int maxEntries = 500, PayloadArtifactPurpose purpose = PayloadArtifactPurpose.NextRunContext)`

Writes a bounded Markdown tree for repository-relative files and directories.

- **Effect:** `ArtifactWrite`
- **Idempotence:** Repository-read-only; unchanged inputs deterministically rewrite the same artifact.

**Guarantees**

- Sorts inputs and entries and records truncation.

**Refusals**

- Refuses null/empty inputs, invalid or missing paths, invalid artifact roots, or invalid maxEntries.

**Limits and boundedness**

- maxEntries is 1..5000; excluded directories are .git, bin, obj, artifacts, and node_modules.

**Example**

```csharp
payload.Inspect.Tree(["src", "tests"], "tree.md");
```

Exact identifiers, protocol markers, paths, hashes, and mutations remain ordinal by default. Case, line-ending, or outer-whitespace tolerance is available only where the signature exposes it explicitly.

No regex replacement, generic YAML editor, arbitrary JSONPath, recursive filesystem deletion helper, Git helper, generalized whitespace-insensitive mutation, or automatic intent repair is part of the SDK.
