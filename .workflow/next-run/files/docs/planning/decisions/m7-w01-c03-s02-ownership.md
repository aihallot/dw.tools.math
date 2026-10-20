# M7 reciprocal PlotModel — finite data and rendering ownership screen

**Scope:** M7-W01-C03-S02-T1 local BCL research only. This is not a qualified chart, public PlotModel API, rendering runtime, UI framework, AURA workflow task or external NuGet provider.

## Independent reciprocal segment oracle

| Witness | Reference result | Contract boundary |
|---|---|---|
| Explicit samples | `[-2,-1,0,1,2]` | Four finite points, one undefined pole gap and **two separate segments** |
| Negative segment | `(-2,-0.5),(-1,-1)` | Do not interpolate into `x=0` |
| Positive segment | `(1,1),(2,0.5)` | Do not connect across `x=0` |
| Pole omitted | `[-2,-1,1,2]` | Still two segments, **one break**, zero sampled pole gaps |
| Exact cap | Eight samples `[-4,-3,-2,-1,1,2,3,4]` | **8 samples / 8 points / 2 segments** maximum for the research fixture |
| Over-cap | Nine samples | Refuse before numeric work, no partial result |
| Bad grid | Nonfinite abscissa, duplicate or descending samples, fewer than two samples | Refuse, no guessed ordering, no implicit sorting |
| Nonfinite reciprocal | `double.Epsilon` | Finite x with infinite `1/x` must refuse without exposing partial results |

The reference is `docs/planning/ValidateM7PlotSampling.cs`, a read-only, source-only **BCL** oracle. Its sole admitted discontinuity is the known `1/x` pole; other rational functions, unknown singularities, sampling errors and numeric conditioning remain unqualified.

## Responsibility matrix

| Responsibility | Owner decision | Missing qualification |
|---|---|---|
| Finite mathematical sample points and explicit segment-break metadata | May be researched by **Math** | Named consumer, immutable domain/grid contract, resources and source-free consumer |
| Computational DAG sharing and caching | Separate **S01**, deferred public API | No implicit graph runtime or new Math dependency |
| PlotModel public API, adaptive sampling and general discontinuity detection | **Not admitted** | Independent function/domain corpus, per-operation proof and budgets |
| Chart rendering, axes, labels, themes, UI/interaction, jobs and access policies | **AURA/client owner** | Explicit cross-owner request and acceptance, not a Math implementation |
| Third-party graphics or sampling engine, native/GPU provider | **Not admitted** | Immutable version/source/license/rights, net10.0/RID, offline smoke, rollback |

## T2 admission / deferral criteria

Require a demonstrated Math consumer, supported scalar/domain/function set, known discontinuities or segment topology, finite sample-point/segment/byte budgets, cancellation and typed failure/partial policies, deterministic evaluation order, proven no-bridge behavior, platform/source/licensing where a provider is involved, and an independent source-free consumer.

Without all relevant evidence, explicitly **defer** a public PlotModel or renderer with a documented re-entry trigger. Finite BCL examples are not numerical error bounds, a universal continuity detector or a UI rendering guarantee.

**Stop-loss:** one BCL read-only source oracle with two witness groups and local DWF structural validators. S02 T1 does not build an engine, add a dependency, run full solution tests or modify siblings.
