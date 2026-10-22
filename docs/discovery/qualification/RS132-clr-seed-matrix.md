# RS132 — Current compiled Math CLR seed signature matrix

**Scope:** four candidate `AgentCallable` static methods selected for local signature evidence only. They remain **proposed**, are **not** `BrokerReady`, cannot be automatically invoked by AURA, and are not an emitted Discovery manifest or Math skill. Protocol `dw.discovery/1.0.0-draft.1`, schema 2, had 13/13 independent C# portable fixtures qualified under RS131.

## Four source-pinned selected members

| Capability ID | Declaring type and member | Public static parameter signature | Result type |
|---|---|---|---|
| `math.quantity.exact-pipeline.evaluate` | `Dw.Tools.Math.Composition.ExactQuantityPipeline.Evaluate` | `IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>` | `Dw.Tools.Math.Composition.ExactPipelineResult` |
| `math.replay.try-evaluate` | `Dw.Tools.Math.Composition.ExactReplayRunner.TryRun` | `IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>, Dw.Tools.Math.Composition.ExactReplayContext, Dw.Tools.Math.Composition.ExactReplayCache, System.Threading.CancellationToken` | `Dw.Tools.Math.Composition.ExactReplayAttempt` |
| `math.ir.canonical.encode` | `Dw.Tools.Math.Ir.IrCanonicalJsonCodec.Encode` | `Dw.Tools.Math.Ir.IrNode` | `System.String` |
| `math.ir.hash.semantic` | `Dw.Tools.Math.Ir.IrCanonicalHashes.SemanticStructuralSha256` | `Dw.Tools.Math.Ir.IrNode` | `System.String` |

Local assemblies for `dw.tools.math.composition` and `dw.tools.math.ir` (both preview `0.3.0-preview.1`), with `dw.quantities` dependency `0.2.0-preview.1`, are built in Release under `/build/artifacts/bin/`. The independent checker computes SHA-256 from a stable canonical CLR signature string **after actual method reflection**; the candidate authoring JSON is not treated as runtime proof.

## Exclusions and science limits

Only the four exact **static** entries above are selected. Additional public methods, constructors, helpers, reflection catalogues, numerical matrix/vector instance members or unadmitted scientific providers are **not** part of the selected manifest. No external Decision/AURA package reference or helper is needed for this local audit.

The RS132 receipt must record four observed method identities, exact source blobs and candidate provenance, current SDK/RID, the RS131 13/13 portable-fixture evidence and **negative** flags for generated skill, compiled invocation examples, user-facing JSON adapters, `BrokerReady`, other RIDs, public registry release and downstream adoption. An exact CLR signature is not proof of mathematical correctness or generalized serialization: original Math source-qualified numerical limits and domain restrictions still apply.

## Supported DWF continuation

All **541** currently accepted native DWF nodes remain done; no synthetic `M8` or `M3-W03` work package is created from this evidence-only qualification. Before implementing a generator or actual agent-facing runtime, the owner must use a supported native DWF topology admission for the existing proposed `M3-W03-C01/C02/C03` roadmap, or explicitly decide another truthful supported scope. Workflow issue #139 is tracked feedback, not permission to write native project JSON.

Independent read-only C# checker: `dotnet run --file docs/discovery/qualification/ValidateClrSeedReflection.cs -- --check`; compiled CLR inspection: the same file with `--reflect`. No Python, no CI, no public publication or external transmission.
