# Exact typed composition pipeline — M2-W02-C02

The public `dw.tools.math.composition` contract composes bounded local **exact** rational quantities, not a general solver, AURA extension, provider executor, or unrestricted expression runtime.

## T1 — one typed execution path

The immutable direct and fluent APIs share `ExactQuantityPipeline.Evaluate`. The closed step vocabulary is `Start`, `Convert`, `Divide`, and `Add`. All units are explicitly supplied `UnitDefinition` values; only linear units with zero affine offset are admitted. There is no global catalog, culture inference, provider discovery, or implicit permission.

Canonical base-unit calculations retain `ExactRational` and `DimensionVector`. Example: `5 km` converted to metres, divided by `2 min`, and displayed in `km/h` is exactly `125/3 m/s = 150 km/h`. Incompatible dimensions (such as mass + time), zero divisors, invalid step order, and absolute affine temperatures are rejected during the complete preflight before arithmetic.

## T2 — resource, cancellation, provenance and integration boundaries

A pipeline contains at most **16 steps**. Both direct and fluent evaluation have `CancellationToken` overloads; a signalled token aborts with `OperationCanceledException` without returning partial results. Checks run before materialization and validation, during preflight and between evaluated steps. A custom blocking enumerator or an uninterruptible arbitrary-precision arithmetic operation is *not* forcibly preempted: this is cooperative cancellation, not a scheduler.

Numerator and denominator components are independently bounded at **256 decimal digits**, measured on the unsigned magnitude; the previous positive 257-digit acceptance boundary is deliberately falsified in a controlled RED and corrected. Input, unit scales, intermediate values, and final display magnitudes are checked. This is a numerical-size contract, not a hard memory or wall-clock guarantee.

`ExactPipelineResult.DetailedSteps` is an immutable, bounded series of snapshots containing the zero-based index, step kind, exact base quantity with physical dimension and optional explicit presentation-unit id. The legacy `Steps` collection remains, and its kinds correspond one-to-one with detailed snapshots. Snapshots are locally computed results, **not** provider execution telemetry, authorization records or external provenance.

Ten independent T2 boundary tests cover positive and negative numeral size, cancellation before/during evaluation, direct/fluent parity, per-stage exact quantity and dimensions, 16-step observations, zero and exponent overflows, invalid later steps, result immutability, and absence of public provider identity. They run alongside T1 tests and composition/IR/foundation regressions, architecture validation and local NuGet package inspection.

## Remaining limits

M2-W02-C02 is closed by RS035 only after its RED/GREEN and package oracles pass. `M2-W02-C03` remains planned. No mathematical provider, AURA adoption, distributed scheduling, real-time cancellation guarantee, provider permission or symbolic execution is delivered.
