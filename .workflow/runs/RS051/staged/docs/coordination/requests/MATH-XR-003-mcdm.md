# MATH-XR-003 — MCDM numerical primitives without ranking changes

Status: draft. Recipient: MCDM agent. Not sent.
Transmission: none. Approval: not requested; adoption: not started.
Trigger: Math M3-W02-C04-T2; candidate scope is generic numeric arithmetic only.
Source baseline: aihallot/dw.tools.math main at 9bbbdde4f0c8a2df30f585f5b1a20e5f6d514cc3 (RS050 successful attempt 1).
Math product evidence: docs/planning/evidence/M3-W02-C04-consumer-qualified.json.
Candidate matrix: docs/coordination/matrices/m3-decision-mcdm-primitive-boundaries.json.

## Qualified Math packages, not a transferred MCDM dependency

- dw.quantities 0.2.0-preview.1 — observed local package SHA-256 f5b0085ebc2540a519a33b9f7af6b19dfcbe06e2d7ad1064f388bb8367fadaf5.
- dw.tools.math.statistics 0.4.0-preview.6 — observed local package SHA-256 b27fc5697235ed111897d634e573ae4ebbb92919b9772f0292d6088903ef1fb6, dependent on dw.quantities.
- Source-side isolated .NET 10 consumer: 31 mathematical probes on observed SDK 10.0.402 win-x64; no MCDM checkout, MCDM tests, transferred package or MCDM acceptance was observed.
- The local NuGet feed build/artifacts/m3-w02-c04-local-feed is not a public, retrievable or authorized recipient feed. The hashes identify only the actual RS050 artifacts, and any new build or delivered artifact needs its own pin.

## Question for MCDM owner

Would a deliberately chosen Math primitive reduce numerical duplication without altering accepted ranking, tie, flow or preference behavior? At the current MCDM revision, first inspect the real implementations and call sites, not merely the indicative names.

1. Inspect CompensatedSum.cs as a candidate for mathematical accumulation. Compare its exact operation order, NaN/overflow handling, numerical precision, cancellation/compensation, and zero/tolerance behavior against finite Math sums. Changed accumulation may move threshold crossings, equalities or rankings.
2. Treat generic SquareMatrix.cs only as a **hold**. No package SHA or isolated consumer proof for dw.tools.math.numerics is established by RS050's two-package C04 consumer. Independently inspect matrix dimensions, relations, exact/finite conversions, consumers and current package identity before offering an adapter.
3. Keep FlowQuantization, criterion Normalization, preference functions, PROMETHEE, alternative ranking, ties and all multicriteria choice semantics under MCDM ownership. Mathematical operators may change only by independent recipient acceptance; a Math helper is not MCDM-ranking equivalent by default.

## Required observation and rollback

Collect the exact current MCDM revision, symbols and data shapes, independent examples including close ties and equal ranks, historical rank/flow outputs, normalization and quantization tolerances, nonfinite and degenerate inputs, package version/hash of transferred artifacts, and measured numerical divergences.

The owning MCDM agent must run its existing preference/PROMETHEE/FlowQuantization and tie/rank oracles, record any changed ordering explicitly, demonstrate old/new facade compatibility and rollback, and decide whether a proposed reuse is worth added dependencies. Preserve original code until migration is independently verified and accepted. Do not ask Math to rewrite a multicriteria solver or infer preference semantics.

Requested response: accepted / rejected / needs_change; reviewed MCDM revision, concrete target files/functions, package identities and actual delivered artifact hashes, comparisons/test results, tie/rank divergence assessment, facade and rollback plan, relevant public API/data impacts, and recipient commit if separately authorized.

**Do not send automatically.** This Math-owned draft is not evidence of MCDM acceptance or repository mutation; neither PROMETHEE nor FlowQuantization is transferred to Math.
