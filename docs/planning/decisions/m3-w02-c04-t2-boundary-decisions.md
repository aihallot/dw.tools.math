# M3-W02-C04-T2 — recipient-boundary decision and proposed interoperability gates

**Status:** RS051 proposed Math-side decision. It is admitted as the completed C04-T2 planning/spike result only after its own DWF success, not simply because this document exists. No external Decision/MCDM repository has acknowledged, installed, tested or adopted these proposals.

## A — question and alternative decision

Question: which already qualified, product-neutral Math primitives may be **proposed** to downstream owners, and which business semantics must remain owned by Decision, MCDM and AURA?

The choices are:
1. **Recommended now:** maintain a bounded, evidence-backed unsent candidate matrix and two explicit requests. The actual consuming owners may accept, reject or request changes after inspecting current contracts and suitable artifacts.
2. **Rejected for now:** copy Math implementations or replace recipient sources based only on numeric resemblance or an isolated Math-owned consumer. It would erase unverified legacy semantics.
3. **Deferred:** offer a separate numerical matrix package or generic solver adapter without its own pinned package and recipient regression evidence. RS050 qualified only the two-package rational/statistics consumer.

Math owns raw arithmetic, explicit quantile conventions, typed correlation statuses, bounded sampling and versioned PRNG mechanics. Decision owns risk, preferences, Monte Carlo scenario meaning, VaR/CVaR, EVPI, MDP, graphs, games and solver policy. MCDM owns PROMETHEE, FlowQuantization, criterion normalization, pairwise preference meaning, rank and tie handling. AURA owns invocation and broker authorization. Neutral dw.discovery attributes/schema/skill generation remain a separate module and must never be inferred from Math package-only executability.

## B — observed evidence and candidate checks

The independently executed RS050 consumer proved 31 Math-local oracles with pinned NuGet-only package references. The authoritative receipt is docs/planning/evidence/M3-W02-C04-consumer-qualified.json, source tip 9bbbdde4f0c8a2df30f585f5b1a20e5f6d514cc3. The actual qualifying environment was .NET SDK 10.0.402 on win-x64.

Two observed LOCAL-ONLY package artifacts:
- dw.quantities 0.2.0-preview.1, SHA-256 f5b0085ebc2540a519a33b9f7af6b19dfcbe06e2d7ad1064f388bb8367fadaf5.
- dw.tools.math.statistics 0.4.0-preview.6, SHA-256 b27fc5697235ed111897d634e573ae4ebbb92919b9772f0292d6088903ef1fb6, with dependency on dw.quantities.

These hashes identify exactly the observed packages; rebuilding them with another SDK may change bytes. Their placement in the Math user's local build/artifacts feed is **not** a transferred package, published feed, sibling compatibility test or available download. The receiving agent must independently hash and verify the actual artifact they obtain; the package IDs alone are not sufficient.

The structured matrix in docs/coordination/matrices/m3-decision-mcdm-primitive-boundaries.json records eight concrete ownership cases:
- Four **candidate** primitive evaluations: Decision exact scale, Decision quantile, Decision deterministic sampling, MCDM compensated summation.
- One **hold**: MCDM generic matrix comparison pending pinned numerical-package availability and owner-scoped tests.
- Three **retained_external** scopes: Decision risk/EVPI/MDP/solvers, MCDM preference/PROMETHEE/FlowQuantization/rank, AURA and neutral discovery/broker surfaces.

In every row, current recipient revision remains unverified, recipient tests remain not_executed, owner decision remains pending, transmission is none and adoption is not_started. The Math-side validator checks exactly those states, the two pinned package SHA-256s and observed local T1 evidence. These are nonadoption checks, not a surrogate external test suite.

### Concrete equality/inequality boundaries

- **Scaling:** consumer-local ExactRational(3/4)*100 yields integer 75; ExactRational(1/3)*100 refuses a lossy integer. This helper is NOT exported by Math. Decision decides the error contract, decimal scale, overflow and legacy compatibility.
- **Quantile:** sorted [0,10] at p=1/4 gives Type7=5/2, NearestRank=0; a Decision percentile or VaR/CVaR policy cannot silently select one.
- **Correlation:** an exactly constant axis has null Pearson and typed UndefinedZeroVariance; an observed Pearson of zero on nonconstant axes is defined and semantically different.
- **Simulation:** SplitMix64/v1 replays a pinned seeded stream; a new stream differs from any prior Decision PRNG unless independently proven. No cryptographic security is claimed.
- **MCDM aggregation:** compensated floating summation can change rounding, threshold crossing, ties and rankings. The recipient must prove equality of accepted business oracles and decide tolerance policy before altering a shared accumulation.
- **Matrices:** no C04 isolated consumer or NuGet artifact SHA supports a generic matrix cutover. Defer this branch until versioned package and recipient input/output contracts are actually observed.

## C — decision, transfer rules and response gate

Keep MATH-XR-002-decision.md and MATH-XR-003-mcdm.md as **draft, not sent**, with observed Math package identities, their actually recorded hashes, current scientific limits and concrete recipient-owned regression/rollback responsibilities. The Math run prepares **documents only** and does not initiate message transmission, change a sibling checkout, delete duplicated sources or imply a consumer acknowledgement.

Recipient workflow if owner later authorizes transmission:
1. Obtain an owner-approved accessible NuGet feed/transfer. Verify package manifests and SHA-256 of artifacts actually obtained; list the receiving revision, transitive dependencies and licenses/rights.
2. Compare current actual recipient signatures, exception/precision/unit policies, quantile definitions, draw counts, persisted schema and business oracles. Record source differences rather than assuming duplication.
3. On the recipient's separately authorized branch/run, test parallel/facade behavior and existing business-domain golden vectors; retain original implementations for rollback.
4. Return accepted / rejected / needs_change with reasoning, touched symbols, observed public/data/agent impacts, test commands/results, actual artifact hashes and recipient commit. Only the recipient may claim verified adoption. No automatic deadline or background sync is promised.

**Non-goals:** no direct Decision or MCDM implementation, historical result equivalence, release of nupkg to an external feed, normal-distribution provider, cryptographic random generator, risk or PROMETHEE ownership transfer, dw.discovery conformance or AURA BrokerReady admission.

**Closure:** When and only when RS051 passes its executed Math-side decision validator, planning validations and source/evidence reconciliation, native C04-T2 A/B/C, T2, C04 and M3-W02 may transition to done. Milestone M3 remains in progress until the independent release gate G-M3 is accepted. M4/M5 and discovery work are not activated by this document.

**Review triggers:** receiver reports an actual signature incompatibility or changed golden vectors; a reproducible package is distributed; recipient ranking changes; a Math numerics artifact is separately pinned; neutral dw.discovery protocol is installed and versioned; business owner accepts or rejects a submitted request.
