# Agent Discovery — Math implementation preparation

Math's agent-facing Discovery must remain **opt-in**, broker-neutral, and compatible with the portable protocol validated locally by Decision. No generated Math skill, CLR introspection tool, agent-binding package, or broker runtime is delivered by the planning files in this directory.

## Pinned interoperability authority

The reference protocol is `dw.discovery/1.0.0-draft.1`, **manifest schema 2**, from `aihallot/dw.tools.decision` commit `3c8a1b3a1ef818039a7c84ca5fa19666215c5a4b` (RS041, succeeded).

- [Pinned schema](interop/decision-1.0.0-draft.1/manifest.schema.json)
- [Pinned 13-case conformance suite](interop/decision-1.0.0-draft.1/conformance.json)
- [Source/commit/blob and scope manifest](interop/decision-1.0.0-draft.1/source.json)

The pinned files were independently fetched into Math and their Git blob identities matched Decision exactly. This **proves file identity, not Math protocol conformance**. The suite's expected outcomes are 2 accepted and 11 rejected cases.

## Math API scope

[Candidate agent surface](agent-surface-candidates.json) gives proposed stable Math capability IDs, observed CLR source locations, numerical meanings and explicit exclusions. It is a **candidate authoring input**, not an emitted skill or runtime authorization. Four Math operations are proposed for the first direct-.NET seed; none is BrokerReady.

Public constructors, parsers, conversion helpers, operation-description lookups and ordinary .NET APIs must not be published merely because reflection finds them. Methods marked `Documented` remain internal or developer-facing; only deliberately selected `AgentCallable` or qualified `BrokerReady` operations belong in the generated agent-facing skill. AgentCallable does not imply JSON/DTO support, installation, routability or authorization.

Instance methods such as `FiniteMatrix64.Multiply` and `FiniteVector64.Dot` are deferred until a verified binding strategy or a stable, explicitly exposed static façade exists. Do not claim these methods are static or infer availability of Math.NET as a runtime provider from its qualified **test-only** numerical smoke.

## Proposed independent work package

See the [dedicated proposed backlog extension](../planning/proposals/math-agent-discovery-work-package.json). Its stable proposed identifiers are `M3-W03-C01` (portable protocol and seed), `M3-W03-C02` (versioned generator and skill), and `M3-W03-C03` (API coverage and broker-neutral admission). The proposal does **not** modify the canonical `docs/planning/backlog.json`, the active native DWF plan, or existing M3 numerical scopes.

The generic native-DWF authoring and safe sibling-continuation request is tracked in [Workflow issue #139](https://github.com/aihallot/dw.tools.workflow/issues/139). The issue is feedback, not a delivered SDK feature.

Current DWF state after RS040: M3 and M3-W01 are in progress, `M3-W01-C02` is the active numerical phase, and its T2 boundary work is still planned. Introducing `M3-W03` requires an explicitly supported topology authoring/reconciliation step. The current SDK has a typed `EnsurePhaseWithTasks` for an **existing** work package, not a typed new-work-package builder; the Math native-plan validator currently fixes 8 milestones, 17 work packages, 53 phases and 496 mapped nodes. Do not hand-write a fake native `ProjectPlan` transition or smuggle Discovery into a numerical task.

**Next safe execution path:** qualify a supported native-DWF roadmap extension first (or request a workflow feedback capability from the owning Workflow project). Only then select the Discovery C01 run with its own native project nodes and RED/GREEN evidence. Keep M2 qualification immutable and do not claim cross-repo interoperability until Math executes the pinned Decision suite itself.

## Planned output once executed

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
