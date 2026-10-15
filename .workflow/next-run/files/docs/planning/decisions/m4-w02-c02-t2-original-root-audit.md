# M4-W02-C02-T2 — Original-root verification and multiplicity boundaries

## Scientific scope

This accepted T2 closes the bounded real equation-solving phase by
checking proposed exact rational candidates **against the original IR
equation**, never against an unrestricted simplification or a binary64
approximation. The already-qualified T1 solution-set certificate is
the only authority for asserting mathematical completeness and for
assigning elementary algebraic multiplicities. A missing rational root
does not prove an empty real solution set.

## Exact operations and evidence

- The new provider-neutral SymbolicRootAudit.Verify(left,right,variable,
  proposedCandidates) admits no more than eight distinct, bounded
  rational points and the existing finite IR graph limits.
- It evaluates the original expression through exact rationals for
  constants, the identified free real variable, sums, differences,
  products, quotients (with denominator zero explicitly excluded),
  squares, bounded nonnegative integer powers and absolute value.
- Retained real assumptions on the target are checked before accepting
  a point: equality, inequality and excluded value. Foreign
  assumptions and dimensional IR nodes receive typed refusals.
- Each point is separately VerifiedRoots, ExcludedFromDomain,
  RejectedCandidates (the original defined relation is false), or
  UnresolvedCandidates (the evaluator has no mathematical proof).
- Only the original-form T1 Finite/Empty certificate allows Complete.
  A T1-proven empty set is distinguished by ProvenEmpty. A verified
  subset or zero verified candidates is never an empty-set proof.
- Multiplicity 1 is admitted for original x=c and both nonzero roots
  of original x²=c. Multiplicity 2 is admitted for the single
  original x²=0 root. Quotients, modified/restricted equations and
  general polynomial candidates carry unknown multiplicity.
- Complete/verified-subset receipts bind the structural SHA-256 of
  BOTH original sides and the free real variable identity. Typed
  refusals cannot expose a root or a provenance/proof rule.

## Explicit boundary tests

- x²=1 with both candidates is complete; one candidate is partial.
- x²=-1 is complete and empty on real domain by the T1 certificate.
  x²=2 with no candidates remains Unknown, not Empty.
- For (x²-1)/(x-1)=0, x=-1 is verified and x=1 excluded; never
  silently cancel the original x=1 pole.
- For (x²-1)/(x-1)=2, the cancellation-derived x=1 candidate is
  excluded, but that alone is NOT a proof that the original solution
  set is empty.
- A foreign restriction, dimensions, binary64 input, unsupported
  sqrt, arbitrary polynomials and budget overruns are not promoted
  into exact root sets or guessed multiplicities.

## Exit criterion and scope boundary

One compiled absent-public-API RED, 40 independent GREEN tests,
eight isolated local NuGet-only probes, exact symbolics/IR/quantities
regressions, plus three independently inspected local product
packages, are required evidence. On durable success only T2 R/G/V,
T2 and phase C02 are complete. W02, milestone M4 and future phase
C03 remain in progress/not-ready as applicable.

No complete CAS or general real/complex polynomial isolation, no
irrational output representation, arbitrary variable assumptions,
automated factorization/cancellation, general inequality solution-set
algebra, conditional roots or numeric approximation is claimed.
No external packages, sibling repositories, agent broker or public
NuGet publication are in scope.
