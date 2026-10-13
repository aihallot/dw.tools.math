# Provider-neutral descriptive statistics — M3-W02-C01

**RS044 T1 delivered.** RS045 T2 additions are candidates until the next DWF result is qualified.

## Public finite and exact contracts

The new standalone package `dw.tools.math.statistics` (target
`0.4.0-preview.2`) provides a bounded `DescriptiveStatistics64.Analyze`
contract over finite IEEE 754 binary64 observations. It returns a copied,
immutable count, sum, mean, min, max, population variance and an optional
sample variance. The sample denominator is `n-1`; population variance
uses `n`. One observation admits population variance zero but makes
sample variance unavailable (`null`), rather than inventing an estimate.

Floating summation uses Neumaier compensation and the variance uses a
two-pass offset relative to an observation. These choices address
representable cancellation and large offsets, but do not guarantee
exactness or a universal numerical error bound. Nonfinite inputs and
unrepresentable intermediate sums or squares are refused, not emitted
as apparently valid aggregates.

The independent `AnalyzeExact` overload operates exclusively on the
existing `dw.quantities.ExactRational` type. Sum, mean and extrema remain
rational. No implicit binary64 widening occurs. Both APIs admit
between 1 and 4096 input observations, and the exact path also enforces
a 4096-bit numerator/denominator bound on operands and intermediate
results. Long or infinite iterators are materialized only up to the
4097-observation refusal boundary.

## Independent falsification

An independently compiled reflection RED must fail before installing
the new product implementation. Independent MSTest oracles verify
`[1,2,3,4]`: mean `5/2`, population variance `5/4`, sample
variance `5/3`; exact `1/3+1/6=1/2`; cancellation
`[10^16,1,-10^16]` preserving the small unit through compensated
summation; centered variance at large finite offsets; singleton,
empty, nonfinite, overflow and budget boundaries. A separate managed
Math.NET 5.0.0 smoke corroborates mean and variance, using the pinned
provider on its actually observed runtime without adding a product
dependency.

## Scientific and integration limits

C01-T2 now proposes an explicitly modeled nullable-missing and positive
integer-frequency contract. It does **not** impute missing values or accept
arbitrary real-valued survey/reliability weights. Long-stream sketches,
higher moments, inference and NaN propagation remain deliberately unsupported.
Quantiles, correlations, and randomness are later M3-W02 phases.

No food-domain `RecordTransformer`, Decision VaR/CVaR, inference about
population distributions, native BLAS, external agent capability,
`BrokerReady` descriptor, or shared `dw.discovery` attribute is
created by this run. Discovery and multi-level skills remain neutral
cross-library responsibilities owned by the separate `dw.discovery`
project. A compiled public method alone is not agent admission.

## RS045 — boundary and frequency-weighted contract

The separate `FrequencyWeightedStatistics64.Analyze` requires an explicit
`FrequencyWeightedObservation64(Value, Frequency)` record. An integer
frequency means that the datum is repeated that many times; it is **not**
a continuous importance/probability weight. Every frequency must be
strictly positive. The total effective frequency and original record
count must independently be within 1..4096. The returned immutable
summary distinguishes `ProvidedCount`, `IncludedCount`, `MissingCount`,
and `EffectiveCount`. `Sum`, `Mean`, minimum and maximum operate only
on included observations. Floating sums use the same bounded Neumaier
compensation as the original contract, without divergent helper code.

Only `null` denotes missing. `MissingObservationPolicy64.Reject` is
the default; `Skip` must be opted into explicitly, counts missing input
records and excludes them from the effective frequency. NaN and infinities
are **never** silently treated as missing and are rejected in either mode.
A dataset with no admitted observations is refused; invalid enum values,
zero or negative frequency, more than 4096 input records, or more than
4096 admitted repetitions also refuse without partial statistics.

For replicated-frequency weights, the population variance denominator
is effective `N`, and sample variance uses effective `N-1` where
`N > 1`. Thus values 1 (frequency 2) and 3 (frequency 1) are equivalent
to the expanded sample [1,1,3]: mean 5/3, population variance 8/9 and
sample variance 4/3. A singleton record repeated twice has sample
variance zero; a single effective observation has undefined sample
variance (`null`).

All arithmetic is finite IEEE binary64. Centered two-pass variance
reduces avoidable cancellation, but very large magnitudes, weighted
products, offset subtraction, or squared deviations can overflow.
They yield an explicit refusal rather than fabricated finite statistics.
There is **no** general numerical accuracy bound or guarantee that
intermediates remain representable merely because the final exact
mathematical result would be finite.

RS045 qualification requires an independently observed missing-contract
RED, 23 isolated GREEN cases, all 44 statistics tests, the prior rational
and numerical suites, local provider-free NuGet preview.2 validation and
an explicit final document distinguishing admitted from deferred behavior.
The C01 phase closes only after these gates pass.

### Deferred scopes

General continuous weights, effective degrees of freedom for
nonfrequency weights, arbitrary missing-value imputation, exact
frequency-weighted rational moments, streaming dataflow aggregation
beyond bounded materialization, and numerically robust high-order moments
are not admitted. No food data pipeline, Decision VaR/CVaR, AURA broker
action or `dw.discovery` generated skill is introduced by this work.
