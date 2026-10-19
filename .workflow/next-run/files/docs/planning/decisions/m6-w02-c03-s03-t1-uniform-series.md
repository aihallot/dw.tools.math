# M6-W02-C03-S03-T1 — bounded uniformly sampled time-series contract

## Authority and scope

RS097 qualifies the first contract/outcome task of the separately planned
M6-W02-C03-S03 child. RS096 durably completed S02; earlier S01 is also done.
The original size-L M6-W02-C03 aggregate remains open, as do S03-T2,
work package M6-W02 and milestone M6.

The BCL-only FiniteUniformSeries64 facade is added to the local
dw.tools.math.scientific 1.1.0-preview.9 package. The previously qualified
preview.8 and existing geometry, direct-signal, special-function and exact
contracts are not silently changed.

## Input, indices and output semantics

- Inputs are 1..32 finite binary64 samples in a single caller-managed
  value unit, with a **strictly positive, finite** explicitly supplied
  sample interval. The interval is returned verbatim as metadata. The
  facade never infers seconds, calendar dates, physical rates, resampling,
  alignment with an absolute origin, or frequency units.
- FirstDifferences requires at least two input values and computes exactly
  N-1 adjacent deltas: result[i] = input[i+1] - input[i]. It does not
  divide by sample interval. It is NOT a derivative, an interpolator,
  a denoising filter or a forecast.
- MovingAverage requires a complete integer window 1..N and returns
  N-window+1 arithmetic means. Result[k] is the mean of input[k] through
  input[k+window-1]; there is no prefix/suffix padding, extrapolation,
  imputation, weighted smoothing or irregular-time resampling.
- Every input magnitude must be <= 1e6. Every output magnitude must be
  <= 2e6. These deliberately conservative local binary64 budgets and
  length bounds are not sharp stability or rounding-error guarantees.
  Differences can be negative. Window 1 is the original signal.
- Outcomes contain either finite samples with runtime window/interval
  metadata and RuleId, or a typed nonpartial refusal with a null samples
  array. Declared statuses are InvalidLength, InvalidSampleInterval,
  InvalidWindow, InvalidInput, OutsideQualifiedRange, NonFiniteResult
  and Cancelled. Pre-cancelled calls are refused before input checks.
- Intervals have no declared calendar or clock unit; callers must
  supply consistent units and actual uniform spacing. This function
  cannot verify whether real observations were uniformly sampled.

## Falsification and local NuGet qualification

The controlled run must compile and execute an independent reflection
RED asserting that the public FiniteUniformSeries64 type is absent from
the previously qualified preview.8 library. A compiler failure is not RED.

After recovering RED, qualify 60 independent GREEN tests including:

- adjacent differences of [1,3,6] => [2,3]
- window-two arithmetic means of [1,3,6] => [2,4.5]
- negative inputs, zero differences, fractional intervals and full
  32-sample windows, while verifying no implicit division by interval
- typed nonpartial refusals for missing/nonfinite samples, invalid
  sample interval, invalid/oversized window, out-of-budget values,
  and pre-emptive cancellation.

Retain **533** prior scientific tests (69 S02-T2, 60 S02-T1,
53 S01-T2, 25 S01-T1, 88 C02-T2, 80 C02-T1, 88 C01-T2,
70 C01-T1) plus four representative Math regression projects.
Verify native-plan projection, planning consistency and transfer audit.

Pack one actual local scientific preview.9 archive and inspect its
net10 DLL, nuspec version, dependency absence, byte length and SHA-256.
Four independently cached exact-package-only eight-probe consumers
must restore from the cleared local feed and agree on real SHA/length:
the new S03 uniform-series client, retained S02-T2 direct-signal audit,
retained S01-T2 geometry audit and retained C02-T2 special-function audit.

Only after durable acceptance close S03-T1 RED/GREEN/V and task T1.
S03-T2 will independently qualify boundary conditioning, sample-interval
interpretation and irregular-time/forecast refusal. Do not close the S03
phase, original C03 size-L aggregate, M6-W02 or M6 on T1 evidence.
No public NuGet, other OS, certified numerical error bound, time-series
forecast, implicit unit conversion or AURA/Decision/MCDM integration
is established by this local task.
