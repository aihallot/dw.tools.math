# RS296 — Activate selected CLR GREEN and check real method admission from a separate process

## Authority and scope

RS295 succeeded on attempt 001 without invocation failures or mutation-boundary violations. Its sole completed node is `M8-W01-C01-T2-R`; the GREEN sibling `M8-W01-C01-T2-G` is `ready`. The bounded next run activates only GREEN through the native DWF `ActivateReadyContinuation` operation; it does not complete GREEN or its parent task.

## Consumer check

Add a Math-owned independent *process caller* of the already-qualified `ProbeM8SelectedClrReflectionRed.cs --validate` public CLI. The caller copies the immutable canonical source manifest to a private ignored `build/artifacts` directory, passes the copy through the CLI and demands four real selected published methods. Next it creates three internally consistent, freshly fingerprinted false declarations (missing method, changed first parameter type, changed return type); all must be refused by the real-CLR admission CLI with an explicit method mismatch. A separate unauthorized execution grant must also be rejected. Source files, private published packages, three pinned archive/DLL identities, and retained historical evidence must not be altered. The external process boundary is deliberate: do not duplicate the reflection-based loader or present the metadata-only checker as real CLR proof.

## Limits and continuation

This GREEN evidence checks source-qualified method *admission* from another process, not broker integration, third-party host installation, execution authorization, signed provenance, public publication or a new package. No method invocation is required for this gate. The historic source-only flags remain false. Only a subsequent evidence-backed run may close GREEN and ready its Verification sibling. Product backlog `0.1.124` remains `planned`.
