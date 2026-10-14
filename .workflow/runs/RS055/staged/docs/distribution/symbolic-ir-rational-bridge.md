# M4-W01-C02-T1 — exact, restricted symbolic IR rational cancellation

The first Symbolics product capability is deliberately small: one **provider-independent** structural identity, over one free **real** IR symbol and one exact rational constant c:

  (x²-c²)/(x-c)  =>  x+c, with x != c preserved

The result is *not* an unconditional equality: at x=c the original rational expression is undefined. This package never parses AngouriMath's human-readable "provided" string as a verified mathematical condition; the proof comes from the fixed exact structural identity and a typed IrRelation with IrRelationKind.NotEqual. Inputs outside the admitted IR form are rejected or marked unsupported without executing arbitrary provider code.

## Contract

- Product: src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj, package dw.tools.math.symbolics 0.5.0-preview.1, referencing dw.tools.math.ir and its transitive exact quantities only; no direct AngouriMath, MathNet.Symbolics, MathNet.Numerics, Python, AURA, Decision, MCDM or broker dependency.
- API: SymbolicRationalCancellationBridge.TryCancel(IrNode) returns a typed SymbolicCancellationResult with statuses Admitted, Unsupported, ContradictoryAssumptions and BudgetExceeded. Only Admitted carries a non-null IrRestrictedExpression with exact x != c.
- Point evaluation: EvaluateExactAt(result,x,point) requires an admitted result and matching free symbol identity. It reports Excluded with null numeric value at the denominator zero, Unsupported for unproven cases, ExactValue with a canonical rational only for points satisfying all carried conditions.
- Supported exact syntax: IrApply.Divide(Subtract(Square(x) or Power(x,2),Exact(c*c)),Subtract(x,Exact(c))). The symbol must be a free IrScalarDomain.Real symbol with stable identity. c may be positive, negative, zero or an exact rational; magnitude is bounded at 128 bits before multiplication.
- Existing assumptions: only identity-matched relations supported by the present canonical IR (Equal, NotEqual, GreaterOrEqual). Existing requirements are preserved and checked during point evaluation; duplicate x!=c is not inserted. Unrepresentable predicates or different bound/free identities are Unsupported, not silently discarded. Contradictory equality at x=c or equality incompatible with an existing restriction is refused.
- Representation: the admitted IrRestrictedExpression is checked through the existing math-ir/1 canonical encode/decode contract. The public IR codec and symbolic result status remain different from an external CAS-provided expression tree.

## Executable RED/GREEN and package gate

1. Install the **real, empty but compilable** symbolics product project, lock files, tests project and solution references. Run the independent reflection RED that requires the missing public TryCancel(IrNode) API. The expected RED is a specific failed MSTest assertion, **not** a compile error, missing project, missing package or invented failure.
2. Install the implementation and 25 independent semantic GREEN tests; run the same RED again as passing, for 26 total targeted passes. Tests include the x=1 pole, negative/zero/fractional poles, identity mismatch, scope, real/integer policies, contradictions, bounded constants, no implicit binary64, canonical typed IR round trip and dependency graph isolation.
3. Run the full relevant symbolics, IR and quantities tests with locked .NET 10 solution restore/build, then compile three scoped NuGet packages to a local feed: dw.quantities 0.2.0-preview.1, dw.tools.math.ir 0.3.0-preview.1, dw.tools.math.symbolics 0.5.0-preview.1. A separate post-packaging .NET C# inspector records actual archive manifests, declared dependencies, byte lengths and SHA-256, without locally re-hashing any DWF or project-planning authority.
4. Restore and execute a *package-only*, independently authored .NET 10 consumer with only dw.tools.math.symbolics PackageReference, no ProjectReference, no external feed and explicit local package-cache isolation. Eight independent checks cover exact x=1 exclusion and null value, x=2 ->3, x=0 ->1, different c and fractional poles, stable canonical typed IR/condition, real domain, and no CAS dependency. Observe the exact marker dw.tools.math/m4-ir-bridge-consumer/0.5 qualified; probes=8.
5. Emit controlled RED and qualified T1 evidence, then reconcile the canonical backlog, native DWF T1 R/G/V and planning projections in the same controlled run. C02 remains in-progress; C02-T2 and M4-W01-C03 stay unstarted. EXT-SYMBOLIC is satisfied **only for its previously accepted, bounded provider-selection and domain/host evidence from C01**, not for a production symbolic CAS adapter.

## Nonclaims and review triggers

No general rational simplification, polynomial factorization, provider AST conversion, full real/complex domain reasoning, symbolic proof of arbitrary expressions, outside-corpus expression evaluation, differential or integral solver, native CAS cancellation, unrestricted compute, public feed publication or AURA/dw.discovery BrokerReady capability. The release-wide G-M4 gate remains separate. Any request for additional forms, complex principal branches or provider-exposed predicates requires a distinct accepted bridge and independent tests; do not silently add a fallback to approximate binary64.
