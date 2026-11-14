# M8-W01 — Discovery acceptance evidence reconciliation

Status: **evidence assessment only**. Native M8, M8-W01, M8-W01-C01 and M8-W01-C02 remain `not-ready`; product backlog remains `planned`. This document does not advance native lifecycle states, grant runtime permission or qualify a release.

## Accepted product authority

`docs/planning/backlog.json`, version `0.1.123`, defines the following accepted chunk gates:

| Chunk | Accepted acceptance | Accepted refusal boundary |
| --- | --- | --- |
| `M8-W01-C01` | Pinned Discovery protocol conformance (13 portable fixtures) and compiled selected exact CLR operations | Reject unselected helpers, premature BrokerReady and invented broker authorization |
| `M8-W01-C02` | Byte-identical opt-in skill/manifest generation, exact CLR fingerprints, examples and drift refusal | Skill metadata never authorizes host action execution |

The native DWF plan additionally owns the task/subtask lifecycle for `M8-W01-C01-T1/T2` and `M8-W01-C02-T1/T2`. Their accepted RED, GREEN and Verification subtasks are not completed by writing a retrospective evidence map.

## Existing durable evidence and interpretation

| Gate surface | Observed durable evidence | Scoped interpretation |
| --- | --- | --- |
| Selected CLR signatures | `.aura/workflow/reports/RS185/attempt-001.json` and generated schema-2 source manifest | Source/compiled signature fingerprint evidence, not blanket execution authorization |
| Manifest determinism and refusals | `.aura/workflow/reports/RS187/attempt-001.json` | Existing targeted adversarial manifest qualification |
| Skill cross-artifact consistency | `.aura/workflow/reports/RS195/attempt-001.json` | Source manifest, generated skill, selected references and versions aligned |
| Immutable private binaries | `.aura/workflow/reports/RS197/attempt-001.json` | Three pinned local package archives and four selected reflection bindings; private feed only |
| Private packaged invocation probes | `.aura/workflow/reports/RS198/attempt-001.json` | Four bounded probes, including intentional Composition refusal/unsupported cases; not four positive end-user examples |
| Bundled manifest canonical LF projection | `.aura/workflow/reports/RS201/attempt-001.json` and `.aura/workflow/reports/RS202/attempt-001.json` | Generated source/bundle parity plus six independent tamper refusals |
| Selected source-project examples | `.aura/workflow/reports/RS204/attempt-001.json` and `.aura/workflow/reports/RS205/attempt-001.json` | Positive Quantity and Replay alongside existing IR encode/hash examples in the source-project seed |
| Generated example navigation and refusals | `.aura/workflow/reports/RS206/attempt-001.json` and `.aura/workflow/reports/RS207/attempt-001.json` | Navigable examples reference and five independent link/claim refusals |

Each cited RS report above has durable `outcome=succeeded` at attempt 001. **First-pass is not synonymous with attempt 001**: RS185, RS198 and RS201 retain earlier invocation failures. Consult their `priorInvocationFailures` records rather than rewriting that history.

## Acceptance gaps before native lifecycle transitions

1. **Protocol acceptance:** separately verify that the full *13 portable fixtures* called for by `M8-W01-C01` are the actual pinned conformance set, with exact provenance and complete admission/refusal evidence. Do not infer the count from an unrelated successful generator.
2. **Consumer boundary:** positive examples are repository source-project tests. Local package SHA and packaged method probes are a distinct proof; they do not establish a newly installed, external skill consumer or AURA adoption.
3. **Delivery contract:** decide explicitly whether the accepted opt-in skill delivery recipe requires another isolated consumer packaging/installation check. If required, make it a bounded separate qualification; do not silently treat existing source examples as installation proof.
4. **Native DWF progress:** reconcile readiness, dependencies, current node states and accepted RED/GREEN/Verification requirements before proposing a state transition. Only transition nodes with exact evidence and matching achievements; do not mark the parent chunk or milestone done by inference.
5. **Authorization:** all four capabilities remain `AgentCallable` with `runtime-only` authorization. `brokerCandidateIds=[]`; no `BrokerReady`, host-action grant, public package publication or sibling-repository modification.

## Next bounded action

Verify the pinned *13 portable protocol fixtures* against their existing conformance proof and map their identities to `M8-W01-C01-T1`. This is a narrow evidence verification, not a rerun of all Discovery tests. Keep the native plan unchanged until complete acceptance and progression requirements are established.

This document is a checkpoint for an eventual coherent M8-W01 qualification, **not** an acceptance verdict.
