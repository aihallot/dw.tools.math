# M8-W02-C01 — .NET 11 / C# 15 migration evidence and unresolved gates

Status: **audit evidence, not migration approval**. Evidence reviewed through RS251 on 2026-11-18. This document is scoped to the thirteen packable Math libraries, not sibling repositories or AURA exposure.

## Current production baseline

- `global.json`: SDK `10.0.401`, `latestPatch`, prerelease disabled.
- `Directory.Build.props`: `net10.0`, C# `14.0`, warnings as errors and deterministic outputs under `build/artifacts/`.
- No production SDK/TFM/compilation-setting migration is declared or performed by this audit.
- Existing private NuGet archives remain immutable. RS244-RS249 now provide bounded independent published-package binary evidence on .NET 11 for the three Quantities-family archives; this is not a universal package or migration certification.

## Observed Math evidence

- **RS214** inventories 13 packable libraries; **RS217/RS220** audit 16 direct `ProjectReference` edges, no graph cycles, and six direct references from `dw.tools.math.distribution`. Source: `docs/engineering/InspectM8Net11PackageGraph.cs`.
- **RS215** observes SDK `11.0.100`; **RS216** proves an isolated offline net11.0/C#15 compiler/runtime environment, not a Math-library build.
- **RS218** qualifies unchanged Foundation source; **RS219** qualifies unchanged exact Quantities source.
- **RS221** probes IR with Quantities; **RS222** probes Quantities Expression with Quantities; **RS223** probes Standard Units with Quantities and Expression.
- **RS224** probes Composition with IR and Quantities; **RS225** probes Codecs with IR and Quantities; **RS226** probes Numerics with IR and Quantities.
- **RS227** probes Statistics with Quantities; **RS228** probes Symbolics with IR and Quantities; **RS229** probes standalone Analysis; **RS230** probes standalone Scientific.
- These twelve distinct packable library families have *bounded source-level compile/run evidence*. The probes are distinct in scope, and a passed type-presence smoke test does **not** establish complete API or numerical/semantic behavior compatibility.
- **RS250-RS251:** the `dw.tools.math.distribution` seven-project, twelve-reference source graph compiled and ran with actual separate project boundaries under Windows and Ubuntu WSL Linux x64 SDK 11.0.100. This is not a published Distribution NuGet consumer.
- **RS231-RS239:** isolated exact arithmetic, parsing, unit conversion and temperature probes across net10/net11; source lexical scans are not semantic proof.
- **RS240-RS243:** WSL host and Quantities source consumers for arithmetic, temperatures and parsing. RS240-RS242 succeeded at attempt 001 only after one preselection refusal each; RS243 was first-pass.
- **RS244-RS249:** immutable, physically published NuGet binary consumers: `dw.quantities 20260.20261102.1001` on Windows net10/net11 and Linux net11; `dw.quantities.expression 20260.20261102.1001` with exact base dependency on Windows net11; and `dw.quantities.standard 20260.20261103.1001` with its two exact dependencies on Windows/Linux net11. SHA-256 checks and bounded original consumer fixtures are not full API or rollout certification.

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

1. **Distribution (source graph evidenced):** RS250-RS251 verify seven project assemblies and twelve references under Windows and Linux net11. The independent published Distribution binary consumer and full behavioral coverage remain unqualified.
2. **Published packages (partial):** RS244-RS249 verify three immutable Quantities-family packages and their dependency closures. Other Math archives, locked restore, rights/notices, broader API compatibility and rollback remain unqualified. No repacking or publication is authorized.
3. **Behavioral regression:** test exact, approximate, symbolic, serialization, units and numerical boundary contracts with independent oracles, especially the official rounding/complex/runtime deltas when applicable.
4. **Official compatibility disposition:** record applicable vs not-applicable findings per library, environment/RID support and downgrade/rollback plan before any SDK/TFM migration proposal.
5. **Product acceptance:** reconcile the canonical backlog and native DWF plan only from true qualifying evidence. `M8`, `M8-W02` and `M8-W02-C01` remain `not-ready`; no RED/GREEN/verification subtask or completion is implied.

