# M8-W01 — Discovery acceptance evidence reconciliation

Status: **protocol GREEN work, not product acceptance**. RS288 durably completed `M8-W01-C01-T1-R` and admitted `M8-W01-C01-T1-G` to `ready`, first-pass. RS289 targets only `M8-W01-C01-T1-G = in-progress`, while the M8 ancestor hierarchy remains `in-progress`. Verification and other Discovery/modernization nodes stay `not-ready`; the canonical backlog remains `planned`. Neither broker authorization nor completion of the parent task is claimed.

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

## RS281–RS283 independent delivery evidence and RS284 milestone readiness

- RS281 produced a reproducible **twelve-file portable Discovery skill export** on Windows .NET 11 and exercised the original four selected operations. Its durable report records three pre-attempt refusals before success at attempt 001; `firstPassSucceeded=false`, not first-pass success.
- RS282 reproduced the exact exported package-only positive sample under default Linux/WSL SDK 11.0.100 with locked private-feed restore and three SHA-pinned binaries. The durable attempt 001 succeeded first-pass with zero prior failures.
- RS283 provided an **unsigned, reproducible integrity receipt** and a standalone C# verifier with three tamper refusals. Its attempt 001 succeeded first-pass. This is local file-integrity evidence only, not trusted distribution provenance or installation into an external broker.
- RS284 changes only the root milestone M8 from `not-ready` to `ready`. Its prerequisite M7 is durably `done`. `ready` means the milestone is eligible for future **forward-only** task readiness work; it is neither `in-progress` nor `done`, and does not retroactively complete any RED/GREEN/verification subtask. See [the exact readiness decision](../../planning/decisions/m8-root-readiness-rs284.md).

All source-only Discovery manifests, package calendars, historical report lineages and external runtime authority remain unchanged.

## RS285 bounded RED admission (target state, not completed work)

The next DWF delivery selects one native Discovery path for new work: `M8-W01 → M8-W01-C01 → M8-W01-C01-T1 → M8-W01-C01-T1-R`. Each of these four nodes is admitted only to `ready` by typed transitions, starting from the RS284-ready M8 parent. The canonical product plan remains `planned` and no historical RED, GREEN, verification, task or phase receives `done` evidence.

The first RED responsibility is pinned portable protocol and conformance, based on the immutable Decision-derived `dw.discovery/1.0.0-draft.1` schema 2 corpus. RS131 and RS209 establish historical thirteen-case identity and results (2 accepted, 11 refused); those reports are prerequisites, not new RS285 execution receipts. The next bounded RED run must create independent falsification on **new inputs** (for example, a controlled schema-pin drift and a broker-authorization escalation), preserving the pinned originals and refusing incorrect acceptance. The runtime broker and other repositories remain separate owners.

See [RS285 protocol RED readiness decision](../../planning/decisions/m8-w01-c01-t1-red-readiness-rs285.md). The target plan is checked by the maintained native DWF adoption validator and the Discovery frontier validator; no new parallel validator is introduced.

## RS286 — new independent protocol RED gap observation

RS286 begins the single M8 protocol RED chain (`M8 → M8-W01 → M8-W01-C01 → M8-W01-C01-T1 → M8-W01-C01-T1-R`) at `in-progress`. An isolated test-only copy of the original pinned `ValidatePortableProtocol.cs` runs the unchanged 13/13 fixtures, then checks three new scenarios using its **actual** `ValidateMathDraftEnvelope` method: a CLR parameter type changed while the signature/fingerprint stay fixed; a CLR member name changed while the signature/fingerprint stay fixed; and a prohibited metadata authorization escalation. The two identity mismatches are currently *admitted* (RED gaps); the authorization escalation is refused (existing safety control).

The new Math-owned `ProbeM8ProtocolRedSignatureMetadata.cs` observes these differences without modifying the historical verifier or pinned schema, rebuilding a Math library, or changing published packages. The RED subtask is **active**, not `done`; an independent stricter binding-metadata gate and separate GREEN/Verification cycles remain necessary. See [RS286 RED decision](../../planning/decisions/m8-w01-c01-t1-red-signature-metadata-rs286.md).

## RS287 — additional strict CLR metadata admission gate (pending validation)

The proposed Math-owned `ValidateM8StrictBindingMetadata.cs` checks that each capability's canonical CLR signature can be reconstructed **exactly** from its declared assembly, assembly version, type, member, return type and ordered parameter types, then recomputes its SHA-256 fingerprint. It supplements the original portable-envelope validation without modifying the historical SHA-pinned verifier or changing the four selected Math capabilities.

RS287 targets four selected Math positive examples and one independently pinned synthetic envelope, plus seven explicit tamper refusals. The two newly reproduced RS286 RED gaps (parameter type and binding member drift) must now be refused by this stricter gate. The existing `runtime-only` authority boundary remains mandatory. It can be invoked read-only with `dotnet run --file docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs -- --validate <manifest-path>` and does not load or authorize CLR assemblies.

