# Exact real polynomial integration and finite limits

The local dw.tools.math.symbolics 0.5.0-preview.9 package offers a
bounded, provider-neutral polynomial calculus contract for .NET 10.

Use SymbolicExactPolynomialCalculus.Integrate(source, freeRealSymbol,
lowerExactRational, upperExactRational). The admitted exact result
contains a zero-constant antiderivative representative, a definite
rational integral, a rule identifier, the source structural semantic
SHA-256, variable identity and the required
"arbitrary-additive-constant" convention.

For the admitted polynomial grammar:
- integral x from 0 to 1 = 1/2 exactly.
- integral x^2 from 0 to 1 = 1/3 exactly.
- integral x from 1 to 0 = -1/2 exactly.
- the derivative of a constructed primitive is admitted by the
  existing formal polynomial derivative and verified by exact
  coefficient comparison.
- FiniteLimit(x^2, x, 2) = 4 exactly by polynomial continuity.

Inputs are limited to one free real symbol, exact rational
coefficients and bounds, scalar products, sums/subtractions, squares
and polynomial powers up to degree seven, with a 128-bit rational
budget. The primitive can reach degree eight, which is precisely the
existing derivative bound.

Units, restrictions, binary64 values, roots, quotients, absolute
value, improper integrals, arbitrary singularities and numerical
quadrature are not silently admitted. This package is local only;
no foreign symbolic engine, agent, public NuGet feed or release
qualification is claimed.
