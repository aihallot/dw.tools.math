# M8-W02-C01 — .NET 11 / C# 15 migration evidence and unresolved gates

Status: **audit evidence, not migration approval**. Evidence reviewed 2026-11-16. This document is scoped to the thirteen packable Math libraries, not sibling repositories or AURA exposure.

## Current production baseline

- `global.json`: SDK `10.0.401`, `latestPatch`, prerelease disabled.
- `Directory.Build.props`: `net10.0`, C# `14.0`, warnings as errors and deterministic outputs under `build/artifacts/`.
- No production SDK/TFM/compilation-setting migration is declared or performed by this audit.
- Existing private NuGet archives remain immutable. Neither a source-only probe nor this document proves that a released archive works under a .NET 11 consumer.

## Observed Math evidence

- **RS214** inventories 13 packable libraries; **RS217/RS220** audit 16 direct `ProjectReference` edges, no graph cycles, and six direct references from `dw.tools.math.distribution`. Source: `docs/engineering/InspectM8Net11PackageGraph.cs`.
- **RS215** observes SDK `11.0.100`; **RS216** proves an isolated offline net11.0/C#15 compiler/runtime environment, not a Math-library build.
- **RS218** qualifies unchanged Foundation source; **RS219** qualifies unchanged exact Quantities source.
- **RS221** probes IR with Quantities; **RS222** probes Quantities Expression with Quantities; **RS223** probes Standard Units with Quantities and Expression.
- **RS224** probes Composition with IR and Quantities; **RS225** probes Codecs with IR and Quantities; **RS226** probes Numerics with IR and Quantities.
- **RS227** probes Statistics with Quantities; **RS228** probes Symbolics with IR and Quantities; **RS229** probes standalone Analysis; **RS230** probes standalone Scientific.
- These twelve distinct packable library families have *bounded source-level compile/run evidence*. The probes are distinct in scope, and a passed type-presence smoke test does **not** establish complete API or numerical/semantic behavior compatibility.
- **Unqualified assembly-graph source candidate:** `dw.tools.math.distribution`, the six-dependency consumer/aggregator. A prior graph check is not a .NET 11 build, package install, or runtime consumer qualification for this library.

## Official migration delta watchlist

The following are *candidate risks to investigate*, not claims that the Math code uses an affected API. Source identities are official Microsoft documentation; recheck each item against actual usage and test results.

| Area | Official .NET 11 / C# 15 change | Math-specific qualification needed |
| --- | --- | --- |
| Exact arithmetic | `Decimal` and `BigInteger` floating-point conversions are correctly rounded | Locate affected conversions; compare exact, boundary and rounding-policy fixtures under both runtimes |
| Numerical semantics | `Math.Round` and `MathF.Round` return correctly rounded results; JIT floating-point-to-small-integer conversions become saturating | Exercise declared numerical tolerances, rounding policies and overflow boundaries independently |
| Complex/numerical | `Complex` special-value behavior follows C23 Annex G | Check whether numerical/scientific implementations consume `Complex`, and test special NaN/infinite inputs if applicable |
| SDK and restore | Restore no longer searches global package cache for higher versions; additional SDK/MSBuild and NuGet behavioral changes are listed | Compare locked restore, feed identity, generated assets and exact packages before any SDK pin change |
| Runtime deployment | Minimum CPU instruction-set requirements change | Qualify target deployment machines and RID environments before shipping `net11.0` assemblies |
| Language | C# 15 adds union types, closed hierarchies, collection-expression arguments and other opt-in constructs | Compiler acceptance does not mandate syntax rewriting; preserve public API and serializer compatibility unless deliberately reviewed |

Official primary sources:
- .NET 11 breaking changes: https://learn.microsoft.com/en-us/dotnet/core/compatibility/11
- .NET 11 features: https://learn.microsoft.com/en-us/dotnet/core/whats-new/
- C# 15: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15

## Gates explicitly not satisfied by these probes

1. **Distribution:** independently compile/run the six-reference dependency closure under SDK 11.0.100 with genuine project boundaries; a flat source merge is not a replacement for the consumer topology.
2. **Published package contracts:** build/pack without overwriting immutable versions, install in independent net10/net11 consumers, verify transitive dependencies, rights, lock files and public API compatibility.
3. **Behavioral regression:** test exact, approximate, symbolic, serialization, units and numerical boundary contracts with independent oracles, especially the official rounding/complex/runtime deltas when applicable.
4. **Official compatibility disposition:** record applicable vs not-applicable findings per library, environment/RID support and downgrade/rollback plan before any SDK/TFM migration proposal.
5. **Product acceptance:** reconcile the canonical backlog and native DWF plan only from true qualifying evidence. `M8`, `M8-W02` and `M8-W02-C01` remain `not-ready`; no RED/GREEN/verification subtask or completion is implied.

This report is a bounded migration **assessment**, not an endorsement of immediate production migration, external AURA adoption, public release or a claim that all thirteen packages are qualified.
