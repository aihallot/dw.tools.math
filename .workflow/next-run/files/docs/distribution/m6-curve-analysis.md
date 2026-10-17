# Bounded Math analysis — interpolation and unweighted regression

Product: dw.tools.math.analysis 1.1.0-preview.1, net10.0, local-only.
Public namespace: Dw.Tools.Math.Analysis.

Use the static BoundedCurveAnalysis64 facade with ordered immutable
CurvePoint64(X,Y) observations. The admitted inputs contain from
2 to 32 points, strictly increasing finite x and finite y values.
The unit and physical meaning of X/Y are caller-owned; the library
does not perform unit conversion or infer uncertainty.

The public methods:
- InterpolateLinear(samples,x): finite piecewise linear interpolation
  only on the original observed x interval.
- InterpolateNaturalSpline(samples,x): zero-curvature endpoint natural
  cubic spline, with a BCL tridiagonal solve. Two observations reduce
  to the linear result.
- FitLeastSquares(samples): unweighted descriptive linear y=a*x+b
  fit, with finite slope, intercept and sum of squared residuals.

Reference contract: for (0,1),(1,3),(2,5), a=2, b=1, SSE=0;
both interpolators at x=0.5 yield 2. The independent curved natural
spline at (0,0),(1,1),(2,0), x=0.5 yields 0.6875.

Statuses: Success, InvalidInput, BudgetExceeded, DuplicateAbscissa,
OutsideDomain, RankDeficient, NonFiniteIntermediate. Failure returns
null numerical fields, not a partially accepted model.

The APIs are bounded finite binary64 calculations, not exact proofs.
They make no claim of extrapolation, arbitrary natural spline
conditioning, outlier robustness, weighted fits, error bars, units,
forecast skill or causality. T2 separately qualifies boundary cases
and more advanced coordinate semantics. No external provider, native
BLAS, AURA, Decision or MCDM runtime is part of this package.

One real compiled API RED plus 47 independent GREEN regression oracles
must pass before qualifying the package. Restore its actual local
nupkg from an isolated feed, independently compare archive SHA-256
and run eight source-free consumer probes. Do not claim public NuGet
publication or cross-OS compatibility from a single local host.
