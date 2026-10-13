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
