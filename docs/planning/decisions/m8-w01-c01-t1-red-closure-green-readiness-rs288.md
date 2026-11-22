# RS288 — Close newly evidenced protocol RED and admit GREEN, enforce strict validation on portable export

## Accepted evidence and DWF lifecycle

RS286 (attempt 001, first-pass) created a fresh, isolated falsification using the unmodified pinned portable verifier. It observed two specific gaps: declared parameter type and declared binding member were not linked to the canonical CLR signature, while an authorization escalation was correctly refused. RS287 then delivered an independent strict Math-owned validator, qualified four selected CLR metadata records plus one synthetic reference and seven tamper refusals. RS287 succeeded attempt 001 after one **preselection failure**, so `firstPassSucceeded=false` must remain visible.

This is enough to **complete the new protocol RED subtask**, not the parent Discovery task. In the RS288 payload, the typed `ProjectPlan.ConvergeNodeToDone("M8-W01-C01-T1-R")` moves the active RED leaf through the canonical implemented/validated/done states and accepts exact done reentry. The contribution declares **exactly one** matching RED achievement. The sibling `M8-W01-C01-T1-G` moves from `not-ready` to `ready` using the typed target-reentrant transition. No GREEN execution is claimed.

The native M8, W01, C01 and T1 ancestors remain `in-progress`. RED becomes `done`; GREEN becomes `ready`; Verification, selected CLR T2, C02, M8-W02 and M9 remain `not-ready`. Workflow activeSubtask becomes null after reconciliation. The product backlog `0.1.124` stays `planned` and no higher-level task or release is completed.

## Integrate the new guard with the actual delivery boundary

The Math-owned `docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs` is an existing fourteen-file portable export and unsigned SHA-256 receipt wrapper. RS288 makes its `ExportWithReceipt` flow invoke the RS287 `ValidateM8StrictBindingMetadata.cs --validate` entrypoint on the **actual exported manifest** after the original twelve-file export and before receipt construction. A mismatch must refuse attestation rather than merely generating metadata with an unrelated optional checker.

RS288 re-runs the focused strict gate and the existing attester's deterministic two-export, standalone verifier and three-tamper boundary suite. It does **not** alter the original pinned thirteen-case Decision schema/fixtures, the historical `ValidatePortableProtocol.cs`, the source-generated skill manifest, private NuGet packages or the unmodified offline verifier semantics. The attestation receipt remains **unsigned** and no trusted external origin proof is invented.

## Next GREEN execution

RS289 may activate the ready GREEN sibling and independently demonstrate rejection of a tampered generated manifest at the real consumer-facing boundary, preserving valid positive exports. RED completion alone does not imply GREEN implementation, Verification completion, BrokerReady, runtime permission, host adoption or public publication.
