# Exact original-equation root verification — local Math preview

Version 0.5.0-preview.8 adds the bounded C# API
SymbolicRootAudit.Verify(left, right, freeRealVariable, candidates).

The input is an original real equality with up to eight distinct
exact rational candidate roots. The result reports candidates that
were verified by exact substitution, rejected by equality, excluded
from the original domain, or left unresolved. The evaluator never
substitutes a floating-point approximation or ignores a denominator.

A Complete result is available only if the original-form T1 exact
solution-set contract independently proves all solutions accounted
for; ProvenEmpty is reserved for its complete empty-set proof.
VerifiedSubset means proven valid candidates, *not* all solutions.
NoVerifiedCandidates is *not* an empty solution set.

For x² = 0, root zero has admitted algebraic multiplicity 2.
For x² = 1, -1 and 1 each have multiplicity 1. General polynomial
or quotient root multiplicity stays unknown (null).

For (x²-1)/(x-1)=0, candidate -1 is valid, while candidate 1
is excluded as the original denominator is zero. The result remains
a VerifiedSubset, not a silently completed solution set.

Original restrictions are checked. Units, unproved radicals,
foreign assumptions, complex domains, arbitrary roots and
implicit numeric approximation are outside the admitted contract.
No public NuGet or cross-repository agent integration is performed.
This document does not substitute for the executable RS062 receipt.
