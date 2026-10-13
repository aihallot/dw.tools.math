# Provider-neutral descriptive statistics — M3-W02-C01

**RS044 qualification candidate.** Its source and documentation become
admitted only after a native Math DWF result with executed tests.

## Public finite and exact contracts

The new standalone package `dw.tools.math.statistics` (target
`0.4.0-preview.1`) provides a bounded `DescriptiveStatistics64.Analyze`
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

This first C01 task does **not** define missing-data imputation, weighted
statistics, multi-pass streaming sketches, general numerically stable
higher moments or non-finite NaN propagation conventions. These
require the independently accepted C01-T2 boundary work. Quantile
types, correlations and randomness are later M3-W02 phases.

No food-domain `RecordTransformer`, Decision VaR/CVaR, inference about
population distributions, native BLAS, external agent capability,
`BrokerReady` descriptor, or shared `dw.discovery` attribute is
created by this run. Discovery and multi-level skills remain neutral
cross-library responsibilities owned by the separate `dw.discovery`
project. A compiled public method alone is not agent admission.
