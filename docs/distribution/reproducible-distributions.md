# Reproducible probability distributions and seeded draws — M3-W02-C03-T1

**Qualification:** RS048 proposes the first accepted distribution/sampling responsibility. Only its successful durable DWF receipt plus scientific evidence admits this T1 contract; T2 and the C03 phase remain in progress.

## Versioned deterministic generator

SplitMix64Random implements a fixed 64-bit generator algorithm with identifier splitmix64/v1. An unsigned seed initializes a 64-bit state. Each NextUInt64() advances the internal state by 0x9E3779B97F4A7C15 modulo 2^64 and mixes that state using the standard two 64-bit xor-shift/multiply rounds. The public unsigned Counter reports the **number of consumed draws**; the constructor can reconstruct the exact next draw directly from (seed, counter). Snapshot()/FromSnapshot() carry explicit algorithm, seed and counter; an unknown algorithm is refused. Reaching the maximum unsigned counter refuses rather than silently wrapping, and seed zero is valid.

NextUnitInterval() consumes one raw 64-bit draw and takes the high 53 bits multiplied by 2^-53, producing one finite IEEE binary64 value in the half-open interval [0,1). It never emits 1. Each mixed NextUInt64 / NextUnitInterval / Bernoulli.Sample call consumes exactly one 64-bit word. No implicit wall-clock or global random state participates.

Published-independent golden word vectors (hex, first five in draw order) freeze the bit contract:
- seed 0: e220a8397b1dcdaf, 6e789e6aa1b965f4, 06c45d188009454f, f88bb8a8724c81ec, 1b39896a51a8749b
- seed 1: 910a2dec89025cc1, beeb8da1658eec67, f893a2eefb32555e, 71c18690ee42c90b, 71bb54d8d101b5b9
- seed 42: bdd732262feb6e95, 28efe333b266f103, 47526757130f9f52, 581ce1ff0e4ae394, 09bc585a244823f2

Algorithm references: Sebastiano Vigna's public-domain SplitMix64 C implementation, https://prng.di.unimi.it/splitmix64.c ; Apache Commons RNG SplitMix64 Java source, https://commons.apache.org/proper/commons-rng/commons-rng-core/jacoco/org.apache.commons.rng.core.source64/SplitMix64.java.html . Vector fixtures are pinned source-independent expected outputs, not merely self-comparisons. Reproducibility is defined for the algorithm, seed, counter, bit conversion and implementation arithmetic, not for unspecified third-party APIs.

**No cryptographic suitability or security-strength claim.** SplitMix64 is not an entropy source, a password/secret generator or a stream cipher. A fixed seed provides repetition, not unpredictable randomness. Statistical quality for every downstream sampling workload is not universally proven.

## Bernoulli distribution

BernoulliDistribution64 accepts a finite binary64 success probability p in [0,1]. Its integer-support PMF is P(X=0)=1-p, P(X=1)=p (zero mass outside {0,1}). Its integer CDF is 0 below 0, 1-p at 0 and 1 at or above 1. The quantile uses the smallest **positive-mass support** whose CDF reaches q: for interior p, q<=1-p maps to 0, larger q to 1. For the degenerate p=1 distribution every q in [0,1], including q=0, maps to 1; for p=0 it always maps to 0. These are intentionally named conventions rather than distribution-independent quantile claims. Invalid finite ranges, NaN and infinity refuse.

Sample(generator) consumes exactly one NextUnitInterval value and returns 1 iff that value is smaller than p. Therefore p=0 deterministically returns 0, p=1 returns 1, and either boundary still advances the counter once per sample. Bernoulli probability arithmetic and uniform conversion are explicitly finite binary64, not exact rational probability models. No Decision-specific VaR/CVaR, scientific confidence interval or cross-repository simulation adapter is created.

## Independent falsification and integration boundary

The T1 RED must compile and fail against the qualified preview.4 product assembly before new source installation. GREEN must pass independently authored PMF/CDF/quantile boundary examples, hardcoded multi-seed SplitMix64 reference vectors, seed/counter checkpoint and stream-consumption assertions, interval exclusion of 1, Bernoulli sample bit oracles, invalid-argument tests, and provider-independent contracts. Also run the prior 103 statistics tests, the locked .NET 10 build, and locally inspect the statistics preview.5 NuGet identity/dependencies without publishing to a feed.

T2 retains exhaustive invalid-parameter/boundary and implementation integration policy, explicit continuous approximate distribution choices, empirical/analytical edge tests, and further reproducibility stress/restriction analysis. T1 does not qualify a numerical normal distribution, cryptographic randomness, indefinite streams, cross-RID provenance not actually observed, or a Decision/AURA/dw.discovery connector.
