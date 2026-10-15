# Exact real pointwise differentiation — local symbolics preview.6

Math now distinguishes *known exact derivative values* from scientific refusal.

SymbolicDerivativePointAnalysis.Analyze(source, withRespectTo, exactPoint) is a provider-free, exact-rational API. Its result statuses are ExactValue, NonDifferentiable, OutsideRealDomain, Unsupported, UnsupportedDomain, UnsupportedUnits, UnsupportedAssumptions, and BudgetExceeded. A refusal has **no numeric result and no proof/provenance string**.

A known real polynomial derivative is first computed by the already qualified SymbolicExactDifferentiation (d(x³)/dx=3x²). Only then is that exact typed polynomial evaluated at a bounded exact rational point: d(x³)/dx at x=2 is exactly 12, not an approximate finite difference. Algebraic inputs outside the closed polynomial subset remain unqualified.

A small independent real-domain subset also recognizes:
- Abs(x): derivative +1 for x>0, -1 for x<0, and NonDifferentiable at zero.
- Sqrt(Square(x)): identical real derivative behavior because the expression is |x|.
- Sqrt(x): OutsideRealDomain for negative x, NonDifferentiable at zero and Unsupported at positive x (no general radical rule is admitted).
- The original (x²-1)/(x-1): OutsideRealDomain at x=1, even after any candidate cancellation; derivative elsewhere stays Unsupported until a quotient rule is qualified.

Inputs containing quantities, retained mathematical assumptions, bound/integer target variables, unsupported analytic functions and binary64 are not silently converted. The point, scalar coefficients and intermediate exact values are bounded by 128-bit numerator/denominator limits, in addition to existing IR graph limits. A successful result carries a versioned local rule ID, the source's math-ir/1 semantic structural digest and differentiation-variable identity, not a universal proof.

Qualification requires a true compiled absent-API RED, 37 scientific GREEN tests, full relevant symbolics/IR/quantity regressions and eight independent local NuGet-only client probes. The NuGet product graph remains dw.quantities 0.2.0-preview.1 -> dw.tools.math.ir 0.3.0-preview.1 -> dw.tools.math.symbolics 0.5.0-preview.6. A separate C# inspector reports observed manifest IDs/dependencies, DLLs, byte lengths and SHA-256s; no public feed is used or published.

Only M4-W02-C01-T2 and the C01 phase close after a successful DWF run. W02/C02, W02/C03, the M4 milestone and any AURA/dw.discovery broker contract remain unqualified.
