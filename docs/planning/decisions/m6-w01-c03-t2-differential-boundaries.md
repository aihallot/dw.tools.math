# M6-W01-C03-T2 — differentiation and ODE audit boundaries

## Accepted scientific scope

The provider-free Math analysis package adds DifferentialBoundaryAudit64 in
1.1.0-preview.6. The prior BoundedDifferentialAnalysis64 methods and results
remain unchanged. This new public sidecar makes unverified caller assumptions
explicit instead of inferring properties from finite samples.

DerivativeRegularity64 is either Unspecified or CallerAssumesSmooth. The
default refuses RegularityUnspecified and returns no partial derivative.
A caller may explicitly assume smoothness; the sidecar then delegates to the
qualified four-sample central difference. Even an admitted approximation
cannot prove mathematical differentiability: abs(x) at zero is an adversarial
case whose symmetric difference happens to be zero. The runtime does not
certify continuity, differentiability or local truncation error. The recorded
h/h2 difference remains an estimate, not a verified bound.

OdeRegime64 is Unspecified, CallerAssumesNonStiffNoEvents, KnownStiff,
EventDriven or PartialDifferentialEquation. Only the explicit caller-assumed
nonstiff, event-free scalar initial-value regime is admitted; all other
known regimes are refused as out of scope, with an unknown enum refused
as invalid policy. No algorithm automatically proves nonstiffness or event
absence. Declaring them cannot transform fixed-step RK4 N/2N estimates
into a guaranteed global-error bound, certified trajectory or stable
solution of stiff equations.

Both adapters preserve T1 finite evaluation budgets, representable step
checks, invalid input/policy, cancellation, callback exception, overflow,
nonfinite and estimated-tolerance refusals. Rejected results expose no
partial mathematical value, step, error estimate or evaluation counts.

## Falsification and evidence

One compiled reflection RED must observe the missing audit public surface
before staging the product implementation. Recover that RED and pass
66 independently asserted GREEN tests, retain all 261 focused tests from
C01, C02 and C03 T1, and run the two relevant Math regression projects.
Verify the actual locally packed BCL-only analysis 1.1.0-preview.6 net10
archive by nuspec identity, SHA-256, and two independent source-free
eight-probe consumers restored from the same isolated feed.

This qualifies documented conditional admission policies, not universal
detection of discontinuity, smoothness, stiffness or correctness. No
adaptive stiffness solver, event detection, vector IVP, PDE, rigorously
verified interval bound, arbitrary precision, nonobserved OS or public
NuGet is certified. RS086 closes C03 T2 and C03 only. M6-W01 and M6
remain in progress; M6-W02 requires separate admissible preparation.