This report is a bounded migration **assessment**, not an endorsement of immediate production migration, external AURA adoption, public release or a claim that all thirteen packages are qualified.

## RS252 reconciliation and explicit residual gates

Durable reports `.aura/workflow/reports/RS240` through `RS251`, not source files alone, support the facts above. All twelve runs succeeded at attempt 001; RS240-RS242 each recorded one preselection refusal and `firstPassSucceeded=false`, whereas RS243-RS251 had zero prior failures and `firstPassSucceeded=true`. The historical RS240-RS242 corrections must not be erased or described as first-pass success.

| Evidence | Qualified boundary | Remaining nonclaim |
| --- | --- | --- |
| RS240-RS243 | WSL host and isolated Quantities source behavior | No binary package certified by source tests |
| RS244-RS246 | Immutable Quantities DLL on Windows net10/net11 and Linux net11 | Not all API endpoints, deployment RIDs or downgrade |
| RS247 | Immutable Expression/base closure on Windows net11, six original checks | Not the Standard closure or all parser behavior |
| RS248-RS249 | Standard/Expression/base closure on Windows/Linux net11, ten original checks | Not every unit profile or domain adoption |
| RS250-RS251 | Seven-project Distribution source graph on Windows/Linux net11 | Not an immutable published Distribution binary |

**Next priority:** target independent locked restore and topology qualification for the distribution graph, then other physically published Math packages, numerical and serialization compatibility deltas, deployment platform requirements and rollback. Preserve production `global.json` at SDK 10.0.401 and `Directory.Build.props` at net10.0/C#14. `M8`, `M8-W02` and `M8-W02-C01` remain `not-ready`; no migration, product acceptance or public release is authorized by this reconciliation.

## RS314 — primary-source SDK baseline audit admission (2026-11-24)

Earlier evidence RS214-RS252 was recorded while M8-W02 was not-ready. RS313 later completed Discovery and admitted W02 to ready; RS314 starts the SDK audit T1 RED only. No historical report or source artifact is retrospectively rewritten.

Repository-observed baseline: global.json pins SDK 10.0.401 with latestPatch; Directory.Build.props supplies net10.0, C# 14.0, warnings-as-errors and build/artifacts output. The thirteen packable projects and eleven test projects are listed in dw.tools.math.slnx. Existing InspectM8Net11SourceBaseline.cs and InspectM8Net11PackageGraph.cs own the package roster and sixteen acyclic direct dependencies; the narrowly new InspectM8W02TestCoverage.cs qualifies the independent eleven-test-to-thirteen-package mapping and its twenty-three direct test edges.

| Packable source project | Direct test project | Direct source dependencies |
| --- | --- | ---: |
| `dw.quantities` | `dw.quantities.tests` | 0 |
| `dw.quantities.expression` | `dw.quantities.tests` | 1 |
| `dw.quantities.standard` | `dw.quantities.tests` | 2 |
| `dw.tools.math.analysis` | `dw.tools.math.analysis.tests` | 0 |
| `dw.tools.math.codecs` | `dw.tools.math.codecs.tests` | 1 |
| `dw.tools.math.composition` | `dw.tools.math.composition.tests` | 2 |
| `dw.tools.math.distribution` | `dw.tools.math.distribution.tests` | 6 |
| `dw.tools.math.foundation` | `dw.tools.math.foundation.tests` | 0 |
| `dw.tools.math.ir` | `dw.tools.math.ir.tests` | 1 |
| `dw.tools.math.numerics` | `dw.tools.math.numerics.tests` | 1 |
| `dw.tools.math.scientific` | `dw.tools.math.scientific.tests` | 0 |
| `dw.tools.math.statistics` | `dw.tools.math.statistics.tests` | 1 |
| `dw.tools.math.symbolics` | `dw.tools.math.symbolics.tests` | 1 |

