# M3-W02-C04-T1 — isolated Math consumers and primitive proposals

**Status:** RS050 proposal, not a delivered external integration. Qualification requires the actual RS050 DWF success receipt and corresponding Math-owned evidence. A Math-local consumer demonstrates package usability, not Decision or MCDM adoption.

## Question: what can an external product safely reuse?

Can a .NET 10 executable with **only local Math PackageReference dependencies** preserve exact-versus-approximate arithmetic, named quantile conventions, typed correlation outcomes and reproducible sampling, while refusing lossy scale factors rather than changing downstream Decision/MCDM business semantics?

The C04-T1 accepted question is explicitly about mathematical primitives, not VaR, CVaR, utility, ranking, risk preferences, integer optimization or policy orchestration. Neither source transfer nor external repository changes are authorized here.

## Observation: bounded independent consumer

The C04 sample lives under tests/consumers/m3-w02-c04/. It references only dw.quantities 0.2.0-preview.1 and dw.tools.math.statistics 0.4.0-preview.6 as NuGet packages. It has **no ProjectReference** or AURA/Decision/MCDM provider dependency. Its NuGet.Config explicitly clears outside sources. The RS050 payload builds two qualified Math products, packs them to a single local feed under build/artifacts/, restores the independent consumer exclusively from that feed, and runs a deterministic executable acceptance marker.

The isolated consumer contains 31 named assertions: loss-free scaling, refused fractional/overflow scaling, exact arithmetic and aggregate results, distinct exact and finite quantile conventions, consumer-owned convention-carrying statistical snapshot, compensated sums, population/sample variance, defined and undefined Pearson results, explicit missing-pair counts, Bernoulli support limits, independent PRNG seed-zero golden word, strict checkpoint replay, bounded half-open uniform sample and source assembly dependency isolation.

Concrete independent acceptance examples:
- Consumer-local TryScaleExactly multiplies an ExactRational by a positive integer factor and refuses a nonintegral or out-of-Int64 result; 3/4 times 100 yields 75, -3/4 yields -75, but 1/3 times 100 refuses without silent rounding. This is an **example adapter inside the consumer**, not an added Math public API nor a validated replacement of Decision ExactDecimalScaler.
- Exact 1/3 plus 1/6 is 1/2; exact mean of the same pair is 1/4. Compensated finite [10^16, 1, -10^16] retains the small unit but remains an explicitly approximate IEEE binary64 computation.
- For [0,10] at p=1/4, exact Type-7 returns 5/2, while nearest rank returns 0. Finite Type-7 returns 2.5, finite nearest rank 0. A consumer snapshot stores the QuantileConvention64 identity next to the output; convention cannot be inferred from the observed number alone.
- Increasing affine Pearson is defined and approximately 1; a constant axis reports UndefinedZeroVariance and a nullable Pearson, **not numeric zero**. SkipPair requires explicit selection.
- Bernoulli p=0/p=1 support and one-draw behavior, SplitMix64/v1 seed-zero first word, strict versioned checkpoint and finite half-open uniform sampling remain bounded mathematical primitives. No cryptographic or statistical inference semantics are attached.

On successful RS050 validation, the evidence JSON records actual package SHA-256s, package ID/version/dependency manifests, .NET SDK and observed RID, direct package-only source contract, exact consumer output marker and probe count. This is local-package independence; it is not evidence of a sibling checkout or public feed.

## Decision: propose primitives; preserve recipient authority

The bounded Math-facing primitives are viable **candidates** for owner review:
- Decision may investigate exact rationals and explicit scale refusals for ExactDecimalScaler, defined quantile policy for generic risk calculations, and versioned random streams for simulations. Its existing 1.x contract, decision-specific VaR/CVaR/EVPI, MDP and solvers remain owned by Decision.
- MCDM may consider finite compensated sums and generic numeric contracts, but must preserve ties, quantization, PROMETHEE, criterion normalization and rank stability under its own independent tests.

Existing coordination drafts MATH-XR-002-decision.md and MATH-XR-003-mcdm.md remain **unsent**. Math does not rewrite their expected recipient decisions, invent an adoption commit, or claim equivalent historical numerical results. Before a request is transmitted, its owner must review package SHA-256s, target consumer signatures, baseline tests, policy differences, rollback and requested acceptance evidence. Neutral dw.discovery and AURA are separate future responsibilities, not transitive qualifications from this consumer smoke.

## Unresolved boundaries and T2

The C04-T1 result is a documented Math-local spike plus an isolated executable; it does **not** complete C04, M3-W02 or M3. T2 must review concrete recipient ownership boundaries and future request content, and distinguish draft requests from accepted, tested implementation. No cross-repository code change, public feed publication, compiled external Decision/MCDM adapter, cryptographic PRNG or automatically generated agent skill is delivered by T1.

**Review trigger:** an owning consumer provides exact signature/API incompatibility, observed business-oracle drift, a different quantile convention, a package compatibility conflict or a verifiable shared dependency needing a coordinated versioned adoption.
