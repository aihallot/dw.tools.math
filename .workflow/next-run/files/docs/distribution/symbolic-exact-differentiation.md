# Exact formal polynomial differentiation — local Math preview.5

dw.tools.math.symbolics introduces two public, provider-independent typed contracts:

- SymbolicRealFunction holds a free real parameter, an IR body and explicitly Real domain/codomain.
- SymbolicExactDifferentiation.TryDifferentiate computes derivatives of a **closed exact scalar polynomial subset**, including x³ -> 3*x², a missing free differentiation symbol -> exact 0, constant -> 0, x² -> 2*x, sums/subtractions and products by canonical exact constants.

Supported direct Power exponents are positive integers 1..8; higher powers have a typed BudgetExceeded outcome. Negative, zero or fractional exponents, arbitrary products, division, sqrt/absolute analytical derivatives, approximate binary64 and matrices are Unsupported. Integer or bound variables are UnsupportedDomain. Existing restricted expressions are UnsupportedAssumptions (rather than losing a predicate). Quantities and all derived-unit interpretation return UnsupportedUnits and never silently yield unitless scalars.

Admitted outputs carry the fixed local formal-polynomial-derivative/exact/v1 rule identifier, an existing math-ir/1 semantic structural SHA-256 for the original source, and the **differentiation variable identity**. Source digests do not prove universal algebraic equivalence. Mathematical outputs remain exact typed IR, round-trippable via the existing canonical codec, with no AngouriMath or MathNet.Symbolics runtime dependence.

Scientific qualification requires a compiled absent-API RED against actual preview.4, 38 independent GREEN oracles, the relevant full symbolics/IR/quantities test suites, a locked build and eight NuGet-only consumer probes. The consumer restores solely from a local three-package feed with dw.quantities 0.2.0-preview.1 -> dw.tools.math.ir 0.3.0-preview.1 -> dw.tools.math.symbolics 0.5.0-preview.5. The separately compiled C# archive inspector records actual package bytes, manifest dependencies and SHA-256 for those products only. No package is published to a public feed.

This is only M4-W02-C01-T1. Differentiability at singular/nondifferentiable points, derived units, conditional restrictions, rule completeness, executable budgets and additional analytic functions are separately owned by C01-T2. The M4-W02 and M4 milestone states remain in-progress after this run.
