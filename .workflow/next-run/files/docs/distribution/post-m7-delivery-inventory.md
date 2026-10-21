# Math post-M7 source/package and consumer inventory — RS124 baseline

**Evidence boundary:** source metadata observed at the RS123 success tree, `a03fb2ee11e947ed4c01621c8b819b2b3e9667d7`. These version strings and paths are **declared csproj metadata, not package-build or remote registry assertions**. RS124 records any *new actual local* build/test/archive evidence separately.

## Thirteen packable source projects in the solution

| Local package / project | Declared Version | Boundary |
|---|---|---|
| `dw.tools.math.foundation` | `0.1.0-preview.1` | .NET foundation smoke, not full Math |
| `dw.quantities` | `0.2.0-preview.1` | Exact quantities; inherited local namespace |
| `dw.quantities.standard` | `0.2.0-preview.1` | Standard-unit catalogue, metadata |
| `dw.quantities.expression` | `0.2.0-preview.1` | Bounded quantity expression parser |
| `dw.tools.math.ir` | `0.3.0-preview.1` | Typed exact mathematical IR |
| `dw.tools.math.composition` | `0.3.0-preview.1` | Bounded exact computation and replay |
| `dw.tools.math.numerics` | `0.4.0-preview.4` | Finite binary64 vector/matrix/sparse contracts |
| `dw.tools.math.statistics` | `0.4.0-preview.6` | Bounded descriptive/statistical distributions |
| `dw.tools.math.symbolics` | `0.5.0-preview.10` | Typed bounded symbolic operations |
| `dw.tools.math.codecs` | `1.0.0-preview.8` | Canonical Math text/JSON/Unicode/Office codecs |
| `dw.tools.math.distribution` | `1.0.0-preview.4` | Local integration package, console vertical |
| `dw.tools.math.analysis` | `1.1.0-preview.6` | Bounded numerical analysis/curves |
| `dw.tools.math.scientific` | `1.1.0-preview.14` | Finite scientific functions and caller-rate finance |

All are net10-class projects via `Directory.Build.props`, with `IsPackable=true`, explicit PackageId/Version and a locked-restore declaration. The solution also includes **11 test projects**. The pre-existing `tests/consumers/` tree has **47 historical consumer directories**; this count is *not* 47 currently qualified NuGet consumers.

## Qualified historical observations versus fresh RS124 checks

| Dimension | Historical recorded evidence | What RS124 actually rechecks |
|---|---|---|
| Source/plan | M0–M7 all accepted under backlog `0.1.122`; 541 DWF nodes done | Current solution paths, 13 + 11 project metadata, exact DWF and source identities |
| Local M5 packaging | M5 gate records 7 local archives, three prior source-free consumers, **no public release** | Fresh Release solution restore/build/test and representative `distribution` local pack |
| Local M6 packaging | M6 gate records `analysis 1.1.0-preview.6` and `scientific 1.1.0-preview.14` historical package receipts and 10 earlier consumer runs | Fresh representative `analysis` and `scientific` local packs; no invented new source-free tests |
| Discovery for agents | Local protocol fixture and candidate API seed only | Source existence and explicit no-generated-skill boundary, **no claim of broker integration** |
| Advanced M7 research | Four families and finite formal witness **studied/deferred** | Continue to distinguish research from shipped modules; no new provider pack |
| External ownership | AURA/Decision/MCDM drafts **unsent** | Reconfirm explicit draft/untransmitted local records; no authorized outgoing action |
| Release rights/platform | Net10 project metadata and historic win-x64 evidence | Record actual SDK/RID; **no remote NuGet, other RID, cross-version ABI or license promotion** |

## Static and dynamic verdicts

**Static baseline now:** 13 packable csprojs in the solution, 11 test csprojs, source-declared preview versions, 47 historical consumer directories, three unsent requests, and M7 internally complete. Package-build success, current full-suite success, three fresh package SHA-256 identities and any compatibility result remain **unobserved before RS124 executes**.

**Success criteria:** the controlled RS124 report and `RS124-post-m7-delivery-qualified.json` record fresh locked restore, Release solution build and test, and byte-checked local `distribution`, `analysis`, `scientific` packages. If the commands fail, no success verdict may be claimed. Retain original 541 DWF nodes without synthetic reopening or completion achievements.

**Remaining gaps even on success:** full 13-package fresh repack, external source-free consumer retest, current public API/ABI comparison, generated `dw.discovery` manifest/skill, multi-OS qualification, license/publication authorization, and actual AURA/Decision/MCDM integration. They are candidate follow-ups, not automatic roadmap entries.
