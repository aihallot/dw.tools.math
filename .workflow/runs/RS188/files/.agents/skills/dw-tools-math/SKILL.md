---
name: dw-tools-math
description: Source-qualified Math Discovery CLR capability guide; not a broker or execution authorization.
---
# Math Discovery

Protocol: `dw.discovery/1.0.0-draft.1`. Skill bundle: `0.1.0`.
Exposure: `AgentCallable` only. This file grants no runtime permission, broker readiness, package installation, or AURA adoption.

## Qualified callable surfaces

### `math.quantity.exact-pipeline.evaluate`

- CLR: `Dw.Tools.Math.Composition.ExactQuantityPipeline.Evaluate` (`dw.tools.math.composition`).
- Signature: `dw.tools.math.composition|0.3.0.0|Dw.Tools.Math.Composition.ExactQuantityPipeline|Evaluate|Dw.Tools.Math.Composition.ExactPipelineResult(System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>)`.
- Fingerprint (SHA-256): `2d0a6e50c25ec8a6fe0fd66c57480c62af20ebeb8552a7c8f52579de2e228fff`.
- Result: `Dw.Tools.Math.Composition.ExactPipelineResult` — Exact bounded quantity-pipeline outcome.
- Numeric model: exact rational.
- Limits: MaximumSteps=16 and MaximumNumeralDigits=256; only zero-offset linear units.
- Guarantee: 5 km / 2 min = exactly 150 km/h, dimensions and bounds checked.
- Source example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Authorization: `runtime-only`; resolve permissions in the caller.

### `math.replay.try-evaluate`

- CLR: `Dw.Tools.Math.Composition.ExactReplayRunner.TryRun` (`dw.tools.math.composition`).
- Signature: `dw.tools.math.composition|0.3.0.0|Dw.Tools.Math.Composition.ExactReplayRunner|TryRun|Dw.Tools.Math.Composition.ExactReplayAttempt(System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>,Dw.Tools.Math.Composition.ExactReplayContext,Dw.Tools.Math.Composition.ExactReplayCache,System.Threading.CancellationToken)`.
- Fingerprint (SHA-256): `8aa8781f285cb4b36bdd7eda6bedada37ee9bba5095de72f564e95b99e50e5ce`.
- Result: `Dw.Tools.Math.Composition.ExactReplayAttempt` — Typed replay attempt with explicit incomplete outcomes.
- Numeric model: exact rational and typed status.
- Limits: Cache 1..32 entries, cooperative cancellation, immutable success/no partial result.
- Guarantee: Reproducible exact result and cache isolation across context versions.
- Source example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Authorization: `runtime-only`; resolve permissions in the caller.

### `math.ir.canonical.encode`

- CLR: `Dw.Tools.Math.Ir.IrCanonicalJsonCodec.Encode` (`dw.tools.math.ir`).
- Signature: `dw.tools.math.ir|0.3.0.0|Dw.Tools.Math.Ir.IrCanonicalJsonCodec|Encode|System.String(Dw.Tools.Math.Ir.IrNode)`.
- Fingerprint (SHA-256): `ec4026982c019292a25cf8a683210890ce30d6cd3da8bc0c58c069a324345676`.
- Result: `System.String` — Canonical math-ir/1 JSON for admitted bounded IR nodes.
- Numeric model: exact and explicitly finite binary64, no silent mixing.
- Limits: MaximumJsonCharacters=262144; IR graph depth/nodes bounded.
- Guarantee: Canonical round-trip preserves exact rational and bounded structural IR.
- Source example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Authorization: `runtime-only`; resolve permissions in the caller.

### `math.ir.hash.semantic`

- CLR: `Dw.Tools.Math.Ir.IrCanonicalHashes.SemanticStructuralSha256` (`dw.tools.math.ir`).
- Signature: `dw.tools.math.ir|0.3.0.0|Dw.Tools.Math.Ir.IrCanonicalHashes|SemanticStructuralSha256|System.String(Dw.Tools.Math.Ir.IrNode)`.
- Fingerprint (SHA-256): `bee43baf778153e5d26959e8c803de9fc25df0026c65aa40f55f16dd189a621c`.
- Result: `System.String` — Structural identity, not algebraic equivalence.
- Numeric model: typed IR structural identity.
- Limits: Only admitted bounded mathematical IR graph.
- Guarantee: Ignore passive labels but preserve domain and quantities; stable across canonical encode/decode.
- Source example: `tests/consumers/m8-discovery-clr-seed/Program.cs`.
- Authorization: `runtime-only`; resolve permissions in the caller.

## Boundaries

Only explicitly typed, qualified, bounded Math inputs are supported. Respect each CLR method's failure and cancellation contract. Structural IR hashes do not imply algebraic equivalence. Do not expose any capability as BrokerReady.

## Evidence

Generated from `docs/discovery/qualification/m8-discovery-manifest.json`; qualified by RS186 and RS187. Recreate with `dotnet run --file docs/discovery/qualification/GenerateM8MathSkill.cs -- --write`; verify drift with `--check`.
