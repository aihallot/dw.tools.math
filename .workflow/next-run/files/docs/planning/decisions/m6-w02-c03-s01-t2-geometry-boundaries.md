# M6-W02-C03-S01-T2 — finite Euclidean boundary and conditioning audit

## Scope and authority

RS094 qualifies the accepted T2 boundary and integration gate for M6-W02-C03-S01. RS093 qualified T1's bounded finite Cartesian 2D/3D norms, distances and projections in local scientific preview.5. RS094 preserves the original FiniteGeometry64 API and introduces a separate FiniteGeometryBoundaryAudit64 diagnostic sidecar in local preview.6.

## Domain and refusal policies

- Only finite 2D/3D binary64 Cartesian coordinates sharing one caller-managed linear unit and frame are admitted. No latitude/longitude, geodesic interpretation, implicit unit conversion, datum, reference ellipsoid or coordinate transformation.
- The audit reuses the T1 numeric backend, mapping its existing InvalidDimension, InvalidInput, ZeroAxis, OutsideQualifiedRange, NonFiniteResult and Cancelled statuses to typed, nonpartial audit refusals. A rejected outcome contains no backend value.
- A pre-cancelled token or cancellation observed during bounded audit operations returns Cancelled. There is no guarantee of arbitrary preemption during a tiny fixed-size operation.
- The existing coordinate budget of 1e100 and nonzero axis maximum absolute component floor of 1e-100 remain unchanged.

## Explicit heuristic boundaries, not error theorems

For successful nonzero T1 distance, relative displacement is computed as distance(start,end) divided by the maximum absolute input coordinate. An observed value below 1e-12 is conservatively refused as RelativeResolutionRisk. Identical points with zero distance remain admitted. This ratio can exceed 1 for widely separated points: it has no artificial upper bound.

For successful T1 projection, normalize the nonzero axis by its maximum absolute component and calculate absolute signed dot product divided by the sum of absolute component products. A nonzero denominator and observed signal ratio below 1e-12 trigger ProjectionCancellationRisk. A zero source vector remains admitted. Even a mathematically orthogonal pair may be conservatively refused, while the original direct T1 method remains callable with the original contract.

These fixed ratio floors are policy heuristics. They do not certify floating-point rounding errors, condition numbers, forward stability, actual cancellation failures, exact results or geodesic meaning.

## Falsification and integration

A controlled RS094 run must observe one compiled, independently failing missing public boundary-audit API RED before introducing the source; then recover RED and pass 52 GREEN tests, 25 retained T1 tests, 326 retained C01/C02 scientific tests and four Math regression projects. Compile, restoration, planned structural checks and native/project planning consistency are required.

One actual locally packed, SHA-256-inspected preview.6 scientific NuGet archive must be consumed by three independently source-free eight-probe clients: T2 audit, retained T1 geometry and retained C02 special-function boundaries. Use exact PackageReference versions, no ProjectReference, cleared external NuGet sources, and separate caches. All three observed package bytes and SHA must match the package inspector. This remains local, not a public release or downstream AURA/Decision/MCDM adoption.

Only S01-T2 RED/GREEN/V, S01-T2 and S01 may close on durable evidence. The original size-L C03 aggregate, S02/S03 children, M6-W02 and M6 remain unfinished. The next independently planned contract is bounded direct DFT/linear convolution, not an inferred FFT or circular convolution.
