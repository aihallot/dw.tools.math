# MATH-XR-004 — Assessa EG-A-01 unbiased bounded integer sampling

Status: **accepted consumer request for Math planning intake; no API implementation, binary delivery, or Assessa adoption qualified**.
Origin: `aihallot/dw.tools.assessa`, GENERATE-001 / EG-A-01 / T002; request received 2026-11-20.
Priority: **P0 consumer unblock**, ahead of optional additional .NET 11 migration package smoke tests, without displacing an already pending DWF run.
Ownership: Math owns reusable integer sampling; Assessa owns pedagogical recipes, profiles, answer protection, identifiers, sessions, and receipts.

## Verified starting point

- RS265 succeeded attempt 001 first-pass and qualified the **existing** immutable Scientific package under Ubuntu WSL net11. This does not deliver any bounded-integer API.
- `src/projects/dw.tools.math.statistics/SplitMix64Random.cs` exposes `splitmix64/v1`, `NextUInt64()`, `NextUnitInterval()`, the seed, consumed-word counter, `Snapshot()` and `FromSnapshot()`.
- `SplitMix64PositionCodec.cs` owns the canonical seed/counter checkpoint string.
- `M3-W02-C03` and `M3-W02` are durably done. Preserve that historical completion; **do not reopen or relabel them**.
- RS262–RS263 independently exercised the existing Statistics 18-check fixture with immutable `dw.tools.math.statistics 20260.20261106.1001` and `dw.quantities 20260.20261102.1001` packages on Windows/Linux net11. **Neither old archive contains a newly written bounded integer API**.

## Minimal new Math contract: acceptance for a new planned work item

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

## Priority sequence under DWF (one pending run at a time)

1. **RS266 planning intake (this run):** record the external request, verified existing reusable surfaces, explicit contract and consumer boundaries. This is not code or native backlog-node admission.
2. **Immediate next bounded run:** refine the *canonical* `docs/planning/backlog.json` and matching `.aura/workflow/plan/project.json` together, with the native adoption validator's fixed 580-node baseline and generated projections deliberately reconciled. Create a **new planned, Math-owned Statistics-extension work item**, not a retrospective change to done M3-W02-C03, nor an unrelated task in modernization M8-W02-C02.
3. Independent **source behavior** run(s): implement/version the minimal bounded transform if needed and qualify small-domain proof, force-rejection/budget behavior, checkpoint and existing SplitMix64 regressions.
4. Independent **package candidate** and **.NET 11 source-free consumer** run(s): prove exact new package identity and API via real `PackageReference`, without claiming publication based on source builds.
5. Separate **release authorization/publication** only if policy permits. Deliver exact package identifiers, versions, SHA-256, dependency closure, runtime evidence and failure boundaries to the Assessa owner. Only Assessa resumes and validates T002 / EG-A-01.

No user action besides `dwf run next` is required for planning intake. No task-level DWF progress/achievement or Assessa adoption is claimed.
