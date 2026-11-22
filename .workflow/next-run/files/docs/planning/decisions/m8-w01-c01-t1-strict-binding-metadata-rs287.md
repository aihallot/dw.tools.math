# RS287 — Enforce strict Math Discovery binding metadata alignment without changing historical protocol evidence

## Purpose

RS286 reproduced two falsifiable gaps in the historical portable envelope verifier: a changed CLR parameter type or binding member could be admitted despite unchanged canonical signature and fingerprint. The historical source and pinned thirteen-case corpus are immutable. RS287 supplies a **separate Math-owned strict admission gate** for metadata consistency rather than modifying the historical protocol verifier.

## Contract and qualification

The additional `docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs` checks the schema-2 draft-1 Math envelope, admits only `AgentCallable` with `runtime-only` authorization and no broker candidates, reconstructs the exact CLR canonical signature from six declared identities (assembly, version, type, member, return, ordered parameter types), and independently recomputes its SHA-256 fingerprint.

The focused check must admit all four currently selected source-qualified Math capability records and the immutable synthetic portable fixture baseline. It must independently refuse at least: parameter type, CLR member, return type, assembly version, signature fingerprint, unauthorized execution grant, and BrokerReady promotion. This is a new explicit boundary, not a new package or runtime execution mechanism.

The gate also accepts `--validate <manifest-json-path>` as a read-only metadata-only inspection. It is not an installer, protocol-authority replacement, DLL loader, or permission engine. It does not claim mathematical correctness, CLR reflection equivalence or AURA consumer integration.

## Native lifecycle and next evidence

The root M8, W01, C01, T1 and protocol RED subtask remain `in-progress`. The GREEN and Verification siblings stay `not-ready`; no native state transition or completion achievement is allowed by RS287. Once this strict gate succeeds, plan the separate RED completion and GREEN readiness sequence from new DWF evidence. Existing prior-run receipts, source-generated manifest version identities and immutable private NuGet packages are preserved.
