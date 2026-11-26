# MATH-XR-004 — Private bounded Statistics package handoff to Assessa

Status: **Math-owned package published and independently qualified; Assessa adoption pending**.
Consumer: `aihallot/dw.tools.assessa`, `GENERATE-001 / EG-A-01 / T002`. No Assessa repository edits are performed here.

## Exact published private identities

| Identity | Exact value |
| --- | --- |
| Package ID | `dw.tools.math.statistics` |
| Version | `20260.20261120.1001` |
| Feed | `NUGET_LOCAL` (private; no public registry publication) |
| Package TFM | `net10.0` |
| Physical archive SHA-256 | `feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1` |
| Packaged DLL SHA-256 | `684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea` |
| Dependency | `dw.quantities [20260.20261102.1001]` |
| Dependency archive SHA-256 | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` |
| Windows binary consumer | Windows `net11.0` / C# 15 / SDK `11.0.100`; source-free locked NuGet `PackageReference`; 12 bounded-sampling checks (RS273) |
| Linux/WSL binary consumer | Default WSL Linux distribution, `net11.0` / C# 15 / SDK `11.0.100`; source-free locked NuGet `PackageReference`; same 12 bounded-sampling checks (RS277) |

The prior published `dw.tools.math.statistics 20260.20261106.1001` is immutable and **does not** contain the new API. No version overwrite or repeat publication is authorized.

## Versioned Math API

```csharp
var rng = new SplitMix64Random(seed);
ulong value = rng.NextBoundedUInt64(exclusiveUpperBound, maxRawDraws);
long signedValue = rng.NextInt64Inclusive(lowerInclusive, upperInclusive, maxRawDraws);
var checkpoint = SplitMix64PositionCodec.Format(rng.Snapshot());
```

- Raw PRNG: `SplitMix64Random.AlgorithmId = "splitmix64/v1"`, original golden vectors and replay checkpoints unchanged.
- Bounded transform: `SplitMix64Random.BoundedTransformId = "splitmix64-bounded-rejection/v1"`.
- Exclusive bound `n > 0` yields `[0,n)`; inclusive signed interval accepts `lo <= hi` and the full 64-bit signed domain without overflow.
- Exact rejection threshold `(2^64 mod n)`; accepted residues have equal preimage counts assuming uniform raw 64-bit inputs. No cryptographic claim.
- Positive `maxRawDraws` includes **rejected** words; invalid arguments refuse before consuming; budget/counter exhaustion refuses with `InvalidOperationException`. Every raw rejection advances the shared snapshot counter.
- `SplitMix64PositionCodec` checkpoint replay remains versioned, deterministic and rejects incompatible algorithm identifiers.

## Durable qualification provenance

| Run | Verified responsibility |
| --- | --- |
| RS268 | Source API and exact bounded rejection oracles |
| RS269 | New unpublished NuGet candidate |
| RS270 | Independent source-free, package-only locked net11 consumer |
| RS271 | Private publication readiness, first-pass |
| RS272 | Authorized guarded publisher rehearsal, disposable feed |
| RS273 | **Actual NUGET_LOCAL physical readback and independent Windows net11 consumer, first-pass** |
| RS277 | **Independent Linux/WSL net11 source-free locked consumer of the same immutable published packages, first-pass** |

RS273 report: `.aura/workflow/reports/RS273/attempt-001.json`; `succeeded`, attempt `001`, `priorInvocationFailures.count=0`, `firstPassSucceeded=true`.
RS273 commit: `f31003c94f722cb0b15f1f4d4697002c3d36e788`.
Reader source: `docs/engineering/ProbeM9PublishedStatisticsReadback.cs`, `docs/engineering/VerifyM9BoundedStatisticsPrivateFeed.cs`, fixture `docs/engineering/fixtures/M9BoundedCandidateNet11Consumer.cs`.

RS277 report: `.aura/workflow/reports/RS277/attempt-001.json`; `succeeded`, attempt `001`, `priorInvocationFailures.count=0`, `firstPassSucceeded=true`, no mutation-boundary violations. RS277 result commit: `aed67c6e5b2a4ccceb87e9737a74bbf8e1f0ff6b`.
Linux/WSL probe: `docs/engineering/ProbeM9WslNet11PublishedBoundedStatistics.cs`. The default WSL Linux distribution independently restored the exact two-package closure with a locked `net11.0` `PackageReference`, validated the published archive and DLL SHA-256 identities, and passed the same twelve-check binary fixture. This extends platform evidence only: no rebuild, republication, Assessa adoption, or new M9 lifecycle completion is claimed. The Linux distribution name/version was not separately pinned by this probe.

## Assessa-owned acceptance gate

Pin the real version via private `PackageReference` and independently verify the selected DLL and lockfile. Reuse generic unbiased bounded selection in Assessa's `fraction-part-of-whole` and `fraction-compare` recipes. Consumer-specific bounds: F0 denominator 2–4, reference integer <=24; F2 denominator 2–24, LCM <=144. These are **Assessa limits**, not Math API limits. Never silently widen them. Assessa owns recipes, item generation, profiles, answer protection, sessions, identifiers, receipts and EG-A-01/T002 acceptance.

Use existing `ExactRational` for fraction operations only after separately verifying the relevant published dependency and zero-denominator contracts; the Statistics readback does not attest this distinct surface.

At the historical RS278 handoff, Math's M9 backlog and native plan states were planned/not-ready. Subsequent actual native RED/GREEN/Verification and parent closure runs closed M9 (RS364), then RS365 reconciled the canonical backlog. No retroactive task completion is inferred from the historical candidate or readback runs. No new publication or downstream adoption follows from M9 closure. Only the Assessa owner may claim GENERATE-001 / EG-A-01 / T002 successful adoption.
