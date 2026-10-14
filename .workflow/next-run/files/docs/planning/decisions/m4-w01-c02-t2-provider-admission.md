# M4-W01-C02-T2 — Strict typed IR admission of proposed symbolic results

Status: RS056 proposed, not qualified until its own DWF execution succeeds.

## Question

How can a Math caller compare an *untrusted* external symbolic proposal with the existing, independently proven exact mathematical result without silently accepting an erased restriction, an invented predicate or executable provider text?

T1 already qualified the source form (x²-c²)/(x-c) over one free real IR symbol, with typed exclusion x!=c, bounded exact constants and exact point evaluation. C01 separately qualified AngouriMath 2.5.0 for a tiny example and supervised experimental child process. Neither proof authorizes arbitrary CAS expression parsing, provider object execution, unchecked pretty-print comparison or bridge admission to agents.

## Accepted decision

Add a **strict structural admission gate**, rather than a CAS adapter or a string parser:

- API: SymbolicProviderResultAdmission.TryAdmit(IrNode source, object? providerProposedResult), with a typed SymbolicProviderAdmissionResult and explicit statuses Admitted, UnrepresentableOutput, ConditionalMismatch, UnsupportedSource, ContradictoryAssumptions and BudgetExceeded.
- Proposed output must already be a native IrRestrictedExpression. All opaque objects, arbitrary provider ASTs, strings (including "provided x != 1"), unconditional IrApply and binary64 scalars receive UnrepresentableOutput. The gate never calls a foreign object's ToString, reflection methods or callbacks.
- First derive a **local proof** using the T1 SymbolicRationalCancellationBridge.TryCancel(source), which is limited to a known exact algebraic identity and existing IR domain policy. Unsupported input, bounded constants and contradictory original restrictions remain typed refusals.
- Canonically encode and decode the proposed typed result through math-ir/1. Reject unknown IR shapes, unreferenced assumptions and noncanonical restrictions.
- Require exact canonical structural identity of the proposed result and the locally proven restricted output. A missing, added, changed, reordered or relabeled restriction, or a different symbol identity, yields ConditionalMismatch; even a mathematically plausible variation is not silently promoted to an accepted result.
- On admitted equality return the **new locally proved restriction** rather than the caller's supplied object. The bridge remains immutable, exact, provider-neutral and limited to the previously qualified source form.

This is a deliberately conservative, testable *admission filter*, not a general symbolic equivalence proof. It does not establish that AngouriMath or Math.NET Symbolics actually supplies a native IR object with a verified provenance. Provider-specific typed decoding and reliable condition extraction would need a distinct proof and contract.

## RED/GREEN falsification and evidence

Stage the RED reflection test first against the existing, compilable preview.1 symbolics product. The exact controlled RED must identify the missing public TryAdmit(IrNode,object) API; no project-missing, compiler-error or artificial rejection is accepted as RED.

Only after recording that RED, install the new source and 29 independent GREEN tests. The GREEN corpus independently checks domain x=1 exclusion and exact x=2 result; missing/wrong/extra restrictions; matching pre-existing source assumptions and rejection of silently dropped assumptions; identity, display label and integer domain distinctions; contradictory source restrictions, budget overflow, fractional poles, exact canonical IR roundtrip; hostile provider ToString never called; unsupported opaque or foreign AST; and absence of external CAS/Decision/MCDM/AURA product references. Require 30 targeted passes including the recovered RED.

Restore/build the full Math solution in locked mode and run the relevant symbolics, IR and quantities test projects. The isolated NuGet-only T2 consumer must execute eight additional cross-boundary oracles with a single direct dw.tools.math.symbolics 0.5.0-preview.2 PackageReference and transitive IR/quantities, no ProjectReference and no external NuGet feed. Inspect three newly built local archives (dw.quantities, dw.tools.math.ir and dw.tools.math.symbolics) with a separately staged product-only SHA-256/manifest verifier.

## Completion and limits

When successful, T2 R/G/V and T2, plus C02, close inside one DWF transaction. M4 and M4-W01 remain in progress; the next phase C03 is still unstarted. No new external provider dependency, public feed, AURA skill, neutral dw.discovery metadata, native CAS parser, general symbolic proof, solver, arbitrary untrusted AST execution or broker authorization is delivered.

Review trigger: a provider-specific, signed/typed condition transport with actual runtime receipts is ready for a separately scoped integration test, or callers require a stronger equivalence proof or different mathematical identity.
