# Conventioned quantiles and paired covariance — M3-W02-C02-T1

**Qualification boundary.** RS046 implements the accepted T1 contract, contingent on the DWF terminal report and independent tests. T2 and the parent C02 phase are explicitly not complete.

## Quantile definitions and number systems

The provider-neutral QuantileStatistics64 API has two named QuantileConvention64 values. Type7 sorts the input ascending and linearly interpolates at the zero-based position (n-1)*p. NearestRank selects the one-based sorted ceiling(n*p), with p=0 selecting the first datum. On [0,10] at p=1/4, Type7 yields 2.5 and nearest-rank yields 0; these are different conventions, not interchangeable approximations.

Evaluate consumes finite IEEE binary64 observations and a binary64 probability. It returns a finite binary64 sample quantile and can refuse an intermediate subtraction or interpolation that overflows even where a differently evaluated mathematical result would be finite. The binary64 probability and interpolation may lose precision. EvaluateExact accepts dw.quantities.ExactRational observations and an exact rational probability. It implements the same named conventions with exact integer rank arithmetic and rational interpolation; it does not convert to double. Each operand and resulting rational intermediate is limited to 4096 numerator/denominator bits. Both surfaces require 1..4096 observations, an admissible probability in [0,1] and a declared convention.

## Paired finite statistics

CorrelationStatistics64.Analyze consumes two aligned finite IEEE binary64 vectors, each of length 2..4096. The immutable result contains count, two means, population covariance (divide by N), sample covariance (divide by N-1), and dimensionless Pearson correlation. Centering is relative to first observed values; Neumaier compensation is reused rather than cloned. Independent [1,2,3] and [2,4,6] yield population covariance 4/3, sample covariance 2 and Pearson 1; opposite affine direction yields Pearson -1. Swapping pair axes and translating them preserve covariance and correlation. The sample covariance is descriptive and does not infer population distributions.

All inputs and intermediates must be finite; unrepresentable intermediate products, sums and normalizers are explicitly rejected, so mathematical finiteness alone does not imply computability under this bounded algorithm. No quantitative universal rounding bound, missing-value imputation, continuous weighting, pairwise-deletion policy, or complete probabilistic inference is claimed.

## Independent qualification

The T1 evidence chain requires a compiled reflection RED against the previously qualified preview.2, then 26 independently authored GREEN tests and 44 existing statistics tests, exact rational and numerical regressions, a .NET 10 locked build and a local package preview.3 manifest inspection without a provider dependency. The runner retains the RED marker, exact source identities, platform and runtime, and a terminal report. Success marks only T1 plus R/G/V done and keeps C02 in-progress.

## Deferred T2 boundaries

T2 owns typed undefined correlation for constant and degenerate vectors, empty/singleton and NaN policies beyond the bounded T1 refusal baseline, null and missing-data semantics, alternative normalizations and integration contracts for Decision/MCDM. A constant vector currently refuses rather than returning a fabricated Pearson value; this refusal is not the future typed undefined outcome. Exact rational covariance, weighted quantiles/covariance, huge or infinite streams, an agent skill, neutral dw.discovery packaging, broker admission, and public feed publication are not qualified by T1.

## RS047 — typed correlation outcomes and explicit missing pairs

This T2 boundary is proposed by RS047 and must not be considered qualified until its durable success report. The existing defined-only CorrelationStatistics64.Analyze remains available and continues to refuse degenerate inputs. The new CorrelationStatistics64.AnalyzeWithStatus accepts two index-aligned nullable IEEE binary64 sequences and an optional MissingPairPolicy64 (default Reject; explicit SkipPair). Null in either component marks the entire pair missing. SkipPair excludes that complete pair; it never zips independently filtered columns. ProvidedPairs, IncludedPairs and MissingPairs remain separate and count source records. Both source vectors must be equal length with 1..4096 source pairs; all-skipped and empty sources refuse. Even a non-null NaN/infinity in the other component of a skipped pair is always refused. No imputation or inferential missing-data claim is implied.

The immutable CorrelationOutcome64 exposes a typed CorrelationDefinitionStatus64 and nullable PearsonCorrelation. Defined has a finite Pearson plus N/N-1 population/sample covariance. UndefinedZeroVariance applies to truly constant admitted axes with at least two pairs: Pearson is null and both covariances are mathematically zero. UndefinedInsufficientPairs applies to exactly one admitted pair: Pearson is null, population covariance is zero and sample covariance is null. A numerical computation failure is not a typed zero-variance outcome and still refuses through arithmetic exceptions; empty or all-missing samples are rejected, not assigned a fabricated mean or correlation. The boolean IsDefined and Precision=FiniteBinary64 are convenience metadata, not a statistical estimate.

T2 also fixes Type-7 binary64 quantiles when p identifies an exact observed knot and the adjacent finite samples have an overflowing difference. In particular [double.MinValue,double.MaxValue] at p=0 returns double.MinValue; p=.5 still refuses because its current interpolating intermediate overflows. Exact rational quantiles and nearest-rank semantics remain unchanged.

The portable mathematical consumer convention is: read the explicit definition status first, only consume PearsonCorrelation when Defined and HasValue, then read population/sample covariance according to the documented N versus N-1 denominator; never equate null with 0, never treat SkipPair as imputation, and never infer VaR/CVaR or Decision-specific risk semantics. These DTO conventions can be used by downstream Decision/MCDM consumers without Math importing those repositories. RS047 does not verify or mutate a Decision adapter, install dw.discovery, expose an AURA broker action, or publish a public NuGet package.

### Qualification gate

A compiled reflection RED must observe missing public status/policy contracts against the RS046 preview.3 baseline before installing T2. Qualification then demands 31 independent GREEN cases, all 103 statistics tests (71 prior, 31 new and one RED after GREEN), relevant quantity and numerical regression tests, locked .NET 10 build, native/project planning validators and provider-free local NuGet preview.4 inspection. RS047 completion closes T2 and C02, not M3-W02 or M3. Any failure retains the same run and its real attempt/correction lineage.

