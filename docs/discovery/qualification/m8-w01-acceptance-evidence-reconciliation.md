# M8-W01 — Discovery acceptance evidence reconciliation

Status: **selected CLR RED closure and GREEN readiness under RS295, not M8 release acceptance**. RS293 and RS294 both succeeded first-pass, providing new compiled-method falsification and a single reusable reflection-backed admission routine. RS295 targets T2-RED `done` with exactly one achievement and T2-GREEN `ready`; parent T2 and M8/W01/C01 remain `in-progress`, T2 Verification remains `not-ready`, and the canonical backlog v0.1.124 remains `planned`. No external host, signed origin or broker authorization is granted.

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

## RS287 — qualified strict CLR metadata admission gate

The qualified Math-owned `ValidateM8StrictBindingMetadata.cs` checks that each capability's canonical CLR signature can be reconstructed **exactly** from its declared assembly, assembly version, type, member, return type and ordered parameter types, then recomputes its SHA-256 fingerprint. It supplements the original portable-envelope validation without modifying the historical SHA-pinned verifier or changing the four selected Math capabilities.

RS287 qualified four selected Math positive examples and one independently pinned synthetic envelope, plus seven explicit tamper refusals. The two newly reproduced RS286 RED gaps (parameter type and binding member drift) are refused by this stricter gate. The existing `runtime-only` authority boundary remains mandatory. It can be invoked read-only with `dotnet run --file docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs -- --validate <manifest-path>` and does not load or authorize CLR assemblies.

**Scope:** This additional qualification is Math-specific metadata alignment, not a retrospective replacement for portable protocol conformance or proof of cross-repository broker routing. At RS287, RED was `in-progress` and GREEN/Verification remained `not-ready`; later RS288/RS289 reports provide the distinct forward-only state transitions.

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

## RS290 — evidence-backed GREEN closure and Verification readiness

RS288 and RS289 are separate successful first-pass executions. RS288 completed only the newly evidenced RED subtask, with one truthful achievement. RS289 activated GREEN and ran the **actual Math attestation boundary**, preserving two deterministic fourteen-file positive exports, independent offline verification and three historical post-receipt tamper refusals. It additionally confirmed three new before-receipt refusals: mismatched CLR member, mismatched parameter type and unauthorized execution grant; each was rejected by the RS287 strict gate before the exporter emitted a receipt or verifier.

RS290 records the resulting **GREEN completion** with exactly one matching DWF achievement and admits only its Verification sibling to `ready`. The parent `M8-W01-C01-T1` and its M8 ancestor chain remain `in-progress`. Verification must still perform distinct fresh acceptance checks and cannot be completed by the GREEN evidence alone. This is a forward-only progress transition, not a retrospective completion of historical protocol fixtures.

The maintained `ValidateM8W01EvidenceFrontier.cs` now validates the durable RS288/RS289 lineage and exact new target states. This avoids rerunning all attestation exports merely to change lifecycle status. The canonical signed-or-unsigned trust boundary remains unchanged: the receipt is **unsigned** and confers no provenance authentication, broker authorization or AURA installation.

See [RS290 GREEN/Verification decision](../../planning/decisions/m8-w01-c01-t1-green-closure-verification-readiness-rs290.md).

## RS291 — independent public-export Verification boundary

RS290 durably completed the GREEN leaf and admitted Verification to `ready` on attempt 001, first-pass. RS291 activates **only** the existing Verification leaf under its active ancestor chain. No `done` achievement or parent-task completion is declared.

The new Math-owned `ProbeM8VerificationPortableConsumer.cs` exercises the **public** `AttestM8PortableDiscoveryExport.cs --export` command into a fresh disposable destination, not the wrapper's private `--check` path. It independently copies all fourteen resulting files into a consumer directory, checks the unsigned receipt's thirteen file hashes, verifies the four selected binding identities and the pinned thirteen-case protocol source/receipt, invokes the existing RS287 strict gate, then executes the copied `verify-export.cs` from a separate host directory under .NET 11. Two new consumer-side negative cases (altered CLR member and unauthorized execution grant) must be refused both by strict validation and the standalone checksum verifier. The original exported manifest is restored between cases and its clean verification repeated.

