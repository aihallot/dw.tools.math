# M6-W02-C03-S01-T1 — finite Euclidean geometry contract

RS093 qualifies only task T1 in the S01 child of the unchanged original size-L C03 aggregate.
The source contract is `Dw.Tools.Math.Scientific.FiniteGeometry64` in the locally versioned
`dw.tools.math.scientific` preview.5 package. Preview.4 remains the prior qualified baseline.

- Input: two or three finite `double` Cartesian coordinates, all measured in the same caller-managed linear unit and coordinate frame. No implicit unit, reference ellipsoid, datum, longitude/latitude, coordinate conversion or geodesic interpretation.
- Operations: `Norm`, `Distance` and `Project` (orthogonal vector projection onto a supplied nonzero axis). Each returns a typed scalar or vector outcome; no mixed or partial result.
- Arithmetic: bounded binary64, maximum absolute input coordinate 1e100. Scalar and component outputs exceeding this budget are refused. Projection axis maximum absolute coordinate must be at least 1e-100; a precisely zero axis is separately refused. These are declared conservative implementation budgets, not proved sharp stability/error thresholds.
- Scalar norm uses max-magnitude scaling before sum of squares. Distance forms finite differences then uses the same scaled magnitude. Projection scales its axis before the dot and denominator calculations.
- Outcomes: `Success`, `InvalidDimension`, `InvalidInput`, `ZeroAxis`, `OutsideQualifiedRange`, `NonFiniteResult`, `Cancelled`. Refusals expose no scalar or projected vector. Cancellation is checked before and inside bounded operations. No guaranteed rounding-error bound or exactness claim.
- Independent falsification: norm(3,4)=5, distance((0,0),(3,4))=5 and project((3,4),(1,0))=(3,0), with selected 3D/scale/refusal checks. RED is a compiled test using reflection to demonstrate the missing public type, not a compiler error.
- Verification: one recovered RED, 24 independent GREEN test methods, retained C02 T1/T2 and C01 T1/T2 tests, targeted Math regressions, one actual locally packed preview.5 package and two separate NuGet-only eight-probe consumers whose received package bytes have identical SHA-256.

T2 retains independent detailed numeric conditioning, cancellation/dimension/refusal integration
and source-free package audit acceptance; passing selected T1 examples is not closure of
the S01 phase, any S02/S03 child, parent C03, work package M6-W02, or milestone M6.
No public NuGet publication, cross-platform guarantee, geospatial semantics, exact error
bounds or AURA/Decision/MCDM consumer adoption follows from this local task.
