# RS369 - Decision D7 numeric subsidiarity consultation

Decision request received through the operator on 2026-11-26. Authority: [Decision ff04ff40](https://github.com/aihallot/dw.tools.decision/commit/ff04ff409291fbd7ca9e51566fad4c741916fa18), its OWNER-REQUEST, RS157 evidence matrix, RS193 qualification and durable report. The Math historical MATH-XR-002 draft remains unsent in repository history. This is an owner-side planning decision, not a downstream adoption or publication.

Math baseline: RS368 first-pass success, result e658eb36c28df115eaab8541880ee25bb7d9e4dd, five M10 nodes ready, no production cutover. Preserve that readiness and all historical evidence. Before actual M10 cutover, review the existing numerical surface and admit a distinct bounded functional scope for the accepted extensions. Do not change SDK, framework, language, packages, Decision or the existing native hierarchy in RS369.

| Responsibility | Disposition | Boundary |
| --- | --- | --- |
| SplitMix64/checkpoints | EXISTS | Reuse Math SplitMix64Random and codec; Decision owns signed count, legacy serialized identity and facade exhaustion parity. |
| Bernoulli/uniform | EXISTS | One word per sample; preserve intentional Math half-open rounding and historical Decision behavior. |
| Larger generic quantiles | ACCEPT-PROPOSAL | Extend existing conventioned API with explicit budget; preserve legacy 4096 cap, target one million finite binary64 observations and independent oracles. |
| Exact rational to Int64 | ACCEPT-PROPOSAL | Reuse ExactRational; generic integer factor, nonintegral/overflow refusals, exact signed boundaries, no rounding. |
| Resumable moments | ACCEPT-PROPOSAL | Generic finite mean/population/sample variance, O(1) state, one million observations, typed budgets/refusals, versioned replay, no silent state corruption. |
| Bounded integer draws | EXISTS | Existing M9 versioned rejection sampler; optional new consumer capability, not legacy replacement. |

Existing public source: src/projects/dw.tools.math.statistics (SplitMix64Random, SplitMix64PositionCodec, BernoulliDistribution64, ContinuousUniformDistribution64, QuantileStatistics64, DescriptiveStatistics64) and src/projects/dw.quantities/ExactRational.cs. The remote code search was inconclusive; the local tracked-source roster and candidate inventory must be reviewed before claiming a missing API. Do not create redundant code. The accepted extensions are NOT-PUBLISHED and NOT-TESTED; signatures and budgets will be finalized in RED.

Published private Statistics 20260.20261120.1001 (net10.0): archive feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1, DLL 684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea. Exact Quantities 20260.20261102.1001: archive e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d, DLL c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4. Math RS273 physical Windows net11 consumer evidence and Decision RS193 Windows net10 consumer evidence are separate, bounded proofs. Historical RS154/155/156 observations refer to original binaries, not requalification of the new API. No Linux/all-RID claim.

Proposed sequence after inventory review: separately admit canonical and native future scope, then exact conversion, resumable moments, larger quantiles, independently qualify additive candidates and seek publication authorization if needed; refresh M10 baseline before cutover. Keep historical packages and APIs for rollback. Decision owns risk, VaR/CVaR, Monte Carlo, cancellation, business budgets, solver orchestration and public Decision compatibility. No sibling changes or new release authorization.

Inventory: [tracked source inventory](../evidence/decision-numeric-source-inventory-rs369.md). This report is a lexical review aid; semantic review and eventual independent executable oracles remain required.
