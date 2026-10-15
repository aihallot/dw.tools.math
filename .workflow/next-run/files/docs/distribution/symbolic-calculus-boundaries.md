# Exact real calculus boundaries — local symbolics preview.10

SymbolicRealCalculusBoundaries.AnalyzeIntegral and AnalyzeLimit
provide a bounded exact real calculus interface over closed math-ir/1.
The local package reuses the already-qualified rational polynomial
primitive and limit contract.

- integral(x,0,1) = 1/2 exactly, preserving the arbitrary +C
  convention for the primitive.
- integral(1/(x-1),0,2) returns SimplePole at 1, without an invented
  finite value or principal-value interpretation.
- For (x^2-1)/(x-1), x=1 is an original-domain removable hole,
  not a pole. Its two-sided limit is exactly 2 while the ORIGINAL
  function value remains undefined.
- Integral of that rational function from 2 to 3 is 7/2 exactly,
  because the qualified identity x+1 is valid on the entire interval
  and the original excluded point x=1 lies outside it.
- A generic quotient, unproved radical, or logarithmic primitive
  remains UnknownConvergence rather than an approximate result.
- Units, retained domain assumptions, binary64 and out-of-budget
  exact rationals are not silently promoted.

No general convergence classification, numerical quadrature,
one-sided infinity, principal value, arbitrary CAS, complex
branches or external agent interface is admitted. Version
0.5.0-preview.10 is a local package only, not a public NuGet
release. The complete M4 milestone needs a separate gate.
