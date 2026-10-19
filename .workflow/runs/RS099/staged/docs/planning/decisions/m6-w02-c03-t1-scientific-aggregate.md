# M6-W02-C03-T1 — cross-family bounded scientific aggregate contract

## Scope and authority

RS099 is the FIRST aggregate task on the original retained size-L
M6-W02-C03 parent. RS093–RS098 independently delivered all three
child phases S01 Euclidean geometry, S02 direct DFT/convolution, and
S03 uniformly sampled first differences/moving averages. The structural
split RS092 left original parent T1/T2 acceptance requirements and
release dependencies in place. No child success substitutes for a
separately compiled parent T1 RED/GREEN and local package qualification.

## Useful managed C# facade

The BCL-only public `M6W02C03Contract.Evaluate` takes an explicit
`M6W02C03Request64` with distinct caller-owned fields for Cartesian
geometry points/axis, complex discrete samples, real convolution inputs,
and uniform real samples with positive interval and moving window.

It executes seven already-qualified direct T1 public operations,
without changing their implementations:

1. finite norm of GeometryEnd;
2. finite distance GeometryStart to GeometryEnd;
3. orthogonal vector projection of GeometryEnd onto ProjectionAxis;
4. forward direct O(N²) DFT of DiscreteSamples, negative exponent;
5. zero-extended REAL linear convolution of both convolution inputs;
6. adjacent unscaled first differences of UniformSamples;
7. complete-window unweighted arithmetic moving average.

The single result returns ALL seven typed prior-family outcomes,
or NONE. A null aggregate request returns InvalidRequest. Any
geometry refusal returns GeometryRefused, any signal refusal returns
SignalRefused, and any uniform-series refusal returns
UniformSeriesRefused; pre-emptive/observed cancellation returns
Cancelled. The aggregate never exposes partial outcomes after a
later family refusal. The status labels identify the failed family,
NOT a new precision theorem or a substitute for detailed T2 refusals.

All existing family budgets are retained: finite 2D/3D Cartesian
coordinates within 1e100, direct signal arrays of length 1..32 with
input components <=1e6, and uniform-series arrays of length 1..32,
positive finite declared sample interval, complete moving windows
and original output budgets. Each domain's frame and units are
independent caller responsibilities. No implicit coordinate conversion,
geodesic interpretation, FFT, periodic convolution, irregular-time
resampling, frequency units or forecasting are introduced.

The cross-family facade deliberately invokes DIRECT qualified kernels.
Prior conservative S01/S02/S03 T2 audit APIs stay available on their
own. Their coordinated mixed-input boundary and integration policy
remains **original C03-T2**, not this aggregate T1.

## Independent falsification

The controlled DWF run must compile and execute an independent
reflection-based missing public `M6W02C03Contract` API RED against
the qualified preview.10 provider before staging this facade.
A compilation error is not RED evidence. After staging the source,
recover the RED and qualify 56 independent GREEN assertions:

- norm(3,4) = 5; distance((0,0),(3,4)) = 5;
- project((3,4),(1,0)) = (3,0);
- direct DFT impulse [1,0,0,0] => [1,1,1,1];
- zero-extended convolution [1,2]*[1,1] => [1,3,2];
- uniform first differences [1,3,6] => [2,3];
- moving average of window 2 => [2,4.5];
- variant 3D geometry, complex signs, complete windows,
  typed null/nonfinite/budget/window/cancellation refusal branches;
- preservation of ALL-or-NONE output on every refusal.

Preserve 701 previously qualified scientific tests (107 S03-T2,
61 S03-T1, 69 S02-T2, 60 S02-T1, 53 S01-T2, 25 S01-T1,
88 C02-T2, 80 C02-T1, 88 C01-T2 and 70 C01-T1), the full scientific
test suite, four independent Math regression projects and the native
planning/transfer validators.

Pack one actual BCL-only scientific `1.1.0-preview.11` NuGet archive.
Inspect compiled net10 assembly and nuspec identity/dependency absence,
byte length and SHA-256. Require five **independently restored, source-free**
exact-package-only eight-probe clients with isolated caches and one
cleared local NuGet feed: new aggregate, retained S03-T2 uniform audit,
S02-T2 signal audit, S01-T2 geometry audit, and C02-T2 special functions.
Every client must report byte/SHA agreement with the archive inspector.

## Closure and nonclaims

On durable acceptance close ONLY original C03-T1 RED/GREEN/V and T1;
transition parent C03 to in-progress, NOT done. Original C03-T2 stays
planned/not-ready. M6-W02 and M6 stay in-progress. This is local
qualification, not a public package, cross-platform certification,
DW discovery/Aura/Decision/MCDM adoption, or a proved binary64
rounding-error guarantee.
