# MATH-XR-002 — Shared primitives with Decision

Status: draft. Recipient: Decision agent. Not sent.
Trigger: M3-W02-C04; additions in M6-W02-C05.
Package/version/hash/feed and evidence: TO PROVIDE before transmission.

Inside Decision only, assess whether qualified Math primitives can replace part of ExactDecimalScaler.cs, Simulation.cs (PRNG/distributions), and RiskAndRobustness.cs (generic quantiles) without changing semantics.
Compare signatures, exceptions, units, quantile conventions, PRNG sequences, and replay guarantees. Existing 1.x APIs remain protected: keep a facade or explicitly propose a major version when necessary.
Keep EVPI, VaR/CVaR as risk policies, robustness, decision Monte Carlo, MDP, graphs, LP/MIP, CP-SAT, SMT, and games.
Replace nothing without concrete duplication and independent evidence. Qualify loss/overflow-free scaling, PRNG golden vectors, and quantiles; preserve historical results.
Respond with decision, affected files, cutover/rollback plan, commit and tests if implemented. Requests for new primitives go to Math; this run never develops them inside the Math repository.
