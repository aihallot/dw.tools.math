# Exact real elementary equation solving — local preview

The Math symbolics package 0.5.0-preview.7 will expose
SymbolicExactRealSolving.Solve(IrNode left, IrRelationKind relation,
IrNode right, IrSymbol variable, IrScalarDomain.Real) once the
RS061 scientific qualification succeeds. This document describes
the candidate bounded contract; it is not itself an execution receipt.

The independently demonstrable examples are:
- x² = 1 over the reals: complete finite set {-1, 1}.
- x² = -1 over the reals: complete empty set.
- x² = 4/9: exact {-2/3, 2/3}.
- x² = 2: Unknown, not empty; real irrational solutions exist,
  but algebraic-number representation is not yet qualified.
- x >= 2: exact closed lower ray; 2 >= x: exact upper ray.

Only Finite, Empty, AllReal, LowerClosedRay, UpperClosedRay
are complete qualified results. Unsupported, unknown and
budget-exceeded results carry neither candidates nor proof rule.
The original IR sides, not an unqualified rewritten equation, are
recorded through stable structural semantic hashes.

This release is not a general CAS, no complex roots or unqualified
quotient cancellation, no derived-unit algebra, no approximate roots,
no multiplicity or arbitrary candidate substitution. C02-T2 will
analyze these boundaries independently. Product archives are local
only; no public NuGet feed or sibling agent integration.
