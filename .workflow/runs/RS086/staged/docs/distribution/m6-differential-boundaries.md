# Numerical differentiation and ODE boundary audits (local preview.6)

Package: dw.tools.math.analysis 1.1.0-preview.6, net10, BCL only.

DifferentialBoundaryAudit64.Differentiate(function, at, regularity,
optionalPolicy, cancellationToken) refuses by default unless the caller
declares DerivativeRegularity64.CallerAssumesSmooth. This is an unverified
hypothesis; the four-sample h/h2 central derivative and heuristic error
estimate are not mathematically certified even on admitted results.
RegularityUnspecified, invalid policy, nonrepresentable step, budget,
nonfinite callback, cancellation and estimated-tolerance refusals all
return null numeric fields.

DifferentialBoundaryAudit64.IntegrateOde(function, start, end, initial,
regime, optionalPolicy, cancellationToken) admits only
OdeRegime64.CallerAssumesNonStiffNoEvents. Unknown regime refuses;
KnownStiff, EventDriven and PartialDifferentialEquation refuse as
unsupported. This is a scalar, forward-time, fixed-step classical RK4
calculation. No stability, event absence or global-error guarantee is
independently established. The N/2N difference divided by 15 is an
uncertified heuristic observation.

Independent oracles include d(x*x)/dx at x=2 near 4 and y'=y,
y(0)=1, y(1) near e. Adversarial cases cover abs(x) at zero, sign
jumps, unsupported regimes, tiny/unrepresentable steps, finite budgets
and overflow. The original derivative, RK4, curve and univariate APIs
retain their signatures and previously qualified behavior.

RS086 requires 66 new GREENs and a recovered compiled RED, 261
retained tests, two focused regressions and 16 source-free NuGet probes
over one SHA-inspected preview.6 archive. C03 closes only upon durable
evidence; W01/M6 stay open. No stable/public NuGet or downstream
agent, Decision or MCDM adoption is claimed.
