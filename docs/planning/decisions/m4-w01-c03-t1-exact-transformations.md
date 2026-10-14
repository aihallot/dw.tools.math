# M4-W01-C03-T1 — bounded exact symbolic transformation decisions

**Status:** RS057 proposal. Qualification requires the real DWF run and its scientific receipt. The prior M4-W01-C02 proof remains authoritative for provider-independent cancellation and strict conditional IR proposal admission.

## Question

Which algebraic expansion, factorization, substitution and real-domain simplification primitives can be implemented as bounded exact mathematical rules, without claiming a generic CAS, binding capture-avoiding engine or assumptions that the source does not prove?

## Chosen closed subset

The first bounded provider-free transformations reside in dw.tools.math.symbolics 0.5.0-preview.3. The product has no AngouriMath, MathNet.Symbolics, AURA, Decision, MCDM or neutral discovery runtime reference.

- **Expand:** for one free real symbol x and an exact rational a (numerator and denominator at most 128 bits), expand Square(Add(x,a)) to Add(Square(x), Multiply(2a,x), a²). The output is a canonical typed IR polynomial. Only this structurally recognized form is admitted.
- **Factor:** reverse the exact canonical three-term binomial expansion when 2a and a² agree with the source. A false constant or reordered/mismatched pattern is Unsupported rather than guessing a proof.
- **Substitute:** replace an explicitly identified free real symbol in a closed exact Add/Subtract/Multiply/Square tree with an exact scalar or other free real symbol. Symbol identity, not display name, decides replacement. Bound symbols and restricted expressions are refused instead of silently capturing a binder or dropping a domain condition. This is *safe by refusal*, not a general capture-avoiding substitution algorithm.
- **Real sqrt:** normalize sqrt(square(x)) to abs(x) over the real domain without a sign proof. Under a typed x>=0 hypothesis, return x **as a restricted expression** carrying that assumption, not unconditionally x.
- **Assumptions:** preserve existing conditions on the exact same free real symbol in the closed expansion/factor rules. Refuse unknown-scope restrictions; explicit incompatible equality and inequality assumptions return ContradictoryAssumptions. The admitted IR codec checks the resulting typed restriction and expanded node/depth limits.
- **Results:** typed statuses Admitted, Unsupported, ContradictoryAssumptions and BudgetExceeded, with a versioned operation-specific local proof-rule ID. A failed transform contains no expression. No opaque provider string is accepted as evidence.

The selected mathematical rules are locally proved elementary identities, not consequences of runtime AngouriMath string rendering. The previously qualified SymbolicProviderResultAdmission gate stays unchanged and must continue to reject arbitrary external output.

## RED/GREEN and independent falsification

RS057 must first compile the existing .NET 10 symbolics product as version preview.2 and execute a reflection RED proving the absence of the new ExpandSquare(IrNode) public contract. An unresolved symbol or build failure is not a valid RED.

Then install the T1 transformation source, version preview.3 and 28 distinct GREEN oracles. The corpus includes x+1 and x-2 expansions, exact 1/2 and zero constants, closed factor/expand roundtrips, preservation of typed restrictions and math-ir/1 serialization, contradictory/external assumptions, excessive operands, integer/bound symbols, free-variable identity and bound-variable capture refusal, refusing restricted substitution, and both sign cases for sqrt(square(x)). Require 29 targeted passes including the recovered RED.

Run the relevant full symbolics, typed IR and quantities test projects after locked .NET solution restore/build. Pack three local .NET 10 product archives with a provider-free dependency graph: dw.quantities 0.2.0-preview.1 -> dw.tools.math.ir 0.3.0-preview.1 -> dw.tools.math.symbolics 0.5.0-preview.3. Observe actual nuspec IDs, exact dependencies, byte sizes and SHA-256 with a separate C# archive-only helper; do not rehash workflow or project authorities locally.

Use a separately compiled .NET 10 NuGet-only consumer with only dw.tools.math.symbolics preview.3 as direct PackageReference, no ProjectReference and a closed external feed. Eight independent package-only oracles check algebraic expansion/factoring, exact coefficients and restrictions, substitution scope refusals, real sqrt domain behavior and no external CAS runtime. Success marker: dw.tools.math/m4-exact-transform-consumer/0.5 qualified; probes=8.

## Deferrals and explicit nonclaims

T1 does NOT admit arbitrary factoring/simplification, substitution under binders via alpha-renaming, all polynomial degrees, complex sqrt branches, general symbolic equivalence, all contradictory inequality detection, provider-cooperative interruption, general input budgets, unknown conditional IR objects, AURA permissions or a generated dw.discovery skill.

These boundaries belong in the separately planned M4-W01-C03-T2 review, with node/operation budgets, stronger uncertainty/cancellation and assumption-transfer tests. RS057 completes only C03-T1 R/G/V and T1. The C03 phase, M4-W01 work package and M4 symbolic release stay in progress. A subsequent release gate is required for milestone completion.

Review trigger: a caller supplies real binder-scoped expressions needing capture-free alpha-renaming, a provider-backed transform with typed domain predicates, or broader polynomial algebra beyond the admitted exact binomial subset.
