# M4-W02-C03-T2 — Real calculus singularity and original-domain qualification

## Scope, dependencies, and scientific meaning

The accepted C03-T2 boundary completes M4-W02 by distinguishing
a provable real simple pole, an original-domain removable hole,
a qualified finite removable limit, exact integration on a hole-free
interval, and an unqualified convergence question. M4-W02-C01
qualified derivatives, C02 qualified elementary equation sets and
original-root auditing, and C03-T1 qualified exact degree-seven real
polynomial primitives and finite limits. None constitutes a general
CAS or general improper-integration theorem.

## Exact admitted rules

- SymbolicRealCalculusBoundaries.AnalyzeIntegral and AnalyzeLimit
  accept closed real IR, one free real target, and exact rational
  endpoints/points within the 128-bit rational budget.
- Polynomial operations delegate to the admitted T1 engine, including
  its explicitly arbitrary additive integration constant and stable
  original-source structural semantic SHA-256.
- The structurally exact k/(x-c) family, k nonzero, has a simple pole
  at c. Integrals on intervals including c are classified as
  SimplePole without an integral value or principal-value claim.
  Away from c, the finite real limit is k/(point-c), exactly rational.
  Its logarithmic definite integral outside the pole is left
  UnknownConvergence until a qualified logarithmic contract exists.
- The k=0 numerator has a removable original-domain hole at c.
  Its two-sided limit at c is exactly zero but is not the original
  function value. An interval strictly away from c has exact zero
  integral; an interval touching c is classified OriginalDomainHole,
  with no proper-integral value.
- The already-qualified rational cancellation bridge admits the
  precise (x^2-c^2)/(x-c) = x+c rule subject to x != c. At the
  original hole the finite limit equals 2c exactly but the function
  remains undefined there. A closed interval that excludes c
  integrates the admitted polynomial x+c exactly. One containing c
  is classified OriginalDomainHole, not a nonremovable pole.
- Reversed bounds and endpoints are checked against the same
  original closed interval. No hidden cancellation, binary64
  approximation, automatic principal value, branch or domain change.

## Refusals and provenance

- Unknown functions, generic quotients, radical limits and arbitrary
  improper integrals remain UnknownConvergence, not false proofs
  of divergence or zero.
- Units, retained assumptions, bound/integer variables, binary64
  values and oversized exact inputs get typed refusals. Nested units
  and assumptions are not ignored.
- Exact values and precisely classified original-domain poles/holes
  carry a known rule ID, the original IR structural source SHA-256
  and the target identity. ExactRemovableLimit exposes both the
  limit value and the exceptional point, never a fabricated original
  function value.
- No successful result is derived from approximate computation.

## Scientific work-package closure

A compiled absent-public-API RED must be recovered, followed by
51 independent GREEN tests and eight isolated NuGet-only probes.
The full relevant symbolics tests, locked .NET solution build,
IR/quantities regressions, three locally packed product manifest
and SHA-256 checks, native plan and product validator must pass.
An aggregate M4-W02 qualification record binds the previously
qualified C01/C02 receipts and C03 evidence without claiming M4
release approval.

Only C03-T2 RED/GREEN/verification, C03-T2, C03 and M4-W02
transition to done in this run. M4 stays in-progress and its
G-M4 gate must be considered in a separate controlled run.
No public NuGet publication, foreign CAS dependency, AURA integration,
numerical quadrature, general logarithmic primitive or complex
calculus is admitted.
