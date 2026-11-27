# dw.tools.math

Independent .NET 11 mathematical computation and representation libraries for applications, scientific consumers and agents. **Math does not depend on AURA, Decision or MCDM.**

## Current status — completed M0–M9 and M11; M10 readiness

M0–M9 remain complete. Canonical backlog `0.1.193` reconciles the completed [M11 numeric subsidiarity scope](docs/planning/versions/M11.md); M10 baseline refresh succeeded in RS399; RS400 activated the cutover-readiness RED after one preselection correction; RS401 closed native RED and readied GREEN; RS402 activated GREEN and classified projects; RS403–RS407 qualified the isolated target graph, fragile tests, maintained tools, inspector parity and actual rollback. RS408 closed native GREEN first-pass and readied independent Verification. RS409 independently verified the bounded cutover contract first-pass. RS410 closed readiness C01 first-pass and readied C02 RED. RS411 established the production cutover RED guard after one preselection correction. RS412 closed native RED after one preselection correction. RS413 delivered the actual SDK 11.0.100 / net11.0 / C# 15 cutover after two preselection corrections, with 24 projects, 35 targeted tests and both tools qualified. RS414 closed native production GREEN first-pass and readied Verification. RS415 independently verified the delivered production delta, dependency graph preservation and recoverability of all original inputs first-pass. RS416 closed production cutover C02 first-pass. RS417 reconciled canonical C02 closure and activated post-cutover certification RED first-pass; RS418 reconciled that activation and closed native RED first-pass. RS419 reconciled RED closure, activated GREEN and passed 35 focused and 2,722 full regression tests on SDK 11.0.100/win-x64 first-pass. RS420 qualified thirteen isolated unpublished net11 archives and sixteen exact edges first-pass, preserving twelve historical archives. RS421 qualified two independent locked Windows consumers of all thirteen exact candidate assemblies first-pass. RS422 qualified the identical locked consumer recipe under Ubuntu 24.04 WSL/linux-x64 first-pass. RS423 recovered the exact 30 rollback inputs, rebuilt 24 net10.0 projects and passed nine smoke tests while preserving 13 candidate and 12 shared Math archives first-pass. RS424 consolidated the six GREEN gates and readied independent Verification first-pass. RS425 independently verified all six gates and the exact physical identities in attempt 002 after a nullable filename compiler correction. RS426 closes native certification C03; work package and milestone closure remain pending. Streaming moments source qualification closed in RS383; larger-sample quantile RED closed in RS385; RS386 delivered additive explicitly bounded quantile overloads; RS387 closed GREEN; RS388 independently verified the bounded quantile contracts; RS389 closed the three bounded source scopes; RS390 established physical package candidate RED; RS391 closed package RED; RS392 qualified isolated numeric candidates after one allocated correction; RS393 closed GREEN; RS394 independently verified exact candidate archives and historical rollback; RS395 closed the candidate phase; RS396 closed the four-phase numeric work package under DWF 0.1.81; RS397 closed native M11 at G-M11; RS398 reconciles its durable completion to the canonical release. All 22 M11 nodes are complete in both authorities. No new package publication is authorized; C03 post-cutover certification remains pending. The owner authorized prospective migration to .NET 11/C# 15. See the [M10 roadmap](docs/planning/versions/M10.md) and [admission decision](docs/planning/decisions/m10-production-net11-migration-admission-rs367.md).

### Completed M0–M9 checkpoint — RS366 (2026-11-26)

**M0–M9 are complete in the accepted bounded roadmap.** At the RS366 checkpoint, canonical backlog `0.1.142` had 597 completed nodes and native DWF had 591 completed nodes. M10 added 17 planned nodes and M11 adds 22 planned nodes to both authorities without changing these completions. Six historic subtasks remain intentionally omitted from the native hierarchy; no history is reconstructed. See [M9 final reconciliation](docs/planning/decisions/m9-bounded-sampling-canonical-reconciliation-rs365.md) and the [planning index](docs/planning/README.md).

- M8: selected portable Discovery protocol, four CLR capabilities, opt-in unsigned portable export and bounded SDK qualification; no BrokerReady permission, external host installation or production .NET migration.
- M9: deterministic noncryptographic bounded integer sampling with exact rejection, raw-draw budget and checkpoint replay; independently qualified immutable private Statistics `20260.20261120.1001` and exact Quantities `20260.20261102.1001` with locked Windows and Linux/WSL net11 consumers. [Contract](docs/distribution/bounded-integer-sampling.md) · [Assessa handoff](docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md).
- The historical M0–M9 checkpoint used .NET 10 / C# 14; the current production target is .NET 11 / C# 15. No new public NuGet publication, universal cross-version compatibility, or downstream adoption is claimed. Any new capability or public release requires separately accepted scope and evidence.

## Historical local qualification — 2026-10-21 (RS130)

