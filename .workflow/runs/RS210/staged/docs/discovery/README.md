# Agent Discovery — Math qualification and boundaries

Math Discovery is **opt-in**, broker-neutral, and constrained to the pinned `dw.discovery/1.0.0-draft.1` schema-2 protocol. The Math-owned source-qualified skill exists at [SKILL.md](../../.agents/skills/dw-tools-math/SKILL.md), with [four selected runtime-only capabilities](../../.agents/skills/dw-tools-math/references/manifest.json), [progressive navigation](../../.agents/skills/dw-tools-math/references/index.md) and [source-project runnable examples](../../.agents/skills/dw-tools-math/references/examples.md). These files are metadata and local Math evidence, **not** broker authorization, an independently installed consumer, public packages or AURA adoption. No capability is `BrokerReady`.

## Current qualification checkpoint — RS209 (2026-11-14)

- **Portable protocol:** RS131 ran the independent Math C# verifier against all 13 immutable Decision-pinned fixtures (2 accepted, 11 refused). RS209 independently matched the source hashes, identities and recorded verdicts to that receipt; see [RS131 protocol evidence](qualification/RS131-portable-protocol-qualified.json) and `.aura/workflow/reports/RS209/attempt-001.json`.
- **Selected CLR and binaries:** RS185 qualified selected fingerprints; RS197 checked four CLR bindings in three immutable private `NUGET_LOCAL` archives; RS198 ran four packaged probes, including deliberately negative Composition cases. These probes are not four positive installed-consumer examples.
- **Skill and examples:** RS187, RS195, RS201 and RS202 qualified deterministic selected metadata, cross-artifact consistency and bundled-manifest drift refusals. RS204 and RS205 added positive Quantity and Replay calls to the independent **source-project** seed alongside positive IR behavior. RS206 generated navigable example references, and RS207 verified their links and five refusals.
- **Lifecycle:** M8, M8-W01 and its C01/C02 phases remain `not-ready` in the native DWF plan; the accepted backlog remains `planned`. No completion achievements or release/adoption claims follow from the evidence-only runs. The [M8 acceptance reconciliation](qualification/m8-w01-acceptance-evidence-reconciliation.md) distinguishes proven surfaces from outstanding admission decisions.

The canonical source manifest, private package versions and the generated skill version are distinct identities. Existing published packages remain immutable. Runtime permission, broker routing, consumer installation and cross-repository adoption remain owned and independently qualified by their respective consumers.

## Historical preparation context (RS130–RS131)

The sections below preserve historical planning decisions and the pre-RS131 authoring baseline; prospective statements there describe their original time and must not override the current checkpoint above.

## Pinned interoperability authority

The reference protocol is `dw.discovery/1.0.0-draft.1`, **manifest schema 2**, confirmed at `aihallot/dw.tools.decision` commit `5d3af8580ea984425b314d589def18e031172c2c` (Decision 1.6 release candidate). The earlier RS041 alignment revision remains recorded in the pinned source metadata; the schema and fixture Git blobs did not change.

- [Pinned schema](interop/decision-1.0.0-draft.1/manifest.schema.json)
- [Pinned 13-case conformance suite](interop/decision-1.0.0-draft.1/conformance.json)
- [Source/commit/blob and scope manifest](interop/decision-1.0.0-draft.1/source.json)

The pinned files were independently fetched into Math and their Git blob identities matched Decision exactly. This **proves file identity, not Math protocol conformance**. The suite's expected outcomes are 2 accepted and 11 rejected cases; their Math C# execution and receipt were subsequently qualified in RS131 and reconciled in RS209.

## Independent portable protocol verification

