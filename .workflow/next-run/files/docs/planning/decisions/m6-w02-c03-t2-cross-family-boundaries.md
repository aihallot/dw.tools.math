# M6-W02-C03-T2 — cross-family conservative scientific boundary aggregation

## Accepted authority and exact closure

RS100 is the final independently accepted T2 boundary and integration task
of the original retained size-L M6-W02-C03 parent. The three size-M child
phases S01 Euclidean geometry, S02 direct DFT/linear convolution and S03
uniform time series are durably qualified and done; RS099 T1 delivered a
separate all-or-nothing direct-kernel parent facade. RS100 must not claim
parent completion without its own compiled RED, GREEN, regression, NuGet
and DWF native-plan evidence.

## New explicit BCL-only boundary facade

`M6W02C03BoundaryAggregate64.Evaluate` accepts an explicit
`M6W02C03BoundaryRequest64` containing the existing T1
`M6W02C03Request64`, separately supplied numeric sample-time
coordinates, an independent-units acceptance policy, a discrete-sampling
acceptance policy and a convolution topology policy.

The unit-policy option `CallerManagesIndependentUnits` means that
Cartesian linear coordinates, discrete complex signal samples and
uniform-series time/value coordinates remain the CALLER'S responsibility.
It is not a unit conversion, common physical reference frame, Nyquist
certificate, geodesic operation or proof that separate data streams
are comparable. The default or unknown unit policy is rejected.
Discrete samples must be explicitly accepted, and convolution must
explicitly request zero-extended LINEAR topology. Requested circular
convolution is refused rather than fabricated.

The new facade composes the **seven prior S01/S02/S03 conservative
audits** in declared order:

1. audited finite 2D/3D Euclidean norm;
2. audited Cartesian distance including relative displacement risk;
3. audited projection including near-orthogonal cancellation risk;
4. negative-exponent direct O(N^2) complex DFT with declared
   discrete-sampling policy and conservative bin-cancellation screening;
5. zero-extended real linear convolution with explicit topology and
   conservative signed product cancellation screening;
6. uniform-grid first differences with supplied numeric sampleTimes
   and conservative timestamp/difference resolution checks;
7. complete-window moving arithmetic means with the same supplied
   times and conservative signed mean cancellation checks.

ALL seven prior-family audited outcomes are returned together on
success. Any family refusal immediately yields one typed, empty
`M6W02C03BoundaryOutcome64`, with GeometryRefused,
SignalRefused or UniformSeriesRefused plus **one family-specific reason**.
Any pre-cancellation or observed family cancellation returns Cancelled.
There is no partial value disclosure even when geometry or signal
calculations had succeeded before a later series refusal.

The refusal order is unit-policy check, S01 norm/distance/projection,
S02 DFT/convolution and S03 uniform differences/mean. Status and reason
are diagnostic metadata and do not imply a new absolute/relative error
theorem. Direct RS099 T1 numeric composition and all separately
qualified T1/T2 family facades remain callable unchanged.
The conservative T2 ratio thresholds may refuse mathematically
valid results (e.g., an exactly cancelling DFT bin or sliding mean).
No FFT, circular convolution, auto resampling, physical-unit inference,
forecast, time derivative or implicit coordinate transform is added.

## Independent RED/GREEN and local NuGet falsification

A controlled DWF run must compile a stand-alone
`M6W02C03T2RedTests` against the accepted preview.11 product BEFORE
staging the new facade, and observe one runtime RED for the absent
public `M6W02C03BoundaryAggregate64` type. Compiler failure is not RED.

Stage preview.12 code and recover RED with **78 independent GREEN tests**
for the seven canonical outcomes: norm(3,4)=5, distance((0,0),(3,4))=5,
projection((3,4),(1,0))=(3,0), impulse DFT=[1,1,1,1],
linear convolution [1,2]*[1,1]=[1,3,2], adjacent first differences
[1,3,6]=[2,3], two-point moving averages=[2,4.5]. Additional GREEN
tests must cover actual typed domain/numeric refusals per family,
explicit policies, caller-managed units, sample-grid hazards,
pre-cancellation, family refusal precedence, no partial numeric
results and retained direct facade semantics.

Require the **758 previously qualified scientific tests**:
57 parent T1; 107 S03-T2, 61 S03-T1; 69 S02-T2, 60 S02-T1;
53 S01-T2, 25 S01-T1; 88 C02-T2, 80 C02-T1, 88 C01-T2,
70 C01-T1. Run the entire scientific test project, four representative
Math regressions and native/project planning and transfer validations.

Pack ONE real BCL-only local scientific `1.1.0-preview.12` NuGet
archive. Verify its actual compiled net10 DLL, nuspec package identity
and dependency absence, archive byte length and SHA-256. Qualify SIX
independently restored exact-version eight-probe consumers against
one cleared local feed and isolated caches: new C03-T2 boundary
aggregate, retained C03-T1 direct aggregate, retained S03-T2 uniform
audit, retained S02-T2 signal audit, retained S01-T2 geometry audit,
and retained C02-T2 special-function boundary. All six SHA and
byte-count receipts must match the inspected actual archive.

## Parent closure and nonclaims

Only after durable successful RS100 evidence may the original C03-T2
RED/GREEN/V, C03-T2 and original C03 parent become `done` in both the
backlog and native DWF project plan. The independently qualified
S01/S02/S03 subphases remain done. Work package M6-W02 and milestone
M6 remain `in-progress`; C04/C05 are not silently completed.

This is a local .NET 10 proof of admitted behavior, NOT a public NuGet
release, cross-OS certification, scientific precision theorem or
AURA/Decision/MCDM/dw.discovery runtime adoption.
