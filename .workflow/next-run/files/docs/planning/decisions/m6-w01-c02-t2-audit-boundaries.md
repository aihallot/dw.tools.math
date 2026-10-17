# M6-W01-C02-T2 — residual and regularity boundary audit

## Decision

Keep the original BoundedUnivariateAnalysis64 methods and their prior qualified
semantics unchanged. Add a new public UnivariateBoundaryAudit64 sidecar in
dw.tools.math.analysis 1.1.0-preview.4, net10, BCL only.

**Root finding:** T1 bisection admits convergence when either the computed
residual or the observed bracket width meets the requested threshold.
For a sign-changing jump such as f(x)=-1e200 for x<0 and +1e200 otherwise,
the bracket can shrink while no root and no small residual exists.
The new AuditRoot is deliberately stricter: it admits only a finite
T1 success whose observed residual is at or below the caller-selected
AbsoluteResidualTolerance. Otherwise it returns typed ResidualNotMet,
with null Root, AbsoluteResidual and ObservedBracketWidth. It does not
prove continuity, uniqueness or an error guarantee. The original T1
operation continues to exist with its own explicitly unverified semantics.

**Quadrature:** No finite sampling can establish general continuity or
four-times differentiability, so Simpson |S(2n)-S(n)|/15 is not a certified
global error bound. The new AuditQuadrature refuses RegularityUnspecified
by default. A caller must explicitly select CallerAssumesSmooth; this is
not independently verified and does not turn the estimate into a theorem.
Typed refusal also applies to unsupported regularity enums, invalid
functions or intervals, budget exhaustion, cancellation, callback errors,
nonfinite outcomes and nonconvergence, with no partial result values.

These boundaries preserve two distinct contracts: T1 computes approximate
observations with unproved guarantees; T2 makes conditional admission
explicit without changing the original APIs. There is no general-purpose
discontinuity detector, certified interval arithmetic, adaptive
Gauss-Kronrod algorithm, arbitrary precision or mathematical guarantee
for caller-provided functions.

## Independent falsification and distribution

One compiled reflection RED observes the missing audit public surface
on the already qualified T1 source. The later implementation recovers it,
passes 50 independent GREEN tests, 44 retained T1 tests, 48 retained
C01 T1 tests and 60 retained C01 T2 tests plus relevant Math regressions.
A deterministic local preview.4 nupkg is checked with SHA-256 and a
pinned NuGet-only eight-probe new audit consumer. The retained T1
eight-probe NuGet-only consumer runs against the same archive bytes.

The completed result closes M6-W01-C02-T2 R/G/V, T2 and phase C02.
M6-W01 and M6 remain in progress; C03 numerical differentiation and
ODE is the next planned phase. No stable/public NuGet, cross-OS,
Decision/MCDM/AURA adoption or root/integration theorem is claimed.
