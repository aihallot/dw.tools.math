# RS286 — Activate the Math Discovery protocol RED leaf and observe unbound CLR metadata

## Activation

The preceding RS285 first-pass result made one complete protocol RED chain ready. RS286 enters it into `in-progress`: `M8 → M8-W01 → M8-W01-C01 → M8-W01-C01-T1 → M8-W01-C01-T1-R`. All five transitions use the documented typed, target-reentrant DWF API and preserve the 591-node plan. GREEN and Verification siblings, all other M8 branches, and M9 remain `not-ready`. No node reaches `implemented`, `validated` or `done`; no achievement is declared.

## Newly falsifiable RED boundary

The new `docs/discovery/qualification/ProbeM8ProtocolRedSignatureMetadata.cs` copies the **unchanged** historical pinned verifier `ValidatePortableProtocol.cs` and the exact immutable Decision-origin schema and thirteen-case corpus to an ignored disposable directory under `build/artifacts/`. It inserts three controlled in-memory metadata mutations into a test-only copy and runs the **actual original** `ValidateMathDraftEnvelope` function. The original 13-case protocol conformance must still pass 13/13.

- **New parameter type drift:** change the first synthetic capability declared parameter type from `System.Double` to `System.String`, retaining the canonical signature and its SHA-256 fingerprint. The current verifier admits this mismatched parameter metadata. That observation is the intended new RED failure of signature alignment.
- **New CLR member drift:** change the first synthetic capability binding member from `Multiply` to `NotMultiply`, retaining the canonical signature and fingerprint. The current verifier admits this mismatched member metadata. That observation is a second new RED failure.
- **Safety control:** change the execution authorization from `runtime-only` to `granted`. The current verifier must refuse the escalation. The control distinguishes a real alignment gap from a completely disabled verifier.

The probe passes only when those exact three **current** behaviors are observed and the canonical pinned 13/13 corpus remains unmodified. It is a test of known missing guarantees, **not** proof that malformed metadata is safe or that the RED subtask is finished.

## Forward-only GREEN/Verification path

Implement a narrow independent Math-owned strict metadata-alignment gate, keeping the original historical verifier and its SHA-pinned reports immutable. Show correct matching signatures still accepted, both mismatches refused, and runtime-only authority preserved. Only then consider transitioning the active RED subtask according to its current DWF lifecycle, with separate durable evidence for GREEN and Verification.

The source manifest, private NuGet archives, M8/M9 product backlog v`0.1.124`, external repositories, AURA permissions, and publication policy are unchanged.
