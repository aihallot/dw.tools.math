# Math M6 curve boundary and unit policy (preview.2)

Artifact: dw.tools.math.analysis 1.1.0-preview.2; net10, local NuGet.
This is a bounded mathematical utility, not an agent skill or decision
model. The prior three operations from T1 retain their signatures.

## Public admission

CurveBoundaryAudit64.EvaluateLinear(samples, query, units, queryXUnit,
optionalPolicy) evaluates an in-domain line and, only with
CurveExtrapolation64.LinearSegment explicitly selected, extrapolates
the nearest external LINEAR segment. Default Refuse returns a typed
OutsideDomain result with NO partial value.

CurveBoundaryAudit64.FitDescriptive(samples, units, optionalPolicy)
returns an unweighted binary64 slope, intercept and sum-squared
residual for accepted sorted points. A declared unit pair such as
CurveAxisUnits64("s","m") gives a literal "m/s" slope unit label,
without actually checking or converting sample magnitudes.

Both methods enforce from 2 to 32 sorted finite samples, unique X,
a finite relative X span at least the caller's documented threshold
(default 1e-12), and finite results. Policy values outside
(0, 0.01] or unrecognized extrapolation modes are refused.

Boundary statuses distinguish InvalidInput, BudgetExceeded,
DuplicateAbscissa, OutsideDomain, InvalidUnit, IncompatibleUnit,
InvalidPolicy, NearRankDeficiency and NonFiniteIntermediate.

For inputs (0,1),(1,3),(2,5), a unit-tagged query at 0.5 yields 2,
with slope 2 m/s; an explicit query at 3 yields 7 but flags
Extrapolated=true and PredictiveGuarantee=false. A query unit "min"
against "s" is refused. The data near x=1e12 with relative span
~1e-14 are refused under the default 1e-12 policy.

## Limitations and next gate

Unit tags are simple caller-declared symbols, not a physical unit
system; "m/s" is metadata only. There is no unit conversion, no
general spline extrapolation, confidence interval, weighted fit,
causal inference or forecast validation. Strict refusal does not
mathematically prove reliable conditioning for all admitted inputs.
No public stable version or non-observed OS is claimed.

T2 qualification requires a compiled RED, 59 GREEN test oracles,
independent preview.2 NuGet inspection and two eight-oracle source-
free consumers, including all prior T1 scientific acceptance probes.
C01 may close only after the durable gate; M6 and W01 remain open.