This qualifies only portable **consumer-side structural metadata and integrity**, not a new Math calculation, signed origin, actual broker host installation, external AURA acceptance or runtime permission. The existing source package versions, previously qualified private DLLs, pinned protocol corpus and historic DWF evidence remain unchanged. See [RS291 Verification decision](../../planning/decisions/m8-w01-c01-t1-verification-independent-consumer-rs291.md).

## RS292 — close protocol T1; admit selected CLR T2 RED

The durable RS291 attempt 001 succeeded **first-pass**, with zero preparation failures or boundary violations. Its fresh Math-owned test invoked the actual public `AttestM8PortableDiscoveryExport.cs --export` command, copied fourteen files into an isolated consumer, checked the unsigned receipt's thirteen SHA-256 file identities, cross-checked four selected CLR binding declarations and immutable 13-case Decision protocol pins, and executed the copied offline verifier from a different working directory. It independently refused altered CLR member and authorization metadata through both the strict gate and recorded file-hash boundary, restoring clean verification after each test. **This is structural/metadata consumer proof**, not independent broker installation or an additional compiled Math runtime execution.

RS292 now proposes **two separate, truthful DWF completions**: the active `M8-W01-C01-T1-V` Verification leaf, using fresh RS291 evidence; then its parent `M8-W01-C01-T1`, whose RED (RS288), GREEN (RS290) and Verification (RS292) are all completed. Each completion has one matching contribution achievement. Only `M8-W01-C01-T2` and its RED leaf `M8-W01-C01-T2-R` become `ready`; neither is executed or completed.

The existing maintained native adoption and Discovery frontier validators enforce the exact 591-node target and the durable RS290/RS291 evidence, without rerunning expensive cross-platform or private-package campaigns. The next work should obtain fresh negative and positive evidence specifically for the selected compiled CLR bindings, not backfill the T2 RED/GREEN/Verification lifecycles from older Windows/WSL consumer tests.

See [RS292 closure and next RED admission](../../planning/decisions/m8-w01-c01-t1-verification-closure-t2-red-readiness-rs292.md).

## RS293 — actual published CLR reflection RED falsification

RS292 durably completed protocol T1 and its Verification leaf at attempt 001, first-pass. RS293 activates only the already-ready T2 and T2-RED lineage; it does not complete RED, GREEN, T2, C01 or M8.

The new Math-owned ProbeM8SelectedClrReflectionRed.cs reads the three already-published immutable private NuGet archives from NUGET_LOCAL, checks their pinned archive and embedded DLL SHA-256 identities, extracts their net10 binaries into ignored build/artifacts and loads them into a disposable AssemblyLoadContext. Actual reflected public static method signatures are compared against all four selected source-pinned CLR declarations, including assembly version, owning type, method name, ordered parameter types, return type and SHA-256 fingerprint. The probe does not invoke the mathematical operations or restore/publish any package.

Three new independently falsifiable RED cases forge an internally consistent declaration: a nonexistent method, a wrong parameter type, and a wrong return type. Each has its canonical signature and fingerprint coherently recomputed. The existing RS287 metadata-only validator should admit these structurally consistent forgeries, while the actual compiled DLL reflection boundary must refuse every one. A metadata-granted execution authorization remains forbidden by RS287 as a safety control. The result identifies a missing real-CLR admission step, **not** a defect in the metadata-only validator's documented scope.

All four positive selected CLR bindings, original source manifests, package hashes and the source-only historical binding flags remain unchanged. An independently qualified future GREEN gate must bind the runtime method to the declared metadata; do not count the RED observations or RS279/282 previous positive consumers as GREEN or broker authorization.

See the RS293 bounded RED decision at docs/planning/decisions/m8-w01-c01-t2-red-reflection-falsification-rs293.md.

## RS294 — qualified reusable real published CLR binding admission

RS293 succeeded at attempt 001, first-pass, demonstrating that metadata-only validation can admit three internally coherent but nonexistent CLR descriptions (unknown member, changed first parameter type, changed result type), whereas reflection over the three SHA-pinned published private DLLs refuses them. The actual four selected methods still exist and match their source-qualified canonical signatures. T2-RED remains `in-progress`; no new completion is asserted.

