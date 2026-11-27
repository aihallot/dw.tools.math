# dw.tools.math

Independent .NET 10 mathematical computation and representation libraries for applications, scientific consumers and agents. **Math does not depend on AURA, Decision or MCDM.**

## Current status — completed M0–M9; M10 readiness and M11 numeric subsidiarity

M0–M9 remain complete. Canonical backlog `0.1.160` adds the [M11 numeric subsidiarity scope](docs/planning/versions/M11.md); M10 readiness remains intact, and actual production cutover depends on M11 candidate qualification. Streaming moments source qualification closed in RS383; larger-sample quantile RED closed in RS385; RS386 delivered additive explicitly bounded quantile overloads; RS387 closes GREEN and readies independent verification. No new package publication or production cutover has occurred. The owner authorized prospective migration to .NET 11/C# 15. See the [M10 roadmap](docs/planning/versions/M10.md) and [admission decision](docs/planning/decisions/m10-production-net11-migration-admission-rs367.md).

### Completed M0–M9 checkpoint — RS366 (2026-11-26)

**M0–M9 are complete in the accepted bounded roadmap.** At the RS366 checkpoint, canonical backlog `0.1.142` had 597 completed nodes and native DWF had 591 completed nodes. M10 added 17 planned nodes and M11 adds 22 planned nodes to both authorities without changing these completions. Six historic subtasks remain intentionally omitted from the native hierarchy; no history is reconstructed. See [M9 final reconciliation](docs/planning/decisions/m9-bounded-sampling-canonical-reconciliation-rs365.md) and the [planning index](docs/planning/README.md).

- M8: selected portable Discovery protocol, four CLR capabilities, opt-in unsigned portable export and bounded SDK qualification; no BrokerReady permission, external host installation or production .NET migration.
- M9: deterministic noncryptographic bounded integer sampling with exact rejection, raw-draw budget and checkpoint replay; independently qualified immutable private Statistics `20260.20261120.1001` and exact Quantities `20260.20261102.1001` with locked Windows and Linux/WSL net11 consumers. [Contract](docs/distribution/bounded-integer-sampling.md) · [Assessa handoff](docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md).
- Production source remains .NET 10 / C# 14. No new public NuGet publication, universal cross-version compatibility, or downstream adoption is claimed. Any new capability or public release requires separately accepted scope and evidence.

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
