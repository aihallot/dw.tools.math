# M6-W01-C01-T1 — bounded linear, natural spline and least-squares contract

## Accepted source and scientific question

M5's independent local G-M5 gate is completed as a scientific capability
milestone, not as a stable/public 1.0 publication. M6-W01-C01 owns
bounded interpolation and descriptive regression, without introducing
decision policy, market data, storage or an agent-host dependency.

Accepted T1 recipe: given ordered 2D observations (0,1),(1,3),(2,5),
a line fit has slope 2, intercept 1 and sum of squared residuals 0;
linear and natural spline interpolation at x=0.5 both yield 2.

## Decision and implementation

Introduce actual lowercase dw.tools.math.analysis and corresponding
MSTest .NET 10 projects and locked project/central package metadata.
Expose immutable CurvePoint64, CurveValueResult64 and CurveFitResult64,
typed CurveAnalysisStatus64, and BoundedCurveAnalysis64. Admit at most
32 sorted finite binary64 points. Compute:
- piecewise linear interpolation within original x range, no silent
  extrapolation;
- a natural cubic spline with zero second derivatives at endpoints and
  bounded Thomas tridiagonal solver, including the genuinely curved
  fixture (0,0),(1,1),(2,0) => 0.6875 at x=0.5;
- unweighted ordinary least squares of y on x with measured slope,
  intercept and sum-squared residual; no predictive or causal guarantee.

All three operations refuse duplicate/nonascending abscissae,
nonfinite values and over-budget input. Rank-deficient/overflow
intermediates have typed refusal results with no partial numeric claim.
The caller owns units and coordinate meaning; T1 does not infer them.
There is no native package, external CAS, provider or AURA requirement.

## Independent verification

Create a compiled REFLECTION RED on the absent public analysis API
before adding implementation. Qualify 47 independent GREEN MSTests,
recover the RED, run the complete new analysis suite and relevant
numerics, quantities, IR and composition regressions. Pack the actual
local dw.tools.math.analysis 1.1.0-preview.1 nupkg, independently inspect
its BCL-only nuspec, assembly and SHA-256, and restore/run a clean
source-free NuGet console with eight hand-checked numeric probes.

T1 advances only M6, M6-W01, M6-W01-C01 and T1 R/G/V. T2 remains
separately planned for extrapolation policies, mixed units, near-rank-
deficient conditioning, spline work budgets, compatibility and
additional legitimate numerical methods.

The numerical outputs are finite binary64 approximations; no exact
rational identity, confidence region, causal inference, forecast or
general polynomial spline certification is claimed.
