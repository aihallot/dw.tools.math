# RS132 — Compiled CLR signatures for the candidate Math Discovery seed

## Objective and authorization

The owner approved the order **A → B → D**: repair documentation, then qualify Math Discovery/AURA, then prepare interproject coordination. RS130 completed A, first-pass. RS131 completed the **independent C# portable dw.discovery schema-2 envelope corpus**: all **13** pinned Decision fixtures (2 accepted, 11 rejected) matched, on SDK 10.0.402/win-x64, with no skill or broker. RS131 was **first-pass YES**, attempt 001. RS132 takes the next bounded evidence-only step in B: **reflect exactly four intentionally selected static candidate CLR operations from actual current-source compiled Math assemblies**, and independently bind their full parameter and return signatures.

This run does **not** admit the draft `M3-W03` work package, modify the accepted `0.1.122` backlog or 541 done native DWF nodes, generate `.agents/skills/dw-tools-math/`, implement a binding adapter, serialize arbitrary Math DTOs, compile method invocation examples, claim BrokerReady, publish NuGet, or communicate with AURA/Decision/MCDM. The new-work-package topology remains unadmitted with Math-installed DWF 0.1.59 (Workflow issue #139); do not hand-edit the native plan or smuggle an implementation under old M3 numerical work.

## Exact evidence boundary and candidate identities

The existing immutable `docs/discovery/agent-surface-candidates.json` is only a **non-authoritative opt-in authoring seed**. Four candidate methods are selected; their exact package, version, assembly, declaring CLR type, public static method name, parameter types, result type and source Git blob IDs must remain unchanged:

1. `math.quantity.exact-pipeline.evaluate` — `Dw.Tools.Math.Composition.ExactQuantityPipeline.Evaluate(IEnumerable<ExactPipelineStep>)`; returns `ExactPipelineResult`, not an instance/parameterless overload.
2. `math.replay.try-evaluate` — `Dw.Tools.Math.Composition.ExactReplayRunner.TryRun(IEnumerable<ExactPipelineStep>, ExactReplayContext, ExactReplayCache, CancellationToken)`; returns `ExactReplayAttempt`; optionality does not eliminate CLR parameters.
3. `math.ir.canonical.encode` — `Dw.Tools.Math.Ir.IrCanonicalJsonCodec.Encode(IrNode)`; returns `string`. This encodes bounded typed IR, not arbitrary symbolic evaluation.
4. `math.ir.hash.semantic` — `Dw.Tools.Math.Ir.IrCanonicalHashes.SemanticStructuralSha256(IrNode)`; returns `string`. Its digest is structural identity, **not** a mathematical equivalence proof.

Both source projects `dw.tools.math.composition 0.3.0-preview.1` and `dw.tools.math.ir 0.3.0-preview.1` depend locally on `dw.quantities 0.2.0-preview.1`. Candidates marked `Documented`, provider helpers, constructors, `FiniteMatrix64.Multiply` and `FiniteVector64.Dot` are **not** automatically admitted as agent operations; the latter two remain instance-method and adapter work, not false static bindings.

## Controlled verification

A **read-only C# BCL** verifier `docs/discovery/qualification/ValidateClrSeedReflection.cs` has two modes. `--check` independently verifies source-pinned candidate JSON, source Git blobs, package metadata, RS131's C# fixture receipts and the accepted 541-node plan. It also validates the RS132 receipt when present. `--reflect` additionally loads the **actual Release-built** Math `dw.quantities`, IR and composition managed DLLs from ignored `build/artifacts/bin/<project>/Release/net10.0/`, binds four public static `MethodInfo` objects to exact declared signatures using .NET reflection (including generic enumerable parameter resolution), and prints ordered `RS132-CLR|capabilityId|signatureSha256|assembly` identities. A duplicate, missing, nonpublic, instance, generic-method, overload-ambiguous, stale signature, source blob or return-type mismatch refuses.

The RS132 DWF payload stages this decision, a compact four-row binding matrix and the standalone verifier. It performs **one locked solution restore, one Release build of the composition project and its local dependency closure**, and a read-only `--reflect` check before writing the source-pinned receipt `docs/discovery/qualification/RS132-clr-seed-qualified.json`. A target `--check` then validates exact returned signature hashes. No DLLs are rewritten, no unmanaged provider loaded, and no source file mutates except the declared decision/matrix/checker/receipt. Runtime reflection here is an **observed selected method binding audit**, not a compiled/typed invocation example or an installed agent binding.

**Result if successful:** four current CLR signatures observed and hashed, 13 previously qualified portable protocol cases retained, no generated skill or BrokerReady exposure, no public API/ABI or external authorization claim.

## Next gate

The next meaningful B work is **a supported native-DWF work-package admission** and deliberately selected compiled C# invocation examples, then deterministic Math generator/manifest and skill qualification (C01/C02/C03), never speculative JSON adapters. The owner-approved D phase follows admitted B only; actual outgoing recipient transmission still needs explicit action-level authorization. Do not create M3-W03 by manually patching `.aura/workflow/plan/project.json`; seek the owning workflow's supported topology authoring path instead.

**Constraints:** Math-only repository; C#/.NET10; no Python, CI/GitHub Actions, package publication, sibling repository edits or opportunistic provider installation. Local outputs under ignored `/build`. Historical RS131 decision/README/checker/evidence SHA-pins are immutable.
