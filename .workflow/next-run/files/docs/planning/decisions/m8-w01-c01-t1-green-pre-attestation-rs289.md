# RS289 — Qualify real pre-attestation metadata refusals during the active Discovery GREEN cycle

## Native DWF scope

RS288 succeeded first-pass, closing `M8-W01-C01-T1-R` with one matching achievement and admitting `M8-W01-C01-T1-G` to `ready`. RS289 applies the documented `ProjectPlan.ActivateReadyContinuation("M8-W01-C01-T1-G")` operation, preserving `M8 → M8-W01 → M8-W01-C01 → M8-W01-C01-T1` as the active ancestor chain. The sole new native transition is GREEN `ready → in-progress`. RED remains `done`; Verification and all other M8/M9 descendants remain `not-ready`. No completion achievement is declared.

## Positive and negative executable acceptance

The already qualified `AttestM8PortableDiscoveryExport.cs` uses the RS281 deterministic twelve-file export, calls `ValidateM8StrictBindingMetadata.cs --validate` on the **actually generated** portable manifest, then emits a standalone verifier and an unsigned integrity receipt only if the gate succeeds. RS289 adds a private `--check`-only injection callback immediately before that strict call.

The `--check` gate must preserve two valid, byte-identical fourteen-file attested exports, independent copied-verifier execution, three preexisting post-attestation corruption refusals and original source manifest bytes. It additionally produces three independently disposable malformed exports using the normal exporter: a mismatched CLR member, a mismatched declared parameter type and a metadata-granted execution authorization. Each malformed export must be refused for the exact RS287 diagnostic **before** any receipt or exported verifier is emitted.

The public `--export` path does not expose mutation injection, and the standalone `--verify` path remains a local unsigned checksum verifier, not an authenticity signature. No old report, package, pinned schema, generated canonical skill, cross-repository source or external provider is modified.

## Continuation

Only new, durable RS289 evidence can justify later GREEN completion and Verification readiness. The accepted product backlog `0.1.124` remains `planned`; no task/phase or M8 milestone completion, broker authorization, AURA installation, public release or NuGet publication is implied.
