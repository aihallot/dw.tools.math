# M8-W01 — Discovery acceptance evidence reconciliation

Status: **evidence assessment only**. Native M8, M8-W01, M8-W01-C01 and M8-W01-C02 remain `not-ready`; product backlog remains `planned`. This document does not advance native lifecycle states, grant runtime permission or qualify a release.

## Accepted product authority

`docs/planning/backlog.json`, version `0.1.124`, defines the following accepted chunk gates:

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
| Published private packages, independent Windows .NET 11 consumer | `.aura/workflow/reports/RS279/attempt-001.json` | Four previously selected CLR bindings and five existing positive example checks compiled against the actual immutable `Composition → IR → Quantities` three-package closure via source-free, locked `PackageReference`; first-pass true. No installed skill or broker authorization |
| Bundled manifest canonical LF projection | `.aura/workflow/reports/RS201/attempt-001.json` and `.aura/workflow/reports/RS202/attempt-001.json` | Generated source/bundle parity plus six independent tamper refusals |
| Selected source-project examples | `.aura/workflow/reports/RS204/attempt-001.json` and `.aura/workflow/reports/RS205/attempt-001.json` | Positive Quantity and Replay alongside existing IR encode/hash examples in the source-project seed |
| Generated example navigation and refusals | `.aura/workflow/reports/RS206/attempt-001.json` and `.aura/workflow/reports/RS207/attempt-001.json` | Navigable examples reference and five independent link/claim refusals |

Each cited RS report above has durable `outcome=succeeded` at attempt 001. **First-pass is not synonymous with attempt 001**: RS185, RS198 and RS201 retain earlier invocation failures. Consult their `priorInvocationFailures` records rather than rewriting that history.

## Revised RS280 qualification frontier

The accepted portable protocol gate is no longer an open request to repeat the 13 fixtures: `RS131` durably executed all 13 pinned `dw.discovery/1.0.0-draft.1` cases (2 accepted, 11 rejected), and `RS209` separately cross-checked source pins and recorded verdicts. This supports `M8-W01-C01-T1` technical evidence only; it does not create retroactive RED/GREEN/Verification lifecycle transitions.

The previously selected `M8-W01-C01-T2` four bindings gained a distinct source-free **Windows .NET 11** positive runtime check in `RS279`, attempt 001, first-pass true. The actual private closure was:
- `dw.tools.math.composition 20260.20261107.1001`, archive SHA-256 `b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f` (direct);
- `dw.tools.math.ir 20260.20261104.1001`, archive SHA-256 `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480` (transitive);
- `dw.quantities 20260.20261102.1001`, archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` (transitive).

The net11 locked restore and loaded DLL SHA checks were exercised by `docs/engineering/ProbeM8Net11PublishedDiscoveryBindings.cs`, reusing `tests/consumers/m8-discovery-clr-seed/Program.cs`. It is **positive private-package binary evidence**, not a version rewrite: the canonical source-generated `m8-discovery-manifest.json` still declares source package version `0.3.0-preview.1`; the physical calendar versions are independent. The historical `m8-selected-clr-bindings.json` keeps its source-only flag and remains immutable in meaning.

## Acceptance gaps before native lifecycle transitions

1. **Protocol acceptance:** pinned *13 portable fixtures* have independent RS131 and RS209 proof; verify the same exact pins during readiness assessment instead of re-running the protocol merely for ceremony. Completion remains a distinct native lifecycle decision.
2. **Consumer boundary:** positive source-project examples and RS279's positive immutable private-package consumer both exist. Neither proves installation of the generated skill bundle in a fresh external consumer, authority to invoke through a broker, or AURA adoption.
3. **Delivery contract:** decide explicitly whether the accepted opt-in skill delivery recipe requires another isolated consumer packaging/installation check. If required, make it a bounded separate qualification; do not silently treat existing source examples as installation proof.
4. **Native DWF progress:** reconcile readiness, dependencies, current node states and accepted RED/GREEN/Verification requirements before proposing a state transition. Only transition nodes with exact evidence and matching achievements; do not mark the parent chunk or milestone done by inference.
5. **Authorization:** all four capabilities remain `AgentCallable` with `runtime-only` authorization. `brokerCandidateIds=[]`; no `BrokerReady`, host-action grant, public package publication or sibling-repository modification.

## Next bounded action

Use the verified RS131/RS209 protocol proof and RS279 positive private-package consumer to design a **forward-only** native M8-W01 task-readiness path. Inspect actual permissible DWF lifecycle operations and their source/projection validators before any transition. Do not recreate historical RED/GREEN/Verification completions, alter the source-only manifest version, or infer installed-broker readiness.

This document is a checkpoint for an eventual coherent M8-W01 qualification, **not** an acceptance verdict.
