# RS125 source-free NuGet consumers — isolated current-tree qualification

**Source and scope:** post-M7 delivery qualification, source at current Math `main` descended from RS124 success `660362d5cb98af44eb666b93277d9ea3ea1366be`. Two existing consumer programs are re-executed against fresh local packs; this does not recertify all historical consumers or publish packages.

## Source-to-package-to-consumer table

| Dimension | Analysis | Scientific |
|---|---|---|
| Product csproj | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` |
| Exact NuGet identity | `dw.tools.math.analysis [1.1.0-preview.6]` | `dw.tools.math.scientific [1.1.0-preview.14]` |
| Current local output feed | `build/artifacts/m6-w01-c03-t2-local-feed` | `build/artifacts/m6-w02-c04-t2-local-feed` |
| Source-free consumer | `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj` | `tests/consumers/m6-w02-c04-t2/m6-dated-finance-consumer.csproj` |
| Package reference graph | One exact NuGet PackageReference; **no ProjectReference** | One exact NuGet PackageReference; **no ProjectReference** |
| Independent oracle family | Finite scalar derivative/ODE, refusals for regularity, invalid step, unsupported regimes and constrained budget | Explicit ACT/365 and ACT/360 finance, dated IRR, invalid dates/roots, cancellation and retained numerical oracles |
| Mandatory pass marker | `dw.tools.math/m6-differential-boundary-consumer/1.1 qualified; probes=8` | `dw.tools.math/m6-dated-finance-consumer/1.1 qualified; probes=8` |

The two programs parse their given `*.nupkg`, verify its expected parent feed name, `nuspec` metadata and `lib/net10.0` assembly, then independently calculate its byte length and SHA-256. An isolated NuGet package cache plus `NuGet.Config` clear-feed policy prevents success based on unexamined global/remote package caches.

## Historical versus fresh evidence

| Evidence | Historical truth | RS125 target |
|---|---|---|
| RS124 | Full solution restore/build/test, three representative packages, attempt 001 but **first-pass NO** due to one pre-attempt refusal | Preserve historical counters unchanged |
| M6 package clients | Ten previous source-free consumers with 80 original probes on prior package revisions | **Two** newly executed clients with **16** probes on fresh local current-source archives |
| DWF and backlog | Eight milestones, 17 work packages, 58 chunks, **541 done nodes**, `plan_version=0.1.122` | No new nodes, transitions or completion achievements |
| Publication | `publicly_published=false`, no external recipient acceptance, no new M7 provider/API | Keep all publication and client-adoption flags false |
| Discovery | Protocol/source fixture candidate, no generated Math skill | Remains candidate, not a broker or public agent binding |

## Receipt and release boundary

If successful, `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` contains exact observed SDK/RID, nine command exit-0 records, two current archive SHA-256 identities and byte sizes, source-free isolated restore confirmation, `source_free_consumers=2`, `fresh_consumer_probes=16`, and explicit flags denying remote publication and broader ABI claims. A failed command cannot be replaced by a historical green receipt.

No GitHub Actions, NuGet.org publication, generated Discovery skill, cross-repository transmission, or public license/semver decision is authorized. Next evidence should concern other current packages or API/ABI support boundaries, separately and proportionately.
