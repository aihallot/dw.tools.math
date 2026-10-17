# M6-W01-C03-T1 — bounded numerical differentiation and scalar ODE

## Scope and scientific admission

The existing \`dw.tools.math.analysis\` net10 BCL-only package adds
\`BoundedDifferentialAnalysis64\` in local version 1.1.0-preview.5.
This is provider-neutral and does not change the qualified curve, root,
or Simpson APIs. Two finite binary64 operations are admitted:

- \`Differentiate(Func<double,double>,at,DerivativePolicy64,CancellationToken)\`
  evaluates a four-sample central difference at step h and h/2. It
  returns the refined derivative and the absolute coarse/fine
  difference divided by 3 as a **heuristic** discretization-error
  estimate. Default h=1e-3 and allowed h in (0,1]. The caller sets
  MaximumEstimatedError. No error bound or function smoothness is
  independently proven. Step representation and evaluation budget
  are checked before starting callbacks.

- \`IntegrateOde(Func<double,double,double>,start,end,initial,OdePolicy64,
  CancellationToken)\` integrates one scalar initial-value ODE forward
  with fixed-step classical RK4. It computes N and 2N refinements and
  reports |y(2N)-y(N)|/15, a **noncertified** global-error estimate under
  unverified smoothness and stability assumptions. Default N=32;
  N must be 1..256 and the two complete passes require 12N evaluations.
  This is not an adaptive integrator or a guarantee for stiff systems,
  discontinuities, events, unstable initial-value problems or PDEs.

Both APIs use typed refusals and null numeric outputs on invalid input,
invalid policy, budget, cancellation, callback exceptions, nonfinite
evaluations/intermediates, nonrepresentable steps or an estimated-error
criterion that is not met. Such refusal is not a guarantee that a
successful estimate bounds the true error.

## Falsification and reproducibility

A compiled public reflection RED on the prior package must demonstrate
absence of the differential/ODE entry points before staging source.
Recover that RED, pass 57 independently asserted GREEN tests, preserve
the 51 C02 T2, 44 C02 T1, 60 C01 T2 and 48 C01 T1 tests, and run relevant
numerics and composition regressions in locked net10 Release.

Scientific oracles: f(x)=x² at x=2 has derivative 4, with a finite
difference estimate; y'=y, y(0)=1 at t=1 agrees with e within
1e-7 under the qualified default step and budget. Negative, invalid
and nonfinite cases prove typed nonpartial refusals and no claimed
global-error proof.

Inspect the actual local 1.1.0-preview.5 nupkg bytes/nuspec/SHA-256;
restore an independent eight-probe NuGet-only derivative/ODE client
from an isolated feed, and rerun a second eight-probe prior C02 T2
NuGet-only client against the **same** archived bytes.

Close T1 R/G/V and T1 only; M6-W01-C03 remains in progress until T2
qualifies overflow, stiffness, event exclusions and meaningful local
versus global error. W01 and M6 remain in progress. No public NuGet,
cross-platform, agent/Decision/MCDM adoption or certified ODE solver.
