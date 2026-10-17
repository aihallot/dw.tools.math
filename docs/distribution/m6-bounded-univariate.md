# Bounded univariate numerical analysis (local preview.3)

Package: dw.tools.math.analysis 1.1.0-preview.3, net10, BCL only.

\`BoundedUnivariateAnalysis64.SolveBracketed(function, lower, upper,
optionalPolicy, cancellationToken)\` uses endpoint sign checks and
bisection. RootPolicy64 requests absolute X and residual thresholds
(default 1e-10 each) with at most 128 iterations (hard limit 256).
Result properties disclose root, actual |f(root)|, observed bracket
width, iteration/evaluation counts. This is not a certified root error
bound and cannot validate caller function continuity or uniqueness.

\`BoundedUnivariateAnalysis64.IntegrateSimpson(function, lower, upper,
optionalPolicy, cancellationToken)\` uses composite Simpson refinement
with even power-of-two panels, default <=128 (hard limit 256), and at
most 1025 callback evaluations. The reported |S(2n)-S(n)|/15 is a
heuristic smooth-case absolute-error estimate; it is not proven for
discontinuous or otherwise irregular functions.

Core independent references: f(x)=x²-2 on [1,2] yields sqrt(2) within
the observed tolerance; integral x² from 0 to 1 approximates 1/3.
Null, invalid domains, invalid budgets, exceptions, NaN/Infinity,
missing sign bracket and nonconvergence return typed refusal without
partial numerical values.

The local qualified evidence only covers the executing SDK/RID and
the named scientific oracles. T2 must independently characterize
discontinuity, nonconvergence and numerical budgets. There is no
release, provider, multi-root, agent, Decision or MCDM adoption claim.
