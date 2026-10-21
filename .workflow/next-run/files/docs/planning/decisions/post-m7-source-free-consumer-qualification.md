# RS125 — fresh source-free package consumption, not publication

## Question and authority

After RS124 successfully qualified the current `dw.tools.math` source, full Release suite and three representative local archives, can **two existing independent consumers** be freshly restored and executed from **new current-source local NuGet packages**, with zero Math `ProjectReference` and zero network package feeds, without reopening M0–M7 or publishing anything?

The owner previously authorized track A (delivery qualification without publication). This is the next bounded, evidence-producing step in that track. M7 is already `done`: RS125 intentionally uses existing DWF scope `M7` **without any progress transition or completion achievement**. No new DWF milestone, public API, release, or cross-repository work is authorized.

## Pinned baseline

- RS124 succeeded on attempt 001 with **one pre-attempt refusal** and `firstPassSucceeded=false` (truthful DWF attribution). Its `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` records a fresh .NET 10.0.402 win-x64 locked restore, Release solution build, three focused test groups, full solution tests, and local preview archives for distribution, analysis and scientific. Those archives were ephemeral and are **not** assumed present in the RS125 disposable tree.
- The accepted product roadmap remains version `0.1.122` and `done`; all **541 native DWF nodes** are `done`.
- Historical `tests/consumers/m6-w01-c03-t2` analysis client has one pinned `PackageReference Include="dw.tools.math.analysis" Version="[1.1.0-preview.6]"`, and `tests/consumers/m6-w02-c04-t2` scientific client has one pinned `PackageReference Include="dw.tools.math.scientific" Version="[1.1.0-preview.14]"`. Both have `ManagePackageVersionsCentrally=false`, `IsPackable=false`, **no ProjectReference**, and `NuGet.Config` clears all feeds.
- The analysis client independently checks eight bounded derivative/ODE and refusal oracles. The scientific client checks eight dated finance/day-count/IRR oracles and explicit unsupported/invalid statuses. Both verify the supplied local `.nupkg` ZIP, nuspec, `lib/net10.0` DLL, and print the package identity, size and SHA-256 themselves. Source origin and test expectations are already controlled in the current Math tree.
- Existing M5/M6 `gate` receipts and source-free clients are *historical*; RS125 does not replay all 47 consumer directories or claim broad API/ABI/OS compatibility.

## Exact controlled execution

The independent read-only C# verifier `docs/planning/ValidatePostM7ConsumerIsolation.cs` checks the baseline RS124 report and receipt, the immutable native/product plan, both exact consumer projects and source files, their `NuGet.Config` clear-feed policy, source-declared NuGet versions and negative publication/adoption flags.

Within one controlled RS125 payload:
1. Restore the unchanged `dw.tools.math.slnx` with locked sources; Release-build the two library csprojs from source.
2. Locally `dotnet pack --no-build --no-restore` `analysis` to ignored `build/artifacts/m6-w01-c03-t2-local-feed` and `scientific` to ignored `build/artifacts/m6-w02-c04-t2-local-feed`. These folder names are the **existing consumers' explicit anti-substitution fixtures**, not remote feeds.
3. For each client, `dotnet restore` its csproj using **only its local absolute archive feed** and a **fresh isolated `NUGET_PACKAGES` cache under ignored `build/artifacts/rs125-isolated-packages`**. A local `NuGet.Config` removes all other sources. The client has no source ProjectReference.
4. Launch each existing client `dotnet run --project ... -c Release --no-restore -- <full archive path>` with the same isolated NuGet package cache. Require its exact `probes=8` output marker and separately parse its package SHA-256, version and positive byte length.
5. Record all nine mandatory successful execution commands, observed SDK/RID, independent package digests and **16 freshly rerun consumer probes** in `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`. The completed receipt must be reaccepted by the read-only validator, without changing backlog or native DWF nodes.

If any build, restore, offline feed selection, package identity, API oracle, or refusal assertion fails, RS125 must **fail**, without manufacturing a successful consumer record. Archive digests are measured from the **current newly packed bytes**; equality with RS124's historical zip SHA is not required, and a SHA alone does not certify arbitrary binary compatibility.

## Acceptance and nonclaims

**Admitted on success:** two independent net10 consumers running against exact current-source local preview NuGet packages, eight checked probes each, and reproducible package identities/hash provenance; this qualifies those specific consumer usages on the observed SDK/RID.

**Not admitted:** a 1.1/1.2 public release, third-party feed, new API/ABI stable semver guarantee, all thirteen packages, other operating systems, general solver/proof providers, generated Math Discovery skill, AURA/Decision/MCDM integration or authorization to transmit their three unsent drafts.

**Next product gap:** separately qualify public API/ABI and source-free multi-package `distribution` consumer, followed by explicit rights/recipient/publication decisions. No M8 is invented by this task.

**Cost and controls:** zero subagents; two real package builds, two archives, two offline client restores, two existing independent eight-probe clients; one bounded BCL validator and structural DWF guards. No Python, GitHub CI, new package feeds, sibling changes or manual operator repair.