Rather than introduce a second near-identical reflection validator, RS294 durably extended the **same Math-owned probe** `ProbeM8SelectedClrReflectionRed.cs` with `--validate <manifest-path>`, sharing exactly one `AdmitRealPublishedBindings` implementation with the existing RED `--check` mode. It verifies protocol/owner/schema identity, exactly four selected capabilities and source package identities, `AgentCallable` plus `runtime-only` exposure, three immutable private package/DLL digests, the actual public static CLR signature by reflection and its pinned canonical SHA-256 identity. The `--check` path additionally proved the real gate refuses the three coherent forgeries, while the existing RS287 metadata-only validator continues to accept them for its documented limited purpose.

This is a local private-feed **admission/qualification tool**, not a broker, method-invocation engine, cross-repository integration or public package release. No historical RED code path is removed and no source-only pins or published archives are modified. RS294 succeeded attempt 001 first-pass, with both the new CLI-mode and adversarial checks qualified. A distinct DWF run must still close RED and ready GREEN. See [RS294 bounded admission decision](../../planning/decisions/m8-w01-c01-t2-real-clr-admission-rs294.md).

## RS295 — finish selected CLR RED and admit GREEN

RS293 proved **three new coherent but non-existent published CLR bindings** by independently inspecting SHA-pinned Composition, IR and Quantities DLLs: missing method, wrong first parameter type and wrong result type. They remained acceptable to the RS287 *metadata-only* consistency check but could not be resolved in the actual published CLR. RS294 then qualified the same reflection probe with a reusable `--validate <manifest-path>` mode, sharing one admission routine for the four selected real methods and the three forgery refusals. Both executions are durable attempt-001 first-pass successes, with zero boundary violations and no method invocation, new package publication, or runtime authorization.

RS295 proposes exactly one completion: `M8-W01-C01-T2-R` goes from `in-progress` through typed DWF convergence to `done`, with exactly one matching RED achievement. Its sibling `M8-W01-C01-T2-G` moves from `not-ready` to `ready` using typed target-reentrant DWF state transition. Parent T2 stays `in-progress`, its Verification sibling stays `not-ready`, and the C01 phase, W01 and M8 remain `in-progress`. The maintained native 591-node and Discovery frontier validators now require the RS293/RS294 durable evidence and target states rather than replaying the costly private-binary test campaigns merely to record lifecycle changes.

The next GREEN cycle must produce new independent use of the **real CLR admission API** against the chosen runtime delivery boundary; RED completion alone does not qualify external consumers, broker authorization or mathematical method invocation. See [RS295 RED closure and GREEN readiness decision](../../planning/decisions/m8-w01-c01-t2-red-closure-green-readiness-rs295.md).

## Acceptance gaps before native lifecycle transitions

1. **Protocol acceptance:** pinned *13 portable fixtures* have independent RS131 and RS209 proof; verify the same exact pins during readiness assessment instead of re-running the protocol merely for ceremony. Completion remains a distinct native lifecycle decision.
2. **Consumer boundary:** positive source-project examples and RS279's positive immutable private-package consumer both exist. Neither proves installation of the generated skill bundle in a fresh external consumer, authority to invoke through a broker, or AURA adoption.
3. **Delivery contract:** decide explicitly whether the accepted opt-in skill delivery recipe requires another isolated consumer packaging/installation check. If required, make it a bounded separate qualification; do not silently treat existing source examples as installation proof.
4. **Native DWF progress:** RS293/RS294 provide durable RED and new real-method admission evidence. RS295 targets T2-RED `done` and T2-GREEN `ready` only; T2 Verification and all unrelated M8 work remain `not-ready`, with C01/W01/M8 still `in-progress`.
5. **Authorization:** all four capabilities remain `AgentCallable` with `runtime-only` authorization. `brokerCandidateIds=[]`; no `BrokerReady`, host-action grant, public package publication or sibling-repository modification.

## Next bounded action

After RS295 closes T2-RED and admits GREEN, activate GREEN in a separate forward-only run and qualify the real published-CLR admission API at a bounded consumer boundary, including refusal of coherently forged methods. Preserve canonical pins, immutable packages and runtime-only authority.

This document is a checkpoint for an eventual coherent M8-W01 qualification, **not** an acceptance verdict.