Official Microsoft delta anchors for subsequent GREEN disposition: [.NET 11 release overview](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/overview), [.NET 11 breaking changes](https://learn.microsoft.com/en-us/dotnet/core/compatibility/11), [C# 15 changes](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15), and [global.json SDK selection](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json). SDK selection and target framework selection are distinct. A successful historical net11 source-graph smoke test does not certify every public API, numerical corner case, test suite or runtime deployment platform.

Unqualified gates: per-project official API delta applicability, complete net11 test and package graph compatibility, CPU/RID constraints, locked restore behavior, bounded migration options and rollback. This RED entry does not update production SDK/TFM, repack old archives, authorize external brokers or claim M8-W02/C01 acceptance.

## RS315 — SDK baseline RED completion and GREEN readiness

RS314 succeeded first-pass on allocated attempt 001 with no prior refusals. The existing source baseline, thirteen-project dependency graph, stable SDK 11 inventory and historical RS240-RS251 reconciliation validators all passed. The new eleven-test coverage inventory checked twenty-three direct references covering all thirteen packable Math libraries; deliberately removing the unique analysis-test owner from the in-memory projection was rejected. RS315 completes the RED inventory leaf and merely makes the official API/toolchain inventory GREEN leaf ready. This is not a new .NET 11 or C# 15 migration, change to production global.json or Directory.Build.props, full behavioral certification, signed provenance or package publication. Subsequent work must trace Microsoft changes to each actual Math project and independent test evidence without claiming every suggested language feature is applicable.

## RS316 — candidate-only official API delta matrix GREEN entry

RS315 succeeded first-pass at attempt 001, closing SDK baseline RED and readying official SDK/toolchain GREEN. RS316 activates GREEN only and creates [the candidate disposition matrix](m8-w02-c01-official-delta-candidates-rs316.json) from the earlier nine-candidate [research proposal](../planning/proposals/net11-numerics-impact-matrix.json), not from hypothetical compilation success. All thirteen packable Math libraries and their direct test owners are represented; nine prior candidate IDs are preserved with exact owner links and four official Microsoft sources. Each row explicitly stays *unqualified* and names a needed independent oracle. A missing proposal candidate means 'not enumerated', **not** 'inapplicable'.

The existing numeric lexical scanner and the new matrix validator must both pass. Lexical Math.Round, BigInteger, Complex and conversion matches cannot assert which overloads are reached or prove numerical compatibility. The official [.NET 11 libraries](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/libraries), [.NET 11 compatibility catalog](https://learn.microsoft.com/en-us/dotnet/core/compatibility/11), [C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15) and [global.json](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json) references delimit further investigation. T1 GREEN remains in-progress; the baseline SDK 10.0.401/net10.0/C#14 is preserved, and no migration, full test certification or publication is authorized. See [RS316 decision](../planning/decisions/m8-w02-c01-t1-green-official-delta-matrix-rs316.md).

## RS317 — candidate-only official delta GREEN closure; Verification ready

RS316 succeeded first-pass at allocated attempt 001 without previous failures or mutation-boundary violations. The [candidate matrix](m8-w02-c01-official-delta-candidates-rs316.json) preserves all thirteen packable projects, eleven direct test owners and exactly nine historical research IDs against four Microsoft source categories. The dedicated validator rejected missing project and forged proposal-ID counterexamples, and the existing lexical API scan, test coverage, baseline SDK and native plan validations passed. RS317 completes only the **inventory/disposition GREEN leaf**, not compatibility certification, and readies T1 independent Verification for a future run. All rows remain `unqualified`; original SDK 10.0.401/net10.0/C#14, immutable NuGet archives and external authority boundaries remain unchanged.

## RS318 — independent copied-consumer Verification, not compatibility approval

RS317 succeeded first-pass, completing SDK delta candidate **inventory GREEN** and readying Verification. RS318 tests the existing thirteen-package/nine-proposal/four-Microsoft-source matrix verifier from an isolated copy of 29 Math-owned input files. It requires positive acceptance plus separate rejections of fabricated proposal risk IDs, missing packable source rows, compatibility promoted to `qualified`, and mismatched direct test ownership; it restores each copy before a fresh positive check and compares original input hashes afterward. This is an external-copy verification of source mapping and authorization-neutral documentation, not proof that any nine candidates actually apply to the implementation or that all thirteen libraries behave identically under .NET 11. Production remains SDK 10.0.401/net10.0/C#14; older archive contents, SDK pins, sibling projects and broker authority are unchanged. T1 Verification remains in-progress pending a separate closure decision.
