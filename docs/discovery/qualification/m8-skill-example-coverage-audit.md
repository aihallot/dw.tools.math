# M8 Math Discovery — runnable-example coverage audit

Status: evidence-limited assessment; no M8 acceptance or runtime invocation authorization implied.

## Authority and scope

The canonical schema-2 manifest is `docs/discovery/qualification/m8-discovery-manifest.json`. Its four selected `AgentCallable` bindings remain `runtime-only`; `brokerCandidateIds` is empty. The packaged versions qualified by the private-feed consumer are calendar versions, separate from the `0.3.0-preview.1` source library release.

This audit reads existing repository-owned sources and retains their distinct meanings. It does not rerun, revise or replace any of their durable qualifications.

## Observed example coverage

| Selected capability | Existing source-project seed | Existing immutable private-package probe | Outstanding delivery evidence |
| --- | --- | --- | --- |
| `math.quantity.exact-pipeline.evaluate` | `tests/consumers/m8-discovery-clr-seed/Program.cs` reflects the exact public signature, but does not invoke Evaluate | `VerifyM8PrivateBinaryBindings.cs --verify-private-invocations` invokes the packaged method with an empty step sequence and observes an argument refusal | A standalone positive, bounded typed pipeline example using valid steps, with explicit result and unit assertions |
| `math.replay.try-evaluate` | The seed reflects TryRun and its parameters, but does not invoke it | The packaged method is invoked with empty steps and a constructed replay context; a typed Unsupported, nonfinal attempt with no receipt is required | A positive reproducible replay example with valid steps, context isolation and a bounded cache policy |
| `math.ir.canonical.encode` | The seed executes Encode on an exact rational scalar and checks canonical numerator and denominator | The private DLL probe encodes finite binary64 0.5 and checks the canonical `math-ir/1` payload | A user-facing copyable example tied to the skill, retaining exact versus binary64 distinction |
| `math.ir.hash.semantic` | The seed checks structural hash stability through canonical round-trip and passive-label invariance | The private DLL probe checks repeatability and SHA-256 shape | A user-facing copyable example emphasizing structural identity, not algebraic equivalence |

The source seed reports `bindings=4|examples=3` and proves real typed IR behavior, not four positive selected-operation examples. The private packaged verifier reports `methods=4`, including the two Composition refusal cases. Neither statement should be restated as four positive end-user examples.

## Delivery decision

1. Do not mark `M8-W01-C02`, its descendants or `M8` ready/done from current examples.
2. First deliver bounded positive Quantity and Replay examples, preferably through an independent typed consumer rather than claiming a refusal is a successful computation.
3. Then attach navigable, reproducible examples for all four methods to the skill bundle, with provenance, explicit budgets, refusal behavior and no invocation permission implied.
4. Qualify the final selected skill/example/manifest linkage before any M8 lifecycle transition. Existing qualified packages are immutable; no public publication, AURA adoption or BrokerReady promotion is authorized.

## Evidence anchors

- `tests/consumers/m8-discovery-clr-seed/Program.cs`
- `docs/discovery/qualification/VerifyM8PrivateBinaryBindings.cs` (`--verify-private-binaries`, `--verify-private-invocations`)
- `docs/discovery/qualification/ValidateM8SkillBundleConsistency.cs`
- `docs/discovery/qualification/ValidateM8BundledManifest.cs`
- `.agents/skills/dw-tools-math/SKILL.md` and `.agents/skills/dw-tools-math/references/manifest.json`
- `.aura/workflow/reports/RS198/attempt-001.json` and `.aura/workflow/reports/RS202/attempt-001.json`
