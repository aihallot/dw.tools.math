# M6-W02-C02-T2 — bounded special-function and branch audits

## Decision

Keep the qualified 1.1.0-preview.3 BCL-managed T1 provider and facade
unchanged. Add SpecialFunctionBoundaryAudit64 in the local provider-neutral
dw.tools.math.scientific 1.1.0-preview.4 package.

Trigonometric real input now requires the caller to supply Radians or Degrees.
Degrees are approximately multiplied by pi/180 and then passed to the
same qualified managed radian provider. This is not an exact rational
conversion or a new arbitrary-precision approximation. After conversion,
the original finite real argument policy still applies: at most 20 radians
by default; an opt-in narrower limit is honored. A nonzero subnormal
degree input that rounds to zero radians refuses UnderflowRisk rather
than claiming a trustworthy zero angle. Other functions require the
NotApplicable angle marker; supplying a fictitious unit is invalid policy.

The original Gamma(x) approximation is restricted to positive real
0.5 <= x <= 20. Exact nonpositive integers are now classified as
GammaPole with null output, while other negative or positive out-of-subset
arguments retain typed refusal. Complex Gamma remains unsupported, even
at a positive real complex argument. Pole reporting is a domain refusal,
not a computation of Gamma by analytic continuation or proof of error.

Complex logarithms use the managed BCL principal branch. The negative
real-axis cut is refused by default as BranchCutRequiresOptIn; callers
may explicitly choose CallerAcceptsPrincipalCut to observe log(-1+0i)
approximately equal to +pi*i. Inputs on the upper and lower side of
the cut produce positive/negative principal phases respectively.
No alternative logarithm branch or choice of imaginary winding number
is implemented, and no branch continuity guarantee is implied.

Exponential arguments beyond binary64 overflow (~709) or underflow
(~-745) thresholds refuse before calling T1. These are conservative
prechecks for risk, not measured overflow/underflow. Other values
beyond T1's much smaller admitted real +/-20 or complex component
+/-12 bounds still refuse OutsideQualifiedRange. No extended domain
is silently admitted.

Every rejection carries a typed status and null number. Admitted
values disclose the managed provider and rule and never claim a
certified absolute or relative error bound.

## Falsification and boundaries

One compiled reflection RED observes the absent public boundary
API before staging the implementation. Recover the RED and pass
87 independent GREEN scientific oracles, 80 retained C02 T1 tests,
70 retained C01 T1 and 88 retained C01 T2 tests, plus the complete
scientific test suite and four core Math regression projects.

Pack the actual BCL-only net10 local 1.1.0-preview.4 NuGet archive,
inspect assembly/nuspec/SHA-256, then restore two separate pinned
source-free eight-probe consumers (new boundary and retained T1) from
the same isolated feed and compare actual archive bytes/digests.

This closes C02-T2 and C02 only. M6-W02 and M6 remain in progress.
C03 geometry/signal/series remains planned and its L-size split
requirement is not bypassed. No public stable package, unobserved
platform, CAS, external provider adoption or AURA/Decision/MCDM
integration is certified.