## RS296 — bounded independent real-CLR GREEN consumer (prepared scope)

RS295 succeeded on attempt 001 with T2-RED `done` and T2-GREEN `ready`. RS296 is a forward-only GREEN activation plus separate-process admission probe, retaining the published four-method positives and proving the real CLI refuses three coherent false CLR declarations and an unauthorized metadata grant. It may set T2-GREEN to `in-progress` only, never `done`, while its Verification sibling remains `not-ready`. It does not grant an external broker or modify immutable package archives. See [RS296 GREEN decision](../../planning/decisions/m8-w01-c01-t2-green-independent-admission-rs296.md).

## RS297 — selected CLR GREEN completion and Verification readiness

RS296 succeeded at allocated attempt 001 but **not global first-pass**: a prior pre-attempt target-reentry refusal was durably recorded. Its independent caller checked the actual four-method compiled CLR admission against three coherently forged signatures and refused metadata-granted execution without granting a broker. RS297 closes only GREEN with a single evidenced DWF achievement and admits the Verification sibling to `ready`; Verification, T2 and parent M8 boundaries remain unfinished. See [RS297 decision](../../planning/decisions/m8-w01-c01-t2-green-closure-verification-readiness-rs297.md).

## RS298 — isolated portable export and real-CLR Verification boundary

RS297 durably completed the selected T2 GREEN leaf on attempt 001 after **two** pre-attempt preparation refusals; this was not a first-pass result. It admitted T2 Verification to `ready`, without completing it. RS298 activates only this Verification leaf and qualifies a fresh Math-owned boundary: public portable export copied to a separate consumer, an independently launched offline checksum verifier, and the existing single real published-CLR admission CLI over the copied manifest. Coherently re-signed false method declarations must be rejected by actual CLR admission and portable integrity checks. The private published three-package SHA-pinned closure, signed-origin limits and runtime-only authorization remain unchanged; no broker grant or T2 completion is implied. See [RS298 decision](../../planning/decisions/m8-w01-c01-t2-verification-isolated-real-clr-rs298.md).

## RS299 — close selected compiled CLR Verification and T2

RS298 succeeded first-pass at attempt 001. Its fresh test exercised the actual public portable `--export` path, copied fourteen files into an isolated consumer, verified thirteen unsigned SHA-256 receipt identities from a third working directory, and submitted the copied four-method manifest to the existing reflection-backed real published CLR gate. Two coherent false method declarations were refused by both the real-CLR gate and copied offline verifier before restoring the original clean consumer. RS297 previously completed GREEN, RS295 completed RED. RS299 records two exact native achievements: T2 Verification `done` and parent selected CLR T2 `done`. Phase C01, work package W01 and M8 remain `in-progress`; C02, W02, M9 remain `not-ready`. Neither unsigned integrity nor local compiled method existence grants broker authority or external installation. See [RS299 decision](../../planning/decisions/m8-w01-c01-t2-verification-task-closure-rs299.md).

## RS300 — close pinned protocol/selected CLR C01 and admit C02 readiness

RS299 durably completed both the selected CLR Verification leaf and its parent T2 in allocated attempt 001, following one pre-attempt target-reentry refusal; it was not global first-pass. T1 had already completed from RS292. RS300 closes only phase `M8-W01-C01` with one evidence-backed native achievement and changes `M8-W01-C02` from `not-ready` to `ready`, without activating or completing its skill generator or delivery tasks. The pinned thirteen-case portable conformance, selected four published CLR methods and refusal boundaries remain qualified at the local Math boundary. The public portable receipt is unsigned, no external broker or host installation is certified, and W01/M8 remain `in-progress`. See [RS300 decision](../../planning/decisions/m8-w01-c01-closure-c02-readiness-rs300.md).

## RS301 — C02 skill-generator contract RED activation and isolated drift probe

RS300 succeeded first-pass and completed C01 while admitting C02 only to `ready`. RS301 activates the C02/T1/RED lineage as `in-progress` and qualifies a fresh isolated reproduction of the existing `GenerateM8MathSkill.cs --check` contract. This copies the generator, source-qualified four-method manifest, generated skill and bundled reference manifest into a disposable workspace, then requires a clean check and explicit refusal of skill text drift, bundled-manifest drift and metadata-granted execution. The normal generator remains unchanged, all canonical files are immutable, and this only **starts RED**; no RED/GREEN/Verification completion, external agent installation or broker authorization is claimed. See [RS301 decision](../../planning/decisions/m8-w01-c02-t1-red-isolated-skill-drift-rs301.md).

