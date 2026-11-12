# Math Discovery — selected capability reference

Generated from `docs/discovery/qualification/m8-discovery-manifest.json` (schema 2).

This is source-project CLR evidence, not a package-installed or broker-authorized consumer. No public distribution or AURA adoption is implied.

## `math.quantity.exact-pipeline.evaluate`

- Package: `dw.tools.math.composition` (`0.3.0-preview.1`).
- CLR binding: `Dw.Tools.Math.Composition.ExactQuantityPipeline.Evaluate`.
- Signature SHA-256: `2d0a6e50c25ec8a6fe0fd66c57480c62af20ebeb8552a7c8f52579de2e228fff`.
- Result: `Dw.Tools.Math.Composition.ExactPipelineResult`.
- Interpretation: Exact bounded quantity-pipeline outcome.
- Numerical limits: MaximumSteps=16 and MaximumNumeralDigits=256; only zero-offset linear units
- Failure policy: Respect the method's explicit result or exception contract.
- Evidence example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Exposure: `AgentCallable`; authorization: `runtime-only`.
- Parameters:
  - `steps`: `System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>` (required).

## `math.replay.try-evaluate`

- Package: `dw.tools.math.composition` (`0.3.0-preview.1`).
- CLR binding: `Dw.Tools.Math.Composition.ExactReplayRunner.TryRun`.
- Signature SHA-256: `8aa8781f285cb4b36bdd7eda6bedada37ee9bba5095de72f564e95b99e50e5ce`.
- Result: `Dw.Tools.Math.Composition.ExactReplayAttempt`.
- Interpretation: Typed replay attempt with explicit incomplete outcomes.
- Numerical limits: Cache 1..32 entries, cooperative cancellation, immutable success/no partial result
- Failure policy: Respect the method's explicit result or exception contract.
- Evidence example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Exposure: `AgentCallable`; authorization: `runtime-only`.
- Parameters:
  - `steps`: `System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>` (required).
  - `context`: `Dw.Tools.Math.Composition.ExactReplayContext` (required).
  - `cache`: `Dw.Tools.Math.Composition.ExactReplayCache` (optional).
  - `cancellationToken`: `System.Threading.CancellationToken` (optional).

## `math.ir.canonical.encode`

- Package: `dw.tools.math.ir` (`0.3.0-preview.1`).
- CLR binding: `Dw.Tools.Math.Ir.IrCanonicalJsonCodec.Encode`.
- Signature SHA-256: `ec4026982c019292a25cf8a683210890ce30d6cd3da8bc0c58c069a324345676`.
- Result: `System.String`.
- Interpretation: Canonical math-ir/1 JSON for admitted bounded IR nodes.
- Numerical limits: MaximumJsonCharacters=262144; IR graph depth/nodes bounded
- Failure policy: Respect the method's explicit result or exception contract.
- Evidence example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Exposure: `AgentCallable`; authorization: `runtime-only`.
- Parameters:
  - `node`: `Dw.Tools.Math.Ir.IrNode` (required).

## `math.ir.hash.semantic`

- Package: `dw.tools.math.ir` (`0.3.0-preview.1`).
- CLR binding: `Dw.Tools.Math.Ir.IrCanonicalHashes.SemanticStructuralSha256`.
- Signature SHA-256: `bee43baf778153e5d26959e8c803de9fc25df0026c65aa40f55f16dd189a621c`.
- Result: `System.String`.
- Interpretation: Structural identity, not algebraic equivalence.
- Numerical limits: Only admitted bounded mathematical IR graph
- Failure policy: Respect the method's explicit result or exception contract.
- Evidence example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Exposure: `AgentCallable`; authorization: `runtime-only`.
- Parameters:
  - `node`: `Dw.Tools.Math.Ir.IrNode` (required).

No item is BrokerReady; invocation must be authorized independently by the runtime.
