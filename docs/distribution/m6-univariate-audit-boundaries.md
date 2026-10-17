# Math audited univariate numerical boundaries (local preview.4)

Artifact: dw.tools.math.analysis 1.1.0-preview.4, net10, BCL only.

UnivariateBoundaryAudit64.AuditRoot(function, lower, upper,
optionalRootPolicy, cancellationToken) reuses the previously qualified
bisection, but a width-only success with excessive observed residual
is refused with ResidualNotMet. All refusals erase numeric results.
Root continuity, uniqueness and an absolute root-error bound remain
unproved, even when the residual is below requested tolerance.

UnivariateBoundaryAudit64.AuditQuadrature(function, lower, upper,
regularity, optionalQuadraturePolicy, cancellationToken) requires the
caller to choose QuadratureRegularity64.CallerAssumesSmooth. Default
Unspecified yields typed RegularityUnspecified refusal. A caller's
declaration does NOT independently validate smoothness or make the
Simpson |S(2n)-S(n)|/15 refinement a rigorous error bound. Evaluations,
panels, residuals and heuristic estimates stay bounded by T1 policies.

Independent acceptance: sqrt(2) root from x*x-2 on [1,2] with
observed small residual; linear/sign-jump with high residual refused;
integral x*x on [0,1] is approximately 1/3 with explicit smoothness
assumption; unknown regularity, no bracket, callback failures, NaN,
budget and nonconvergence expose no partial numerical output.

Original T1 entry points remain available unchanged. This contract
does not certify arbitrary discontinuities, a global integration
error bound, a root enclosure, other OSes, stable NuGet, or integration
into decision/agent applications.

C02 closes only after durable T2 evidence and its same-archive
source-free consumer receipts. W01/M6 stay open.
