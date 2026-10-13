# M3-W02-C03 — versioned distributions and random sampling policy (T1)

**Proposed:** RS048; admitted only after controlled RED/GREEN and DWF terminal success. Scope T1 only, preserving the accepted C03-T2 boundary.

1. A true standalone product need exists for discrete Bernoulli CDF, mass, quantile and deterministic sampling. Generic Math owns distribution arithmetic; Decision/MCDM own risk/preferences. Do not invent an external provider migration or import an unqualified Decision RNG implementation.
2. Use SplitMix64 fixed-increment modulo-2^64 output mixing as a small portable algorithm frozen by AlgorithmId=splitmix64/v1, 64-bit unsigned seed, consumed-draw counter, serialized logical position, and high-53-bit NextUnitInterval conversion. Exact result bit patterns must be stable under restoration and independently corroborated by published seed 0, seed 1 and seed 42 vectors.
3. A high-53-bit fraction is in [0,1) on every finite result and never returns 1. Every generator call, including degenerate Bernoulli p=0 or p=1 samples, consumes exactly one 64-bit word. An exhausted counter refuses rather than wrapping. The current implementation uses no global/OS entropy.
4. Bernoulli p is admitted only as a finite binary64 value in [0,1]. PMF/CDF are discrete integer-support contracts. Quantile uses the lowest positive-mass support reached by q, making p=1 => quantile 1 even at q=0; the convention is explicit, not a generic unbounded inverse CDF.
5. The raw algorithm and test-vector references are independent: https://prng.di.unimi.it/splitmix64.c (public-domain source) and https://commons.apache.org/proper/commons-rng/commons-rng-core/jacoco/org.apache.commons.rng.core.source64/SplitMix64.java.html . Document and test exact UInt64 vectors and cross-run checkpointing without adding a product provider dependency.
6. The generator is **not cryptographic**. Reproducible arithmetic is not a prediction/inference guarantee, statistical safety claim, secure random service or validated cross-platform/RID delivery. Random distributions beyond Bernoulli (including normal approximations) and complete invalid/boundary policy belong to T2 or later accepted scope.
7. T1 requires independent RED against preview.4, focused GREEN tests including boundaries and golden vectors, 103 prior statistics regressions and provider-free local preview.5 package inspection. The phase C03 remains in progress. No public feed release, Decision adoption, neutral dw.discovery version binding or AURA broker admission follows.

**Review trigger:** downstream consumers request deterministic multi-stream or parallel sampling, validated cross-runtime serialization, distribution-specific tails, an alternative PRNG, stronger numerical stability, or an actual cryptographic RNG.

## RS049 — T2 finite continuous and checkpoint policy

**Proposed; executable admission requires successful RS049 terminal evidence.**

1. Add one bounded continuous uniform family to complete the accepted continuous-vs-discrete boundary without claiming normal/Gaussian distribution coverage. Its finite IEEE binary64 endpoints, interval width, reciprocal density, and finite arguments are explicit preconditions. Any unrepresentable intermediate refuses rather than leaking nonfinite values.
2. Specify continuous uniform PDF on [a,b), CDF with clamped normalized displacement, generalized-inverse quantile q in [0,1], and half-open finite sampled outputs. The sampler consumes one SplitMix64/v1 raw word and uses the maximum-adjacent binary64 representable number if a rounded interpolation would equal the upper bound. Rounding changes sampling on the representable grid; it cannot substantiate an exact continuous law.
3. Keep Bernoulli's *positive-mass support* quantile convention, especially p=0 and p=1 at q=0/1, separate from the continuous-uniform inverse-CDF convention. Reject NaN, infinity and invalid probability/domain parameters. Do not alter RS048 generator golden vectors or counter increments.
4. A stable machine-readable checkpoint codec writes splitmix64/v1, unsigned seed and consumed counter as exactly two sixteen-digit lowercase invariant hex fields separated by pipes. Treat all alternate spellings and unknown versions as invalid. This binds a deterministic *next draw*, not security or a cross-project protocol adoption.
5. SplitMix64 remains reproducible and non-cryptographic. Neither a fixed seed nor its serialization is an entropy source or safe password/secret generator. Statistical inference, parallel independent substreams, nonlocal provider portability and cryptographic suitability remain outside this phase.
6. Qualify a compiled controlled RED, 30 new GREEN tests, all 166 statistics tests, the applicable quantity/numeric regressions and local provider-neutral statistics preview.6 package. Then and only then mark T2 and C03 complete with exact native DWF achievements and matching backlog/projection evidence. Keep M3-W02 in progress for C04.
7. Do not mutate AURA, Decision, MCDM or dw.tools; do not claim neutral dw.discovery readiness, implemented consumer interoperability, public package release or cross-platform observations not in the report.

**Review trigger:** a real consumer requires normal tails, exact rational probability arithmetic, cryptographic sampling, validated parallel streams, a different portable byte format or independently tested Decision interoperability.