**Scope:** This additional qualification is Math-specific metadata alignment, not a retrospective replacement for portable protocol conformance or proof of cross-repository broker routing. M8's protocol RED leaf remains `in-progress`; GREEN/Verification sibling states remain `not-ready` until a separate forward-only DWF decision and run.

## RS288 — RED completion, GREEN readiness and actual export enforcement

The new RED observations (RS286: signature/member drift admitted by the historical validator, while authorization escalation remained refused) and the RS287 strict metadata guard (four selected Math positives, one synthetic positive and seven refusals) justify a **new DWF-recorded RED subtask completion**, not fabricated historical progress. RS287 attempt 001 succeeded **after one pre-attempt refusal** (`firstPassSucceeded=false`).

RS288 uses the typed target-reentrant DWF `ConvergeNodeToDone` operation only for `M8-W01-C01-T1-R`, with exactly one corresponding completion achievement. It then makes its GREEN sibling `ready`. The parent task, phase, work package and milestone remain `in-progress`, and no other subtask or release becomes `done`.

The existing Math-owned `AttestM8PortableDiscoveryExport.cs` must now call the already qualified strict gate in `--validate` mode **against the actual generated export's manifest**, before producing the unsigned SHA-256 receipt. The wrapper still checks deterministic exports, three tamper refusals and standalone offline verification. Its independent `--verify` path continues to verify recorded hashes and authorization metadata without needing the Math repository or running the strict gate. The historic twelve-file exporter, generated source manifest, published packages and all prior qualification receipts remain untouched.

See [RS288 RED/GREEN delivery decision](../../planning/decisions/m8-w01-c01-t1-red-closure-green-readiness-rs288.md). This integration is Math-owned and **does not** grant broker authority, prove external host adoption, or authenticate an unsigned receipt.

## RS289 — activate GREEN and test pre-attestation refusals

The RS288 DWF report records `RED = done` with exactly one evidence-backed achievement and `GREEN = ready`, first-pass at attempt 001. RS289 activates **only** `M8-W01-C01-T1-G` using the target-reentrant `ProjectPlan.ActivateReadyContinuation` operation. All four ancestors remain `in-progress`; RED remains `done`; Verification remains `not-ready`; the product backlog stays `planned`. This step does not declare GREEN complete.

RS289 extends the existing Math-owned `AttestM8PortableDiscoveryExport.cs --check` validation to inject three independent, test-only corruptions in freshly generated disposable exports, **before** the established RS287 strict metadata validation and **before** creation of `verify-export.cs` or the unsigned SHA-256 receipt:

- CLR member name changed while the canonical signature/fingerprint are unchanged
- CLR parameter type changed while the canonical signature/fingerprint are unchanged
- Runtime-only authorization escalated to `granted`

For each case, the actual attestation path must surface the precise RS287 strict validator refusal and produce **no receipt or exported verifier**. The same check retains two byte-identical valid exports, the copied standalone offline verifier, all three previous post-attestation tamper refusals, and unchanged canonical Math metadata. The injection is internal to `--check`, not a public `--export` option. Consult [the RS289 GREEN decision](../../planning/decisions/m8-w01-c01-t1-green-pre-attestation-rs289.md).

This adds independent end-to-end GREEN evidence, not a new protocol, publicly signed provenance, external skill installation, mathematical runtime execution or broker authorization.

## Acceptance gaps before native lifecycle transitions

1. **Protocol acceptance:** pinned *13 portable fixtures* have independent RS131 and RS209 proof; verify the same exact pins during readiness assessment instead of re-running the protocol merely for ceremony. Completion remains a distinct native lifecycle decision.
2. **Consumer boundary:** positive source-project examples and RS279's positive immutable private-package consumer both exist. Neither proves installation of the generated skill bundle in a fresh external consumer, authority to invoke through a broker, or AURA adoption.
3. **Delivery contract:** decide explicitly whether the accepted opt-in skill delivery recipe requires another isolated consumer packaging/installation check. If required, make it a bounded separate qualification; do not silently treat existing source examples as installation proof.
4. **Native DWF progress:** RS288 recorded the RED `done` receipt and GREEN `ready` target, while RS289 activates the GREEN leaf to `in-progress` for fresh end-to-end refusal observations. Neither GREEN nor its parent is completed; Verification and other M8 children remain `not-ready`.
5. **Authorization:** all four capabilities remain `AgentCallable` with `runtime-only` authorization. `brokerCandidateIds=[]`; no `BrokerReady`, host-action grant, public package publication or sibling-repository modification.

## Next bounded action

After RS289 qualifies the actual pre-attestation refusal boundary, review the newly recorded GREEN evidence before a separate forward-only GREEN completion and Verification readiness decision. Preserve the historical schema, private package identities and runtime-only broker boundary.

This document is a checkpoint for an eventual coherent M8-W01 qualification, **not** an acceptance verdict.
