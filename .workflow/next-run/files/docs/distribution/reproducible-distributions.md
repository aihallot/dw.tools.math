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

## RS049 — finite continuous uniform boundaries and portable PRNG checkpoints

**Qualification boundary.** This T2 addition is admitted only by an actual successful RS049 report. RS048 T1 remains the separately qualified Bernoulli and raw SplitMix64/v1 baseline. C03 closes only after both native tasks have durable scientific proof.

### ContinuousUniformDistribution64: an explicitly approximate continuous example

The standalone ContinuousUniformDistribution64 represents a mathematical uniform distribution on finite endpoints a and b with b greater than a. The interval width b-a and the density 1/(b-a) must also be finite binary64; parameterizations with overflowing widths or unrepresentably high densities are refused. This is an intentional resource and arithmetic limit: not every valid real-number interval is admissible. Both bounds are immutable.

The probability density is 1/(b-a) on [a,b) and zero outside; zero density at b is a documented endpoint convention. The CDF returns 0 at or below a, 1 at or above b, and clamps the finite binary64 estimate (x-a)/(b-a) inside. The continuous generalized-inverse quantile returns a at q=0, b at q=1 and the bounded binary64 affine interpolation a+q*(b-a) for 0<q<1. All CDF/density arguments must be finite and all quantile probabilities finite within [0,1]. Because the operations are binary64 approximations, an interior quantile may round to an endpoint. No exact rational continuous-distribution contract is claimed.

Sample(generator) consumes exactly one NextUnitInterval output from the already-qualified SplitMix64Random. It returns an approximation in the half-open interval [a,b), clamping a rounded-up affine sample to Math.BitDecrement(b). This guarantees the upper bound is never emitted by sampling, even for adjacent representable endpoints. The finite floating output grid does not itself form a mathematically continuous measure and must not be advertised as perfect distributional sampling. No Gaussian/normal CDF, inverse-erf quantile, convergence theorem, or tail accuracy certification is admitted.

### Canonical versioned text for seed/counter restoration

SplitMix64PositionCodec implements a strict machine-and-culture-independent ASCII checkpoint with three pipe-delimited fields:

    splitmix64/v1|0123456789abcdef|0000000000000005

The first field is exactly the previously qualified algorithm identifier. The next two fields each contain exactly sixteen lowercase ASCII hexadecimal digits: first unsigned seed, then the number of draws **already consumed**. The position denotes the *next* draw. Format rejects unknown algorithm identities; Parse rejects uppercase hex, whitespace, nonhex characters, abbreviated/extra fields and unknown algorithms. Parsing does not advance the PRNG. To resume, pass the decoded value to SplitMix64Random.FromSnapshot; a maximal counter still refuses on the next draw instead of wrapping. This is a Math-owned public wire convention, not an implemented or qualified Decision/MCDM adapter or an AURA broker protocol.

### Bernoulli and security restrictions remain explicit

RS048 already established the p=0/p=1 PMF and quantile endpoints, finite invalid-parameter refusal, half-open high-53-bit unit interval, one-word consumption, and seed 0/1/42 independent golden vectors. Continuous-uniform and Bernoulli quantile conventions are distinct: the first is a continuous generalized inverse, the second is the smallest *positive-mass* support reaching q. No sample distribution is intrinsically cryptographic. SplitMix64/v1 is non-cryptographic; public seed/counter serialization does not add entropy, secrecy, resistance to prediction, or cryptographic security. No user secrets should be generated with it.

### Independent qualification gate

RS049 must first compile and observe its focused reflection RED against preview.5 before installing new product types. The GREEN suite has **30 independent tests**, and the complete statistics suite is expected to include **166 tests** (135 existing, 30 new GREEN and one new RED). Independent analytic uniform-density/CDF/quantile oracles, exact high-53-bit source word, sampler upper-bound proofs, canonical external checkpoint literal and refusal tests supplement all existing statistics assertions. The run also checks pertinent numerical/quantity regressions, the native plan and a provider-free local preview.6 package.

Do not infer independent cross-RID execution, public feed publication, qualified external Decision/MCDM consumers, normal distribution providers, neutral dw.discovery packages, or agent/Aura skills from C03 completion.

