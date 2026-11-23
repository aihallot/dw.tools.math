# RS298 — Activate CLR Verification with isolated portable export and real-CLR admission

## Durable prerequisites

RS297 succeeded at attempt 001 after two recorded pre-attempt refusals (not global first-pass); it completed only `M8-W01-C01-T2-G` and admitted `M8-W01-C01-T2-V` to `ready`. Its parent T2, phase C01, work package W01 and milestone M8 remain `in-progress`; T1 is already `done`.

## Bounded verification

Activate only `M8-W01-C01-T2-V`. Create a new Math-owned verification caller that invokes the actual public portable `--export` CLI into an isolated new directory, copies its fourteen files to a second isolated consumer, and verifies their recorded thirteen SHA-256 identities using a copied standalone offline verifier from a third working directory. The **copied** manifest is then submitted to the existing `ProbeM8SelectedClrReflectionRed.cs --validate` admission CLI, which loads exactly the SHA-pinned published Composition, IR and Quantities assemblies and validates four source-selected methods.

Prove new consumer-side negatives by coherently recomputing CLR signatures and fingerprints for a nonexistent member and incorrect parameter type, then requiring that the existing real CLR gate refuse both despite self-consistent metadata; also require the copied offline verifier to refuse the altered file. Restore the copied original bytes and reverify clean integrity and real published methods. No duplicate reflection loader, package publication, source manifest rewrite or new authorization is allowed.

## Scope and continuation

This is independently copied portable consumer plus local published-CLR admission evidence. The unsigned receipt does not establish authenticity; local method existence does not authorize broker invocation or prove an external agent installation. The canonical backlog `0.1.124` remains `planned`. T2 Verification stays `in-progress` after this run; a separate evidence-backed lifecycle run is needed to close it.
