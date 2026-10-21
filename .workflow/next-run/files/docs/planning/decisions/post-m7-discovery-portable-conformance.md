# RS131 — Independent Math C# portable Discovery protocol conformance (evidence only)

## Authorized stage B and strict scope

Following **RS130 successful first-pass**, the owner-approved sequence is **A documentation → B Math Discovery/AURA qualification → D coordination**. Documentation A is complete. This RS131 run is the **first independent executable boundary in B**, not the entire implementation of a Math agent skill or a new DWF work package. It admits only execution of the existing **Math-owned C# portable envelope verifier** against pinned `dw.discovery/1.0.0-draft.1` fixtures, with no Decision binary or external broker needed.

All eight previously accepted M0–M7 milestones remain `done` (541 native DWF nodes; backlog `0.1.122`). RS131 is an evidence-only controlled run scoped to already-done `M7`, with **zero new product achievements**. This limited conformance gate does not smuggle code changes into an unrelated M3 numerical task and must not be described as `M3-W03-C01` product completion.

## Immutable verified inputs

- Reference Decision commit `5d3af8580ea984425b314d589def18e031172c2c`; portable protocol `dw.discovery/1.0.0-draft.1`, manifest schema **2**. The pinned Math copies are `docs/discovery/interop/decision-1.0.0-draft.1/manifest.schema.json` (SHA-256 `678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170`) and `conformance.json` (SHA-256 `5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e`).
- The pinned corpus holds **13** deterministic cases: **2 accepted and 11 rejected**. The negative cases challenge unsupported schema/protocol, Documented helper leakage, wrong owner, stale signatures, missing execution limits, premature BrokerReady, metadata-based authorization, bogus versions, duplicate capability and catalogue leakage.
- The existing standalone Math C# verifier `docs/discovery/qualification/ValidatePortableProtocol.cs` was **authored but not durably executed in Math DWF**. The existing `portable-protocol-preflight.json` is a preliminary **JavaScript** observation; it must remain historical and must not be rewritten as a C# receipt.
- The candidate agent-surface roster contains **four intentionally proposed static methods**, seven unselected public/helper candidates and three explicitly deferred numerical operations. The original Git blobs of those four source files still match the proposed source pins. Their existence alone proves **neither actual CLR method-binding selection nor compiled invocation examples**. All four are marked `AgentCallable` *candidates*, `brokerReady=false`; zero BrokerReady adapters or generated skill artifacts exist.
- RS130 report `.aura/workflow/reports/RS130/attempt-001.json` is durable success (attempt 001, **first-pass YES**, no prior refusal); the current README and its 2026-10-07 archived baseline are reconciled without publication. No new release/rights or recipient acceptance was granted.

## Execution, validation and evidence

RS131 stages this decision, a carefully updated `docs/discovery/README.md`, and a **separately authored, read-only C# independent verifier** `docs/discovery/qualification/ValidateDwfPortableProtocolGate.cs`. The verifier checks the accepted product/native plans, prior RS130 lineage, source-pinned protocol/fixture SHA-256 values, exactly thirteen real fixture identities, unchanged draft candidate source Git blobs and no unapproved publication or generated skill.

The payload runs the **already existing** Math portable C# verifier through one bounded local command:

`dotnet run --file docs/discovery/qualification/ValidatePortableProtocol.cs`

It must exit 0 and emit **exactly thirteen** per-case `CASE ... PASS` verdicts, the pinned Decision commit, protocol/schema identity, both SHA-256 source identities, and `RESULT PASS 13/13 (2 accepted, 11 rejected) independent Math C# envelope verifier`. Any missing/extra/failed verdict or a wrong source hash refuses qualification. It reads the observed SDK and RID, records those exact per-case results and only then writes `docs/discovery/qualification/RS131-portable-protocol-qualified.json` using the typed DWF Files SDK. The independent checker runs **both before and after** receipt generation and independently refuses a false C# execution claim.

No earlier report or historical receipt is overwritten. No public Math package, SourceFree consumer, broader scientific API, prior DWF status, candidate binding metadata or sibling repo is changed. All source-controlled Markdown/C# output uses canonical English.

## Native planning boundary for next B runs

The owner-approved proposed work package `M3-W03` (`C01` portable/binding seed, `C02` deterministic generator/skill, `C03` exposure coverage and broker admission) is **still a draft** in `docs/planning/proposals/math-agent-discovery-work-package.json`. The installed Math `dw.tools.workflow 0.1.59` SDK documents `EnsurePhaseWithTasks` only for an **existing** work package and no typed creation of a complete new work package; Workflow issue [#139](https://github.com/aihallot/dw.tools.workflow/issues/139) remains open. The completed 541-node plan cannot be edited through arbitrary JSON mutation to make a false Discovery achievement. A compatible **typed plan extension / admitted rebaseline** is required before the actual product generator and skill work begins; this run documents that blocker and does not claim to fix DWF.

A complete Discovery rollout additionally requires exact **compiled CLR signatures and examples** for deliberately selected Math operations, a deterministic generated Math manifest/skill, explicit `Documented`/helper exclusions, and broker-specific serialization/authorization testing in AURA's own repository. All are **not** delivered by RS131.

## Nonclaims and next action

**On success:** accept only independent C# conformance of the **portable envelope corpus** in the observed local SDK/RID, preserving status/provenance and rejection guarantees. It is not full Math/Decision runtime interoperability, BrokerReady, an AURA plugin, a generated skill, a source/binary ABI guarantee or product milestone C01 completion.

**Next B gate:** qualify DWF-compatible native planning / owner-approved work package `M3-W03` admission before implementing the source-introspected Math binder, compiled examples and generated skill. If tooling support is not yet available, report that precise blocker and do not bypass it. Stage D coordination and outbound request transmission are later and remain separately scoped. No Python, GitHub Actions/paid CI, network provider installation, public release, sibling modifications or synthetic M8 are authorized.
