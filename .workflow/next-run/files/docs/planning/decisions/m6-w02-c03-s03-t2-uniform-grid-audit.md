# M6-W02-C03-S03-T2 — explicit uniform timing and conservative signal audit

## Scope and authority

RS098 independently qualifies the boundaries/integration task of S03,
following RS097's local T1 uniform-series qualification. The original
FiniteUniformSeries64 API remains unchanged and callable. A separate
FiniteUniformSeriesBoundaryAudit64 sidecar is staged for the BCL-only
local scientific package 1.1.0-preview.10.

All input values share caller-owned value units. Times are caller-supplied
finite numeric coordinates in an explicitly common unit; no timezone,
calendar, physical time unit, clock origin, interpolation, forecasting,
resampling, derivative computation or implicit missing-data filling
is provided.

## Accepted time grid and refusal rules

- The T2 audit requires an explicit sampleTimes array of exactly the
  same length as the T1 sample array. Missing times return
  SampleTimesRequired; mismatched lengths return InvalidTimeLength.
  Nonfinite time coordinates return InvalidTimeValue. The maximum
  absolute time coordinate is 1e15, a bounded input budget.
- Sample intervals are strictly positive and finite under the retained
  T1 contract. Consecutive time coordinates must strictly increase.
  Negative time origins are acceptable. Equal/decreasing coordinates
  refuse NonIncreasingTime when not already classified as low-resolution.
- With a nonzero time-coordinate magnitude, if the declared interval
  divided by the largest absolute adjacent time coordinate is below
  1e-12, return TimestampResolutionRisk. This can conservatively refuse
  representable timestamps; it is not a measured rounding error.
- Adjacent actual time deltas must agree with the declared interval
  within a fixed relative deviation of 1e-10, using
  abs((time[i]-time[i-1])/interval - 1).
  Otherwise return IrregularSampling, without trying to resample.
  Binary64 decimal grid artifacts below this tolerance are admitted.
- T1 sample budget remains 1..32 values, absolute <=1e6, output
  absolute <=2e6; complete moving windows remain 1..N. Cancellation
  is checked before validation and during every small bounded audit.

## Conservative numeric diagnostics

For each nonzero direct T1 adjacent delta, measure the ratio of its
magnitude to the largest absolute magnitude of its two input values.
If this ratio is below 1e-12, refuse RelativeDifferenceRisk without
exposing a partial output. Equal values with mathematically zero
difference remain valid, because no nonzero relative signal is claimed.

For each complete moving-average window, measure the absolute result
mean divided by the mean of input absolute magnitudes. When that
absolute-magnitude mean is positive and the ratio is below 1e-12,
refuse MeanCancellationRisk. All-zero windows remain admitted.
Conservatively refusing a true zero signed average does not imply
an incorrect direct T1 computation.

Ratios and tolerances are fixed warning heuristics, NOT proven
rounding-error bounds, exact interval reconstructions, or certified
condition numbers. The original T1 functions remain available to
callers who explicitly accept their existing numerical limitations.
Refused audit outcomes never leak partial numeric samples.

## DWF falsification and integration

The controlled RS098 run must compile a missing-public-audit API
reflection RED against the retained preview.9 provider, observing
one actual failing test (not a compiler error). After staging the
sidecar, recover the RED and pass 106 independent GREEN checks:
finite uniform/decimal/negative-origin grids, raw differences,
complete windows, missing/invalid/irregular time coordinates,
time resolution, conservative near-cancellation and typed refusal.

Retain 594 separately qualified scientific tests (61 S03-T1,
69 S02-T2, 60 S02-T1, 53 S01-T2, 25 S01-T1 and 326 C01/C02)
and four representative Math regression projects. Verify structural
project plan and transfer validators and git diff check.

Pack a real local 1.1.0-preview.10 scientific NuGet archive.
Inspect compiled net10 DLL, nuspec identity, dependency absence,
byte count and SHA-256. Requalify **five** source-free exact-package
eight-probe consumers, each independently restored from only a
cleared local feed into its own cache: new S03 T2 audit, retained
S03 T1 uniform-series core, S02 T2 signal audit, S01 T2 geometry,
and C02 T2 special-function boundary. Require all five to report
identical archive byte count and SHA-256.

Only S03-T2-R/G/V, S03-T2 and S03 may close with durable evidence.
The original size-L M6-W02-C03 aggregate tasks T1/T2, work package
M6-W02 and milestone M6 must remain unfinished. Next scope:
parent C03 aggregate T1 across independently delivered S01/S02/S03,
without claiming the original aggregate done before its T2 gate.
