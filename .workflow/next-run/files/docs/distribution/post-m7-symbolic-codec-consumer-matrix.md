# RS128 — isolated Math symbolics and codec consumer qualification

All versions below are **source-declared preview versions**, not public registry releases. The RS128 success claim requires new local archive bytes, offline restore and actual consumer execution.

## Four-package closed dependency graph

| Package | Version | Direct NuGet dependencies |
|---|---|---|
| `dw.quantities` | `0.2.0-preview.1` | none |
| `dw.tools.math.ir` | `0.3.0-preview.1` | `dw.quantities` |
| `dw.tools.math.symbolics` | `0.5.0-preview.10` | `dw.tools.math.ir` |
| `dw.tools.math.codecs` | `1.0.0-preview.8` | `dw.tools.math.ir` |

New local package feed: `build/artifacts/rs128-symbolic-codec-feed`. Dedicated NuGet cache: `build/artifacts/rs128-isolated-packages`. The packages must carry exact nuspec IDs/versions/dependency IDs and nonempty `lib/net10.0/<PackageId>.dll`. The independent C# checker emits four byte-size and SHA-256 receipts.

## Two independent existing consumers

| Existing application | Exact direct PackageReference | Fresh, verified output |
|---|---|---|
| `tests/consumers/m4-gate/m4-gate-consumer.csproj` | `dw.tools.math.symbolics [0.5.0-preview.10]` | **32** boundary and refusal probes; `dw.tools.math/m4-nuget-gate/0.5 qualified; probes=32` |
| `tests/consumers/m5-w01-c04-t2/m5-math-display-boundary-consumer.csproj` | `dw.tools.math.codecs [1.0.0-preview.8]` | **8** OMML/Markdown/display/refusal probes; `dw.tools.math/m5-markdown-omml-boundary-consumer/1.0 qualified; probes=8` |

Each consumer remains source-free (no `ProjectReference`), directly targets net10.0, clears inherited package feeds via `NuGet.Config`, restores only from the named local archive feed, and runs with an isolated package cache. A historical consumer source file is not a fresh success without executable output.

## Scientific nonclaims and historical continuity

The 13-package RS127 inventory qualified two different clients (M3 31 probes, exact quantities smoke) and succeeded after **two pre-attempt refusals**, therefore its historical first-pass is **NO**. RS125 and RS126 succeeded first-pass on separate consumer scopes. The new RS128 40-probe result is recorded independently, not added to a fictitious unified test coverage percentage.

The accepted product backlog remains `0.1.122` **done**, with **541/541 native DWF nodes done**. No additional achievement or milestone is inferred from post-M7 delivery audits.

Passing M4 symbolic oracles does not certify arbitrary CAS/proof kernels. Passing M5 OMML/Markdown oracles does not certify full Office documents or typography fidelity. Even a fresh ZIP/package/hash is not public NuGet publication or cross-version ABI certification; other operating systems/RIDs, new provider APIs, generated Math Discovery skill, external owner acceptance and AURA/Decision/MCDM request transmission remain unqualified.

The source-pinned success receipt is `docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json`. It holds **12** command receipts, four archive identities, two source-free clients and **40** fresh probes. The independent gate is `dotnet run --file docs/planning/ValidatePostM7SymbolicCodecConsumers.cs -- --check`. The read-only archive mode is `--packages`. No Python or GitHub CI is used.
