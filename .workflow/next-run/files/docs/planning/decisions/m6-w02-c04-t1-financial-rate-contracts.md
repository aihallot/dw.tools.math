# M6-W02-C04-T1 — bounded pure financial effective-period mathematics

## Accepted scope and authority

RS101 is the first independent code task of planned size-M
M6-W02-C04 (pure financial mathematics) after RS100 durably completed
the C03 geometry/signal/time-series aggregate. The local BCL-only
dw.tools.math.scientific package advances from preview.12 to
1.1.0-preview.13. This task is parameter-only arithmetic, NOT a pricing
service, financial recommendation or investment suitability model.

## Explicit discount and rate conventions

`M6W02C04Contract.Discount(amount,effectiveRate,periods)` uses the
**effective discrete rate per caller-managed period** convention:

`presentValue = amount / (1+effectiveRate)^periods`.

The input amount and output use the caller's same monetary/value unit.
Periods are integer counts 0..32, NOT dates or years inferred from
a calendar. Rates are finite values within [-0.9, 4] per period,
meaning base 1+r is strictly positive. Input amount has
absolute magnitude <=1e6; finite present values are bounded
by 1e40. No continuous compounding, force of interest, nominal
APR conversion, day-count conventions, curve bootstrapping, taxes,
fees, market data or implicit foreign exchange conversion.
The independent primary oracle is:

`Discount(100,0.1,1) = 1000/11 = 90.909090...`.

## Constrained internal-rate-of-return contract

`M6W02C04Contract.SolveIrr(cashflows,minRate,maxRate)` accepts
2..16 finite cashflows, each of absolute magnitude <=1e6.
Cashflows[0] is at **period zero**; later elements are evenly indexed
integer periods in the same caller-managed period basis.
Both minRate and maxRate are explicitly supplied finite effective
discrete rates satisfying -0.9 <= minRate < maxRate <= 4.

The admitted candidate root solves

`NPV(r) = sum(t=0..N-1) cashflows[t] / (1+r)^t = 0`.

T1 admits only a cashflow sequence with **exactly one sign change
after ignoring zero terms**, and with the root bracketed by the
specified caller-provided endpoints. It evaluates the two endpoints
and performs at most 96 bracket-preserving bisection iterations.
This is an approximate binary64 numerical solution, NOT a certified
root bound, a global search or a prediction of returns.

- No sign change => typed `NoSignChange` refusal.
- More than one sign change => typed
  `AmbiguousCashflowPattern` refusal, without asserting how many
  roots actually exist.
- A one-sign-change case not bracketed on the caller's interval
  => `RootNotBracketed`, NOT a global claim that no IRR exists.
- Null, nonfinite, oversized, out-of-budget or invalid-rate inputs
  => corresponding typed nonpartial refusals.
- Cancellation is pre-emptive and checked in bounded loops; all
  refusals have `Value=null` and never leak a partial calculation.

The independent primary IRR oracle is
`SolveIrr([-100,110],0,0.5) ≈ 0.1`.
Other oracles include reversed-sign flows, zero intervening periods,
negative but admitted rates, and rejecting an unbracketed/mixed-sign
cashflow pattern.

## Independent local RED/GREEN evidence

The controlled DWF run first stages only the independent reflection
RED for the missing public `M6W02C04Contract` API against the
qualified preview.12 source. Compile and observe one runtime RED
with the exact missing-public-API assertion; compiler failure does
not count.

Then stage the new contract source, preview.13 metadata and
66 independent GREEN tests (27 discount, 39 IRR), retaining
the previously accepted **837 scientific tests**:
79 original C03-T2, 57 original C03-T1,
107 S03-T2, 61 S03-T1, 69 S02-T2, 60 S02-T1,
53 S01-T2, 25 S01-T1,
88 C02-T2, 80 C02-T1, 88 C01-T2 and 70 C01-T1.
Also run the full scientific suite, four Math regression projects,
planning and transfer architecture validators.

Pack one actual local BCL-only `1.1.0-preview.13` scientific NuGet
archive. Inspect compiled net10 DLL, nuspec identity/version and
absence of foreign dependencies, actual byte count and SHA-256.
Qualify six independently restored pinned source-free eight-probe
consumers from only the same cleared local feed, with independent
caches: new C04 finance, retained C03-T2 aggregate boundary,
retained C03-T1 direct aggregate, retained S03-T2 uniform audit,
retained S02-T2 signal audit and C02-T2 special-function boundary.
All must agree with the archive inspector on bytes and SHA-256.

## Closure and deferred financial boundaries

On success close ONLY M6-W02-C04-T1-R/G/V and C04-T1; parent C04
becomes in-progress, C04-T2 remains planned/not-ready.
C05, work package M6-W02 and milestone M6 remain unfinished.

C04-T2 independently addresses explicit date/day-count conventions,
multiple/no-root cases, numerical boundary conditioning and error
classification beyond the admitted single-change bracketed T1 scope.
No public financial services, live prices, trading advice, public
NuGet release, untested operating system or AURA/Decision/MCDM runtime
adoption is certified by this local task.
