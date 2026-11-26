# MATH-XR-004 — Assessa EG-A-01 unbiased bounded integer sampling

Status: **Math-owned bounded-integer API and private NuGet delivery independently qualified through RS273; Assessa EG-A-01 / T002 adoption not yet qualified**.
Current Math lifecycle: M9 is done in the native plan and canonical backlog following RS364/RS365. The immutable private delivery and downstream adoption boundary are unchanged. See [M9 reconciliation](../../planning/decisions/m9-bounded-sampling-canonical-reconciliation-rs365.md).

Origin: `aihallot/dw.tools.assessa`, GENERATE-001 / EG-A-01 / T002; request received 2026-11-20.
Priority: **P0 consumer unblock**, ahead of optional additional .NET 11 migration package smoke tests, without displacing an already pending DWF run.
Ownership: Math owns reusable integer sampling; Assessa owns pedagogical recipes, profiles, answer protection, identifiers, sessions, and receipts.

## Historical starting point (as of RS266 intake)

- RS265 succeeded attempt 001 first-pass and qualified the **existing** immutable Scientific package under Ubuntu WSL net11. This does not deliver any bounded-integer API.
- `src/projects/dw.tools.math.statistics/SplitMix64Random.cs` exposes `splitmix64/v1`, `NextUInt64()`, `NextUnitInterval()`, the seed, consumed-word counter, `Snapshot()` and `FromSnapshot()`.
- `SplitMix64PositionCodec.cs` owns the canonical seed/counter checkpoint string.
- `M3-W02-C03` and `M3-W02` are durably done. Preserve that historical completion; **do not reopen or relabel them**.
- RS262–RS263 independently exercised the existing Statistics 18-check fixture with immutable `dw.tools.math.statistics 20260.20261106.1001` and `dw.quantities 20260.20261102.1001` packages on Windows/Linux net11. **Neither old archive contains a newly written bounded integer API**.

## Accepted contract (historical intake requirements)

First inspect all existing Math public APIs and tests for an equivalent bounded-integer operation. Reuse if genuinely present; otherwise extend the **existing** SplitMix64 engine, never invent a second generator.

The public generic API must offer:
1. Unbiased draws in `[0, exclusiveUpperBound)` with a positive unsigned bound, and inclusive integer range selection with explicitly documented overflow-safe semantics.
2. A **versioned** bounded transformation, using exact 64-bit rejection sampling (for example threshold `(0UL - bound) % bound` under unchecked arithmetic, reject raw values below threshold). Simple modulo without rejection and floating-point integer selection are prohibited.
3. An explicit maximum raw-draw budget, including rejected words; a typed refusal on budget exhaustion, invalid bounds, exhausted PRNG counter, invalid range or incompatible checkpoint. Never silently extend the budget.
4. Replay by existing `splitmix64/v1` seed/counter checkpoint without altering existing vectors or canonical codec. Define transform-version compatibility independently of the PRNG; do not claim cryptographic quality.
5. Exact or independently computed oracles: bounds 0, 1, 2, 3, power-of-two, non-power-of-two and extreme limits; small inclusive ranges; forced rejection and consumed-word count; seeded goldens; checkpoint continuation and typed refusals. The reduction-bias proof must be analytic/exhaustive on a reduced domain, not only empirical frequencies.
6. Source regression verification, then a distinct **new** candidate package binary consumer under net11 with exact NuGet closure, version, SHA-256 and `PackageReference` only. Existing published packages remain immutable, and publication stays a separate gated decision.

Assessa usage constraints are **consumer test cases**, not Math types:
- `fraction-part-of-whole`, `fraction-compare`;
- F0 denominator 2–4, reference integer up to 24;
- F2 denominator 2–24, LCM at most 144.
Assessa must never receive a silently widened sampling range. Existing `ExactRational` fraction comparisons and zero-denominator refusals are to be reused and independently checked; no Study Buddy behavior is added to Math.

## Original priority sequence at intake (historical, now executed)

1. **RS266 planning intake (this run):** record the external request, verified existing reusable surfaces, explicit contract and consumer boundaries. This is not code or native backlog-node admission.
2. **Immediate next bounded run:** refine the *canonical* `docs/planning/backlog.json` and matching `.aura/workflow/plan/project.json` together, with the native adoption validator's fixed 580-node baseline and generated projections deliberately reconciled. Create a **new planned, Math-owned Statistics-extension work item**, not a retrospective change to done M3-W02-C03, nor an unrelated task in modernization M8-W02-C02.
3. Independent **source behavior** run(s): implement/version the minimal bounded transform if needed and qualify small-domain proof, force-rejection/budget behavior, checkpoint and existing SplitMix64 regressions.
4. Independent **package candidate** and **.NET 11 source-free consumer** run(s): prove exact new package identity and API via real `PackageReference`, without claiming publication based on source builds.
5. Separate **release authorization/publication** only if policy permits. Deliver exact package identifiers, versions, SHA-256, dependency closure, runtime evidence and failure boundaries to the Assessa owner. Only Assessa resumes and validates T002 / EG-A-01.

No user action besides `dwf run next` is required for planning intake. No task-level DWF progress/achievement or Assessa adoption is claimed.

## Delivered Math qualification — durable evidence as of RS274

The historical intake and plan above are retained for traceability; their present-tense
statements describe the original RS266 scope, **not** the current delivery state.

- **RS267:** canonical M9 Math roadmap admission, success attempt 001 after two pre-attempt refusals.
- **RS268:** the versioned `SplitMix64Random` rejection-based bounded sampling API, raw-word budget,
  exact reduced-domain oracle, forced rejection, overflow-safe inclusive ranges and checkpoint replay.
- **RS269:** versioned unpublished NuGet candidate, followed by **RS270** independent
  source-free Windows .NET 11 / SDK 11.0.100 package-only consumer with twelve checks.
- **RS271:** first-pass private release readiness.
- **RS272:** controlled private publisher rehearsal with independent binary check; actual private
  publication was separately authorized and invoked afterward.
- **RS273:** **first-pass** physical `NUGET_LOCAL` readback, exact SHA-256 identities, immutable
  earlier archives, locked .NET 11 package-only consumer and twelve checks.
- **RS274:** delivered the Math-owned Assessa handover,
  `docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md`.

Published private identities:
- `dw.tools.math.statistics 20260.20261120.1001`
- Physical archive SHA-256: `feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1`
- Packaged DLL SHA-256: `684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea`
- Exact dependency: `dw.quantities [20260.20261102.1001]`, SHA-256
  `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`.
- Durable release evidence: `.aura/workflow/reports/RS273/attempt-001.json`,
  GitHub commit `f31003c94f722cb0b15f1f4d4697002c3d36e788`.
- Durable delivery evidence: `.aura/workflow/reports/RS274/attempt-001.json`,
  GitHub commit `06590d4fe6183b7f0c3628c68c627a68659f9524`.

**Owner boundary:** Assessa must select the new private package version and independently
qualify `GENERATE-001 / EG-A-01 / T002`, including F0/F2 domain-specific range limits
and any separate `ExactRational` dependency. Math does not claim that this downstream
adoption occurred. At RS274 the accepted M9 backlog was `planned` and the native DWF plan was `not-ready`. Those historical states were superseded by the subsequent controlled M9 lifecycle and the durable RS364/RS365 closure and reconciliation.
