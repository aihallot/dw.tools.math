# Current local Math delivery and consumption — qualified snapshot 2026-10-21

**Scope:** the present Math source and its **bounded local** acceptance results, not a public registry release or promise of stable binary compatibility. See [repository README](../../README.md) for current product status. All commands here are local; no other GitHub repository is modified.

## Verified package roster (13 packable projects)

| PackageId | Source-declared preview version | Qualified local boundary |
|---|---|---|
| `dw.tools.math.foundation` | `0.1.0-preview.1` | Local net10 ZIP identity in RS127; no fresh independent foundation client |
| `dw.quantities` | `0.2.0-preview.1` | Exact quantities, rational arithmetic; source-free exact smoke |
| `dw.quantities.standard` | `0.2.0-preview.1` | Standard unit catalogue and conversions; source-free exact smoke |
| `dw.quantities.expression` | `0.2.0-preview.1` | Bounded unit/expression parser; source-free exact smoke |
| `dw.tools.math.ir` | `0.3.0-preview.1` | Typed exact IR; bounded domain/source checks |
| `dw.tools.math.composition` | `0.3.0-preview.1` | Exact composition and bounded replay; distribution consumer |
| `dw.tools.math.numerics` | `0.4.0-preview.4` | Finite vector/matrix contracts; independent M3 consumer |
| `dw.tools.math.statistics` | `0.4.0-preview.6` | Bounded statistics/distributions; independent M3 consumer |
| `dw.tools.math.symbolics` | `0.5.0-preview.10` | Explicit-domain finite symbolics; independent M4 consumer |
| `dw.tools.math.codecs` | `1.0.0-preview.8` | Text/JSON/Markdown/OMML bounded conversion; independent M5 consumer |
| `dw.tools.math.distribution` | `1.0.0-preview.4` | Seven-package exact/numerical/symbolic distribution; independent package-graph consumer |
| `dw.tools.math.analysis` | `1.1.0-preview.6` | Numerical analysis/differential boundaries; independent M6 consumer |
| `dw.tools.math.scientific` | `1.1.0-preview.14` | Finite scientific functions/dated finance; independent M6 consumer |

All 13 source projects declare `IsPackable=true` and are present in the `dw.tools.math.slnx` solution with 11 test projects. RS127 freshly packed and SHA/nuspec/net10-inspected this 13-package roster on **.NET 10.0.402 / win-x64**. Packages and their dependency edges are not public support guarantees.

## Verified evidence by run

| Evidence | What actually ran | First-pass truth |
|---|---|---|
| [RS124](../planning/evidence/RS124-post-m7-delivery-qualified.json) | Locked restore, Release solution build/tests, 3 representative local archives | NO; 1 pre-attempt refusal |
| [RS125](../planning/evidence/RS125-post-m7-nuget-consumers-qualified.json) | 2 independent source-free analysis/scientific clients, 16 numbered probes | YES |
| [RS126](../planning/evidence/RS126-post-m7-distribution-qualified.json) | 7 local dependency-closure archives, 1 independent client, 8 numbered probes and current API fingerprints | YES |
| [RS127](../planning/evidence/RS127-post-m7-complete-packaging-qualified.json) | 13 fresh local packages, 2 independent clients, 31 M3 probes and 1 unnumbered exact smoke | NO; 2 pre-attempt refusals |
| [RS128](../planning/evidence/RS128-post-m7-symbolic-codec-qualified.json) | 4 local packages, 2 independent M4/M5 clients, 40 numbered probes | YES |
| [RS129](../planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json) | Source-pinned consolidation of these 5 historic receipts, **no new package/test run** | YES |

**Correct total:** 7 independent *consumer executions* over RS125–RS128; 16+8+31+40 = **95 numbered probes** plus one unnumbered exact-quantity smoke. The repository's 47 historical `tests/consumers/` directories are not 47 fresh consumer executions. The 13 archives are a unique roster, **not a sum of all packages repacked in different runs**.

## Local developer workflow

With the repository pinned to its existing `global.json` policy and .NET 10 installed:

```powershell
dotnet restore dw.tools.math.slnx --locked-mode
dotnet build dw.tools.math.slnx -c Release --no-restore
dotnet test dw.tools.math.slnx -c Release --no-build --no-restore
dotnet run --file docs/planning/ValidatePlan.cs -- --check
```

For an offline NuGet consumer, **first create the required package dependency closure from current source** into a local ignored `build/artifacts/<local-feed>/` directory (the historical DWF evidence describes exact versions and pack commands). Then restore the consumer with `--source <absolute-local-feed>`, `--packages <absolute-isolated-cache>`, and `-p:NuGetAudit=false`. Keep `NUGET_PACKAGES` set to the same isolated directory when running the consumer with `--no-restore`. Existing examples: `tests/consumers/exact-math-smoke` (quantities), `tests/consumers/m3-gate` (numerics/statistics), `tests/consumers/m4-gate` (symbolics), `tests/consumers/m5-w01-c04-t2` (codecs), `tests/consumers/m5-w02-c02-t2` (distribution), `tests/consumers/m6-w01-c03-t2` (analysis), and `tests/consumers/m6-w02-c04-t2` (scientific).

**Do not** install packages from an untrusted remote feed, assume a specific historical local archive persists in a fresh clone, or count a source-only consumer directory as an executed test. This guide does not imply the command to package one csproj also creates all dependent `.nupkg` archives.

## Open release and agent gates

No public NuGet packages, stable source/binary ABI guarantee, inherited-rights/license grant, non-Windows RID certification, advanced M7 tensor/precision/interval/DAG/PlotModel/formal engine, or complete arbitrary Office document fidelity is established. `docs/discovery/` is a protocol and API **candidate**, not a generated Math skill or qualified broker runtime. The AURA, Decision and MCDM request files remain **unsent drafts**.

The next owner-authorized order is **documentation reconciliation → Math Discovery/AURA qualification → authorized cross-repository coordination**, with the distinct next scopes separately DWF prepared and tested. No source-free package consumption implies transmission rights.