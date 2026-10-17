# M6-W01-C02-T1 — bounded binary64 root and quadrature contract

## Independent scientific decision

Expose a separate \`BoundedUnivariateAnalysis64\` API from the provider-free
\`dw.tools.math.analysis\` net10 package, locally versioned 1.1.0-preview.3.
Do not rewrite the qualified curve/interpolation/OLS APIs.

Bracketed bisection on a finite, strictly increasing real interval requires
opposite endpoint signs (or an exact zero endpoint) and evaluates the
caller-supplied real callback with a bounded iteration budget.
Admitted results expose actual residual, observed bracket width,
evaluation count and requested policy. A sign-changing bracket alone
does **not** establish continuity, uniqueness, or a mathematically certified
root-error interval: GuaranteedRootError is always false.

Composite Simpson quadrature evaluates bounded even power-of-two panel
counts and reports a smooth-case refinement estimate
|S(2n)-S(n)|/15. That quantity is **not** a general error bound; it relies
on unverified regularity and can miss a discontinuity. The result explicitly
sets ErrorBoundGuaranteed=false. Caller functions must be deterministic,
finite and sufficiently regular for the advertised numerical use.

Inputs, budget, policy, callback exceptions, cancellation, missing brackets,
nonfinite intermediates and nonconvergence have typed refusals with no
partial integral or root number. Out-of-scope general discontinuity proof,
verified intervals, adaptive Gauss-Kronrod, arbitrary precision and multi-root
methods remain for T2/future decisions.

## Reproducible falsification

Observe one genuinely compiled absent-public-contract reflection RED before
staging the new product source. Recover that RED and pass all new independently
asserted GREEN MSTests plus retained C01 T1/T2 analysis tests. Build locked
Release with .NET 10; run relevant numerics and composition regressions.

Create the actual source-free local 1.1.0-preview.3 nupkg, inspect SHA-256,
nuspec identity, target assembly and closed dependencies. Restore a distinct
eight-probe NuGet-only consumer from the same isolated feed and compare
identical package bytes and digest. Record the executing SDK, RID and limits.

RS083 closes only M6-W01-C02-T1 R/G/V and T1. M6-W01-C02, M6-W01 and
M6 stay in progress. No public/stable NuGet or other platform is admitted.
