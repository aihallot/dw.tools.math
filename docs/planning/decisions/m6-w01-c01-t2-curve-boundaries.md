# M6-W01-C01-T2 — accepted numerical conditioning, units and extrapolation boundary

## Scientific question

Can the existing qualified finite-binary64 line/spline and OLS operations
be exposed with an auditable *caller-selected* abscissa-conditioning
threshold, explicit axis unit tags, and opt-in piecewise LINEAR
extrapolation, without turning descriptive fitting into causality,
physical unit conversion or an unqualified forecast?

## Decision and product scope

Keep T1 BoundedCurveAnalysis64 unchanged; add a public
CurveBoundaryAudit64 sidecar in the existing dw.tools.math.analysis
net10 package, now locally versioned 1.1.0-preview.2.

CurveAxisUnits64 stores caller-declared X and Y unit symbols (ASCII
letters, digits and underscores, 1–24 chars, first char a letter).
It does not check an external quantity ontology or perform a physical
conversion. A query whose X unit differs from the declared X unit
must be refused, never silently scaled. The slope unit tag is a literal
Y/X ratio; it is metadata, not an algebraic simplifier.

CurveBoundaryPolicy64 stores MinimumRelativeXSpan with the qualified
finite interval (0, 0.01], default 1e-12, and one of Refuse or
LinearSegment extrapolation. The relative abscissa span is
(lastX-firstX) / max(1, abs(firstX), abs(lastX)). Near-rank-deficient
input below threshold is a typed refusal, not a confident OLS fit.
Duplicate, descending, nonfinite, over-budget and intermediate
nonfinite inputs are refused, and numeric result fields are null.
The original strict sorted 2–32 point limit remains unchanged.

LinearSegment extrapolation is explicitly requested and uses only the
outermost first/last TWO observed points with the caller's units.
It is tagged as extrapolated and never promises forecasting accuracy.
Natural cubic spline remains in-domain-only; T2 does not invent spline
extrapolation or certify all spline condition numbers. OLS remains
unweighted descriptive regression with measured finite SSE and no
causal or predictive guarantee. A tighter relative threshold is an
explicit caller risk choice, not an inferred error bound.

## Reproducible falsification

One genuinely compiled reflection RED must prove the absent public
unit/conditioning API before staging the product source. Recover the
RED and pass **59 independent GREEN** MSTests across units, relative
conditioning, explicit extrapolation, point order/budgets, nonfinite
intermediates, no partial output, fitted coefficients and the retained
natural spline. Rerun the complete new analysis test project (including
T1), plus numerical, typed IR, quantities and composition regressions.

Pack the real local version 1.1.0-preview.2 NuGet assembly into one
controlled local feed. Inspect the package bytes/nuspec/SHA-256 with
the versioned independent C# helper. Restore/run a new eight-oracle
source-free NuGet client and the retained eight-oracle T1 consumer,
each pinned to preview.2 and the SAME actual nupkg bytes. No direct
filesystem copy or undeclared repository mutation.

The actual executing SDK/RID must be observed in the DWF receipt.
The result neither certifies Linux/macOS, arbitrary units, weighted
least squares, CI, stable/public NuGet nor AURA/Decision/MCDM adoption.
T2 completes its R/G/V stages and closes C01 only. M6-W01, M6 and
subsequent numerical analysis work remain in progress.