The following dated M0–M7 snapshot is retained for traceability. Its original source-pinned documents and validators can be inspected at [the RS130 result](https://github.com/aihallot/dw.tools.math/commit/174e92c9738a508a16044df0d042d488f0bc9697); it is not the current roadmap or package inventory.

**As of 2026-10-21: implemented libraries and local delivery qualification completed; no public NuGet release.**

The accepted M0–M7 product plan is `done` (backlog `0.1.122`, 541/541 DWF nodes). This means the **bounded accepted scope** is complete: it does **not** mean every mathematical family or planned advanced provider is implemented.

Verified current-source local delivery evidence:

- **13 packable .NET 10 source libraries** and **11 solution test projects**. All thirteen preview packages were freshly packed together and independently inspected under RS127.
- Local Release build and full solution tests passed under RS124. Selected consumers were independently restored from **local-only NuGet feeds with isolated caches**.
- RS125–RS128 observed **7 independent source-free consumer executions** and **95 numbered functional probes**, plus a separately passing **unnumbered exact-quantity smoke**. These are selected finite tests, not universal API coverage.
- Observed tooling: **.NET SDK 10.0.402, win-x64**. `global.json` requests `10.0.401` with `latestPatch`; do not infer another operating-system/RID qualification from this.
- **Not published:** no public NuGet distribution, stable semver/API/ABI compatibility certificate, other-RID support, generated `dw.discovery` Math skill, external recipient migration or automatic advanced M7 provider is approved.

The consolidated source-pinned evidence is [RS129 delivery portfolio](docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json); see the [current local delivery and consumption guide](docs/distribution/current-local-consumption.md) for package names, runnable commands and bounded claims.

## What is actually available?

The source projects provide exact rationals, physical quantities and units, bounded expression parsing, a typed exact mathematical IR, finite symbolic operations with explicit domains, numerical and statistical utilities, canonical text/JSON/Unicode/Markdown/Office OMML representations, a local distribution vertical, and bounded analysis/scientific/financial operations. Each family has its own input/refusal and approximation limitations.

Do **not** infer a universal CAS, theorem prover, arbitrary-precision interval/tensor engine, complete Office-document converter, solver guarantee or provider installation from those project names. Advanced M7 precision, tensors, pure DAG, PlotModel and general formal kernels were **investigated/deferred**, not automatically delivered.

## Build, test and inspect locally

Use the repository's **existing** .NET 10 SDK policy:

```powershell
dotnet restore dw.tools.math.slnx --locked-mode
dotnet build dw.tools.math.slnx -c Release --no-restore
dotnet test dw.tools.math.slnx -c Release --no-build --no-restore
dotnet run --file docs/planning/ValidatePlan.cs -- --check
```

These commands operate locally. Building or passing tests does **not** publish NuGet packages or install an agent. For a package-based consumer, build the required dependency graph into an ignored local feed and restore using only that feed and an isolated NuGet cache, as demonstrated by the existing `tests/consumers/` applications.

Example of a validated exact-value operation from the `dw.quantities` library:

```csharp
using dw.quantities;

var half = new ExactRational(1, 3) + new ExactRational(1, 6);
var expected = new ExactRational(1, 2);
if (half != expected) throw new InvalidOperationException("Exact arithmetic changed.");
```

Consult the [implementation plan](docs/planning/README.md), [architecture](docs/planning/architecture.md), [current package guide](docs/distribution/current-local-consumption.md), [agent Discovery preparation](docs/discovery/README.md), and [unsent cross-repository requests](docs/coordination/requests/README.md) for their distinct authority boundaries.

## Status, ownership and publication boundaries

- **Local package evidence is not a registry release.** Source-declared `*-preview.*` versions are not public releases. Rights, license/NOTICE obligations, support matrix and source/binary compatibility need separate verification and owner authorization.
- **Downstream adoption remains independently owned.** Unsent AURA, Decision and MCDM requests are distinct from the Math-owned private Statistics handoff to Assessa. Math does not transmit new requests or modify sibling repositories merely because its packages work locally.
- **Agent Discovery is bounded and opt-in.** The generated Math-owned skill, four selected CLR bindings and unsigned portable export are qualified within M8. No BrokerReady grant, installation in an external agent host, or broker runtime is claimed.
- **DWF owns execution.** The native plan and durable run reports determine accepted progress; documentation claims must be grounded in those records.

## Historical baseline (2026-10-07) — archived statement, not current status

The original README statement is retained verbatim below **only as historical text**, so older source-pinned DWF evidence and immutable validators can still identify the original planning baseline:

> **Status on 2026-10-07: initial planning; no mathematical engine has been implemented in this repository yet.** Candidate extraction code exists in AURA; its presence there is not a Math delivery.

That sentence described the repository on **2026-10-07**, *before* M0–M9 and the later delivery qualifications. It must not be interpreted as the present project status. The historic validator source files and prior receipt hashes remain unchanged; their live-document checks apply to the original result checkout, not the evolved current documents.
