# M4-W02-C01-T2 — exact pointwise derivative, domain and refusal boundary

**Status:** RS060 proposal. No new capability is admitted until the controlled DWF RED/GREEN, package-consumer tests and durable evidence succeed.

## Question

M4-W02-C01-T1 qualified one exact provider-neutral formal derivative for bounded real polynomials: d(x^3)/dx = 3*x^2, d(x^3)/dy = 0 for a free independent variable, and explicit refusals for quantities, retained conditions, bound/integer symbols and unsupported analytic operations. T1 does **not** prove differentiability of absolute value, square root or rational expressions at specific points.

How can a Math consumer ask about a real exact point while distinguishing a known exact derivative from a genuinely non-differentiable point, a point outside the original real domain and a case whose derivative is simply **not qualified**, without numerical estimation?

## Chosen bounded contract

Add SymbolicDerivativePointAnalysis.Analyze(IrNode,IrSymbol,ExactRational) and the SymbolicRealFunction overload to the local dw.tools.math.symbolics **0.5.0-preview.6** package, with explicit SymbolicDerivativePointStatus:

- ExactValue: provide an exact rational only after the qualified T1 polynomial derivative and a **bounded exact IR point evaluator**, or one of two independently known elementary real-domain local rules.
- NonDifferentiable: for Abs(x) at x=0, Sqrt(Square(x)) at x=0, and Sqrt(x) at x=0 with no finite two-sided derivative.
- OutsideRealDomain: for Sqrt(x) at a negative input and for the original removable-singularity quotient (x^2-1)/(x-1) at x=1. Algebraic cancellation never fills the original excluded point.
- Unsupported: for Sqrt(x) at a positive point (its analytic derivative is not qualified in this closed exact rational evaluator), general quotients away from their removable pole, unrelated symbols, negative exponents, unknown functions and binary64.
- UnsupportedDomain, UnsupportedUnits, UnsupportedAssumptions and BudgetExceeded preserve the source T1 distinctions. Unit-bearing expressions are not coerced to unitless derivatives, and prior IR restrictions are not silently dropped.

Over the reals, |x| has derivative +1 at x>0 and -1 at x<0, but is not differentiable at zero. The same rule applies to Sqrt(Square(x)) because its real value is |x|. No unqualified principal complex-branch or general radical derivative is exposed.

For the existing T1 exact polynomial class, evaluate the **already proven derivative** at an exact rational point; do not perform symbolic simplification from untrusted input. The evaluator admits exact canonical constants, the differentiation variable, sums, subtraction, products, squares and positive powers up to eight. It refuses other IR nodes. A conservative 128-bit bound applies to point and intermediate exact-rational numerators/denominators; the existing math-ir/1 node/depth cap and local evaluation depth limit also remain effective.

Every ExactValue carries a fixed local pointwise rule identifier, canonical *source* math-ir/1 semantic structural SHA-256, and the differentiation-variable identity. These identify the admitted calculation and are not algebraic-equivalence theorems, third-party provider attestations or permission to invoke a broker. Refused results carry no numeric value, rule/proof ID or hash.

## Alternatives rejected or deferred

- Do not use finite differences (including a double-based infinitesimal) as exact derivative evidence.
- Do not claim that Sqrt(x) is differentiable at its real-domain boundary zero or that Abs(x) has a derivative there.
- Do not treat the simplified x+1 form of (x^2-1)/(x-1) as having a pointwise derivative at x=1; the original quotient is undefined at that point.
- Do not infer units of a derivative without an independently qualified dimension arithmetic/units contract.
- Do not parse or execute AngouriMath/MathNET ASTs or condition text; C01 here is a Math-owned exact boundary, not a provider adapter.
- Defer numerical differentiation methods, interpolation, extended function families, complex analysis, one-sided derivatives, piecewise functions, arbitrary quotient rules and broader runtime budgets.

## Falsification and dependency gates

RS060 first compiles the current local preview.5 package and executes an independent reflection RED requiring the absent Analyze(IrNode,IrSymbol,ExactRational) API. A missing project or compiler error does not count as a scientific RED.

Then install the new API and **37 independent GREEN tests**, with **38 targeted passing tests including the recovered RED**. The tests independently cover x^3 point values (including negative/zero arguments), exact sums and fractional multipliers, free/absent variables, absolute and sqrt-square real branch/kink behavior, sqrt(x) outside/boundary/unknown, rational pole preservation, typed units and assumptions, budgeted exact inputs, structural provenance and no binary64 or foreign CAS fallback.

Require locked .NET 10 restore/build, complete relevant symbolics, math IR and exact-quantity test suites, and eight additional **NuGet-only** consumer probes with one direct pinned dw.tools.math.symbolics PackageReference. Rebuild three **local-only** packages and inspect their actual net10 DLLs, package dependencies, byte lengths and SHA-256s using a separate C# archive verifier. The old IR and quantities package versions remain unchanged; only symbolics advances to preview.6.

If all gates pass, T2 R/G/V, T2 and the C01 phase become done in one native DWF transaction. **M4-W02 remains in-progress** because C02/C03 (solving and calculus boundaries) are still unstarted. M4 and G-M4 stay open.

**Review triggers:** need for a truly numerical differentiation API; a broader analytic derivative; dimensional/unit results; signed one-sided derivative semantics; provider-specific conditional ASTs; or external agent invocation rights.