## RS302 — close isolated generator RED and admit GREEN readiness

RS301 succeeded at attempt 001 after one pre-attempt readiness refusal (not a global first-pass result). Its isolated copy of the existing `GenerateM8MathSkill.cs --check` accepted pristine generated bytes, refused corrupted skill text and bundled manifest, and denied a metadata-granted execution authorization; original tracked inputs remained unchanged. RS302 completes only native `M8-W01-C02-T1-R` with one evidence-matched RED achievement and transitions `M8-W01-C02-T1-G` from `not-ready` to `ready`. Task T1 and phase C02 remain `in-progress`; T1 Verification and C02/T2 remain `not-ready`. This is not GREEN completion, signed provenance, installed third-party agent or broker authorization. See [RS302 decision](../../planning/decisions/m8-w01-c02-t1-red-closure-green-readiness-rs302.md).

## RS303 — deterministic generator GREEN with reusable strict CLR gate

RS302 succeeded first-pass at attempt 001, completing only C02/T1-RED and admitting GREEN to `ready`. RS303 activates GREEN without completing it. The existing `GenerateM8MathSkill.cs` generator now calls the qualified RS287 strict source-metadata gate in both `--check` and `--write` paths before either output can be written. A fresh isolated test produces two independent identical skill/manifest pairs, tests their clean check and rejects altered CLR fingerprints and signature metadata before any output mutation. The RS301 RED regression probe is adjusted only to copy the reused validator into its isolated workspace. No private archives, historical reports, source-qualified manifest or external broker permission change. See [RS303 decision](../../planning/decisions/m8-w01-c02-t1-green-strict-generator-rs303.md).

## RS304 — close deterministic generator GREEN and admit Verification readiness

RS303 succeeded first-pass at attempt 001, qualifying two byte-identical independently generated skill/manifest pairs and refusal of forged source CLR fingerprints and parameter metadata before output changes. The earlier RS301 isolated RED regression also passed with the reused RS287 strict validator. RS304 completes only `M8-W01-C02-T1-G` with one matching GREEN achievement and readies `M8-W01-C02-T1-V`, without activating Verification. Parent T1/C02/W01/M8 remain in-progress; C02/T2 remains not-ready; no signed provenance, agent installation, broker grant or public release is claimed. See [RS304 decision](../../planning/decisions/m8-w01-c02-t1-green-closure-verification-readiness-rs304.md).

## RS305 — independent copied skill-generator Verification activation

RS304 succeeded first-pass, closing deterministic skill GREEN and admitting Verification to `ready`. RS305 starts the Verification leaf and qualifies a fresh **copy-only** generated skill consumer. The test copies the canonical source manifest, generated skill, bundled manifest and the existing generator and strict CLR gate into a disposable consumer; it checks byte identities and the four selected runtime-only CLR declarations, runs the existing strict validator on the **copied** manifest and the generator's public `--check` on the copied skill. It requires both a forged fingerprint and a modified generated skill to be refused without changing source files. This demonstrates an isolated, locally copied skill-consumer verification boundary, not external agent installation, signed origin, broker authorization or task completion. See [RS305 decision](../../planning/decisions/m8-w01-c02-t1-verification-copied-skill-rs305.md).

## RS306 — complete generator Verification and T1; admit delivery RED

RS305 succeeded first-pass at attempt 001 and independently verified the copied, four-capability generated skill through the single existing strict CLR metadata and generator check entrypoints. Its disposable copied manifest fingerprint and skill-text tampering were rejected and restored; the canonical source and published packages remained unchanged. With RED closed by RS302 and GREEN by RS304, RS306 records two separate native achievements for `M8-W01-C02-T1-V` and its parent `M8-W01-C02-T1`. It readies `M8-W01-C02-T2` and its RED leaf without activating delivery. C02/W01/M8 remain in-progress, and runtime broker authorization, signed origin and external host installation are outside this boundary. See [RS306 decision](../../planning/decisions/m8-w01-c02-t1-verification-task-closure-delivery-readiness-rs306.md).

