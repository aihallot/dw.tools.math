# M6-W02-C03 — size-L split and native DWF projection

## Decision and authority

RS092 refines, without completing, the accepted size-L chunk
`M6-W02-C03` (geometry, signals and time series). The original
chunk, its stable identity `M6-W02-C03`, historical requirements
`M6-W02-C03-R1/R2`, original T1/T2 tasks, source coverage, and
dependencies are preserved. The parent remains **planned**, not ready
or done. No scientific functionality, package revision, product
claim or historical execution evidence is manufactured by a split.

Three explicit size-M child chunks are introduced under
`parent_chunk = M6-W02-C03`, with full native DWF phase/task/subtask
projections. They remain **planned / not-ready** until independent
implementation runs qualify each one.

### S01 — finite geometry and projection

`M6-W02-C03-S01`: Euclidean finite 2D/3D vector norm, distance
and vector projection on a nonzero axis. Independent oracles:
norm(3,4)=5, distance((0,0),(3,4))=5, projection of
(3,4) onto (1,0) equals (3,0). Bound dimensions (2/3),
finite binary64 intermediates, near-zero/zero axes, units and
no implicit coordinate-system or geodesic interpretation.
T1 delivers limited outcomes with a compiled RED/GREEN gate;
T2 independently qualifies boundaries, cancellation and
source-free NuGet consumption.

### S02 — discrete signal kernels

`M6-W02-C03-S02`: bounded direct DFT (no invented FFT claim) with
documented forward sign exp(-2*pi*i*k*n/N), inverse 1/N, zero-based
sampling, and exact direct convolution on finite arrays as an
approximate binary64 operation. Independent oracles:
DFT([1,0,0,0])=[1,1,1,1] and convolution
[1,2]*[1,1]=[1,3,2]. Explicit dimension, input size,
aliasing, periodic/circular versus linear convolution and
overflow budget boundaries. T1 and T2 remain separate.

### S03 — uniformly sampled time series

`M6-W02-C03-S03`: finite, uniformly spaced time-series
first differences, two-sample moving average and explicit sample
interval, without irregular-time resampling or forecast claims.
Independent oracles: differences([1,3,6])=[2,3] and
moving average(window 2,[1,3,6])=[2,4.5].
Time units, interval positivity, missing samples, nonfinite values,
window limits and aliasing remain explicit. T1 and T2 remain separate.

## Dependency sequence and aggregate gate

S01 depends on qualified C02 special functions and M6-W01-C03.
S02 depends on S01; S03 depends on S02. This sequence bounds the
number of simultaneously active capabilities and allows retained
cross-family testing. The parent C03 remains planned while children
are delivered. Its original T1/T2 aggregate objectives are not
silently replaced by a geometry-only deliverable. A distinct gate
can close parent C03 only after S01, S02, S03 and the original parent
criteria are independently verified; M6-W02-C05 still depends on
the parent C03.

The native DWF topology grows from 496 to 523 nodes:
8 milestones, 17 work packages, 56 phases, 112 tasks, and
330 subtasks. The adoption validator's exact inventory count is
updated, **not removed**; the 1:1 product/native stable-ID,
dependency and parent mappings continue to be enforced.
All newly added nodes remain `not-ready`, with no fabricated
completed achievement or package publication.

RS092 is a structural planning qualification only. No product
source, NuGet package, target OS or downstream AURA/Decision/MCDM
adoption is certified. Its acceptance is the coherent product
backlog, regenerated projections, exact native DWF topology, and
durable split manifest, not product tests.
