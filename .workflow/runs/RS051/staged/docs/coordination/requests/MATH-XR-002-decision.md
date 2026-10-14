# MATH-XR-002 — Shared mathematical primitive review in Decision

Status: draft. Recipient: Decision agent. Not sent.
Transmission: none. Approval: not requested; adoption: not started.
Trigger: Math M3-W02-C04-T2; any additional math primitives require a separately accepted future scope.
Source baseline: aihallot/dw.tools.math main at 9bbbdde4f0c8a2df30f585f5b1a20e5f6d514cc3 (RS050 successful attempt 1).
Math product evidence: docs/planning/evidence/M3-W02-C04-consumer-qualified.json.
Candidate matrix: docs/coordination/matrices/m3-decision-mcdm-primitive-boundaries.json.

## Observed source-package qualification (Math only)

- dw.quantities / 0.2.0-preview.1 — local .nupkg SHA-256 f5b0085ebc2540a519a33b9f7af6b19dfcbe06e2d7ad1064f388bb8367fadaf5.
- dw.tools.math.statistics / 0.4.0-preview.6 — local .nupkg SHA-256 b27fc5697235ed111897d634e573ae4ebbb92919b9772f0292d6088903ef1fb6; depends on dw.quantities.
- .NET 10 SDK 10.0.402, observed win-x64, 31 Math-local independent package-only consumer probes.
- Artifact availability: LOCAL-ONLY in the source execution workspace under build/artifacts/m3-w02-c04-local-feed. No public feed, downloadable release, Decision transfer, installation or recipient package restoration was observed. These SHA-256s identify the exact RS050 build artifacts only; any separately rebuilt or transferred binaries require their own integrity verification. A Git commit alone does not make the .nupkg obtainable by the recipient.

## Question for the owning Decision agent

At a pinned, recipient-observed Decision revision, is there real, semantically duplicate generic mathematics that could be replaced or wrapped without changing the published Decision 1.x behavior?

The indicative source candidates are ExactDecimalScaler.cs, Simulation.cs and RiskAndRobustness.cs; they are not verified current call-site inventories or a command to replace them.

1. Exact scaling: evaluate Math ExactRational for an explicit lossless rational-to-scaled-integer adapter. The Math C04 sample is **consumer-local**: 3/4 multiplied by 100 gives 75; 1/3 multiplied by 100 refuses a nonintegral result; overflow outside Int64 refuses. That sample is not a new public Math exact-decimal-scaling API.
2. Quantiles: compare named Math Type7 and NearestRank with Decision's actual VaR/CVaR/robustness sign, tails, interpolation and missing conventions. On sorted [0,10] at p=1/4, exact Type7 gives 5/2 and NearestRank gives 0. No Math quantile implies a business risk policy.
3. Simulation: compare versioned SplitMix64/v1 high-53-bit draws, seed/counter checkpoints and Bernoulli/uniform boundaries against Decision's actual PRNG, replay and Monte Carlo observations. A deterministic new stream that differs from the historical stream is a compatibility difference, not a transparent migration. SplitMix64 is NOT cryptographically secure.

## Protected recipient boundaries

Decision retains EVPI, VaR/CVaR, risk/robustness policy, preferences, simulation meaning, MDP, games, graphs, LP/MIP, CP-SAT, SMT and solver orchestration. Math may own only generic numerical operations with explicit independent tests. Existing Decision 1.x contracts, exceptions, serialized data, CLI/agent outputs and scientific results are protected. If preserving those requires a facade, use a facade; if breaking compatibility is unavoidable, propose a major version with an explicit owner decision.

## Decision-owner verification gate

The owner must first provide: current Decision commit, exact target signatures and real call-site inventory, separately obtainable Math artifacts and their SHA-256s, parameter/return/error semantics, typed units and precision, historical oracles, and executable acceptance/rollback commands.

The owning agent must run lossless-scaling and overflow tests; existing VaR/CVaR and missing-value golden vectors; seed 0/1/42 PRNG and replay checks alongside current Decision simulation histories; relevant risk, MDP and solver regression suites; and package/adaptor compatibility checks. Record changed results rather than overriding historical truth. Document the proposed cutover, fallback, source retention, and when duplicate removal is permissible.

Requested response: accepted / rejected / needs_change, rationale, reviewed Decision revision, exact files/symbols, observed baseline and changed oracles, package IDs/versions/actual accessible artifact hashes, tests with results, interface/data/skill impact, rollback state and own result commit (only if implementation was authorized and actually published).

**Do not send automatically.** Drafting this request is neither recipient acknowledgement nor implementation. No cross-repository code change is authorized by this Math run. Transmission requires the owner's explicit decision and a practical artifact transfer channel.