[Standalone Math C# verifier](qualification/ValidatePortableProtocol.cs) contains the Math-owned, independently authored JSON envelope validator and the 13-case fixture runner. It validates exact SHA-256 source bytes before applying each accepted/rejected case. The original RS131 verifier ran under the then-pinned .NET 10 SDK with this command:

```powershell
dotnet run --file docs/discovery/qualification/ValidatePortableProtocol.cs
```

The earlier [portable preflight receipt](qualification/portable-protocol-preflight.json) is an assistant-environment **JavaScript observation**, not an execution of Math C#. RS131 subsequently executed the independent Math C# verifier under DWF and recorded the 13 per-case verdicts (2 accepted, 11 rejected), SDK/RID and pinned SHA-256 identities in [RS131 C# protocol evidence](qualification/RS131-portable-protocol-qualified.json). RS209 checked the pinned fixture/receipt correspondence without replaying RS131. A qualified portable envelope is not an agent callable binding or broker runtime.

**Do not label full Math/Decision runtime interoperability as qualified.** Selected Math CLR signatures, examples and generated manifest/skill now have bounded Math-owned evidence, but no end-to-end cross-repository runtime interoperability or authorization gate is admitted. A passed portable corpus alone grants no BrokerReady or AURA authorization.

## Math API scope

[Candidate agent surface](agent-surface-candidates.json) gives proposed stable Math capability IDs, observed CLR source locations, numerical meanings and explicit exclusions. It is a **candidate authoring input**, not an emitted skill or runtime authorization. Four Math operations are proposed for the first direct-.NET seed; none is BrokerReady.

Public constructors, parsers, conversion helpers, operation-description lookups and ordinary .NET APIs must not be published merely because reflection finds them. Methods marked `Documented` remain internal or developer-facing; only deliberately selected `AgentCallable` or qualified `BrokerReady` operations belong in the generated agent-facing skill. AgentCallable does not imply JSON/DTO support, installation, routability or authorization.

Instance methods such as `FiniteMatrix64.Multiply` and `FiniteVector64.Dot` are deferred until a verified binding strategy or a stable, explicitly exposed static façade exists. Do not claim these methods are static or infer availability of Math.NET as a runtime provider from its qualified **test-only** numerical smoke.

## Proposed independent work package

See the [dedicated proposed backlog extension](../planning/proposals/math-agent-discovery-work-package.json). Its stable proposed identifiers are `M3-W03-C01` (portable protocol and seed), `M3-W03-C02` (versioned generator and skill), and `M3-W03-C03` (API coverage and broker-neutral admission). The proposal does **not** modify the canonical `docs/planning/backlog.json`, the active native DWF plan, or existing M3 numerical scopes.

The generic native-DWF authoring and safe sibling-continuation request is tracked in [Workflow issue #139](https://github.com/aihallot/dw.tools.workflow/issues/139). The issue was historical workflow feedback, not a product delivery receipt. The later accepted M8 backlog and native scope supersede this unmerged M3-W03 proposal.

Historical DWF state after RS130 (2026-10-21): the accepted backlog v`0.1.122` and all **541 native DWF nodes across M0–M7 are done**; RS130 finished first-pass and reconciled the public README. The installed Math DWF 0.1.59 SDK exposes `EnsurePhaseWithTasks` only below an **existing** work package; an approved typed new-work-package constructor is not available in its documented SDK. [Workflow issue #139](https://github.com/aihallot/dw.tools.workflow/issues/139) remains open. The proposed `M3-W03` is a distinct **draft**, not merged into the accepted plan. Do not edit workflow plan JSON directly or smuggle Discovery into a completed numerical task.

**Historical next safe execution path (now completed):** RS131 was a bounded **evidence-only C# protocol gate** scoped to the already-completed M7, with zero achievements or roadmap mutations. It does not admit a product Discovery work package. For the later CLR binder and generator/skill implementation, first establish a supported DWF topology/rebaseline, then independently qualify C01/C02/C03 under their admitted scope. Keep the older M2/M3 Math qualifications immutable.

## Original planned output layout (historical; not all entries delivered)

```text
.agents/skills/dw-tools-math/
  SKILL.md
  references/
    index.md
    invocation-csharp.md
    manifest.json
    skill-version.json
    families/
    capabilities/
    examples/
```

The generated `manifest.json` is the machine-readable discovery protocol consumed by AURA or another broker; the Markdown supports progressive reading by an agent, **not** direct authorization or execution. The broker exclusively owns eligibility checks, permissions, confirmations, isolation, actual provider loading, routing, action execution and operational logging.

Version the underlying Math package, manifest schema, generator and skill bundle separately. Compare deterministic `generate/check` outputs, compiled examples, CLR binding fingerprints and all 13 shared fixtures before any readiness claim.
