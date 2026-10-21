# RS127 — thirteen-package current local snapshot and source-free consumer matrix

The RS127 objective is **all declared product packages freshly packed on one observed host**, plus two current local source-free consumers. This is not a public NuGet release or universal ABI qualification.

## Complete package graph, as declared in current source

| Package ID | Source version | Direct Math dependencies |
|---|---|---|
| `dw.tools.math.foundation` | `0.1.0-preview.1` | none |
| `dw.quantities` | `0.2.0-preview.1` | none |
| `dw.quantities.standard` | `0.2.0-preview.1` | `dw.quantities`, `dw.quantities.expression` |
| `dw.quantities.expression` | `0.2.0-preview.1` | `dw.quantities` |
| `dw.tools.math.ir` | `0.3.0-preview.1` | `dw.quantities` |
| `dw.tools.math.composition` | `0.3.0-preview.1` | `dw.quantities`, `dw.tools.math.ir` |
| `dw.tools.math.numerics` | `0.4.0-preview.4` | `dw.tools.math.ir` |
| `dw.tools.math.statistics` | `0.4.0-preview.6` | `dw.quantities` |
| `dw.tools.math.symbolics` | `0.5.0-preview.10` | `dw.tools.math.ir` |
| `dw.tools.math.codecs` | `1.0.0-preview.8` | `dw.tools.math.ir` |
| `dw.tools.math.distribution` | `1.0.0-preview.4` | `dw.quantities`, IR, codecs, symbolics, numerics, composition |
| `dw.tools.math.analysis` | `1.1.0-preview.6` | none |
| `dw.tools.math.scientific` | `1.1.0-preview.14` | none |

All package assets must be real, bounded net10.0 `*.nupkg` ZIP archives containing a matching `lib/net10.0/<id>.dll` and matching nuspec ID/version/direct dependency roster. Distribution must include `readme.md` and `NOTICE.md`. Only local `build/artifacts/rs127-complete-local-feed` is used; source declared metadata is not equivalent to a new archive.

## Fresh source-free consumers

| Existing client | NuGet dependencies | New observed test scope |
|---|---|---|
| `tests/consumers/m3-gate/m3-gate-consumer.csproj` | `numerics [0.4.0-preview.4]` and `statistics [0.4.0-preview.6]` | **31** independent numerical/statistics contracts and explicit refusal cases; `dw.tools.math/m3-nuget-gate/0.4 qualified; probes=31` |
| `tests/consumers/exact-math-smoke/exact-math-smoke.csproj` | `dw.quantities`, `dw.quantities.expression`, `dw.quantities.standard` all `[0.2.0-preview.1]` | Exact rational, unit systems, conversion and expression parser behavior; `dw.tools.math/exact-consumer/0.2 qualified`, **no invented probe count** |

Both project files must lack `ProjectReference`; package restore uses one explicit local feed and a private `NUGET_PACKAGES` cache, and run uses `--no-restore`. `NuGet.Config` is local-only. Source is current Math; no sibling repository is touched.

## Qualification lineage and pending gaps

RS124: full solution Release tests and 3 local package archives, attempt 001 after one pre-attempt refusal, **first-pass NO**. RS125: 2 fresh independent analysis/scientific clients, 16 probes, **first-pass YES**. RS126: all seven original distribution packages and 8 current public-surface checks, **first-pass YES**. These are separate observations and package archive hashes are not assumed identical across runs.

RS127 accepts only 13 freshly packed packages, **2 freshly run clients**, 31 counted statistics/numerics probes and a passing unnumbered exact smoke. Foundation receives a ZIP identity check, not a newly claimed independent smoke test. Other past consumers, cross-version API/ABI binary guarantees, alternative RIDs, imported providers, generated `dw.discovery` skills, public NuGet and recipient adoption remain **unqualified/unauthorized**.

Read-only gate: `dotnet run --file docs/planning/ValidatePostM7CompletePackaging.cs -- --check`. After local archives exist, `--packages` computes their observed SHA-256. The only durable output is the DWF source-pinned qualification receipt. All 541 existing project-plan nodes remain done.
