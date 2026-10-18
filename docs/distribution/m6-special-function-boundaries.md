# Special-function boundary audit — local preview.4

Package dw.tools.math.scientific 1.1.0-preview.4, net10 BCL-only.

SpecialFunctionBoundaryAudit64.EvaluateReal requires an explicit
SpecialAngleInput64.Radians or Degrees for Sine/Cosine. Degrees
are converted approximately to radians before the original bounded
managed provider is invoked. Nonangular functions require
NotApplicable; missing angular units, invalid unit enums,
nonfinite inputs, excessive ranges and subnormal conversion
underflow return typed refusal.

GammaPole identifies x=0 or a negative integer. Gamma remains
Lanczos-approximated only on [0.5,20], with no certified error
bound and no real negative analytic continuation. Complex Gamma
always refuses with ComplexGammaUnsupported.

SpecialFunctionBoundaryAudit64.EvaluateComplex defaults to
RejectNegativeRealCut for NaturalLogarithm. The caller must
explicitly set CallerAcceptsPrincipalCut to admit log(-1+0i)
approximately as +pi*i. No alternative branches, winding
numbers or guaranteed branch continuity are provided.

Exponential inputs with extreme real magnitudes pre-refuse
OverflowRisk or UnderflowRisk before computation. These are
risk classifications, not observations that binary64 overflow
or underflow occurred. Previously qualified T1 range limits
are retained and remain more restrictive for ordinary inputs.

One compiled RED, 87 new GREEN tests, 238 retained scientific tests,
four relevant Math regressions, one SHA-qualified real local
preview.4 archive and two eight-probe NuGet-only consumers must
pass before C02 closes. M6-W02 and M6 stay open.

No stable/public NuGet, untested OS, automatic foreign provider,
nonprincipal complex log branch, complex Gamma, exact numerical
error theorem or downstream agent adoption is qualified.