## RS307 — opt-in portable skill delivery RED and independent refusal boundary

RS306 succeeded first-pass at attempt 001 and completed the deterministic generator T1, admitting C02 delivery T2/RED only to `ready`. RS307 activates that RED lineage and qualifies the existing portable attestation tool's public `--export` and `--verify` commands in disposable Math-owned directories. It must export to an empty path, refuse an existing occupied destination without changing it, require a copied independent verifier to accept an unmodified unsigned receipt, and reject a tampered portable skill, forged receipt authority and unexpected extra file. Source-only/immutable private packages remain unchanged; no external host installation, trusted signature or broker permission is asserted. RED remains `in-progress`, with GREEN and Verification `not-ready`. See [RS307 decision](../../planning/decisions/m8-w01-c02-t2-red-opt-in-portable-delivery-rs307.md).

## RS308 — complete opt-in portable delivery RED and admit GREEN

RS307 succeeded first-pass at attempt 001 and exercised the existing public `--export` and `--verify` commands using an independent copied verifier. The fresh fourteen-file portable export had thirteen checksummed files, retained an unsigned runtime-only receipt, and refused an occupied destination, tampered skill content, an unexpected extra file and forged receipt authorization without altering the original source. RS308 completes only native `M8-W01-C02-T2-R` with one matching achievement and moves `M8-W01-C02-T2-G` from `not-ready` to `ready` without activating GREEN. C02/T2 stays in-progress and Verification stays not-ready. No signed provenance, third-party installation or broker authorization is claimed. See [RS308 decision](../../planning/decisions/m8-w01-c02-t2-red-closure-green-readiness-rs308.md).

## RS309 — activate delivery GREEN with independent reproducible portable exports

RS308 succeeded first-pass at attempt 001, closing delivery RED and admitting GREEN to `ready`. RS309 activates GREEN only. A new Math-owned process caller runs the existing attester's public `--export` twice into separate empty directories, compares all fourteen files by relative path and exact SHA-256 bytes, checks thirteen recorded checksum entries and unsigned runtime-only scope, and invokes a copied standalone verifier from a third working directory against both exports. An independent corrupted-copy refusal and pristine restoration prevent treating an untested copy as deployed. This local export reproducibility does not imply a signed origin, externally installed agent, host-execution authority, package publication or completed GREEN. See [RS309 decision](../../planning/decisions/m8-w01-c02-t2-green-reproducible-portable-delivery-rs309.md).

## RS310 — close reproducible opt-in delivery GREEN and ready Verification

RS309 succeeded first-pass at attempt 001, qualifying two independently exported fourteen-file portable Discovery bundles with byte-identical contents, thirteen unsigned SHA-256 receipt records, four runtime-only Math capabilities, independently launched copied verification and a separate tampered skill refusal with restoration. RS310 completes only `M8-W01-C02-T2-G` with one matching delivery GREEN achievement and admits `M8-W01-C02-T2-V` to `ready` without activating Verification. RED and generator T1 remain done, while delivery T2, phase C02, W01 and M8 stay in-progress. This is bounded local delivery integrity, not a signed origin, external agent installation, runtime broker authorization or public publication. See [RS310 decision](../../planning/decisions/m8-w01-c02-t2-green-closure-verification-readiness-rs310.md).

## RS311 — independently received portable skill and package-only Verification

RS310 succeeded first-pass at attempt 001, closing delivery GREEN and admitting T2 Verification to ready. RS311 activates only T2 Verification and tests a freshly exported fourteen-file unsigned package in a distinct received directory. A copied verifier checks all thirteen receipt entries and refuses altered Program.cs and consumer.csproj before restoring unchanged bytes. An independent working copy restores and builds its .NET 11 package-only example from a private feed containing only the three physically SHA-pinned published Math archives, then runs all five source-free examples. Its locked dependency graph and DLL hashes are checked; the original delivery remains unmodified. This does not constitute signed provenance, broker permission, public publication, external agent-host installation or completed Verification. See [RS311 decision](../../planning/decisions/m8-w01-c02-t2-verification-package-consumer-rs311.md).
