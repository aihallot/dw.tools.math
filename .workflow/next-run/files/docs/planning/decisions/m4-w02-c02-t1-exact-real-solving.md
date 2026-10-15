# M4-W02-C02-T1 — Exact real elementary solution sets

## Responsibility and prior authority

The accepted M4-W02-C02-T1 outcome is an exact, domain-aware public
contract for a bounded subset of real equations and inequalities. C01
qualified elementary formal and pointwise differentiation; it did not
qualify equation solving. This decision is applied only when RS061's
compiled RED, exact GREEN oracles and independent local NuGet consumer
actually succeed.

## Admitted contract

- SymbolicExactRealSolving.Solve(left, relation, right, target, Real)
  accepts finite math-ir/1 nodes, one explicitly free real target and
  Equal or GreaterOrEqual.
- Completeness is provable for x=c, c=x, x²=c, c=x², constant
  relations, x>=c, c>=x, x>=x, x²>=c for nonpositive c,
  and c>=x² for nonpositive c.
- For exact real quadratic equations, a negative exact rational c
  gives the complete empty set. Zero gives {0}; positive c whose
  numerator and denominator are perfect integer squares gives the
  complete distinct ordered rational roots. For positive nonsquare
  rational c the result is Unknown, NEVER Empty.
- x>=c gives a closed lower ray; c>=x gives a closed upper ray.
  x²>=positive gives Unknown, not a false interval proof.
- Finite, Empty, LowerClosedRay, UpperClosedRay, and AllReal
  are the only qualified complete set shapes. Admitted outputs retain
  a local rule ID, the structural semantic SHA-256 of both original
  relation sides and the variable identity.
- All outputs are exact rational values or symbolic real set shapes.
  No binary64, floating epsilon, finite differences, or external CAS.

## Exclusions and deferred T2 boundary

- No complex domain: IR currently has no admitted Complex domain.
  Integer, bound-symbol and unqualified domains are rejected.
- Nonrational radicals, linear combinations, arbitrary polynomials,
  products, generic quotients, radicals, conditional relations,
  nontrivial inequalities and unrecognized predicates remain unknown
  rather than invented successes or false empty sets.
- Restriction-bearing IR, units, implicit numeric approximations and
  out-of-budget 128-bit rational inputs have typed refusals.
- This T1 does not qualify multiplicity, candidate substitution back
  into arbitrary original equations, spurious-root elimination for
  algebraic divisions, partial solution sets or complex roots. T2
  remains open for these scientific and integration boundaries.
- A proof identifier is an admitted bounded rule identity, not a
  general automated theorem prover or provider certificate.
- Math owns the solver. AURA, decision, MCDM and dw.discovery remain
  independently owned and are not modified here.

## Evidence/exit criteria for RS061

1. One compiled missing-public-API RED against symbolics preview.6,
   then its recovery after installing the new preview.7 code.
2. 36 independent GREEN MSTest oracles covering exact and unknown
   real roots, order, zero, rational squares, real rays, original input
   provenance, domain and unit refusals and budget restrictions.
3. The changed symbolics suite and related IR/quantities suites,
   locked .NET 10 solution build and plan/transfer checks.
4. Eight independent NuGet-only client probes with one pinned
   symbolics package, no ProjectReference, offline local-only feed.
5. Three generated product archives independently inspected for
   net10 assembly, nuspec graph, byte length and SHA-256.
6. Native DWF T1 R/G/V and T1 complete; C02 begins in-progress,
   M4-W02/M4 stay in-progress and C02-T2/C03 remain not-ready.
   Claims are contingent on the RS061 durable execution report.
