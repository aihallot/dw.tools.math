# M4-W01-C02-T1 — decision to admit a structural typed-IR bridge

**Status:** proposed for controlled RS055 RED/GREEN qualification; not a declared product capability until its executed DWF receipt and evidence exist.

## Question

How can a Math consumer represent and evaluate an algebraic rational cancellation without silently changing its mathematical domain, without exposing raw AngouriMath AST/text, and without treating a plausible shortened string as a symbolic proof?

Existing Math IR already offers exact rationals, free/bound real symbols with stable identities, typed Divide/Subtract/Square/Power/Add operations, IrRelationKind.NotEqual and IrRestrictedExpression. C01 provider qualification established AngouriMath as a **provisional candidate only** and tested host-process boundaries, but did not deliver a domain-preserving IR adapter.

## Selected minimal approach

Implement one exact, structurally recognized mathematical identity:

  (x²-c²)/(x-c)  ->  restricted(x+c, x != c)

A non-zero denominator is the source of the exclusion, even if the simplified numerator also vanishes at x=c. This does not invoke a CAS to invent or prove algebraic simplifications. A separate typed point-evaluation path checks every retained relation before returning a canonical rational.

A corresponding controlled RED requires a real, compilable symbolics package to expose the absent public cancellation contract. A GREEN suite exercises exact hand-computable points, fractional and negative poles, the original singular point, bound/free-symbol mismatch, contradictory assumptions and round-trip through the already-qualified math-ir/1 codec. No unknown input is admitted merely because it can be converted to a string.

The new package depends on IR (and transitively quantities), **not** the optional AngouriMath or MathNet.Symbolics packages. This honors the selected architecture: provider-independent public contracts, separately qualified optional adapters. Existing symbolic-provider and host qualification supports closure of EXT-SYMBOLIC as a *bounded provider review* prerequisite, not blanket admission of a public CAS executor.

## Rejected alternatives

- Unconditionally return x+c for a simplified rational expression; it would silently assert a false value at x=c.
- Trust a provider's printed "provided" condition as an executable Math IR assumption or accept opaque text as mathematics. C01 did not qualify that translation.
- Import all provider AST nodes into Math's public model or coerce exact quantities into binary64. No such behavior is admitted in T1.
- Add a general-purpose symbolic solver, a mandatory Python child process, a new AURA command or dw.discovery skill as a prerequisite for this simple exact rule.

## Scope decision and follow-up

T1 is a meaningful public *narrow* symbolic package contract with exact Unsupported, ContradictoryAssumptions, BudgetExceeded, Excluded, and ExactValue outcomes. The next accepted T2 must qualify unrepresentable symbolic provider predicates, input identity/capture, general conditional transport limits, budgets, and proper refusal rather than accepting unsupported behavior. No Math release or M4-W01 completion is claimed by T1 alone.

**Review triggers:** malformed or incompatible conditional IR, conflicting constraints, provider versions or domain semantics changing, client code requiring complex roots or non-polynomial operations, or an independently demonstrated need for a broader provider-backed transform.
