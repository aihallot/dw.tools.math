# M3-W02-C02 — T1 quantile and paired-statistics conventions

**Decision status:** proposed by the RS046 payload; becomes admitted only on its durable successful DWF report. Retain T2 as a separate acceptance scope.

1. Quantiles are definition-dependent: Type-7 uses position (n-1)*p and linear interpolation, nearest-rank uses ceiling(n*p) (first datum at p=0). Both choose sorted sample elements, not an inferred distribution. The [0,10], p=1/4 disagreement is intentional.
2. ExactRational sample quantiles use exact p and interpolation without binary64 conversion, with a 4096-bit numerator/denominator budget and 4096-element input bound. Finite binary64 sample quantiles use finite binary64 interpolation and refuse nonfinite intermediate arithmetic. No universal error bound is promised.
3. Finite paired covariance is the mean product of centered paired residuals (population N; sample N-1), and Pearson is the centered cross-product divided by the geometric mean of two centered square sums. The result is descriptive, unweighted and provider-neutral. Input is aligned by index; no implicit joining or missing-value convention.
4. Constant vectors cannot yield a meaningful Pearson coefficient. T1 refuses such inputs; T2 must qualify a typed undefined result and additional boundary semantics rather than treating thrown exceptions as that typed outcome.
5. Each vector is limited to 2..4096 finite observations; extreme finite arithmetic can still overflow/underflow intermediate operations, which must refuse rather than silently produce NaN or infinity. The result cannot claim statistical robustness at unbounded magnitudes.
6. RED must be compiled and observed before installing implementation. GREEN uses independent hand-calculated covariance and exact rational quantile oracles, permutation/translation symmetry, bounds and failure tests, plus the previous 44 statistics regressions. The preview.3 NuGet package remains local with only dw.quantities as product dependency.
7. Decision/MCDM are downstream consumers, not source owners; no business VaR/CVaR, neutral discovery feature, AURA skill or public package release is authorized.

**Follow-up:** M3-W02-C02-T2 boundary/integration qualification, including typed undefined Pearson correlation and consumer-facing conventions. Do not mark C02 complete until its own T2 proof.

## RS047 — T2 boundary qualification proposal

The T1 contract remains a baseline and not an unconditional provider claim. RS047 adds a distinct typed CorrelationOutcome64 facade over the T1 finite-binary64 covariance/Pearson engine. This deliberate facade avoids copying numerical implementations or substituting an exception message for a machine-readable status.

- MissingPairPolicy64.Reject is default. SkipPair is explicit and removes one *aligned source pair* on either null, while retaining source/included/missing pair counts; NaN/infinity remain invalid whether a peer is missing or not. Empty and entirely missing inputs refuse. Source vectors must align and each contain no more than 4096 rows before filtering.
- Defined correlation and the existing unweighted covariance denominators N and N-1 are reused from T1. With at least two complete pairs and an exactly constant axis, UndefinedZeroVariance reports null Pearson but zero population and sample covariance. One admitted pair produces UndefinedInsufficientPairs with population covariance 0 and nullable sample covariance null. Those are mathematically distinct from a defined Pearson coefficient of zero.
- Numeric overflow, nonfinite intermediates and rounding limitations do not become typed undefined conditions. For sufficiently small nonzero variation that cannot be resolved by the qualified T1 arithmetic, the computation can refuse rather than invent a zero-variance classification. No guarantee of a universal floating error envelope.
- Type-7 binary64 quantiles at exact sorted knots bypass subtraction of adjacent extremes; nontrivial interpolation can still refuse. Exact-rational and nearest-rank contracts are unchanged.
- Consumers can interpret the typed mathematical status and nullable coefficient through publicly documented names and explicit arithmetic conventions. This is **not** proof that Decision or MCDM has adopted any implementation, nor does it confer neutral dw.discovery schema conformance, generated skills or AURA broker admission.
- Independent RED/GREEN, regression and preview.4 manifest evidence must precede completing native task T2 and its C02 phase. Work package M3-W02 continues toward distributions/randomness and other future chunks.

**Review trigger:** a downstream consumer requests alternative complete-case semantics, statistical inference, exact paired moments, robust rescaling of extreme Pearson inputs, or a versioned cross-repository interoperability adapter.
