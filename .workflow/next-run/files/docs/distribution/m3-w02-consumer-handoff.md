# Math M3-W02 candidate consumer handoff — not an adoption receipt

This file documents a Math-only preparation outcome for C04-T2. It becomes a qualified planning decision only after the RS051 terminal report and docs/planning/evidence/M3-W02-C04-boundaries-qualified.json exist and agree. The package-only executable and 31 direct mathematical checks were actually qualified in RS050, but **neither Decision nor MCDM has been shown to consume these packages**.

## What can be evaluated

The protected Math baseline is:
- dw.quantities 0.2.0-preview.1; RS050 observed local nupkg SHA-256 f5b0085ebc2540a519a33b9f7af6b19dfcbe06e2d7ad1064f388bb8367fadaf5.
- dw.tools.math.statistics 0.4.0-preview.6; RS050 observed local nupkg SHA-256 b27fc5697235ed111897d634e573ae4ebbb92919b9772f0292d6088903ef1fb6.
- SDK 10.0.402 on win-x64, exact consumer output: dw.tools.math/c04-isolated-consumer/0.4 qualified; probes=31.

The local-only packages are not a public download or an accessible recipient feed; the receiving owner must independently pin any actual transferred/rebuilt bits.

## Owner-specific candidate requests

- Decision: see docs/coordination/requests/MATH-XR-002-decision.md. The owner can evaluate exact rational scaling, explicit Type-7 versus nearest-rank quantiles, and versioned deterministic sampling. VaR/CVaR, EVPI, MDP, games, decision solvers and risk interpretation remain Decision-only.
- MCDM: see docs/coordination/requests/MATH-XR-003-mcdm.md. The owner can evaluate a compensated sum only while preserving tie/rank, FlowQuantization, criterion normalization, PROMETHEE and preferences. Generic matrix migration is held without a separately qualified product package/consumer artifact.
- AURA, agent discovery and invocability: no candidate in this handoff. Calling a Math method through NuGet is not an agent skill, neutral protocol descriptor, authorization decision or BrokerReady action.

The structured matrix docs/coordination/matrices/m3-decision-mcdm-primitive-boundaries.json represents 4 candidate reviews, 1 hold and 3 explicitly retained external scopes. None is adopted, transmitted or tested in a recipient repository.

## Safety and delivery gate

The Math-owned validator docs/planning/ValidateC04ConsumerBoundaries.cs checks pinned RS050 proof, local SHA-256 metadata, owner/request boundaries and the absence of fabricated adoption. It does not recalculate workflow hashes and does not contact external services. The RS051 DWF payload qualifies these *documents*, updates project/native statuses with exact achievements, and retires the completed M3-W02 active cursor. It must not mark M3 released or send the requests.

A later owner-side decision requires source revision, actual transferred package hashes and manifests, signature comparison, regression runs with domain baselines, change controls, rollback and a response receipt. The release gate G-M3 is an independent Math follow-up.
