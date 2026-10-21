# Post-M7 delivery qualification — accepted owner-authorized audit A

## Question and authorization

The owner selected **A: qualification of the actual Math delivery without publication** after RS123 closed the accepted M0–M7 roadmap. Which projects are actually present, buildable and locally packageable today, which evidence-backed Math contracts can be consumed independently, and which capabilities remain historical, candidate, deferred or external?

This is a **read-only product qualification run scoped to the already-done M7 node**, not a synthetic M8 completion, milestone restart, source extraction, cross-repository transmission, or public package release. Project and backlog states must remain byte-identical. Contributions contain **no completion achievements**, because no previously incomplete project node will become done.

## Source-controlled observations at selection baseline

- `dw.tools.math.slnx` lists **24 source/test projects**: **13 product class libraries** under `src/projects/` and **11 test projects** under `tests/projects/`. Each product project declares an explicit distinct NuGet PackageId/Version and `IsPackable=true`; all compile for `net10.0` from `Directory.Build.props` with analyzer warnings treated as errors. SDK policy `global.json` is `10.0.401` with `latestPatch`, not a guaranteed installed SDK or another-RID support.
- The repository has **47 historical local consumer directories** in `tests/consumers/`. Their presence is a source inventory, not a fresh source-free NuGet test pass. `docs/planning/evidence/M5-gate.json` records seven local archives and three prior consumers; `M6-gate.json` records historical analysis/scientific archives and ten source-free eight-probe consumers. Prior receipts remain historical until locally rechecked.
- The most recent product csproj metadata advertises preview package versions, including `dw.tools.math.distribution 1.0.0-preview.4`, `dw.tools.math.analysis 1.1.0-preview.6` and `dw.tools.math.scientific 1.1.0-preview.14`. A source-declared version is not a current archive or registry publication.
- The root `README.md` still contains a **2026-10-07 initial-planning status** inconsistent with the later M0–M7 code and reports. Treat this as documented publication/readme debt; do not silently rewrite public claims during this bounded qualification.
- `docs/discovery/` contains a pinned portable protocol and candidate Math agent API metadata only. It explicitly does **not** deliver a generated `.agents/skills/dw-tools-math` bundle, qualified CLR binding, AURA authorization or consumer-ready Discovery package.
- `docs/coordination/requests/README.md` records three **draft and unsent** AURA/Decision/MCDM requests. Local Math dependency gate satisfaction does not imply recipient adoption.
- RS123 succeeded attempt 001 and first-pass, closed all **541 native nodes** and accepted backlog version `0.1.122`; its M7 receipt explicitly withholds public 1.2.0 publication, new advanced/formal product claims and new roadmap admission.

## Execution and independent acceptance

The read-only BCL `docs/planning/ValidatePostM7DeliveryInventory.cs` verifies exact solution roster, package identifiers and source-declared versions, relative ProjectReference safety, local test and consumer counts, source-tree evidence, completed 541-node DWF plan, RS123 lineage and distinct old/new publication claims. It checks baseline when no RS124 receipt exists and target when the receipt exists, without creating or modifying source.

The controlled RS124 payload installs this decision, a distribution inventory and the validator, and runs locally:

1. `dotnet restore dw.tools.math.slnx --locked-mode`
2. `dotnet build dw.tools.math.slnx -c Release --no-restore`
3. `dotnet test dw.tools.math.slnx -c Release --no-build --no-restore`
4. `dotnet pack` the three representative `distribution`, `analysis` and `scientific` csprojs to a **local ignored** `build/artifacts/rs124-local-packages/` directory using `--no-build --no-restore`.

Each successful command records actual exit code zero, stdout/stderr SHA-256, observed SDK and RID. Each selected archive is checked for actual existence and byte SHA-256; representative package tests are not equivalent to repacking all thirteen libraries or source-free consumer reruns. Failure aborts RS124 before a success receipt; no old historical witness is retroactively refreshed.

The evidence receipt `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` must state exactly what completed: 13 current packable csprojs, 11 test projects, 47 historical consumer directories, 3 freshly packed representative local archives **only if commands succeed**, the current local full-solution test command outcome, and no user-authorized public publication or external adoption. Existing native plan/backlog node identities and states must remain unchanged.

## Accepted result and next decisions

A successful RS124 qualifies the **local source, build/test and representative-package evidence observed during this run**, not production-grade cross-platform API/ABI stability, rights beyond inherited attestations, source-free consumption of every currently built package, generated Discovery skills or current NuGet.org publication.

The next owner decision can request: deeper reproducible source-free consumer/API/ABI qualification; separate authorized coordination with AURA, Decision and MCDM; an explicit DWF-compatible M8 roadmap admission; or a publication-readiness audit. **No M8 is created and no publication or outbound request is sent by RS124.** Any future native roadmap extension must use an accepted DWF authoring/replanning contract, not a direct arbitrary rewrite of the completed 541-node plan.

**Resource limits:** zero subagents, single locked solution build/test pass and three local representative packs, no opportunistic provider installation, cross-repository edits, GitHub Actions/paid CI, Python or public push. All claims are reproducible from committed Math sources plus actual controlled-run receipts.
