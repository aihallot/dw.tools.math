# M3-W02-C01 — missing-value and frequency-weight policy

**Target qualification:** RS045. This document states a bounded contract; the
result is not admitted until the DWF terminal report and independent tests
succeed.

## Frozen first responsibility

RS044 qualified the provider-neutral `dw.tools.math.statistics` T1 surface:
exact rational sum/mean/extrema and bounded finite-binary64 compensated sum,
mean, population/sample variance and min/max. The T1 tests, local preview.1
NuGet package and independent managed Math.NET 5.0.0 corroboration are the
baseline. Neither continuous weighting nor an implicit missing-value policy
was previously admitted.

## Admitted T2 policy on completion

An input is a record `FrequencyWeightedObservation64(double? Value, int
Frequency)`. Frequency is a **positive integer replication count**. A
frequency of 2 means two observations identical to the supplied datum.
It does not mean survey importance, probability mass or a precision
weight. Arbitrary real-valued weights require a separately qualified
semantics and denominator convention.

Missing means `Value == null` only. The default
`MissingObservationPolicy64.Reject` refuses the input. A caller can
request `Skip` explicitly: missing source records are counted but their
frequencies are excluded from all aggregates. NaN and infinity are
always refused even with Skip. Zero or negative frequencies always
refuse, including on missing records. Inputs with no admitted values
never yield a fake zero sample.

The result distinguishes total source records, included source records,
missing source records, and effective replicated observation count.
The independent source-record and total effective-frequency budgets
are each at most 4096. The counted weighted sum and centered two-pass
variance are finite IEEE-binary64 computations with Neumaier compensation.
Any nonfinite intermediate, including overflowed weighted products or
squares, refuses the result rather than exposing a partial statistic.

Frequency-replicated population variance divides by effective N; sample
variance divides by effective N-1. A single datum repeated twice has
sample variance zero; only one effective observation yields a nullable
sample variance. An independent oracle is [1,1,3], represented by
frequency weights [1 (frequency 2), 3 (frequency 1)]:
mean 5/3, population variance 8/9, sample variance 4/3.

A large finite exact mathematical aggregate may still be refused when
binary64 intermediates cannot represent it. No universal floating error
or convergence rate is promised; arithmetic is explicitly approximate.
No implicit widening of exact rational T1 inputs occurs.

## Independent qualification and rollback

Require the new capability to be absent in an independently compiled RED,
then qualify 23 GREEN tests, the recovered RED, previous 20 T1 tests,
109 quantity tests and 81 numerical tests, the existing managed provider
smoke, and the local package `dw.tools.math.statistics`
`0.4.0-preview.2` containing only its `dw.quantities` product dependency.
Retain all evidence and exact .NET SDK/RID observations. On failure,
preserve C01 as in-progress and repair the same DWF run; on durable
success, complete T2 and C01, not M3-W02 or M3.

## Explicit deferrals

Missing-value imputation, missing-not-at-random inference,
arbitrary real-valued frequency/importance weights, reliability-weight
sample variance, exact rational weighted variance, unbounded streams,
censored samples, high-order moment stability and probabilistic
distribution inference are not admitted by this bounded responsibility.
Future consumers must not confuse a frequency-variance denominator
with a generalized unbiased survey estimator.

The library has no provider or Aura runtime dependency. It does not
implement a `dw.discovery` manifest, public agent skill, `BrokerReady`
adapter or Decision/MCDM business-specific VaR/CVaR semantics. Those
require separately owned neutral metadata and producer qualification.

**Review trigger:** a real statistics, Decision, MCDM or Aura consumer
requires nonfrequency weights, different missing conventions or
reproducible streaming scale beyond the bounded contract.
