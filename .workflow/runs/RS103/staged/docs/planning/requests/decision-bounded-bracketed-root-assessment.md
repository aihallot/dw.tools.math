# Decision reuse assessment request — bounded bracketed scalar root solving

**Status:** proposal for owner triage only. No duplicate implementation in
`aihallot/dw.tools.decision` has been established by this request,
and no cross-repository write, provider cutover or release is authorized.

## Candidate mathematical primitive

The bounded deterministic scalar root-bracketing/bisection loop used
inside `src/projects/dw.tools.math.scientific/M6W02C04Contract.cs`
(`SolveIrr`) and
`src/projects/dw.tools.math.scientific/M6W02C04Boundary64.cs`
(`SolveDatedIrr`), currently admitted as local scientific
`1.1.0-preview.14`. Request the Decision owner to **evaluate**
whether an independently useful scalar bracketed-root primitive exists
or is planned there and whether consuming the stable Math primitive
would eliminate actual duplication.

## Stable contract to preserve in any future shared utility

- Pure deterministic `double` scalar function with finite values;
  caller supplies a finite ordered bracket and a bounded iteration
  budget (existing financial implementation: max 96 bisections).
- Bracket endpoints require a sign change or a root at an endpoint;
  no silent global root search, no multiple-root certification.
- Cancellation is explicit and checked in bounded steps. Rejection
  gives a typed status and **no partial root or caller value**.
- Numerical result is approximate binary64; no certified rounding
  error theorem, monotonicity assumption, market data or decision
  policy is smuggled into the primitive.
- Ownership boundaries: root computation belongs to
  `dw.tools.math`; objective/criterion policy remains with
  `dw.tools.decision`. Avoid new circular project/package references.

## Independent acceptance oracles for assessment

- `f(x)=x-0.1` with bracket `[0,0.5]` => approximately `0.1`.
- `f(x)=x*x-4` with bracket `[0,3]` => approximately `2`.
- `f(x)=x+1` with bracket `[0,0.5]` => typed non-bracket refusal,
  NOT proof of global no-root.
- Pre-cancelled invocation => Cancelled with no returned root.

These are **proposed future unit oracles**, not tests run in RS103,
and are not claims about a current Decision API.

## Request, alternatives and owner gate

The Decision owner should (1) check whether this root-solving
capability is actually needed and duplicated, (2) either reject reuse
with a reason or approve a concrete design/revision pin, (3) require
independent source-free consumer tests, semver and notices review
before any dependency transition, and (4) keep original scientific
and Decision domains intact.

If no duplicate or consumer requirement exists, **do nothing**:
keep the qualified Math implementation and close the assessment as
not applicable. The original local Math C04/T1/T2 contracts remain
admitted regardless of that independent owner decision.
