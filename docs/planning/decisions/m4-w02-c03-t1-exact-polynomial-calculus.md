# M4-W02-C03-T1 — Bounded exact real polynomial calculus

## Accepted scope and existing mathematical authorities

M4-W02-C03-T1 introduces one provider-neutral polynomial calculus
contract over free REAL IR symbols and exact rationals. It depends on
the already-qualified formal degree-eight polynomial differentiation
from M4-W02-C01-T1 and exact source/target provenance. It does not
license general CAS integration, improper limits or a complex branch.

## Product contract

- Exact admitted grammar: canonical rational constants, the declared
  free real symbol, direct square or nonnegative integer powers up to
  degree seven, additive/subtractive polynomial combinations, and
  constant scalar multiplication.
- Input is checked for any dimensional quantity, source restriction,
  binary64 approximation, integer/bound variable or another free
  symbol before any cancellation or simplification. Typed refusal
  retains no mathematical primitive, integral value or proof.
- The canonical polynomial is a bounded exact array of rational
  coefficients. A primitive is constructed by the formula
  sum(c_k/(k+1) * x^(k+1)) with a representative additive constant
  equal to zero. The result explicitly states that the entire
  indefinite family includes an *arbitrary additive constant*.
- Degree seven is the maximum admitted integrand degree: its primitive
  can have degree eight, matching the already-qualified derivative.
  Differentiating the produced primitive must succeed using the
  admitted formal rule, and its independently extracted exact
  coefficients must equal the original polynomial, not merely a
  similar-looking expression or binary64 approximation.
- The definite integral is primitive(upper)-primitive(lower) for
  bounded exact rational endpoints, including reversed bounds.
  Every exact intermediate must fit the 128-bit rational budget.
- A finite two-sided real polynomial limit at an exact rational
  point is the exact polynomial value by global real polynomial
  continuity. No general singularity or convergence classification
  is claimed.
- Admitted results bind an explicit rule ID and the canonical IR
  structural semantic SHA-256 plus the integration variable identity;
  refusals contain no value, primitive or proof.

## Tests, packaging, and scope boundary

The run must observe one compiled missing-public-API RED before
the new code is installed, 50 independent GREEN MSTest oracles,
eight NuGet-only consumer proofs, a locked .NET 10 solution build,
relevant symbolics/IR/quantities regressions and three independently
inspected local NuGet products with version/assembly/dependency
and SHA-256 checks.

T1 completes its R/G/V children and task only. Phase C03 remains
in-progress because T2 still must qualify singularity/pole refusal,
unknown convergence, additive constant and incomplete/improper
boundary handling. M4-W02 and M4 also remain in-progress.
No derived units, general piecewise integrals, numerical quadrature,
external provider, public feed or sibling-repository changes.
