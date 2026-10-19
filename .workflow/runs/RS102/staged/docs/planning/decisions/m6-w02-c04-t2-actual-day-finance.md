# M6-W02-C04-T2 — explicit actual-day finance and conservative IRR topology

## Authority, purpose and no implicit financial advice

RS101 independently qualified effective-discrete-per-period discounting and a
one-sign-transition bracketed IRR in local scientific preview.13.
RS102 qualifies T2 of the retained size-M `M6-W02-C04` pure-financial
mathematics parent. It adds `M6W02C04Boundary64` in one local
`dw.tools.math.scientific` preview.14 package, preserving all T1 APIs.
No automatic capital market lookup, prices, yield curve, financial advice,
forecast, fees, taxes or currency conversion are introduced.

## Date and rate convention, explicitly selected by the caller

`FinancialDayCountConvention64` accepts two **literal fixed-denominator**
actual calendar-day conventions: `Actual365Fixed` (actual elapsed days /
365) and `Actual360` (actual elapsed days / 360). Both count dates via
`DateOnly.DayNumber` — no timezone, business-day calendar, holiday,
ACT/ACT leap-year denominator, annualization inferred from clock time,
payment schedule synthesis, compounding frequency conversion or nominal
APR. Leap days count normally in the numerator, while the denominator
is fixed by the caller's chosen convention.

`DiscountDated(amount,effectiveAnnualRate,valuationDate,paymentDate,dayCount)`
returns `amount / (1+r)^(actualDays/denominator)`. Both dates are
explicitly supplied; zero days admit the unchanged amount and negative
elapsed days refuse `InvalidDates`. The caller owns amount unit and
interpretation of the annually effective rate. Input magnitude <=1e6,
admitted rate [-0.9,4], elapsed date horizon <=36500 actual days and
present value <=1e40 are inherited/conservative bounded domain policies.
The principal ACT/365 oracle is
`DiscountDated(100,0.1,2025-01-01,2026-01-01,Actual365Fixed)=1000/11`;
a 360-day span under ACT/360 gives the same value.

`SolveDatedIrr(cashflows,dates,minRate,maxRate,dayCount)` requires
2..16 finite cashflows paired with STRICTLY INCREASING DateOnly dates.
The first cashflow's date is time zero, not an inferred annual index.
For the same explicitly selected basis, NPV at annual effective rate r is
`sum_i(cashflows[i]/(1+r)^((dates[i]-dates[0])/dayDenominator))`.
A caller-supplied finite, ordered bracket lies in [-0.9,4], and
root-finding uses at most 96 finite binary64 bisection iterations.
The canonical ACT/365 oracle uses [-100,110] at 365 actual-day
separation with IRR approximately 0.1. Irregular date intervals are
allowed *when explicitly provided*; no resampling or implicit regular
calendar period is inferred.

## Root topology and typed refusal distinction

- `IndeterminateZeroFlows`: every cashflow is exactly zero, hence no
  single distinguished IRR is returned.
- `NoSignChange`: all nonzero cashflows share one sign, providing no
  positive-base NPV zero within the admitted rate domain.
- `PotentialMultipleRoots`: two or more sign transitions after zeros
  are discarded. This refusal is conservative and **does not certify
  an exact number of roots**; the example [-100,230,-132] with
  dates at 0,365,730 has two independently verified roots r=0.1 and 0.2.
- `RootNotBracketed`: the admitted one-sign-transition pattern has no
  sign bracket at the two requested endpoints. This is a *local*
  statement about the user-supplied interval, not a global claim that
  an IRR does not exist elsewhere.
- Invalid/missing/non-monotonic dates, rate bounds, input magnitude,
  unspecified/unknown day-count convention and pre-emptive cancellation
  return separate typed refusals.
- Every refusal returns **Value=null** and **ElapsedDays=null**, with
  no partial interest rate or cash amount and no implicit day-count
  metadata. Success exposes the actual day span and selected convention.

No certified global root classification, numerical stability theorem,
interval-arithmetic bound, physical rate accuracy or investment
suitability statement follows from this bounded computation.
The original T1 single-period APIs remain callable.

## Scientific falsification and local NuGet

Observe one independently compiled missing-public `M6W02C04Boundary64`
API RED **before** staging the sidecar implementation. A compiler error
is not RED evidence. After staging the source, require recovery and
**90 independent GREEN tests**, for a targeted total of 91.

Retain 904 prior scientific tests: 67 C04-T1, 79 C03-T2, 57 C03-T1,
107 S03-T2, 61 S03-T1, 69 S02-T2, 60 S02-T1, 53 S01-T2, 25 S01-T1
and 326 C01/C02. Run full scientific tests and four Math regression
projects, structural plan/native adoption and transfer architecture.

Pack one real local `1.1.0-preview.14` NuGet archive; inspect actual
net10 compiled DLL, nuspec identity and dependency absence, length and
SHA-256. Independently restore **seven** pinned PackageReference-only
source-free eight-probe consumers into isolated caches using only a
cleared local package feed: new C04-T2 dated finance, retained C04-T1
pure finance, C03-T2 boundary, C03-T1 direct composition, S03-T2 uniform
grid, S02-T2 direct signal and C02-T2 special-function boundaries.
All seven receipts must match the inspected actual archive SHA and bytes.

## Closure

After durable RS102 scientific evidence close only C04-T2-R/G/V,
C04-T2 and parent C04. C05 optional adoption/quality gate, work
package M6-W02 and milestone M6 remain unfinished. C05 must not
be silently marked accepted by this mathematical delivery.
